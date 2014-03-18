using Microsoft.VisualStudio.TestTools.UnitTesting;
using umbraco; // reference to uQuery.IGetProperty
using ImagePointModel = uComponents.DataTypes.ImagePoint.ImagePoint; // aliased to avoid this namespace and class naming conflict

namespace uComponents.Core.UnitTests.DataTypes.ImagePoint
{
    [TestClass]
    public class ImagePointModelTests
    {
        [TestMethod]
        public void EmptyImagePoint()
        {
            ImagePointModel imagePoint = new ImagePointModel();

            Assert.IsFalse(imagePoint.HasCoordinate);
            Assert.IsNull(imagePoint.X);
            Assert.IsNull(imagePoint.Y);
            Assert.AreEqual(0, imagePoint.PercentageX);
            Assert.AreEqual(0, imagePoint.PercentageY);
            Assert.AreEqual(0, imagePoint.Height);
            Assert.AreEqual(0, imagePoint.Width);
        }

        [TestMethod]
        public void InflateImagePoint()
        {
            ImagePointModel imagePoint = new ImagePointModel();

            // this is the method called when using the uQuery extensions, eg. uQuery.GetCurrentNode().GetPropertyValue<ImagePoint>("propertyAlias");
            ((uQuery.IGetProperty)imagePoint).LoadPropertyValue("<ImagePoint x=\"5\" y=\"10\" width=\"100\" height=\"200\" />");
            
            Assert.IsTrue(imagePoint.HasCoordinate);
            Assert.AreEqual(5, imagePoint.X);
            Assert.AreEqual(10, imagePoint.Y);
            Assert.AreEqual(100, imagePoint.Width);
            Assert.AreEqual(200, imagePoint.Height);            
        }

        [TestMethod]
        public void GetPercentages()
        {
            ImagePointModel imagePoint = new ImagePointModel();

            ((uQuery.IGetProperty)imagePoint).LoadPropertyValue("<ImagePoint x=\"5\" y=\"10\" width=\"100\" height=\"100\" />");

            Assert.AreEqual(5, imagePoint.PercentageX);
            Assert.AreEqual(10, imagePoint.PercentageY);
        }
    }
}
