using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using uComponents.uQueryExtensions;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using umbraco.NodeFactory;
using CmsContentType = umbraco.cms.businesslogic.ContentType;
using CmsProperty = umbraco.cms.businesslogic.property.Property;

namespace uComponents.DataTypes.CheckBoxTree
{
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
					var nodes = uQuery.Helper.Xml.CouldItBeXml(value) ? uQuery.GetNodesByXml(value) : uQuery.GetNodesByCsv(value);
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

			CmsProperty property = new CmsProperty(((DefaultData)this.data).PropertyId);
			DocumentType documentType = new DocumentType(property.PropertyType.ContentTypeId);
			CmsContentType.TabI tab = documentType.getVirtualTabs.Where(x => x.Id == property.PropertyType.TabId).FirstOrDefault();

			if (tab != null)
			{
				this.minSelectionCustomValidator.ErrorMessage = string.Concat("The ", property.PropertyType.Alias, " field in the ", tab.Caption, " tab requires a minimum of ", this.options.MinSelection.ToString(), " selections<br/>");
				this.maxSelectionCustomValidator.ErrorMessage = string.Concat("The ", property.PropertyType.Alias, " field in the ", tab.Caption, " tab has exceeded the maximum number of selections<br/>");
			}

			if (this.options.SelectAncestors || this.options.SelectDescendents)
			{
				this.treeView.Attributes.Add("onclick", "OnCheckBoxCheckChanged(event)");
			}
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.Controls.Add(this.treeView);
			this.Controls.Add(this.minSelectionCustomValidator);
			this.Controls.Add(this.maxSelectionCustomValidator);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.EnsureChildControls();

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
		public void AddTreeNode(TreeNode parentTreeNode, Node node)
		{
			//// bool recurseChildren = true;

			TreeNode treeNode = this.GetTreeNode(node);

			parentTreeNode.ChildNodes.Add(treeNode);

			// how do we configure this recurse children setting?
			// if (recurseChildren)
			// {
			foreach (Node childNode in node.GetChildNodes())
			{
				this.AddTreeNode(treeNode, childNode);
			}
			// }
		}

		/// <summary>
		/// Gets the tree node.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		private TreeNode GetTreeNode(Node node) //// public static explicit operator TreeNode(Node node) ??
		{
			TreeNode treeNode = new TreeNode();

			treeNode.Text = node.Name;
			treeNode.Expanded = false;
			treeNode.SelectAction = TreeNodeSelectAction.None;

			if (this.options.ShowTreeIcons)
			{
				DocumentType documentType = DocumentType.GetByAlias(node.NodeTypeAlias);
				if (documentType != null)
				{
					treeNode.ImageUrl = "~/umbraco/images/umbraco/" + documentType.IconUrl;
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
				this.data.Value = uQuery.Helper.Xml.Split(csv, new[] { comma }, "CheckBoxTree", "nodeId").OuterXml;
			}
			else
			{
				this.data.Value = csv;
			}
		}

		/// <summary>
		/// Writes the <see cref="T:System.Web.UI.WebControls.CompositeControl"/> content to the specified <see cref="T:System.Web.UI.HtmlTextWriter"/> object, for display on the client.
		/// </summary>
		/// <param name="writer">An <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			this.treeView.RenderControl(writer);

			writer.Write(@"
				<style type='text/css'>
					div#" + this.ClientID + @" .tree td div {
						 height: 20px !important;
					}
				</style>
			");

			writer.Write(@"
				<script type=""text/javascript"">

					function OnCheckBoxCheckChanged(evt) { 
					
						var src = window.event != window.undefined ? window.event.srcElement : evt.target; 
						var isChkBoxClick = (src.tagName.toLowerCase() == 'input' && src.type == 'checkbox'); 
						if (isChkBoxClick) { 

							var parentTable = GetParentByTagName('table', src);
							var nxtSibling = parentTable.nextSibling; 
							if (nxtSibling && nxtSibling.nodeType == 1)//check if nxt sibling is not null & is an element node 
							{ 
								if (nxtSibling.tagName.toLowerCase() == 'div') //if node has children 
								{ 
									if (!src.checked) {
										//uncheck children at all levels 
										CheckUncheckChildren(parentTable.nextSibling, src.checked); 
									}
								} 
							} 
							//check or uncheck parents at all levels 
							CheckUncheckParents(src, src.checked); 
						} 
					} 

					function CheckUncheckChildren(childContainer, check) { 
						var childChkBoxes = childContainer.getElementsByTagName('input'); 
						var childChkBoxCount = childChkBoxes.length; 
						for (var i = 0; i < childChkBoxCount; i++) { 
							childChkBoxes[i].checked = check; 
						} 
					} 

					function CheckUncheckParents(srcChild, check) { 
						var parentDiv = GetParentByTagName('div', srcChild); 
						var parentNodeTable = parentDiv.previousSibling;

						if (parentNodeTable) { 
							var checkUncheckSwitch;

							if (check) //checkbox checked 
							{ 
								checkUncheckSwitch = true;
							} 
							else //checkbox unchecked 
							{ 
								var isAllSiblingsUnChecked = AreAllSiblingsUnChecked(srcChild);
								if (!isAllSiblingsUnChecked) {					
									checkUncheckSwitch = true;
								} else {
									checkUncheckSwitch = false;
								}
							}

							var inpElemsInParentTable = parentNodeTable.getElementsByTagName('input'); 
							if (inpElemsInParentTable.length > 0) { 
								var parentNodeChkBox = inpElemsInParentTable[0]; 
								parentNodeChkBox.checked = checkUncheckSwitch; 
								//do the same recursively 
								CheckUncheckParents(parentNodeChkBox, checkUncheckSwitch); 
							} 
						} 
					} 

					//utility function to get the container of an element by tagname 
					function GetParentByTagName(parentTagName, childElementObj) { 
						var parent = childElementObj.parentNode; 
						while (parent.tagName.toLowerCase() != parentTagName.toLowerCase()) { 
							parent = parent.parentNode; 
						} 
						return parent; 
					} 

				</script>
			");
		}
	}
}