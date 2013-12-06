using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco;
using umbraco.NodeFactory;

namespace uComponents.Controls
{
	/// <summary>
	/// RenderTemplate server control.
	/// </summary>
	[DefaultProperty("NodeIds")]
	[ToolboxBitmap(typeof(Constants), Constants.FaviconResourcePath)]
	[ToolboxData("<{0}:RenderTemplate runat=server NodeIds=></{0}:RenderTemplate>")]
	public class RenderTemplate : WebControl
	{
		/// <summary>
		/// Backing field for the <c>uComponents.Controls.RenderTemplate.CurrentPage</c> property.
		/// </summary>
		private int currentPage = 0;

		/// <summary>
		/// Backing field for the <c>uComponents.Controls.RenderTemplate.EntriesPerPage</c> property.
		/// </summary>
		private int entriesPerPage = 0;

		/// <summary>
		/// Backing field for the <c>uComponents.Controls.RenderTemplate.AltTemplateId</c> property.
		/// </summary>
		private int altTemplateId = -1;

		/// <summary>
		/// Backing field for the <c>uComponents.Controls.RenderTemplate.UseChildNodes</c> property.
		/// </summary>
		private bool useChildNodes = false;

		/// <summary>
		/// Gets or sets the current page.
		/// </summary>
		/// <value>The current page.</value>
		[DefaultValue("1")]
		public string CurrentPage
		{
			get
			{
				return this.currentPage.ToString();
			}

			set
			{
				if (!int.TryParse(value, out this.currentPage))
				{
					this.currentPage = 0;
				}
			}
		}

		/// <summary>
		/// Gets or sets the custom property.
		/// </summary>
		/// <value>The custom property.</value>
		[DefaultValue("")]
		public string CustomProperty { get; set; }

		/// <summary>
		/// Gets or sets the doc-type aliases to use the altTemplate.
		/// </summary>
		/// <value>The doc-type aliases to use the altTemplate.</value>
		[DefaultValue("")]
		public string DocTypeAliasesToUseAltTemplate { get; set; }

		/// <summary>
		/// Gets or sets the entries per page.
		/// </summary>
		/// <value>The entries per page.</value>
		[DefaultValue("10")]
		public string EntriesPerPage
		{
			get
			{
				return this.entriesPerPage.ToString();
			}

			set
			{
				if (!int.TryParse(value, out this.entriesPerPage))
				{
					this.entriesPerPage = 0;
				}
			}
		}

		/// <summary>
		/// Gets or sets the node ids.
		/// </summary>
		/// <value>The node ids.</value>
		[DefaultValue("")]
		public string NodeIds { get; set; }

		/// <summary>
		/// Gets or sets the alternative template id, (as an altTemplate).
		/// </summary>
		/// <value>The alternative template id.</value>
		[DefaultValue("-1")]
		public string AltTemplateId
		{
			get
			{
				return this.altTemplateId.ToString();
			}

			set
			{
				if (!int.TryParse(value, out this.altTemplateId))
				{
					this.altTemplateId = -1;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [use child nodes].
		/// </summary>
		/// <value><c>true</c> if [use child nodes]; otherwise, <c>false</c>.</value>
		[DefaultValue("False")]
		public string UseChildNodes
		{
			get
			{
				return this.useChildNodes.ToString();
			}

			set
			{
				if (!bool.TryParse(value, out this.useChildNodes))
				{
					this.useChildNodes = value.Equals("1");
				}
			}
		}

		/// <summary>
		/// Gets or sets the doc-type aliases to filter out if using UseChildNodes.
		/// </summary>
		/// <value>Comma separated doc-type aliases</value>
		[DefaultValue("")]
		public string ExcludeDocTypesForChildNodes { get; set; }

		/// <summary>
		/// Gets or sets the rendered templates.
		/// </summary>
		/// <value>The rendered templates.</value>
		private List<string> RenderedTemplates { get; set; }

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// add custom property
			if (!string.IsNullOrEmpty(this.CustomProperty))
			{
				this.AddCustomProperty("ucCustomProperty", this.CustomProperty);
			}

			// get the nodes.
			var nodes = this.GetNodes();

			// check that there are nodes available
			if (nodes.Count > 0)
			{
				// if 'entries per page' is -1, then display all nodes
				if (this.entriesPerPage <= 0)
				{
					this.entriesPerPage = nodes.Count;
				}

				int take = this.entriesPerPage;
				int skip = (this.currentPage - 1) * take;

				// intialise the output collection.
				if (this.RenderedTemplates == null)
				{
					this.RenderedTemplates = new List<string>(take);
				}

				// loop through each node (with pagination)
				foreach (var node in nodes.Skip(skip).Take(take))
				{
					// make sure that the node is real
					if (node.Id > 0)
					{
						// get the default template id
						int templateId = node.template;

						// if the altTemplate parameters have been set ...
						if (this.altTemplateId > 0 && !string.IsNullOrEmpty(this.DocTypeAliasesToUseAltTemplate))
						{
							// ... compare the doc-type alias ...
							if (this.DocTypeAliasesToUseAltTemplate.Contains(node.NodeTypeAlias))
							{
								// ... and use the altTemplate (id)
								templateId = this.altTemplateId;
							}
						}

						// check that the template is associated with a node (non-associated templates are id = 0)
						if (templateId > 0)
						{
							// render the template for the node
							string renderedTemplate = library.RenderTemplate(node.Id, templateId);

							// check that the rendered template isn't empty
							if (!string.IsNullOrEmpty(renderedTemplate))
							{
								// add the rendered template as a LiteralControl
								this.RenderedTemplates.Add(renderedTemplate);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			this.RenderContents(writer);
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			if (this.RenderedTemplates != null)
			{
				foreach (var renderedTemplate in this.RenderedTemplates)
				{
					writer.WriteLine(renderedTemplate);
				}
			}
		}

		/// <summary>
		/// Adds the custom property.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		private void AddCustomProperty(string key, object value)
		{
			if (!HttpContext.Current.Items.Contains(key))
			{
				HttpContext.Current.Items.Add(key, value);
			}
		}

		/// <summary>
		/// Gets the nodes.
		/// </summary>
		/// <returns>Returns a list of nodes.</returns>
		private List<Node> GetNodes()
		{
			var currentNode = uQuery.GetCurrentNode();
			var nodes = new List<Node>();

			// add the current node id to a HttpItem - in case of reference
			if (currentNode != null)
			{
				this.AddCustomProperty("umbCallingPageId", currentNode.Id);
			}

			// checks whether to use the child nodes.
			if (this.useChildNodes)
			{
				// get list of doc-types to exclude
				var excludedDocTypes = new List<string>();
				if (!string.IsNullOrEmpty(this.ExcludeDocTypesForChildNodes))
				{
					excludedDocTypes.AddRange(this.ExcludeDocTypesForChildNodes.Split(Constants.Common.COMMA).Select(s => s.Trim()));
				}

				if (currentNode != null)
				{
					// loop through each child node
					foreach (Node child in currentNode.Children)
					{
						// test if the doc-type should be excluded
						if (!excludedDocTypes.Contains(child.NodeTypeAlias))
						{
							// add to list
							nodes.Add(child);
						}
					}
				}
			}

			// check if there are any node ids.
			if (!string.IsNullOrEmpty(this.NodeIds))
			{
				// gets the nodes from the CSV list.
				nodes.AddRange(uQuery.GetNodesByCsv(this.NodeIds));
			}

			return nodes;
		}
	}
}