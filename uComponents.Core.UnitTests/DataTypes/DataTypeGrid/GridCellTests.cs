namespace uComponents.Core.UnitTests.DataTypes.DataTypeGrid
{
    using System.Xml;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.MacroEngines;
    using umbraco.cms.businesslogic.datatype;

    [TestClass]
    public class GridCellTests
    {
        [TestMethod]
        public void AsDynamicXml_WhenGivenValidGridCell_ShouldReturnDynamicXml()
        {
            var c = new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" };
            var x = c.AsDynamicXml();

            Assert.IsInstanceOfType(x, typeof(DynamicXml));
        }

        [TestMethod]
        public void AsDynamicXml_WhenGivenUnescapedValue_ShouldReturnEscapedXml()
        {
            var v = "<XPathAutoComplete Type=\"c66ba18e-eaf3-4cff-8a22-41b16d66a972\"><Item Text=\"ABC\" Value=\"1\" /><Item Text=\"XYZ\" Value=\"9\" /></XPathAutoComplete>";
            var c = new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = v };

            var xml = c.AsDynamicXml();

            Assert.AreEqual(xml.InnerText, c.Value);
        }

        [TestMethod]
        public void AsDynamicXml_WhenGivenEscapedValue_ShouldReturnEscapedXml()
        {
            var v = "&lt;XPathAutoComplete Type=\"c66ba18e-eaf3-4cff-8a22-41b16d66a972\"&gt;&lt;Item Text=\"ABC\" Value=\"1\" /&gt;&lt;Item Text=\"XYZ\" Value=\"9\" /&gt;&lt;/XPathAutoComplete&gt;";
            var c = new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = v };

            var xml = c.AsDynamicXml();

            Assert.AreEqual(xml.InnerText, c.Value);
        }

        [TestMethod]
        public void ToString_WhenGivenValidGridCell_ShouldReturnValue()
        {
            var c = new GridCell() { Value = "1234" };
            var x = c.ToString();

            Assert.AreEqual(x, c.Value);
        }
    }
}