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
        /// Gets a value indicating whether both the X and Y values have been set 
        /// </summary>
        public bool HasCoordinate
        {
            get
            {
                return this.X.HasValue && this.Y.HasValue;
            }
        }

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

        /// <summary>
        /// Gets a value between 0 and 100
        /// </summary>
        public byte PercentageX 
        {
            get
            {
                if (this.Width > 0 && this.X > 0)
                {
                    if (this.X >= this.Width)
                    {
                        return 100;
                    }

                    return (byte)(((decimal)this.X / (decimal)this.Width) * 100);
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets a value between 0 and 100
        /// </summary>
        public byte PercentageY
        {
            get
            {
                if (this.Height > 0 && this.Y > 0)
                {
                    if (this.Y >= this.Height)
                    {
                        return 100;
                    }

                    return (byte)(((decimal)this.Y / (decimal)this.Height) * 100);
                }

                return 0;
            }
        }

        /// <summary>
        /// Create this strongly typed object from the xml fragment
        /// </summary>
        /// <param name="value">the xml string</param>
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
