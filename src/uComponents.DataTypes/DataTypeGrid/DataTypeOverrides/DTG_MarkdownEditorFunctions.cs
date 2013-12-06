namespace uComponents.DataTypes.DataTypeGrid.DataTypeOverrides
{
    using System.Reflection;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using umbraco.interfaces;

    internal class MarkdownEditorFunctions
    {
        /// <summary>
        /// Converts the datatype value to a DTG compatible string
        /// </summary>
        /// <param name="dataType">The DataType.</param>
        /// <returns></returns>
        public static string ToDtgString(IDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            try
            {
                var assembly = Assembly.GetAssembly(dataType.GetType());
                var type = assembly.GetType("Our.Umbraco.DataType.Markdown.XsltExtensions");

                return (string)type.InvokeMember("Transform", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, new object[] { value });
            }
            catch // (Exception ex)
            {
                return value;
                // TODO: [LK->OA] Did you want to capture the exception?
            }
        }

        /// <summary>
        /// Configures for DTG.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="container">The container.</param>
        public static void ConfigureForDtg(IDataType dataType, Control container)
        {
            dataType.DataEditor.Editor.Load += (sender, args) =>
                {
                    var wmd = dataType.DataEditor.Editor.Controls[0];
                    wmd.ID = dataType.DataEditor.Editor.ID + "_ctl0";

                    var txt = (TextBox)wmd.Controls[0];
                    txt.ID = wmd.ID + "_ctl0";
                };
        }
    }
}
