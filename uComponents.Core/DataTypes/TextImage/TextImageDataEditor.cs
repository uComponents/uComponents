using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace uComponents.Core.DataTypes.TextImage
{

    #region Referenced Namespaces

    #endregion

    /// <summary>
    ///   TextImage Control
    /// </summary>
    public class TextImageDataEditor : Control, INamingContainer
    {
        #region TextChanged event

        /// <summary>
        /// TextChangedHandler delegate
        /// </summary>
        public delegate void TextChangedHandler(object sender, EventArgs e);

        /// <summary>
        /// Occurs when the text is changed
        /// </summary>
        public event TextChangedHandler TextChanged;

        #endregion

        #region Delete Handler

        /// <summary>
        ///   DeleteHandler delegate
        /// </summary>
        public delegate void DeleteHandler(object sender, EventArgs e);

        /// <summary>
        ///   Occurs when the image is deleted.
        /// </summary>
        public event DeleteHandler Delete;

        #endregion

        #region Controls

        private LinkButton _deleteButton;
        private Image _image;
        private TextBox _textBox;
        private UpdatePanel _updatePanel;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the text.
        /// </summary>
        /// <value>The text.</value>
        internal string Text
        {
            get { return _textBox.Text; }
            set { _textBox.Text = value; }
        }

        /// <summary>
        ///   Gets or sets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        public string ImageUrl
        {
            get { return _image.ImageUrl; }
            set
            {
                _image.ImageUrl = value;

                if (_image.ImageUrl == string.Empty)
                {
                    _deleteButton.Visible = false;
                    _image.Visible = false;
                }
                else
                {
                    _deleteButton.Visible = true;
                    _image.Visible = true;
                }
            }
        }

        /// <summary>
        ///   Gets the XML value.
        /// </summary>
        /// <value>The XML value.</value>
        public XDocument XmlValue
        {
            get
            {
                if (_textBox.Text == string.Empty)
                {
                    _deleteButton.Visible = false;
                    _image.Visible = false;
                    return null;
                }

                return
                    new XDocument(new XElement("TextImage", new XElement("Text", Text), new XElement("Url", ImageUrl)));
            }
            set
            {
                if (value == null)
                {
                    Text = string.Empty;
                    ImageUrl = string.Empty;
                    return;
                }

                Text = value.Descendants().Elements("Text").First().Value;
                ImageUrl = value.Descendants().Elements("Url").First().Value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Deletes the image.
        /// </summary>
        private void DeleteImage()
        {
            if (_image.ImageUrl == string.Empty) return;
            File.Delete(HttpContext.Current.Server.MapPath(_image.ImageUrl));
        }

        /// <summary>
        ///   Saves the image using the specified parameters
        /// </summary>
        /// <param name = "parameters">The parameters.</param>
        public void SaveImage(TextImageParameters parameters)
        {
            //Save image and set the url
            var imageName = string.Format("{0}-{1}-{2}", MediaHelper.CurrentNode.Id, Guid.NewGuid(), _textBox.Text);
            var imageUrl = MediaHelper.SaveTextImage(parameters, imageName);

            // Delete previous image
            if (ImageUrl != imageUrl && ImageUrl != string.Empty)
            {
                DeleteImage();
            }

            ImageUrl = imageUrl;

            if (Text == string.Empty)
            {
                DeleteImage();
            }
        }

        #endregion

        #region Event Methods

        /// <summary>
        ///   Handles the Click event of the _deleteButton control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            _deleteButton.Visible = false;
            _image.Visible = false;

            DeleteImage();
            Delete.Invoke(sender, e);
        }

        /// <summary>
        ///   Raises the <see cref = "E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name = "e">An <see cref = "T:System.EventArgs" /> object that contains the event data.</param>
        /// <exception cref = "T:System.InvalidOperationException">The <see cref = "P:System.Web.UI.UpdatePanel.ContentTemplate" /> property is being defined when the <see cref = "P:System.Web.UI.UpdatePanel.ContentTemplateContainer" /> property has already been created.</exception>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EnsureChildControls();
        }

        /// <summary>
        ///   Called by the ASP.NET page framework to notify server controls that use
        ///   composition-based implementation to create any child controls they contain
        ///   in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            EnsureChildControls();

            // Instantiate controls
            _updatePanel = new UpdatePanel { ID = "tmUpdatePanel", UpdateMode = UpdatePanelUpdateMode.Always };

            _textBox = new TextBox { ID = "tmTextBox", CssClass = "umbEditorTextField" };
            //_textBox.TextChanged += TextBoxText_Changed;

            _deleteButton = new LinkButton { ID = "tmDeleteButon", Text = @"Delete...", Visible = false };
            _deleteButton.Attributes.Add("onclick", "return confirm('Are you sure you want to delete the image?');");
            _deleteButton.Click += DeleteButton_Click;

            _image = new Image { Visible = false };
            _image.Attributes.CssStyle.Add(HtmlTextWriterStyle.MarginTop, "5px");

            var buttonPanel = new Panel {ID = "tmButtons"};
            buttonPanel.Controls.Add(_deleteButton);

            var imagePanel = new Panel {ID = "tmImagePanel"};
            imagePanel.Controls.Add(_image);

            // Add controls
            _updatePanel.ContentTemplateContainer.Controls.Add(_textBox);
            _updatePanel.ContentTemplateContainer.Controls.Add(buttonPanel);
            _updatePanel.ContentTemplateContainer.Controls.Add(imagePanel);
            Controls.Add(_updatePanel);

            //_textBox.Attributes.Add("onkeyup", string.Format("javascript:__doPostBack('{0}','');$('{0}').focus();", _textBox.ClientID));
        }

        /// <summary>
        /// Handles the TextChanged event of the _textBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TextBoxText_Changed(object sender, EventArgs e)
        {
            DeleteImage();
            TextChanged.Invoke(sender, e);
        }

        #endregion
    }
}