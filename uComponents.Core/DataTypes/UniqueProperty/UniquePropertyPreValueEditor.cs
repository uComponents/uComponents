using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core.Shared;
using uComponents.Core.Shared.Extensions;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.Core.DataTypes.UniqueProperty
{
	/// <summary>
	/// The PreValue Editor for the UniqueProperty data-type.
	/// </summary>
	public class UniquePropertyPreValueEditor : Control, IDataPrevalue
	{
		/// <summary>
		/// The underlying base data-type.
		/// </summary>
		private readonly BaseDataType m_DataType;

		/// <summary>
		/// An object to temporarily lock writing to the database.
		/// </summary>
		private static readonly object m_Locker = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="UniquePropertyPreValueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public UniquePropertyPreValueEditor(BaseDataType dataType)
		{
			this.m_DataType = dataType;
		}

		/// <summary>
		/// Gets the editor.
		/// </summary>
		/// <value>The editor.</value>
		public Control Editor
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Gets the selected property alias.
		/// </summary>
		/// <value>The selected property alias.</value>
		public string SelectedPropertyAlias
		{
			get
			{
				var vals = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);

				if (vals.Count >= 1)
				{
					return ((PreValue)vals[0]).Value;
				}
				else
				{
					//return default:
					return string.Empty;
				}
			}
		}

		/// <summary>
		/// Gets or sets the properties text box.
		/// </summary>
		/// <value>The properties text box.</value>
		protected TextBox PropertiesTextBox { get; set; }

		/// <summary>
		/// Saves the data for this instance of the PreValue Editor.
		/// </summary>
		public void Save()
		{
			this.m_DataType.DBType = DBTypes.Nvarchar;

			lock (m_Locker)
			{
				var vals = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);
				if (vals.Count >= 1)
				{
					// update
					((PreValue)vals[0]).Value = this.PropertiesTextBox.Text;
					((PreValue)vals[0]).Save();
				}
				else
				{
					// insert
					PreValue.MakeNew(this.m_DataType.DataTypeDefinitionId, this.PropertiesTextBox.Text);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.EnsureChildControls();

			// Adds the client dependencies.
			this.AddResourceToClientDependency(Settings.PrevalueEditorCssResourcePath, ClientDependency.Core.ClientDependencyType.Css);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!Page.IsPostBack && this.SelectedPropertyAlias.Length > 0)
			{
				this.PropertiesTextBox.Text = this.SelectedPropertyAlias;
			}
		}

		/// <summary>
		/// Creates child controls for this control
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// set-up controls
			this.PropertiesTextBox = new TextBox() { ID = "PropertiesTextBox", CssClass = "guiInputText" };

			// populate the values

			// add controls
			this.Controls.Add(this.PropertiesTextBox);
		}

		/// <summary>
		/// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, Constants.ApplicationName);
			writer.RenderBeginTag(HtmlTextWriterTag.Div); //// start 'uComponents'

			// add property fields
			writer.AddPrevalueRow("Property Alias:", "Please select the property alias of field to check for uniqueness.", this.PropertiesTextBox);

			writer.RenderEndTag(); //// end 'uComponents'
		}
	}
}
