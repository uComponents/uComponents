using Microsoft.VisualStudio.TestTools.UnitTesting;
using umbraco; // reference to uQuery.IGetProperty
using ImagePointModel = uComponents.DataTypes.ImagePoint.ImagePoint; // aliased to avoid this namespace and class naming conflict

namespace uComponents.Core.UnitTests.DataTypes.ImagePoint
{
    [TestClass]
    public class ImagePointModelTests
    {
        [TestMethod]        
        public void InflateImagePointTest()
        {
            ImagePointModel imagePoint = new ImagePointModel();

            ((uQuery.IGetProperty)imagePoint).LoadPropertyValue("<ImagePoint x=\"5\" y=\"10\" width=\"100\" height=\"200\" />");
            
            Assert.IsTrue(imagePoint.HasCoordinate);
            Assert.AreEqual(5, imagePoint.X);
            Assert.AreEqual(10, imagePoint.Y);
            Assert.AreEqual(100, imagePoint.Width);
            Assert.AreEqual(200, imagePoint.Height);            
        }
    }
}
