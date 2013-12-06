namespace uComponents.Core.UnitTests.PropertyEditorValueConverters
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using uComponents.DataTypes.UrlPicker.Dto;
    using uComponents.PropertyEditors.ValueConverters.UrlPicker;

    using Umbraco.Core.PropertyEditors;

    [TestClass]
    public class UrlPickerPropertyEditorValueConverterTests
    {
        private IPropertyEditorValueConverter converter;

        [TestInitialize]
        public void Initialize()
        {
            this.converter = new UrlPickerPropertyEditorValueConverter();
        }

        [TestMethod]
        public void ConvertPropertyValue_WhenGivenValidValue_ShouldReturnUrlPickerState()
        {
            var s = "<url-picker mode='Content'><new-window>False</new-window><node-id>1051</node-id><url>/no/forside/</url><link-title></link-title></url-picker>";

            var v = this.converter.ConvertPropertyValue(s);

            Assert.IsInstanceOfType(v.Result, typeof(UrlPickerState));
        }

        [TestMethod]
        public void ConvertPropertyValue_WhenGivenNull_ShouldReturnNull()
        {
            var s = string.Empty;

            var v = this.converter.ConvertPropertyValue(s);

            Assert.IsNull(v.Result);
        }
    }
}