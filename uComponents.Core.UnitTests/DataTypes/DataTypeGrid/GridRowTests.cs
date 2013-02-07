namespace uComponents.Core.UnitTests.DataTypes.DataTypeGrid
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.MacroEngines;

    [TestClass]
    public class GridRowTests
    {
        [TestMethod]
        public void AsDynamicXml_WhenGivenValidGridRow_ShouldReturnDynamicXml()
        {
            var c = new GridRow();
            var x = c.AsDynamicXml();

            Assert.IsInstanceOfType(x, typeof(DynamicXml));
        }

        [TestMethod]
        public void GetId_WhenGivenValidGridRow_ShouldReturnId()
        {
            dynamic c = new GridRow() { Id = 1 };

            Assert.AreEqual(c.Id, 1);
        }

        [TestMethod]
        public void GetSortOrder_WhenGivenValidGridRow_ShouldReturnSortOrder()
        {
            dynamic c = new GridRow() { SortOrder = 11 };

            Assert.AreEqual(c.SortOrder, 11);
        }
    }
}