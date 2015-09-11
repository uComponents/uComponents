using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.editorControls;
using umbraco.editorControls.SettingControls.Pickers;

namespace uComponents.DataTypes.XmlDropDownList
{
	/// <summary>
	/// The PreValue Editor for the XmlDropDownList data-type.
	/// </summary>
	public class XmlDropDownListPrevalueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
	{
		/// <summary>
		/// The PathPicker control for the XML File Path of the control.
		/// </summary>
		private PathPicker PathPickerXmlFilePath;

		/// <summary>
		/// The TextBox control for the XPath Expression of the control.
		/// </summary>
		private TextBox TextBoxXPathExpression;

		/// <summary>
		/// The TextBox control for the Text column of the control.
		/// </summary>
		private TextBox TextBoxTextColumn;

		/// <summary>
		/// The TextBox control for the Value column of the control.
		/// </summary>
		private TextBox TextBoxValueColumn;

		/// <summary>
		/// Gets the documentation URL.
		/// </summary>
		public override string DocumentationUrl
		{
			get
			{
				return string.Concat(base.DocumentationUrl, "/data-types/xml-dropdownlist/");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlDropDownListPrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">
		/// Type of the data.
		/// </param>
		public XmlDropDownListPrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
			: base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
		{
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public override void Save()
		{
			// set the options
			var options = new XmlDropDownListOptions(true);

			// save the options
			options.XmlFilePath = this.PathPickerXmlFilePath.Value;
			options.XPathExpression = this.TextBoxXPathExpression.Text;
			options.TextColumn = this.TextBoxTextColumn.Text;
			options.ValueColumn = this.TextBoxValueColumn.Text;

			// save the options as JSON
			this.SaveAsJson(options);
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// set-up child controls
			this.PathPickerXmlFilePath = new PathPicker() { ID = "XmlFilePath" };
			this.TextBoxXPathExpression = new TextBox() { ID = "XPathExpression", CssClass = "guiInputText guiInputStandardSize" };
			this.TextBoxTextColumn = new TextBox() { ID = "TextColumn", CssClass = "guiInputText guiInputStandardSize" };
			this.TextBoxValueColumn = new TextBox() { ID = "ValueColumn", CssClass = "guiInputText guiInputStandardSize" };

			// add the child controls
			this.Controls.AddPrevalueControls(this.PathPickerXmlFilePath, this.TextBoxXPathExpression, this.TextBoxTextColumn, this.TextBoxValueColumn);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// get PreValues, load them into the controls.
			var options = this.GetPreValueOptions<XmlDropDownListOptions>();

			// no options? use the default ones.
			if (options == null)
			{
				options = new XmlDropDownListOptions(true);
			}

			// set the values
			this.PathPickerXmlFilePath.Value = options.XmlFilePath;
			this.TextBoxXPathExpression.Text = options.XPathExpression;
			this.TextBoxTextColumn.Text = options.TextColumn;
			this.TextBoxValueColumn.Text = options.ValueColumn;
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddPrevalueRow("XML File Path:", "Specify the path to the XML file.", this.PathPickerXmlFilePath);
			writer.AddPrevalueRow("XPath Expression:", "The XPath expression to select the nodes used in the XML file.", this.TextBoxXPathExpression);
			writer.AddPrevalueRow("Text column:", "The name (or XPath expression) of the field used for the item's display text.", this.TextBoxTextColumn);
			writer.AddPrevalueRow("Value column:", "The name (or XPath expression) of the field used for the item's value.", this.TextBoxValueColumn);
		}
	}
}