using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Web.UI.WebControls;

namespace uComponents.Core.DataTypes.TextImage
{
    internal static class DropDownItems
    {
        /// <summary>
        ///   Gets the fonts.
        /// </summary>
        /// <value>The fonts.</value>
        public static ListItem[] Fonts
        {
            get
            {
                var items = new InstalledFontCollection();

                return items.Families.Select(item => new ListItem(item.Name)).ToArray();
            }
        }

        /// <summary>
        ///   Gets the font styles.
        /// </summary>
        /// <value>The font styles.</value>
        public static ListItem[] FontStyles
        {
            get
            {
                var items = Enum.GetNames(typeof (FontStyle));

                return items.Select(item => new ListItem(item)).ToArray();
            }
        }

        /// <summary>
        ///   Gets the colors.
        /// </summary>
        /// <value>The font colors.</value>
        public static ListItem[] Colors
        {
            get
            {
                var items = Enum.GetNames(typeof (KnownColor));

                return items.Select(item => new ListItem(item)).ToArray();
            }
        }

        /// <summary>
        ///   Gets the Horizontal alignments.
        /// </summary>
        /// <value>The H alignments.</value>
        public static ListItem[] HAlignments
        {
            get
            {
                var items = Enum.GetNames(typeof (HorizontalAlignment));

                return items.Select(item => new ListItem(item)).ToArray();
            }
        }

        /// <summary>
        ///   Gets the Vertical alignments.
        /// </summary>
        /// <value>The V alignments.</value>
        public static ListItem[] VAlignments
        {
            get
            {
                var items = Enum.GetNames(typeof (VerticalAlignment));

                return items.Select(item => new ListItem(item)).ToArray();
            }
        }

        /// <summary>
        ///   Gets the image formats.
        /// </summary>
        /// <value>The image formats.</value>
        public static ListItem[] OutputFormats
        {
            get
            {
                var items = Enum.GetNames(typeof (OutputFormat));

                return items.Select(item => new ListItem(item)).ToArray();
            }
        }
    }
}