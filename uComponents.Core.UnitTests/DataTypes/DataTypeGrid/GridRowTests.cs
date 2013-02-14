namespace uComponents.Core.UnitTests.DataTypes.DataTypeGrid
{
    using System.Xml;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.MacroEngines;

    [TestClass]
    public class GridRowTests
    {
        [TestMethod]
        public void AsDynamicXml_WhenGivenValidGridRow_ShouldReturnDynamicXml()
        {
            var c = new GridRow() { new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" } };
            var x = c.AsDynamicXml();

            Assert.IsInstanceOfType(x, typeof(DynamicXml));
        }

        [TestMethod]
        public void ToString_WhenGivenValidGridRow_ShouldReturnValidXmlString()
        {
            var c = new GridRow() { new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" } };

            var doc = new XmlDocument();
            doc.LoadXml(c.ToString());

            Assert.IsNotNull(doc);
        }

        [TestMethod]
        public void AsDynamicXml_WhenGivenValidDynamicGridRow_ShouldReturnDynamicXml()
        {
            dynamic c = new GridRow() { new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" } };
            var x = c.AsDynamicXml();

            Assert.IsInstanceOfType(x, typeof(DynamicXml));
        }

        [TestMethod]
        public void GetId_WhenGivenValidDynamicGridRow_ShouldReturnId()
        {
            dynamic c = new GridRow() { Id = 1 };

            Assert.AreEqual(c.Id, 1);
        }

        [TestMethod]
        public void GetSortOrder_WhenGivenValidDynamicGridRow_ShouldReturnSortOrder()
        {
            dynamic c = new GridRow() { SortOrder = 11 };

            Assert.AreEqual(c.SortOrder, 11);
        }

        [TestMethod]
        public void GetCellValueByAlias_WhenGivenValidDynamicGridRow_ShouldReturnCellValue()
        {
            dynamic c = new GridRow() { new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" } };

            Assert.AreEqual(c.test.Value, "1234");
        }

        [TestMethod]
        public void GetCellValueByName_WhenGivenValidDynamicGridRow_ShouldReturnCellValue()
        {
            dynamic c = new GridRow() { new GridCell() { Alias = "anotherTest", DataType = -88, Name = "AnotherTest", Value = "1234" } };

            Assert.AreEqual(c.AnotherTest.Value, "1234");
        }
    }
}