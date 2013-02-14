using System.Reflection;
using System.Web.UI;
using uComponents.Core;

[assembly: AssemblyTitle("uComponents.DataTypes")]
[assembly: AssemblyDescription("DataTypes for Umbraco/uComponents.")]

// shared embedded resources
[assembly: WebResource(Constants.PrevalueEditorCssResourcePath, Constants.MediaTypeNames.Text.Css, PerformSubstitution = true)]
[assembly: WebResource("uComponents.DataTypes.Shared.Resources.Scripts.jquery.form.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.Shared.Resources.Scripts.json2.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.Shared.Resources.Scripts.jquery.ucomponents.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.Shared.Resources.Scripts.jquery.ucomponents.dictionary.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.Shared.Resources.Images.ucomponents-logo-small.png", Constants.MediaTypeNames.Image.Png)]
