using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Web;
using uComponents.Core;

namespace uComponents.DataTypes.TextImage
{
    /// <summary>
    ///   Image Generator
    /// </summary>
    public static class ImageGenerator
    {
        #region Public Methods

        /// <summary>
        ///   Generates the text image.
        /// </summary>
        /// <param name = "parameters">The parameters.</param>
        /// <returns></returns>
        public static Bitmap GenerateTextImage(TextImageParameters parameters)
        {
            Graphics graphics;
            Bitmap textImage;
            RectangleF rectF;

            if (parameters.BackgroundMedia == null)
            {
                graphics = GetGraphicsWithoutBackground(parameters, out textImage, out rectF);
            }
            else
            {
                try
                {
                    graphics = GetGraphicsWithBackground(parameters, out textImage, out rectF);
                }
                catch
                {
                    graphics = GetGraphicsWithoutBackground(parameters, out textImage, out rectF);
                }
            }

            // Render Quality
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // Set fore colors
            var foreColor = parameters.ForeColor;
            var fgBrush = foreColor.StartsWith("#")
                              ? new SolidBrush(ColorTranslator.FromHtml(foreColor))
                              : new SolidBrush(Color.Transparent);

            // Get Font Family
            var fontFamily = GetFontFamily(parameters);

            // Get font style
            var fontStyle = GetFontStyle(parameters.FontStyles);

            // Set Font
            var font = new Font(fontFamily, parameters.FontSize, fontStyle, GraphicsUnit.Point);

            // Set font format
            var format = GetNewStringFormat(parameters.HAlign, parameters.VAlign);

            // Draw drop-shadow
            if (parameters.ShadowColor != "transparent")
            {
                DrawDropShadow(parameters.Text,
                               graphics,
                               rectF,
                               DropShadow.BottomRight,
                               font,
                               parameters.ShadowColor,
                               format);
            }

            // Finally, draw the textString to the bitmap
            graphics.DrawString(parameters.Text, font, fgBrush, rectF, format);

            return textImage;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///   Gets the font family.
        /// </summary>
        /// <param name = "parameters">The parameters.</param>
        /// <returns></returns>
        private static FontFamily GetFontFamily(TextImageParameters parameters)
        {
            // Get Font Family
            FontFamily fontFamily;
            if (parameters.CustomFontPath != string.Empty)
            {
                try
                {
                    // try to get custom font file
                    var pfc = new PrivateFontCollection();
                    pfc.AddFontFile(HttpContext.Current.Server.MapPath(parameters.CustomFontPath));
                    fontFamily = pfc.Families[0];
                }
                catch
                {
                    // fallback to selected font
                    fontFamily = GetNewFontFamily(parameters.FontName);
                }
            }
            else
            {
                fontFamily = GetNewFontFamily(parameters.FontName);
            }

            return fontFamily;
        }

        /// <summary>
        ///   Gets the font style.
        /// </summary>
        /// <param name = "fontStyles">The font styles.</param>
        /// <returns></returns>
        private static FontStyle GetFontStyle(IEnumerable<FontStyle> fontStyles)
        {
            return fontStyles.Aggregate(new FontStyle(), (current, style) => current | style);
        }

        /// <summary>
        ///   Gets a new string format.
        /// </summary>
        /// <param name = "hAlign">The h align.</param>
        /// <param name = "vAlign">The v align.</param>
        /// <returns></returns>
        private static StringFormat GetNewStringFormat(HorizontalAlignment hAlign, VerticalAlignment vAlign)
        {
            var horiz = StringAlignment.Center;
            var verti = StringAlignment.Center;

            switch (hAlign)
            {
                case HorizontalAlignment.Left:
                    {
                        horiz = StringAlignment.Near;
                    }
                    break;
                case HorizontalAlignment.Center:
                    {
                        horiz = StringAlignment.Center;
                    }
                    break;
                case HorizontalAlignment.Right:
                    {
                        horiz = StringAlignment.Far;
                    }
                    break;
            }

            switch (vAlign)
            {
                case VerticalAlignment.Top:
                    {
                        verti = StringAlignment.Near;
                    }
                    break;
                case VerticalAlignment.Center:
                    {
                        verti = StringAlignment.Center;
                    }
                    break;
                case VerticalAlignment.Bottom:
                    {
                        verti = StringAlignment.Far;
                    }
                    break;
            }

            return new StringFormat {Alignment = horiz, LineAlignment = verti};
        }

        /// <summary>
        ///   Gets a new bitmap.
        /// </summary>
        /// <param name = "size">The size.</param>
        /// <returns></returns>
        private static Bitmap GetNewBitmap(SizeF size)
        {
            return new Bitmap(Convert.ToInt32(size.Width), Convert.ToInt32(size.Height));
        }

        /// <summary>
        ///   Gets the image without background.
        /// </summary>
        /// <param name = "parameters">The parameters.</param>
        /// <param name = "textImage">The text image.</param>
        /// <param name = "rectF">The rect F.</param>
        /// <returns></returns>
        /// Convert.ToFloat
        private static Graphics GetGraphicsWithoutBackground(TextImageParameters parameters, out Bitmap textImage,
                                                             out RectangleF rectF)
        {
            // Initialize graphics with extra large canvas to
            // set actual canvas size later from measured string
            var sizeF = new SizeF(5000, 5000);
            textImage = GetNewBitmap(sizeF);
            var graphics = Graphics.FromImage(textImage);

            // Measure string using text and font size
            var font = new Font(GetFontFamily(parameters), parameters.FontSize, GetFontStyle(parameters.FontStyles),
                                GraphicsUnit.Point);
            var measureString = graphics.MeasureString(parameters.Text, font);

            // Set actual canvas size based on the measured string
            var canvasSize = new SizeF
                                 {
                                     Height =
                                         parameters.CanvasHeight > 0 ? parameters.CanvasHeight : measureString.Height,
                                     Width = parameters.CanvasWidth > 0 ? parameters.CanvasWidth : measureString.Width
                                 };

            // Reinitialize graphics with actual canvas size
            textImage = GetNewBitmap(canvasSize);
            graphics = Graphics.FromImage(textImage);

            // Set background color
            var bgBrush = parameters.BackColor.StartsWith("#")
                              ? new SolidBrush(ColorTranslator.FromHtml(parameters.BackColor))
                              : new SolidBrush(Color.Transparent);

            // Create and Fill Rectangle with background color
            rectF = new RectangleF(0, 0, canvasSize.Width, canvasSize.Height);
            graphics.FillRectangle(bgBrush, rectF);

            return graphics;
        }

        /// <summary>
        ///   Gets the image with background.
        /// </summary>
        /// <param name = "parameters">The parameters.</param>
        /// <param name = "textImage">The text image.</param>
        /// <param name = "rectF">The rect F.</param>
        /// <returns></returns>
        private static Graphics GetGraphicsWithBackground(TextImageParameters parameters, out Bitmap textImage,
                                                          out RectangleF rectF)
        {
            // set base textImage to background image
            var umbracoFile = parameters.BackgroundMedia.getProperty(Constants.Umbraco.Media.File).Value.ToString();
            var serverFile = HttpContext.Current.Server.MapPath(umbracoFile);
            textImage = new Bitmap(serverFile);
            rectF = new RectangleF(0, 0, textImage.Width, textImage.Height);

            // Initialize graphics
            return Graphics.FromImage(textImage);
        }

        /// <summary>
        ///   Draws the drop shadow.
        /// </summary>
        /// <param name = "textString">The text string.</param>
        /// <param name = "graphics">The graphics.</param>
        /// <param name = "rectF">The rect F.</param>
        /// <param name = "dropShadow">The drop shadow.</param>
        /// <param name = "font">The font.</param>
        /// <param name = "shadowColor">Color of the shadow.</param>
        /// <param name = "format">The format.</param>
        private static void DrawDropShadow(string textString, Graphics graphics, RectangleF rectF, DropShadow dropShadow,
                                           Font font, string shadowColor, StringFormat format)
        {
            var shadowBrush = shadowColor.StartsWith("#")
                                  ? new SolidBrush(ColorTranslator.FromHtml(shadowColor))
                                  : new SolidBrush(Color.Transparent);
            switch (dropShadow)
            {
                case DropShadow.TopLeft:
                    {
                        rectF.Offset(-2, -2);
                        graphics.DrawString(textString, font, shadowBrush, rectF, format);
                        rectF.Offset(+2, +2);
                    }
                    break;

                case DropShadow.BottomLeft:
                    {
                        rectF.Offset(+2, -2);
                        graphics.DrawString(textString, font, shadowBrush, rectF, format);
                        rectF.Offset(-2, +2);
                    }
                    break;

                case DropShadow.TopRight:
                    {
                        rectF.Offset(-2, +2);
                        graphics.DrawString(textString, font, shadowBrush, rectF, format);
                        rectF.Offset(+2, -2);
                    }
                    break;

                case DropShadow.BottomRight:
                    {
                        rectF.Offset(+2, +2);
                        graphics.DrawString(textString, font, shadowBrush, rectF, format);
                        rectF.Offset(-2, -2);
                    }
                    break;
            }
        }

        /// <summary>
        ///   Gets a new font family.
        /// </summary>
        /// <param name = "fontName">Name of the font.</param>
        /// <returns></returns>
        private static FontFamily GetNewFontFamily(string fontName)
        {
            var families = FontFamily.Families;
            foreach (var family in families)
            {
                if (family.Name == fontName)
                    return family;
            }
            return new FontFamily(GenericFontFamilies.SansSerif);
        }

        #endregion
    }
}