using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.UI;
using uComponents.Core.Shared;

// General Information about an assembly is controlled through the following set of attributes.
// Change these attribute values to modify the information associated with an assembly.
[assembly: AssemblyTitle(Settings.APPNAME)]
[assembly: AssemblyDescription("uComponents is a collaborative project for creating components for Umbraco including data types, XSLT extensions, controls and more.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("The Umbraco Community")]
[assembly: AssemblyProduct(Settings.APPNAME)]
[assembly: AssemblyCopyright("Copyright \xa9 The Umbraco Community 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible to COM components.
// If you need to access a type in this assembly from COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("99D05FD1-5C6D-4E7A-816D-C1C21B0E9D84")]

// Version information for an assembly consists of the following four values:
// [Major].[Minor].[Build].[Revision]
[assembly: AssemblyVersion("3.1.1.*")]
//[assembly: AssemblyFileVersion("3.1.1.*")]

// tag prefix for custom controls
[assembly: TagPrefix("uComponents.Core.Controls", Settings.APPNAME)]

// shared embedded resources
[assembly: WebResource(Settings.PrevalueEditorCssResourcePath, MediaTypeNames.Text.Css, PerformSubstitution = true)]
[assembly: WebResource("uComponents.Core.Shared.Resources.Scripts.jquery.tooltip.min.js", MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.Core.Shared.Resources.Images.ucomponents-logo-small.png", MediaTypeNames.Image.Png)]
[assembly: WebResource("uComponents.Core.Shared.Resources.Images.deleteIcon.gif", MediaTypeNames.Image.Gif)]
[assembly: WebResource("uComponents.Core.Shared.Resources.Images.information.png", MediaTypeNames.Image.Png)]
[assembly: WebResource("uComponents.Core.Shared.Resources.Images.throbber.gif", MediaTypeNames.Image.Gif)]