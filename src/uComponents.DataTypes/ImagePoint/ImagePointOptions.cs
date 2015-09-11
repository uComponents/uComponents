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
        /// Initializes a new instance of the <see cref="ImagePointOptions"/> class.
        /// </summary>
        public ImagePointOptions()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the alias to the property for the image url (TODO: make this optional ?)
        /// </summary>
        [DefaultValue("")]
        public string ImagePropertyAlias { get; set; }

        /// <summary>
        /// Gets or sets an optional width
        /// </summary>
        [DefaultValue(0)]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets an optional height
        /// </summary>
        [DefaultValue(0)]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether neighbouring points should be rendered as ghost points
        /// </summary>
        [DefaultValue(false)]
        public bool ShowNeighbours { get; set; }
    }
}