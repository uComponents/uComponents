using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco.interfaces;

namespace uComponents.Controls
{
	/// <summary>
	/// EditProperty server control.
	/// </summary>
	[DefaultProperty("Alias")]
	[ToolboxBitmap(typeof(Constants), Constants.FaviconResourcePath)]
	[ToolboxData("<{0}:EditProperty runat=server Alias= NodeId=></{0}:EditProperty>")]
	public class EditProperty : WebControl
	{
		/// <summary>
		/// Private field to store the data-type.
		/// </summary>
		private IDataType m_DataType;

		/// <summary>
		/// Gets or sets the alias.
		/// </summary>
		/// <value>The alias.</value>
		[DefaultValue("")]
		public string Alias { get; set; }

		/// <summary>
		/// Gets or sets the node id.
		/// </summary>
		/// <value>The node id.</value>
		[DefaultValue("")]
		public string NodeId { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="EditProperty"/> class.
		/// </summary>
		public EditProperty()
			: base(HtmlTextWriterTag.Div)
		{
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (string.IsNullOrWhiteSpace(this.NodeId))
			{
				this.NodeId = uQuery.GetIdFromQueryString();
			}

			// try getting the content from the supplied nodeId.
			var content = this.GetContent(this.NodeId);

			if (content == null)
			{
				// fall-back on the current page.
				content = uQuery.GetCurrentDocument();
			}

			if (content != null)
			{
				var property = content.getProperty(this.Alias);
				if (property != null)
				{
					var propertyType = property.PropertyType;
					var definition = propertyType.DataTypeDefinition;
					var dataType = definition.DataType;

					// set the value and Id of the control
					dataType.DataEditor.Editor.ID = this.Alias;
					dataType.Data.Value = property.Value;

					// store the data-type in a private field for re-use
					this.m_DataType = dataType;

					// add the control to the collection
					this.Controls.Add(dataType.DataEditor.Editor);
				}
			}
		}

		/// <summary>
		/// Saves the property value.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <returns></returns>
		public bool Save(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(this.NodeId))
			{
				this.NodeId = uQuery.GetIdFromQueryString();
			}

			// try getting the content from the supplied Id.
			var content = this.GetContent(this.NodeId);

			if (content == null)
			{
				// fall-back on the current page.
				content = uQuery.GetCurrentDocument();
			}

			if (content != null)
			{
				// get the data-type from  the private field
				var dataType = this.m_DataType;

				// get the property value (from the current node)
				var property = content.getProperty(this.Alias);
				if (property != null)
				{
					// save the value
					dataType.Data.PropertyId = property.Id;
					dataType.DataEditor.Save();

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the content.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		private umbraco.cms.businesslogic.Content GetContent(int id)
		{
			try
			{
				return new umbraco.cms.businesslogic.Content(id);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the content.
		/// </summary>
		/// <param name="nodeId">The node id.</param>
		/// <returns></returns>
		private umbraco.cms.businesslogic.Content GetContent(string nodeId)
		{
			int id;

			if (!string.IsNullOrWhiteSpace(nodeId) && int.TryParse(nodeId, out id))
			{
				return this.GetContent(id);
			}

			return null;
		}
	}
}