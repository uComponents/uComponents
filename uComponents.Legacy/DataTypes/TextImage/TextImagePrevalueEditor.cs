using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.media;
using umbraco.editorControls;
using umbraco.interfaces;
using BaseDataType = umbraco.cms.businesslogic.datatype.BaseDataType;
using ClientDependencyType = ClientDependency.Core.ClientDependencyType;
using DBTypes = umbraco.cms.businesslogic.datatype.DBTypes;
using Image = System.Web.UI.WebControls.Image;

namespace uComponents.DataTypes.TextImage
{
    /// <summary>
    ///   TextImagePrevalueEditor
    /// </summary>
    [ClientDependency.Core.ClientDependency(ClientDependencyType.Javascript, "ui/jqueryui.js", "UmbracoClient")]
    public class TextImagePrevalueEditor : Control, IDataPrevalue
    {
        #region Fields

        private readonly BaseDataType _dataType;
        private DataExtractor _dataExtractor;
        private string _errorMessage = string.Empty;
        private bool _hasError;
        private SortedList _preValues;

        #endregion

        #region Controls

        /// <summary>
        /// </summary>
        protected HtmlInputText BackgroundColorPicker;

        /// <summary>
        /// </summary>
        protected mediaChooser BackgroundMediaChooser;

        /// <summary>
        /// </summary>
        protected TextBox CustomFontPathTextBox;

        /// <summary>
        /// </summary>
        protected DropDownList FontNameDropDownList;

        /// <summary>
        /// </summary>
        protected TextBox FontSizeTextBox;

        /// <summary>
        /// </summary>
        protected CheckBoxList FontStyleCheckBoxList;

        /// <summary>
        /// </summary>
        protected HtmlInputText ForegroundColorPicker;

        /// <summary>
        /// </summary>
        protected DropDownList HorizontalAlignmentDropDownList;

        /// <summary>
        /// </summary>
        protected DropDownList ImageFormatDropDownList;

        /// <summary>
        /// </summary>
        protected TextBox ImageHeightTextBox;

        /// <summary>
        /// </summary>
        protected Image ImagePreview;

        /// <summary>
        /// </summary>
        protected TextBox ImageWidthTextBox;

        /// <summary>
        /// </summary>
        protected HtmlInputText ShadowColorPicker;

        /// <summary>
        /// </summary>
        protected DropDownList VerticalAlignmentDropDownList;

        #endregion

        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TextImagePrevalueEditor" /> class.
        /// </summary>
        /// <param name = "dataType">Type of the data.</param>
        public TextImagePrevalueEditor(BaseDataType dataType)
        {
            _dataType = dataType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the name of the font.
        /// </summary>
        /// <value>The name of the font.</value>
        public string FontName
        {
            get
            {
                try
                {
                    return GetPreValue(0);
                }
                catch
                {
                    return "ARIAL";
                }
            }
        }

        /// <summary>
        ///   Gets the custom font path.
        /// </summary>
        /// <value>The custom font path.</value>
        public string CustomFontPath
        {
            get
            {
                try
                {
                    return GetPreValue(1);
                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        ///   Gets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public int FontSize
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetPreValue(2));
                }
                catch
                {
                    return 12;
                }
            }
        }

        /// <summary>
        ///   Gets the font style.
        /// </summary>
        /// <value>The font style.</value>
        public FontStyle[] FontStyles
        {
            get
            {
                try
                {
                    var fontStyleStrings = GetPreValue(3).Split(new[] {','});
                    return
                        fontStyleStrings.Select(
                            fontStyleString => (FontStyle) Enum.Parse(typeof (FontStyle), fontStyleString)).ToArray();
                }
                catch
                {
                    return new[] {FontStyle.Regular};
                }
            }
        }

        /// <summary>
        ///   Gets the color of the foreground (font color).
        /// </summary>
        /// <value>The color of the foreground.</value>
        public string ForegroundColor
        {
            get
            {
                try
                {
                    return GetPreValue(4);
                }
                catch
                {
                    return "#000000";
                }
            }
        }

        /// <summary>
        ///   Gets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public string BackgroundColor
        {
            get
            {
                try
                {
                    return GetPreValue(5);
                }
                catch
                {
                    return "#FFFFFF";
                }
            }
        }

        /// <summary>
        ///   Gets the color of the shadow.
        /// </summary>
        /// <value>The color of the shadow.</value>
        public string ShadowColor
        {
            get
            {
                try
                {
                    return GetPreValue(6);
                }
                catch
                {
                    return "transparent";
                }
            }
        }

