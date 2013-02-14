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
        public void ToString_WhenGivenValidGridCell_ShouldReturnValue()
        {
            var c = new GridCell() { Value = "1234" };
            var x = c.ToString();

            Assert.AreEqual(x, c.Value);
        }
    }
}