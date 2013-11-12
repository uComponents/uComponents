namespace uComponents.Core.UnitTests.Helpers
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ResolversTests
    {
        [TestMethod]
        public void GetPropertyValueConverters_WhenPropertyValueConvertersExists_ShouldReturnListOfPropertyValueConverters()
        {
            var converters = Helper.Resolvers.GetPropertyValueConverters();

            Assert.IsTrue(converters.Any());
        }
    }
}