        /// <summary>
        ///   Gets the horizontal alignment.
        /// </summary>
        /// <value>The horizontal alignment.</value>
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                try
                {
                    return (HorizontalAlignment) Enum.Parse(typeof (HorizontalAlignment), GetPreValue(7));
                }
                catch
                {
                    return HorizontalAlignment.Center;
                }
            }
        }

        /// <summary>
        ///   Gets the vertical alignment.
        /// </summary>
        /// <value>The vertical alignment.</value>
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                try
                {
                    return (VerticalAlignment) Enum.Parse(typeof (VerticalAlignment), GetPreValue(8));
                }
                catch
                {
                    return VerticalAlignment.Center;
                }
            }
        }

        /// <summary>
        ///   Gets the height of the image.
        /// </summary>
        /// <value>The height of the image.</value>
        public int ImageHeight
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetPreValue(9));
                }
                catch
                {
                    return -1;
                }
            }
        }

        /// <summary>
        ///   Gets the width of the image.
        /// </summary>
        /// <value>The width of the image.</value>
        public int ImageWidth
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetPreValue(10));
                }
                catch
                {
                    return -1;
                }
            }
        }

        /// <summary>
        ///   Gets the background media id.
        /// </summary>
        /// <value>The background media id.</value>
        private int BackgroundMediaId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(GetPreValue(11));
                }
                catch
                {
                    return -1;
                }
            }
        }

        /// <summary>
        ///   Gets the background image.
        /// </summary>
        /// <value>The root media id.</value>
        public Media BackgroundMedia
        {
            get
            {
                try
                {
                    return new Media(BackgroundMediaId);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///   Gets the output format.
        /// </summary>
        /// <value>The output format.</value>
        public OutputFormat OutputFormat
        {
            get
            {
                try
                {
                    return (OutputFormat) Enum.Parse(typeof (OutputFormat), GetPreValue(12));
                }
                catch
                {
                    return OutputFormat.Png;
                }
            }
        }

        /// <summary>
        ///   Gets the sample image URL.
        /// </summary>
        /// <value>The sample image URL.</value>
        protected string ImagePreviewUrl
        {
            get
            {
                try
                {
                    return GetPreValue(13);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        ///   Gets the editor pre values.
        /// </summary>
        /// <value>The editor pre values.</value>
        private SortedList EditorPreValues
        {
            get { return _preValues ?? (_preValues = PreValues.GetPreValues(_dataType.DataTypeDefinitionId)); }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Gets the pre value.
        /// </summary>
        /// <param name = "index">The index.</param>
        /// <returns></returns>
        private string GetPreValue(int index)
        {
            return ((PreValue) EditorPreValues[index]).Value;
        }

        /// <summary>
        ///   Inserts the value.
        /// </summary>
        /// <param name = "index">The index.</param>
        /// <param name = "value">The value.</param>
        /// <returns></returns>
        private void UpdatePreValue(int index, string value)
        {
            if (EditorPreValues.Count >= index + 1)
            {
                //update
                var preValue = (PreValue) EditorPreValues[index];
                preValue.Value = value;
                preValue.Save();
            }
            else
            {
                //insert
                var preValue = PreValue.MakeNew(_dataType.DataTypeDefinitionId, value);
                preValue.Save();
            }
            return;
        }

        #endregion

        #region Implementation of IDataPrevalue

        /// <summary>
        ///   Saves this instance.
        /// </summary>
        public void Save()
        {
            BackgroundMediaChooser.Save();

            _dataType.DBType = DBTypes.Ntext;

            UpdatePreValue(0, FontNameDropDownList.SelectedItem.Text);
            UpdatePreValue(1, CustomFontPathTextBox.Text);
            UpdatePreValue(2, FontSizeTextBox.Text);
            var commaSepratedList = string.Join(",",
                                                (from ListItem listItem in FontStyleCheckBoxList.Items
                                                 where listItem.Selected
                                                 select listItem.Value).ToArray());
            UpdatePreValue(3, commaSepratedList);
            UpdatePreValue(4, ForegroundColorPicker.Value);
            UpdatePreValue(5, BackgroundColorPicker.Value);
            UpdatePreValue(6, ShadowColorPicker.Value);
            UpdatePreValue(7, HorizontalAlignmentDropDownList.SelectedItem.Text);
            UpdatePreValue(8, VerticalAlignmentDropDownList.SelectedItem.Text);
            UpdatePreValue(9, ImageHeightTextBox.Text);
            UpdatePreValue(10, ImageWidthTextBox.Text);
            UpdatePreValue(11, BackgroundMediaChooser.Value);
            UpdatePreValue(12, ImageFormatDropDownList.SelectedItem.Text);

            var sampleText = FontName + " " + FontSize + " " + OutputFormat;
            var imageParameters = new TextImageParameters(sampleText,
                                                          OutputFormat,
                                                          CustomFontPath,
                                                          FontName,
                                                          FontSize,
                                                          FontStyles,
                                                          ForegroundColor,
                                                          BackgroundColor,
                                                          ShadowColor,
                                                          HorizontalAlignment, VerticalAlignment, ImageHeight,
                                                          ImageWidth, BackgroundMedia);

            var imageUrl = MediaHelper.SaveTextImage(imageParameters, "Preview" + Page.Request["id"]);

            UpdatePreValue(13, imageUrl);
        }

        /// <summary>
        ///   Gets the editor.
        /// </summary>
        /// <value>The editor.</value>
        public Control Editor
        {
            get { return this; }
        }

        /// <summary>
        ///   Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        private void SetErrorMessage(Exception exception)
        {
            if (exception == null)
            {
                _hasError = false;
                _errorMessage = "";
            }
            else
            {
                _hasError = true;
                _errorMessage += string.Format("<p><b>{0}</b></p><p>{1}</p><p>{2}</p>", exception.Message,
                                               exception.StackTrace, exception.TargetSite);
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        ///   Override on init to ensure child controls
        /// </summary>
        /// <param name = "e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();

            this.AddJsTextImageClientDependencies();
        }

        /// <summary>
        ///   Called by the ASP.NET page framework to notify server controls that use
        ///   composition-based implementation to create any child controls they contain 
        ///   in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            try
            {
                // Custom Font Path
                CustomFontPathTextBox = new TextBox {ID = "txtCustomFont", CssClass = "umbEditorTextField"};

                // Fonts DropDownList
                FontNameDropDownList = new DropDownList {ID = "ddlFontName", CssClass = "umbEditorDropDownList"};
                FontNameDropDownList.Items.AddRange(DropDownItems.Fonts);

                // Font Size TextBox
                FontSizeTextBox = new TextBox {ID = "txtFontSize", CssClass = "umbEditorNumberField"};

                // Font Style CheckBoxList
                FontStyleCheckBoxList = new CheckBoxList
                                            {ID = "cblFontStyle", RepeatColumns = 2, CssClass = "umbEditorDropDownList"};
                FontStyleCheckBoxList.Items.AddRange(DropDownItems.FontStyles);

                // Fore Color Picker
                ForegroundColorPicker = new HtmlInputText {ID = "cpForeColor"};
                ForegroundColorPicker.Attributes.Add("type", "color");
                ForegroundColorPicker.Attributes.Add("data-hex", "true");

                // Back Color Picker
                BackgroundColorPicker = new HtmlInputText {ID = "cpBackColor"};
                BackgroundColorPicker.Attributes.Add("type", "color");
                BackgroundColorPicker.Attributes.Add("data-hex", "true");

                // Shadow Color Picker
                ShadowColorPicker = new HtmlInputText {ID = "cpShadowColor"};
                ShadowColorPicker.Attributes.Add("type", "color");
                ShadowColorPicker.Attributes.Add("data-hex", "true");

                // Horizontal Alignment DropDownList
                HorizontalAlignmentDropDownList = new DropDownList
                                                      {ID = "ddlAlignHorizontal", CssClass = "umbEditorDropDownList"};
                HorizontalAlignmentDropDownList.Items.AddRange(DropDownItems.HAlignments);

                // Vertical Alignment DropDownList
                VerticalAlignmentDropDownList = new DropDownList
                                                    {ID = "ddlAlignVertical", CssClass = "umbEditorDropDownList"};
                VerticalAlignmentDropDownList.Items.AddRange(DropDownItems.VAlignments);

                // Area Height TextBox
                ImageHeightTextBox = new TextBox {ID = "txtImageHeight", CssClass = "umbEditorNumberField"};

                // Area Width TextBox
                ImageWidthTextBox = new TextBox {ID = "txtImageWidth", CssClass = "umbEditorNumberField"};

                // Background picker
                _dataExtractor = new DataExtractor {Value = BackgroundMediaId};
                BackgroundMediaChooser = new mediaChooser(_dataExtractor, true, true) {ID = "backgroundMedia"};

                // Image Format Picker
                ImageFormatDropDownList = new DropDownList {ID = "ddlImageFormat", CssClass = "umbEditorDropDownList"};
                ImageFormatDropDownList.Items.AddRange(DropDownItems.OutputFormats);

                // Preview Image
                ImagePreview = new Image
                                   {BorderColor = Color.LightGray, BorderStyle = BorderStyle.Dashed, BorderWidth = 1};

                // Add Controls to Editor
                Controls.Add(FontNameDropDownList);
                Controls.Add(CustomFontPathTextBox);
                Controls.Add(FontSizeTextBox);
                Controls.Add(FontStyleCheckBoxList);
                Controls.Add(ForegroundColorPicker);
                Controls.Add(BackgroundColorPicker);
                Controls.Add(ShadowColorPicker);
                Controls.Add(HorizontalAlignmentDropDownList);
                Controls.Add(VerticalAlignmentDropDownList);
                Controls.Add(ImageHeightTextBox);
                Controls.Add(ImageWidthTextBox);
                Controls.Add(BackgroundMediaChooser);
                Controls.Add(ImageFormatDropDownList);
                Controls.Add(ImagePreview);
            }
            catch (Exception exception)
            {
                SetErrorMessage(exception);
            }
        }

        /// <summary>
        ///   Raises the <see cref = "E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name = "e">The <see cref = "T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                // Assign field values
                FontNameDropDownList.SelectedValue = FontName;
                CustomFontPathTextBox.Text = CustomFontPath;
                FontSizeTextBox.Text = FontSize.ToString();
                foreach (var fontStyle in FontStyles.Where(fontStyle => fontStyle != FontStyle.Regular))
                {
                    FontStyleCheckBoxList.Items.FindByText(fontStyle.ToString()).Selected = true;
                }
                ForegroundColorPicker.Value = ForegroundColor;
                BackgroundColorPicker.Value = BackgroundColor;
                ShadowColorPicker.Value = ShadowColor;
                HorizontalAlignmentDropDownList.SelectedValue = HorizontalAlignment.ToString();
                VerticalAlignmentDropDownList.SelectedValue = VerticalAlignment.ToString();
                ImageHeightTextBox.Text = ImageHeight.ToString();
                ImageWidthTextBox.Text = ImageWidth.ToString();
                _dataExtractor.Value = BackgroundMediaId;
                BackgroundMediaChooser.Value = _dataExtractor.Value.ToString();
                ImageFormatDropDownList.SelectedValue = OutputFormat.ToString();

                // Image Preview
                //var image = System.Drawing.Image.FromFile(IOHelper.MapPath(ImagePreviewUrl));
                ImagePreview.Height = ImageHeight < 1 ? Unit.Empty : Unit.Pixel(ImageHeight);
                ImagePreview.Width = ImageWidth < 1 ? Unit.Empty : Unit.Pixel(ImageWidth);
                ImagePreview.ImageUrl = ImagePreviewUrl;
            }
            catch (Exception exception)
            {
                SetErrorMessage(exception);
            }
        }

        /// <summary>
        ///   Sends server control content to a provided <see cref = "T:System.Web.UI.HtmlTextWriter" /> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name = "writer">The <see cref = "T:System.Web.UI.HtmlTextWriter" /> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (_hasError)
            {
                writer.Write(_errorMessage);
                SetErrorMessage(null);
                return;
            }

			writer.AddAttribute(HtmlTextWriterAttribute.Class, Constants.ApplicationName);
            writer.RenderBeginTag(HtmlTextWriterTag.Div); //start 'uComponents'

            writer.AddPrevalueRow("Server Font", FontNameDropDownList);
            writer.AddPrevalueRow("Custom Font Path",
                                  "Relative path to font file; overrides server font<br/>(example: '<strong>/fonts/customfont.ttf</strong>' )",
                                  CustomFontPathTextBox);
            writer.AddPrevalueRow("Font Size", FontSizeTextBox);
            writer.AddPrevalueRow("Font Style", FontStyleCheckBoxList);
            writer.AddPrevalueRow("Foreground Color", ForegroundColorPicker);
            writer.AddPrevalueRow("Background Color", BackgroundColorPicker);
            writer.AddPrevalueRow("Shadow Color", ShadowColorPicker);
            writer.AddPrevalueRow("Horizontal Alignment", HorizontalAlignmentDropDownList);
            writer.AddPrevalueRow("Vertical Alignment", VerticalAlignmentDropDownList);
            writer.AddPrevalueRow("Height", "Enter -1 for auto height", ImageHeightTextBox);
            writer.AddPrevalueRow("Width", "Enter -1 for auto width", ImageWidthTextBox);
            writer.AddPrevalueRow("Background",
                                  "Dimensions of selected background image will override the height and width values",
                                  BackgroundMediaChooser);
            writer.AddPrevalueRow("Output Format", ImageFormatDropDownList);
            writer.AddPrevalueRow("Preview", "NOTE: Border is for preview only and will not show in generated text",
                                  ImagePreview);
            writer.RenderEndTag(); //end 'uComponents'  

            // Color picker script
            //writer.RenderBeginTag(HtmlTextWriterTag.Script);
            //writer.Write("$(window).load(function(){$('.colorpicker').simpleColor({boxWidth:50,boxHeight:15,displayColorCode:true});});");
            //writer.RenderEndTag();
        }

        #endregion
    }
}