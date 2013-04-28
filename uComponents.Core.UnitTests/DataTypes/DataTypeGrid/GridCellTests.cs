namespace uComponents.Core.UnitTests.DataTypes.DataTypeGrid
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using Umbraco.Core.Dynamics;

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
        [Ignore]
        public void GetPropertyValue_WhenGivenValidGridCell_ShouldReturnPropertyValue()
        {
            var c = new GridCell() { Alias = "test", DataType = -41, Name = "Test", Value = "01/04/2013 00:00:00" };
            var x = c.GetPropertyValue();

            Assert.IsInstanceOfType(x, typeof(DateTime));
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