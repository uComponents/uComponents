using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.UI;
using uComponents.Core;

[assembly: AssemblyTitle(Constants.ApplicationName)]
[assembly: AssemblyDescription("uComponents is a collaborative project for creating components for Umbraco including data types, XSLT extensions, controls and more.")]

[assembly: AssemblyVersion("0.0.0.0")]
[assembly: AssemblyFileVersion("0.0.0.0")]
[assembly: AssemblyInformationalVersion("0.0.0.0")]

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("The Umbraco Community")]
[assembly: AssemblyProduct("uComponents")]
[assembly: AssemblyCopyright("Copyright \xa9 The Umbraco Community 2014")]
[assembly: AssemblyTrademark("The Umbraco Community")]
[assembly: AssemblyCulture("")]

// expose internal classes/methods
[assembly: InternalsVisibleTo("uComponents.Core.UnitTests")]
[assembly: InternalsVisibleTo("uComponents.Controls")]
[assembly: InternalsVisibleTo("uComponents.DataTypes")]
[assembly: InternalsVisibleTo("uComponents.DataTypes.RazorDataTypeModels")]
[assembly: InternalsVisibleTo("uComponents.Installer")]
[assembly: InternalsVisibleTo("uComponents.Legacy")]
[assembly: InternalsVisibleTo("uComponents.MacroEngines")]
[assembly: InternalsVisibleTo("uComponents.Mapping")]
[assembly: InternalsVisibleTo("uComponents.NotFoundHandlers")]
[assembly: InternalsVisibleTo("uComponents.PropertyEditors.ValueConverters")]
[assembly: InternalsVisibleTo("uComponents.UI")]
[assembly: InternalsVisibleTo("uComponents.XsltExtensions")]

[assembly: ComVisible(false)]

// shared embedded resources
[assembly: WebResource(Constants.FaviconResourcePath, Constants.MediaTypeNames.Image.Ico)]
[assembly: WebResource(Constants.IconResourcePath, Constants.MediaTypeNames.Image.Png)]
