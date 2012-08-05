using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using uComponents.DataTypes.Shared.Extensions;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using umbraco.editorControls;

namespace uComponents.DataTypes.StyledTextBox
{
    /// <summary>
    /// The PreValue Editor for the Styled TextBox data-type.
    /// </summary>
    public class StyledTextBoxPrevalueEditor : PlaceHolder, IDataPrevalue
    {
        /// <summary>
        /// The underlying base data-type.
        /// </summary>
        private readonly umbraco.cms.businesslogic.datatype.BaseDataType m_DataType;

        /// <summary>
        /// An object to temporarily lock writing to the database.
        /// </summary>
        private static readonly object m_Locker = new object();

        /// <summary>
        /// The default width of the textbox.
        /// </summary>
        private const int DEFAULT_WIDTH = 674;

        /// <summary>
        /// The default style of the textbox.
        /// </summary>
        private const string DEFAULT_STYLE = "font-size:larger; font-weight:bold;";

        /// <summary>
        /// Initializes a new instance of the <see cref="StyledTextBoxPrevalueEditor"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        public StyledTextBoxPrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
        {
            this.m_DataType = dataType;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public string Configuration
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
                    return string.Empty;
                }
            }
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
        /// Gets or sets the width text box.
        /// </summary>
        /// <value>The width text box.</value>
        protected TextBox WidthTextBox { get; set; }

        /// <summary>
        /// Gets or sets the style text box.
        /// </summary>
        /// <value>The style text box.</value>
        protected TextBox StyleTextBox { get; set; }

        /// <summary>
        /// Gets or sets the preview text box.
        /// </summary>
        /// <value>The preview text box.</value>
        protected TextBox PreviewTextBox { get; set; }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            this.m_DataType.DBType = umbraco.cms.businesslogic.datatype.DBTypes.Nvarchar;

            lock (m_Locker)
            {
                var vals = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);
                var data = string.Concat(this.WidthTextBox.Text, Constants.Common.COMMA, this.StyleTextBox.Text);

                if (vals.Count >= 1)
                {
                    // update
                    ((PreValue)vals[0]).Value = data;
                    ((PreValue)vals[0]).Save();
                }
                else
                {
                    // insert
                    PreValue.MakeNew(this.m_DataType.DataTypeDefinitionId, data);
                }
            }

            this.SetPreviewTextbox();
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
            this.RegisterEmbeddedClientResource(typeof(DataTypeConstants), Constants.PrevalueEditorCssResourcePath, ClientDependencyType.Css);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (string.IsNullOrEmpty(this.Configuration) == false)
            {
                var settings = new List<string>(this.Configuration.Split(Constants.Common.COMMA));

                if (settings.Count > 1)
                {
                    this.WidthTextBox.Text = settings[0];
                    this.StyleTextBox.Text = string.Join(new string(Constants.Common.COMMA, 1), settings.Skip(1).ToArray());
                }
            }

            this.SetPreviewTextbox();
        }

        /// <summary>
        /// Creates child controls for this control
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.WidthTextBox = new TextBox();
            this.WidthTextBox.ID = "editorialTitleWidth";
            this.WidthTextBox.Width = Unit.Pixel(30);
            this.WidthTextBox.MaxLength = 4;
            this.WidthTextBox.Text = DEFAULT_WIDTH.ToString();

            this.StyleTextBox = new TextBox();
            this.StyleTextBox.ID = "editorialTitleStyle";
            this.StyleTextBox.Width = Unit.Pixel(500);
            this.StyleTextBox.Text = DEFAULT_STYLE;

            this.PreviewTextBox = new TextBox();
            this.PreviewTextBox.ID = "editorialTitlePreview";
            this.PreviewTextBox.Width = Unit.Pixel(DEFAULT_WIDTH);
            this.PreviewTextBox.Attributes.Add("style", DEFAULT_STYLE);
            this.PreviewTextBox.ReadOnly = true;

            this.Controls.Add(this.WidthTextBox);
            this.Controls.Add(this.StyleTextBox);
            this.Controls.Add(this.PreviewTextBox);
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
            writer.AddPrevalueRow("Textbox width:", this.WidthTextBox);
            writer.AddPrevalueRow("Textbox style:", this.StyleTextBox);
            writer.AddPrevalueRow("Preview:", "Hit the 'Save' button to view preview", this.PreviewTextBox);

            writer.RenderEndTag(); //// end 'uComponents'
        }

        /// <summary>
        /// Sets the preview textbox.
        /// </summary>
        private void SetPreviewTextbox()
        {
            this.PreviewTextBox.Width = WidthTextBox.Text != "" ? Unit.Pixel(int.Parse(WidthTextBox.Text)) : DEFAULT_WIDTH;
            this.PreviewTextBox.Attributes.Add("style", StyleTextBox.Text != "" ? StyleTextBox.Text : DEFAULT_STYLE);
            this.PreviewTextBox.Text = "Styled textbox with preview";
        }
    }
}
