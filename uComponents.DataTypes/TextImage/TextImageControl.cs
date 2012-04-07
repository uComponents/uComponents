namespace uComponents.DataTypes.TextImage
{
    using System;
    using System.Web.UI.WebControls;

    #region Referenced Namespaces

    #endregion

    public class TextImageControl : Panel
    {
        #region Controls

        private Image _imgTextImage;
        private TextBox _txtTextString;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the text.
        /// </summary>
        /// <value>The text.</value>
        internal string Text
        {
            get { return _txtTextString.Text; }
            set { _txtTextString.Text = value; }
        }

        #endregion

        #region Initialize

        /// <summary>
        ///   Initializes the control.
        /// </summary>
        internal void InitializeControl()
        {
            // Instantiate controls
            _txtTextString = new TextBox { CssClass = "umbEditorTextField" };
            _imgTextImage = new Image
                                {
                                    Width = 100,
                                    Visible = false,
                                    BorderStyle = BorderStyle.None,
                                    AlternateText = "uComponent: Text Image Preview"
                                };
        }

        #endregion

        #region Event Methods

        /// <summary>
        ///   Raises the <see cref = "E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name = "e">An <see cref = "T:System.EventArgs" /> object that contains the event data.</param>
        /// <exception cref = "T:System.InvalidOperationException">The <see cref = "P:System.Web.UI.UpdatePanel.ContentTemplate" /> property is being defined when the <see cref = "P:System.Web.UI.UpdatePanel.ContentTemplateContainer" /> property has already been created.</exception>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Add controls for user
            Controls.Add(_txtTextString);
            Controls.Add(new Literal {Text = "<p>Preview: "});
            Controls.Add(_imgTextImage);
            Controls.Add(new Literal {Text = "<p/>"});
        }

        #endregion
    }
}