using System.Web.UI;
using uComponents.Core;
using uComponents.Core.Extensions;
using uComponents.DataTypes.Shared.Extensions;

[assembly: WebResource("uComponents.DataTypes.TextImage.images.color.png", Constants.MediaTypeNames.Image.Png)]
[assembly: WebResource("uComponents.DataTypes.TextImage.images.colorpicker.png", Constants.MediaTypeNames.Image.Png)]
[assembly: WebResource("uComponents.DataTypes.TextImage.images.graybar.jpg", Constants.MediaTypeNames.Image.Jpeg)]
[assembly: WebResource("uComponents.DataTypes.TextImage.images.grid.gif", Constants.MediaTypeNames.Image.Gif)]
[assembly: WebResource("uComponents.DataTypes.TextImage.images.meta100.png", Constants.MediaTypeNames.Image.Png)]
[assembly: WebResource("uComponents.DataTypes.TextImage.images.transparentpixel.gif", Constants.MediaTypeNames.Image.Gif)]
[assembly: WebResource("uComponents.DataTypes.TextImage.scripts.mColorPicker.js", Constants.MediaTypeNames.Application.JavaScript, PerformSubstitution = true)]

namespace uComponents.DataTypes.TextImage
{
    using System.Web.UI;
    using ClientDependency.Core;

    /// <summary>
    /// TextImage Extensions
    /// </summary>
    public static class TextImageExtensions
    {
        /// <summary>
        /// 
        /// jQuery Color Picker -- http://code.google.com/p/mcolorpicker/
        /// 
        /// </summary>
        /// <code>
        ///     control.AddResourceToClientDependency(ColorpickerJs, ClientDependencyType.Javascript);
        /// </code>
        /// <remarks>
        /// Don't forget to add web resources:
        /// 
        ///     [assembly: WebResource("uComponents.DataTypes.TextImage.images.color.png", MediaTypeNames.Image.Png)]
        ///     [assembly: WebResource("uComponents.DataTypes.TextImage.images.colorpicker.png", MediaTypeNames.Image.Png)]
        ///     [assembly: WebResource("uComponents.DataTypes.TextImage.images.graybar.jpg", MediaTypeNames.Image.Jpeg)]
        ///     [assembly: WebResource("uComponents.DataTypes.TextImage.image.grid.gif", MediaTypeNames.Image.Gif)]
        ///     [assembly: WebResource("uComponents.DataTypes.TextImage.images.meta100.png", MediaTypeNames.Image.Png)]
        ///     [assembly: WebResource("uComponents.DataTypes.TextImage.images.transparentpixel.gif", MediaTypeNames.Image.Gif)]
        ///     [assembly: WebResource("uComponents.DataTypes.TextImage.scripts.mColorPicker.js", MediaTypeNames.Application.JavaScript, PerformSubstitution = true)]
        /// 
        /// </remarks>
        public const string ColorpickerJs = "uComponents.DataTypes.TextImage.scripts.mColorPicker.js";

        /// <summary>
        /// Adds the JS required for TextImage
        /// </summary>
        /// <param name="control">The CTL.</param>
        public static void AddJsTextImageClientDependencies(this Control control)
        {
			control.AddResourceToClientDependency(Constants.PrevalueEditorCssResourcePath, ClientDependencyType.Css);
            control.AddResourceToClientDependency(ColorpickerJs, ClientDependencyType.Javascript);
        }
    }
}
