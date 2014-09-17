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
        public void ReInflateImagePoint()
        {
            ImagePointModel imagePoint = new ImagePointModel();

            ((uQuery.IGetProperty)imagePoint).LoadPropertyValue("<ImagePoint x=\"5\" y=\"10\" width=\"100\" height=\"200\" />");

            Assert.IsTrue(imagePoint.HasCoordinate);
            Assert.AreEqual(5, imagePoint.X);
            Assert.AreEqual(10, imagePoint.Y);
            Assert.AreEqual(100, imagePoint.Width);
            Assert.AreEqual(200, imagePoint.Height);

            ((uQuery.IGetProperty)imagePoint).LoadPropertyValue("<ImagePoint x=\"64\" y=\"22\" width=\"640\" height=\"480\" />");

            Assert.IsTrue(imagePoint.HasCoordinate);
            Assert.AreEqual(64, imagePoint.X);
            Assert.AreEqual(22, imagePoint.Y);
            Assert.AreEqual(640, imagePoint.Width);
            Assert.AreEqual(480, imagePoint.Height);
        }

        [TestMethod]
        public void InflateImagePointWithInvalidXmlData()
        {
            ImagePointModel imagePoint = new ImagePointModel();

            ((uQuery.IGetProperty)imagePoint).LoadPropertyValue("<ImagePoint x=\"-5\" y=\"abc\" width=\"xyz\" height=\"200\" />");

            Assert.IsFalse(imagePoint.HasCoordinate);
            Assert.IsNull(imagePoint.X);
            Assert.IsNull(imagePoint.Y);
            Assert.AreEqual(0, imagePoint.Width);
            Assert.AreEqual(200, imagePoint.Height);

            ((uQuery.IGetProperty)imagePoint).LoadPropertyValue("<ImagePoint x=\"abc\" y=\"200\" width=\"xyz\" height=\"100\" />");

            Assert.IsFalse(imagePoint.HasCoordinate);
            Assert.IsNull(imagePoint.X);
            Assert.AreEqual(100, imagePoint.Y);
            Assert.AreEqual(0, imagePoint.Width);
            Assert.AreEqual(100, imagePoint.Height);
        }

        [TestMethod]
        public void GetPercentages()
        {
            ImagePointModel imagePoint = new ImagePointModel();

            ((uQuery.IGetProperty)imagePoint).LoadPropertyValue("<ImagePoint x=\"5\" y=\"10\" width=\"100\" height=\"100\" />");

            Assert.AreEqual(5, imagePoint.PercentageX);
            Assert.AreEqual(10, imagePoint.PercentageY);

            ((uQuery.IGetProperty)imagePoint).LoadPropertyValue("<ImagePoint x=\"101\" y=\"0\" width=\"100\" height=\"100\" />");

            Assert.AreEqual(100, imagePoint.PercentageX);
            Assert.AreEqual(0, imagePoint.PercentageY);

            ((uQuery.IGetProperty)imagePoint).LoadPropertyValue("<ImagePoint x=\"abc\" y=\"300\" width=\"xyz\" height=\"100\" />");

            Assert.AreEqual(0, imagePoint.PercentageX);
            Assert.AreEqual(100, imagePoint.PercentageY);
        }
    }
}
