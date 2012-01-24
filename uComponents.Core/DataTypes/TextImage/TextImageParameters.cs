using System.Drawing;
using umbraco.cms.businesslogic.media;

namespace uComponents.Core.DataTypes.TextImage
{

    #region Referenced Namespaces

    #endregion

    /// <summary>
    ///   Text Image Parameters
    /// </summary>
    public class TextImageParameters
    {
        #region Fields

        private readonly string _backColor;
        private readonly Media _backgroundMedia;
        private readonly int _canvasHeight;
        private readonly int _canvasWidth;
        private readonly string _customFontPath;
        private readonly string _fontName;
        private readonly int _fontSize;
        private readonly FontStyle[] _fontStyles;
        private readonly string _foreColor;
        private readonly HorizontalAlignment _hAlign;
        private readonly OutputFormat _outputFormat;
        private readonly string _shadowColor;
        private readonly string _text;
        private readonly VerticalAlignment _vAlign;

        #endregion

        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TextImageParameters" /> class.
        /// </summary>
        /// <param name = "textString">The node property.</param>
        /// <param name = "outputFormat">The output format.</param>
        /// <param name = "customFontPath">The custom font path.</param>
        /// <param name = "fontName">Name of the font.</param>
        /// <param name = "fontSize">Size of the font.</param>
        /// <param name = "fontStyles">The font style.</param>
        /// <param name = "foreColor">Color of the fore.</param>
        /// <param name = "backColor">Color of the back.</param>
        /// <param name = "shadowColor">Color of the shadow.</param>
        /// <param name = "hAlign">The h align.</param>
        /// <param name = "vAlign">The v align.</param>
        /// <param name = "canvasHeight">Height of the canvas.</param>
        /// <param name = "canvasWidth">Width of the canvas.</param>
        /// <param name = "backgroundMedia">The background image.</param>
        public TextImageParameters(string textString, OutputFormat outputFormat, string customFontPath, string fontName,
                                   int fontSize, FontStyle[] fontStyles, string foreColor, string backColor,
                                   string shadowColor, HorizontalAlignment hAlign, VerticalAlignment vAlign,
                                   int canvasHeight, int canvasWidth, Media backgroundMedia)
        {
            _text = textString;
            _outputFormat = outputFormat;
            _customFontPath = customFontPath;
            _fontName = fontName;
            _fontSize = fontSize;
            _fontStyles = fontStyles;
            _foreColor = foreColor;
            _backColor = backColor;
            _hAlign = hAlign;
            _vAlign = vAlign;
            _canvasHeight = canvasHeight;
            _canvasWidth = canvasWidth;
            _shadowColor = shadowColor;
            _backgroundMedia = backgroundMedia;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the custom font path.
        /// </summary>
        /// <value>The custom font path.</value>
        public string CustomFontPath
        {
            get { return _customFontPath; }
        }

        /// <summary>
        ///   Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return _text; }
        }

        /// <summary>
        ///   Gets the name of the font.
        /// </summary>
        /// <value>The name of the font.</value>
        public string FontName
        {
            get { return _fontName; }
        }

        /// <summary>
        ///   Gets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public int FontSize
        {
            get { return _fontSize; }
        }

        /// <summary>
        ///   Gets the font style.
        /// </summary>
        /// <value>The font style.</value>
        public FontStyle[] FontStyles
        {
            get { return _fontStyles; }
        }

        /// <summary>
        ///   Gets the color of the fore.
        /// </summary>
        /// <value>The color of the fore.</value>
        public string ForeColor
        {
            get { return _foreColor; }
        }

        /// <summary>
        ///   Gets the color of the back.
        /// </summary>
        /// <value>The color of the back.</value>
        public string BackColor
        {
            get { return _backColor; }
        }

        /// <summary>
        ///   Gets the color of the shadow.
        /// </summary>
        /// <value>The color of the shadow.</value>
        public string ShadowColor
        {
            get { return _shadowColor; }
        }

        /// <summary>
        ///   Gets the H align.
        /// </summary>
        /// <value>The H align.</value>
        public HorizontalAlignment HAlign
        {
            get { return _hAlign; }
        }

        /// <summary>
        ///   Gets the V align.
        /// </summary>
        /// <value>The V align.</value>
        public VerticalAlignment VAlign
        {
            get { return _vAlign; }
        }

        /// <summary>
        ///   Gets the height of the canvas.
        /// </summary>
        /// <value>The height of the canvas.</value>
        public int CanvasHeight
        {
            get { return _canvasHeight; }
        }

        /// <summary>
        ///   Gets the width of the canvas.
        /// </summary>
        /// <value>The width of the canvas.</value>
        public int CanvasWidth
        {
            get { return _canvasWidth; }
        }

        /// <summary>
        ///   Gets the background image.
        /// </summary>
        /// <value>The background image.</value>
        public Media BackgroundMedia
        {
            get { return _backgroundMedia; }
        }

        /// <summary>
        ///   Gets the output format.
        /// </summary>
        /// <value>The output format.</value>
        public OutputFormat OutputFormat
        {
            get { return _outputFormat; }
        }

        #endregion
    }
}