using umbraco;

namespace uComponents.DataTypes.ImagePoint
{
    using System.Xml.Linq;

    /// <summary>
    /// Strongly typed obj that's returned from uQuery .GetProperty{ImagePoint}("alias");
    /// </summary>
    public class ImagePoint : uQuery.IGetProperty
    {
        /// <summary>
        /// Gets the X Coordinate (will be null if not set)
        /// </summary>
        public int? X { get; private set; }

        /// <summary>
        /// Gets the Y Coordinate (will be null if not set)
        /// </summary>
        public int? Y { get; private set; }

        /// <summary>
        /// Gets the width boundary
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height boundary
        /// </summary>
        public int Height { get; private set; }


        public decimal PercentageX 
        {
            get
            {
                if (this.Width > 0 && this.X > 0)
                {
                    return (decimal)((decimal)this.Width / this.X) * 100;
                }

                return 0;
            }
        }

        public decimal PercentageY
        {
            get
            {
                if (this.Height > 0 && this.Y > 0)
                {
                    return (decimal)((decimal)this.Height / this.Y) * 100;
                }

                return 0;
            }
        }

        /// <summary>
        /// Create this strongly typed object from the xml fragment
        /// </summary>
        /// <param name="value"></param>
        void uQuery.IGetProperty.LoadPropertyValue(string value)
        {                      
            if (!string.IsNullOrWhiteSpace(value))
            {
                // <ImagePoint x="5" y="10" width="100 height="100" />

                XDocument valueXDocument = this.GetXDocument(value);
                if (valueXDocument != null)
                {
                    XElement imagePointElement = valueXDocument.Element("ImagePoint");

                    if (imagePointElement != null)
                    {
                        int x;
                        if (int.TryParse(imagePointElement.Attribute("x").Value, out x))
                        {
                            this.X = x;
                        }

                        int y;
                        if (int.TryParse(imagePointElement.Attribute("y").Value, out y))
                        {
                            this.Y = y;
                        }

                        int width;
                        if (int.TryParse(imagePointElement.Attribute("width").Value, out width))
                        {
                            this.Width = width;
                        }

                        int height;
                        if (int.TryParse(imagePointElement.Attribute("height").Value, out height))
                        {
                            this.Height = height;
                        }
                    }
                }
                else
                {
                    // LEGACY CODE - WILL BE REMOVED
                    string[] coordinates = value.Split(',');
                    if (coordinates.Length == 2)
                    {
                        int x;
                        int y;

                        if (int.TryParse(coordinates[0], out x))
                        {
                            this.X = x;
                        }

                        if (int.TryParse(coordinates[1], out y))
                        {
                            this.Y = y;
                        }
                    }
                }
            }
        }

        private XDocument GetXDocument(string value)
        {
            try
            {
                return XDocument.Parse(value);
            }
            catch
            {
                // invalid xml
                return null;
            }
        }
    }
}
