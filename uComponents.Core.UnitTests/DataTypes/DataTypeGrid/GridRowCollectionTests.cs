namespace uComponents.Core.UnitTests.DataTypes.DataTypeGrid
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.MacroEngines;

    [TestClass]
    public class GridRowCollectionTests
    {
        [TestMethod]
        public void AsDynamicXml_WhenGivenValidGridRowCollection_ShouldReturnDynamicXml()
        {
            var c = new GridRowCollection();
            var x = c.AsDynamicXml();

            Assert.IsInstanceOfType(x, typeof(DynamicXml));
        }
    }
}