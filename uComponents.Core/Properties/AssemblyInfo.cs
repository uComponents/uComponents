using System.Reflection;
using System.Web.UI;
using uComponents.Core;

[assembly: AssemblyTitle(Constants.ApplicationName)]
[assembly: AssemblyDescription("uComponents is a collaborative project for creating components for Umbraco including data types, XSLT extensions, controls and more.")]

// embedded resources
[assembly: WebResource(Constants.FaviconResourcePath, Constants.MediaTypeNames.Image.Ico)]
[assembly: WebResource("uComponents.Core.Resources.Images.ucomponents-logo-small.png", Constants.MediaTypeNames.Image.Png)]

[assembly: WebResource("uComponents.Core.Resources.Scripts.jquery.form.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.Core.Resources.Scripts.json2.js", Constants.MediaTypeNames.Application.JavaScript)]
