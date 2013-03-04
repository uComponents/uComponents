using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using umbraco.NodeFactory;
using CmsContentType = umbraco.cms.businesslogic.ContentType;
using CmsProperty = umbraco.cms.businesslogic.property.Property;
using umbraco.editorControls;
using System.Web.UI.HtmlControls;

[assembly: WebResource("uComponents.DataTypes.CheckBoxTree.CheckBoxTree.css", Constants.MediaTypeNames.Text.Css)]
[assembly: WebResource("uComponents.DataTypes.CheckBoxTree.CheckBoxTree.js", Constants.MediaTypeNames.Application.JavaScript)]
namespace uComponents.DataTypes.CheckBoxTree
{
	using Umbraco.Web;

	/// <summary>
	/// Data Editor for the CheckBoxTree data-type.
	/// </summary>
	public class CheckBoxTreeDataEditor : CompositeControl, IDataEditor
	{
		/// <summary>
		/// Field for the data.
		/// </summary>
		private IData data;

		/// <summary>
		/// Field for the options.
		/// </summary>
		private CheckBoxTreeOptions options;

		/// <summary>
		/// Field for the tree view.
		/// </summary>
		private TreeView treeView = new TreeView();

		/// <summary>
		/// Field for the selectable nodes.
		/// </summary>
		private Dictionary<int, string> selectableNodes = null;

		/// <summary>
		/// Field for the selected nodes.
		/// </summary>
		private Dictionary<int, string> selectedNodes = null;

		/// <summary>
		/// Field for the minimun selection validator.
		/// </summary>
		private CustomValidator minSelectionCustomValidator = new CustomValidator();

		/// <summary>
		/// Field for the maximun selection validator.
		/// </summary>
		private CustomValidator maxSelectionCustomValidator = new CustomValidator();

