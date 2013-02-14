using System;
using System.Linq;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.interfaces;
using umbraco.presentation.templateControls;
using Umbraco.Core;

namespace uComponents.DataTypes.RenderMacro
{
	/// <summary>
	/// The DataEditor for the RenderMacro data-type.
	/// </summary>
	public class RenderMacroDataEditor : Panel, IDataEditor
	{
		/// <summary>
		/// Field for the data.
		/// </summary>
		private IData Data;

		/// <summary>
		/// The RenderMacroOptions for the data-type.
		/// </summary>
		private RenderMacroOptions Options;

		/// <summary>
		/// Gets a value indicating whether to show a label or not.
		/// </summary>
		/// <value>
		///   <c>true</c> if want to show label; otherwise, <c>false</c>.
		/// </value>
		public bool ShowLabel
		{
			get
			{
				return this.Options.ShowLabel;
			}
		}

		/// <summary>
		/// Gets a value indicating whether treat as rich text editor.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if treat as rich text editor; otherwise, <c>false</c>.
		/// </value>
		public bool TreatAsRichTextEditor
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the editor.
		/// </summary>
		public Control Editor
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RenderMacroDataEditor"/> class.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="options">The options.</param>
		public RenderMacroDataEditor(IData data, RenderMacroOptions options)
		{
			this.Data = data;
			this.Options = options;
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public void Save()
		{
			this.Data.Value = this.Options.MacroTag;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// check if a macro (tag) is specified
			if (!string.IsNullOrEmpty(this.Options.MacroTag))
			{
				// get the node for the current page
				var node = uQuery.GetCurrentNode();

				// if the node is null (either document is unpublished, or rendering from outside the content section)
				if (node == null)
				{
					// then get the first child node from the XML content root
					var nodes = uQuery.GetNodesByXPath(string.Concat("descendant::*[@parentID = ", uQuery.RootNodeId, "]")).ToList();
					if (nodes != null && nodes.Count > 0)
					{
						node = nodes[0];
					}
				}

				if (node != null)
				{
					// load the page reference
					var umbPage = new page(node.Id, node.Version);
					HttpContext.Current.Items["pageID"] = node.Id;
					HttpContext.Current.Items["pageElements"] = umbPage.Elements;

					var attr = XmlHelper.GetAttributesFromElement(this.Options.MacroTag);

					if (attr.ContainsKey("macroalias"))
					{
						var macro = new Macro() { Alias = attr["macroalias"].ToString() };

						foreach (var item in attr)
						{
							macro.Attributes.Add(item.Key.ToString(), item.Value.ToString());
						}

						this.Controls.Add(macro);
					}
				}
				else
				{
					this.Controls.Add(new Literal() { Text = "<em>There are no published content nodes (yet), therefore the macro can not be rendered.</em>" });
				}
			}

			base.OnInit(e);
		}
	}
}