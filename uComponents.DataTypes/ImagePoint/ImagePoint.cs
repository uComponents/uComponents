using umbraco;

namespace uComponents.DataTypes.ImagePoint
{
    /// <summary>
    /// Strongly typed obj that's returned from uQuery .GetProperty{ImagePoint}("alias");
    /// </summary>
    public class ImagePoint : uQuery.IGetProperty
    {
        /// <summary>
        /// Gets the X Coordinate
        /// </summary>
        public int? X { get; private set; }

        /// <summary>
        /// Gets the Y Coordinate
        /// </summary>
        public int? Y { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void uQuery.IGetProperty.LoadPropertyValue(string value)
        {
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