		/// <summary>
		/// Gets a value indicating whether [treat as rich text editor].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [treat as rich text editor]; otherwise, <c>false</c>.
		/// </value>
		public virtual bool TreatAsRichTextEditor { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether [show label].
		/// </summary>
		/// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
		public virtual bool ShowLabel { get { return true; } }

		/// <summary>
		/// Gets the editor.
		/// </summary>
		/// <value>The editor.</value>
		public Control Editor { get { return this; } }

		/// <summary>
		/// Gets the selectable nodes.
		/// </summary>
		/// <value>The selectable nodes.</value>
		private Dictionary<int, string> SelectableNodes
		{
			get
			{
				if (this.selectableNodes == null)
				{
					this.selectableNodes = uQuery.GetNodesByXPath(this.options.SelectableTreeNodesXPath).ToNameIds();
				}

				return this.selectableNodes;
			}
		}

		/// <summary>
		/// Gets the selected nodes.
		/// </summary>
		/// <value>The selected nodes.</value>
		private Dictionary<int, string> SelectedNodes
		{
			get
			{
				if (this.selectedNodes == null)
				{
					string value = this.data.Value.ToString();
					var nodes = Helper.Xml.CouldItBeXml(value) ? uQuery.GetNodesByXml(value) : uQuery.GetNodesByCsv(value);
					this.selectedNodes = nodes.ToNameIds();
				}

				return this.selectedNodes;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CheckBoxTreeDataEditor"/> class.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="options">The options.</param>
		internal CheckBoxTreeDataEditor(IData data, CheckBoxTreeOptions options)
		{
			this.data = data;
			this.options = options;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);            
			
			this.minSelectionCustomValidator.ServerValidate += new ServerValidateEventHandler(this.MinSelectionCustomValidator_ServerValidate);
			this.maxSelectionCustomValidator.ServerValidate += new ServerValidateEventHandler(this.MaxSelectionCustomValidator_ServerValidate);


			CmsProperty property = new CmsProperty(((umbraco.cms.businesslogic.datatype.DefaultData)this.data).PropertyId);
			DocumentType documentType = new DocumentType(property.PropertyType.ContentTypeId);
			CmsContentType.TabI tab = documentType.getVirtualTabs.Where(x => x.Id == property.PropertyType.TabId).FirstOrDefault();

			if (tab != null)
			{
				this.minSelectionCustomValidator.ErrorMessage = string.Concat("The ", property.PropertyType.Alias, " field in the ", tab.Caption, " tab requires a minimum of ", this.options.MinSelection.ToString(), " selections<br/>");
				this.maxSelectionCustomValidator.ErrorMessage = string.Concat("The ", property.PropertyType.Alias, " field in the ", tab.Caption, " tab has exceeded the maximum number of selections<br/>");
			}
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			// wrapping div
			HtmlGenericControl div = new HtmlGenericControl("div");

			div.Attributes.Add("class", "check-box-tree");
			div.Attributes.Add("data-auto-selection-option", ((int)this.options.AutoSelectionOption).ToString());

			this.treeView.ShowLines = true;            
			
			div.Controls.Add(this.treeView);
			div.Controls.Add(this.minSelectionCustomValidator);
			div.Controls.Add(this.maxSelectionCustomValidator);

			this.Controls.Add(div);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.EnsureChildControls();

			this.RegisterEmbeddedClientResource("uComponents.DataTypes.CheckBoxTree.CheckBoxTree.css", ClientDependencyType.Css);
			this.RegisterEmbeddedClientResource("uComponents.DataTypes.CheckBoxTree.CheckBoxTree.js", ClientDependencyType.Javascript);

			string startupScript = @"                
				<script language='javascript' type='text/javascript'>
					$(document).ready(function () {

						CheckBoxTree.init(jQuery('div#" + this.treeView.ClientID + @"'));

					});
				</script>";

			ScriptManager.RegisterStartupScript(this, typeof(CheckBoxTreeDataEditor), this.ClientID + "_init", startupScript, false);

			if (!this.Page.IsPostBack)
			{
				Node startNode;
				if (string.IsNullOrEmpty(this.options.StartTreeNodeXPath))
				{
					startNode = uQuery.GetRootNode();
				}
				else
				{
					startNode = uQuery.GetNodesByXPath(this.options.StartTreeNodeXPath).FirstOrDefault();
				}

				if (startNode != null)
				{
					TreeNode startTreeNode = this.GetTreeNode(startNode);

					foreach (Node childNode in startNode.GetChildNodes())
					{
						this.AddTreeNode(startTreeNode, childNode);
					}

					this.treeView.Nodes.Add(startTreeNode);
				}

				// Open branches to selected
				TreeNode parentTreeNode;
				foreach (TreeNode treeNode in this.treeView.CheckedNodes)
				{
					// open all nodes from current to root
					parentTreeNode = treeNode.Parent;

					while (parentTreeNode != null)
					{
						if (this.options.ExpandOption == CheckBoxTreeOptions.ExpandOptions.Selected)
						{
							parentTreeNode.Expanded = true;
						}

						parentTreeNode = parentTreeNode.Parent;
					}
				}

				// check whether to collapse/expand all tree nodes
				switch (this.options.ExpandOption)
				{
					case CheckBoxTreeOptions.ExpandOptions.None:
						this.treeView.CollapseAll();
						break;

					case CheckBoxTreeOptions.ExpandOptions.All:
						this.treeView.ExpandAll();
						break;

					default:
						break;
				}               
			}
		}

		/// <summary>
		/// Adds the tree node.
		/// </summary>
		/// <param name="parentTreeNode">The parent tree node.</param>
		/// <param name="node">The node.</param>
		private void AddTreeNode(TreeNode parentTreeNode, Node node)
		{
			TreeNode treeNode = this.GetTreeNode(node);

			parentTreeNode.ChildNodes.Add(treeNode);

			foreach (Node childNode in node.GetChildNodes())
			{
				this.AddTreeNode(treeNode, childNode);
			}
		}

		/// <summary>
		/// Gets an ASP.NET TreeNode from an Umbraco Node
		/// </summary>
		/// <param name="node">The Umbraco Node.</param>
		/// <returns>an ASP.NET TreeNode</returns>
		private TreeNode GetTreeNode(Node node)
		{
		    var treeNode = new TreeNode { Text = node.Name, Expanded = false, SelectAction = TreeNodeSelectAction.None };

		    if (this.options.ShowTreeIcons)
			{
				var documentType = UmbracoContext.Current.Application.Services.ContentTypeService.GetContentType(node.NodeTypeAlias);

				if (documentType != null)
				{
					treeNode.ImageUrl = "~/umbraco/images/umbraco/" + documentType.Icon;
				}
			}

			// should the current node have a checkbox ? (is there any XPath filter supplied)
			if (string.IsNullOrEmpty(this.options.SelectableTreeNodesXPath) || this.SelectableNodes.ContainsKey(node.Id))
			{
				treeNode.ShowCheckBox = true;
			}

			treeNode.Value = node.Id.ToString();
			treeNode.Checked = this.SelectedNodes.ContainsKey(node.Id);

			return treeNode;
		}

		/// <summary>
		/// Handles the ServerValidate event of the MinSelectionCustomValidator control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		private void MinSelectionCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (this.options.MinSelection > 0 && this.treeView.CheckedNodes.Count < this.options.MinSelection)
			{
				args.IsValid = false;
			}
			else
			{
				args.IsValid = true;
			}
		}

		/// <summary>
		/// Handles the ServerValidate event of the MaxSelectionCustomValidator control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		private void MaxSelectionCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (this.options.MaxSelection > 0 && this.treeView.CheckedNodes.Count > this.options.MaxSelection)
			{
				args.IsValid = false;
			}
			else
			{
				args.IsValid = true;
			}
		}

		/// <summary>
		/// Called by Umbraco when saving the node
		/// </summary>
		public void Save()
		{
			var values = new List<string>();

			foreach (TreeNode checkedTreeNode in this.treeView.CheckedNodes)
			{
				values.Add(checkedTreeNode.Value);
			}

			var comma = Constants.Common.COMMA.ToString();
			var csv = string.Join(comma, values.ToArray());

			if (this.options.OutputFormat == Settings.OutputFormat.XML)
			{
				this.data.Value = Helper.Xml.Split(csv, new[] { comma }, "CheckBoxTree", "nodeId").OuterXml;
			}
			else
			{
				this.data.Value = csv;                
			}
		}
	}
}