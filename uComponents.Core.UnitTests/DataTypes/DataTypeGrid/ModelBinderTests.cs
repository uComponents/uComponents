namespace uComponents.Core.UnitTests.DataTypes.DataTypeGrid
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using uComponents.DataTypes.DataTypeGrid;
    using uComponents.DataTypes.DataTypeGrid.Model;

    [TestClass]
    public class ModelBinderTests
    {
        [TestMethod]
        public void Init_WhenGivenValidXml_ShouldReturnItemsInCorrectOrder()
        {
            var xml = "<items><item id=\"3\" sortOrder=\"3\"><documentName nodeName=\"\" nodeType=\"0\">Manual</documentName><documentUrl nodeName=\"\" nodeType=\"0\">http://jotul.com/no/produkter.xml/_attachment/6306?_download=true&amp;_ts=13ab17f5a82</documentUrl></item><item id=\"0\" sortOrder=\"1\"><documentName nodeName=\"\" nodeType=\"0\">Manual</documentName><documentUrl nodeName=\"\" nodeType=\"0\">http://jotul.com/no/produkter.xml/_attachment/6306?_download=true&amp;_ts=13ab17f5a82</documentUrl></item><item id=\"1\" sortOrder=\"2\"><documentName nodeName=\"\" nodeType=\"0\">FDV dokumentasjon</documentName><documentUrl nodeName=\"\" nodeType=\"0\">http://jotul.com/no/produkter.xml/_attachment/6305?_download=true&amp;_ts=13ab17f1573</documentUrl></item><item id=\"4\" sortOrder=\"0\"><documentName nodeName=\"\" nodeType=\"0\">FDV dokumentasjon</documentName><documentUrl nodeName=\"\" nodeType=\"0\">http://jotul.com/no/produkter.xml/_attachment/6305?_download=true&amp;_ts=13ab17f1573</documentUrl></item></items>";

            var modelBinder = new ModelBinder();

            object o;

            var result = modelBinder.Init(0, xml, out o);

            Assert.IsTrue(result);

            var c = (GridRowCollection)o;

            Assert.IsTrue(c[0].SortOrder == 0);
            Assert.IsTrue(c[1].SortOrder == 1);
            Assert.IsTrue(c[2].SortOrder == 2);
            Assert.IsTrue(c[3].SortOrder == 3);
        }
    }
}