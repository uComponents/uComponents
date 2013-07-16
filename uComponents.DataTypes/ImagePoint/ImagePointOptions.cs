using System.ComponentModel;
using umbraco.editorControls;

namespace uComponents.DataTypes.ImagePoint
{
    /// <summary>
    /// The options for the Image Point data-type.
    /// </summary>
    public class ImagePointOptions : AbstractOptions
    {
        /// <summary>
        /// Alias to the property to get url for image
        /// </summary>
        [DefaultValue("")]
        public string ImagePropertyAlias { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePointOptions"/> class.
        /// </summary>
        public ImagePointOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePointOptions"/> class.
        /// </summary>
        /// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
        public ImagePointOptions(bool loadDefaults)
            : base(loadDefaults)
        {
        }
    }
}