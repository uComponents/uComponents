using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco.editorControls;
using uComponents.DataTypes.Shared.macroRenderings;

namespace uComponents.DataTypes.ImagePoint
{
    /// <summary>
    /// Prevalue Editor for the Image Point data-type.
    /// </summary>
    public class ImagePointPreValueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
    {
        /// <summary>
        /// Prepopulated Umbraco Propery Picker
        /// </summary>
        private propertyTypePicker imagePropertyAliasPicker = new propertyTypePicker();

        /// <summary>
        /// Optional width
        /// </summary>
        private TextBox widthTextBox = new TextBox();

        /// <summary>
        /// Optional height
        /// </summary>
        private TextBox heightTextBox = new TextBox();

        /// <summary>
        /// Data object used to define the configuration status of this PreValueEditor
        /// </summary>
        private ImagePointOptions options = null;

        /// <summary>
        /// When checked, all other points that use the same image are rendered as ghost points
        /// (chose the word neighbouring, as may be siblings / descendents / ancestors etc...)
        /// </summary>
        private CheckBox showNeighboursCheckBox = new CheckBox();

        /// <summary>
        /// Gets the documentation URL.
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return string.Concat(base.DocumentationUrl, "/data-types/image-point/");
            }
        }

        /// <summary>
        /// Gets the options data object that represents the current state of this datatypes configuration
        /// </summary>
        internal ImagePointOptions Options
        {
            get
            {
                if (this.options == null)
                {
                    // Deserialize any stored settings for this PreValueEditor instance
                    this.options = this.GetPreValueOptions<ImagePointOptions>();

                    // If still null, ie, object couldn't be de-serialized from PreValue[0] string value
                    if (this.options == null)
                    {
                        // Create a new Options data object with the default values
                        this.options = new ImagePointOptions();
                    }
                }

                return this.options;
            }
        }

        /// <summary>
        /// Initialize a new instance of ImagePointPreValueEditor
        /// </summary>
        /// <param name="dataType">ImagePointDataType</param>
        public ImagePointPreValueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
            : base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Nvarchar)
        {
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            this.imagePropertyAliasPicker.ID = "imagePropertyAliasPicker";

            this.widthTextBox.ID = "widthTextBox";
            this.widthTextBox.Width = 30;
            this.widthTextBox.MaxLength = 4;

            this.heightTextBox.ID = "heightTextBox";
            this.heightTextBox.Width = 30;
            this.heightTextBox.MaxLength = 4;

            this.showNeighboursCheckBox.ID = "showNeighboursCheckBox";

            this.Controls.AddPrevalueControls(
                this.imagePropertyAliasPicker,
                this.widthTextBox,
                this.heightTextBox,
                this.showNeighboursCheckBox);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.Page.IsPostBack)
            {
                // Read in stored configuration values
                if (this.imagePropertyAliasPicker.Items.Contains(new ListItem(this.Options.ImagePropertyAlias)))
                {
                    this.imagePropertyAliasPicker.SelectedValue = this.Options.ImagePropertyAlias;
                }

                this.widthTextBox.Text = this.Options.Width.ToString();
                this.heightTextBox.Text = this.Options.Height.ToString();
                this.showNeighboursCheckBox.Checked = this.Options.ShowNeighbours;
            }
        }

        /// <summary>
        /// Saves the pre value data to Umbraco
        /// </summary>
        public override void Save()
        {
            if (this.Page.IsValid)
            {
                this.Options.ImagePropertyAlias = this.imagePropertyAliasPicker.SelectedValue;

                int width;
                int.TryParse(this.widthTextBox.Text, out width);
                this.Options.Width = width;

                int height;
                int.TryParse(this.heightTextBox.Text, out height);
                this.Options.Height = height;

                this.Options.ShowNeighbours = this.showNeighboursCheckBox.Checked;

                this.SaveAsJson(this.Options);  // Serialize to Umbraco database field
            }
        }

        /// <summary>
        /// Used to remove styling from the built in propertyTypePicker 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            this.imagePropertyAliasPicker.CssClass = string.Empty; // Remove guiInputTextStandard 

            // Sort properties in the built in property picker control
            ListItem[] imagePropertyAliasListItems = this.imagePropertyAliasPicker.Items.Cast<ListItem>().OrderBy(x => x.Text).ToArray();

            this.imagePropertyAliasPicker.Items.Clear();
            this.imagePropertyAliasPicker.Items.AddRange(imagePropertyAliasListItems);
        }

        /// <summary>
        /// Replaces the base class writer and instead uses the shared uComponents extension method, to inject consistant markup
        /// </summary>
        /// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddPrevalueRow("Image Property Alias", "(recursive) property to use as source for image", this.imagePropertyAliasPicker);
            writer.AddPrevalueRow("Width", "width in px (0 = calculate from image)", this.widthTextBox);
            writer.AddPrevalueRow("Height", "height in px (0 = calculate from image)", this.heightTextBox);
            writer.AddPrevalueRow("Show Neighbours", "show other points using this datatype, with the same image", this.showNeighboursCheckBox);
        }
    }
}