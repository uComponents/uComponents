using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.UI;
using uComponents.Core;

[assembly: AssemblyTitle(Constants.ApplicationName)]
[assembly: AssemblyDescription("uComponents is a collaborative project for creating components for Umbraco including data types, XSLT extensions, controls and more.")]

// expose internal classes/methods
[assembly: InternalsVisibleTo("uComponents.Controls")]
[assembly: InternalsVisibleTo("uComponents.DataTypes")]
[assembly: InternalsVisibleTo("uComponents.Installer")]
[assembly: InternalsVisibleTo("uComponents.Legacy")]
[assembly: InternalsVisibleTo("uComponents.MacroEngines")]
[assembly: InternalsVisibleTo("uComponents.NotFoundHandlers")]
[assembly: InternalsVisibleTo("uComponents.UI")]
[assembly: InternalsVisibleTo("uComponents.XsltExtensions")]

// shared embedded resources
[assembly: WebResource("uComponents.Core.Resources.Images.favicon.ico", Constants.MediaTypeNames.Image.Png)]
