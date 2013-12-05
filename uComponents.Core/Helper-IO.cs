using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using umbraco;
using umbraco.IO;

namespace uComponents.Core
{
	/// <summary>
	/// Generic helper methods
	/// </summary>
	internal static partial class Helper
	{
		/// <summary>
		/// IO helpers
		/// </summary>
		public static class IO
		{
			/// <summary>
			/// The locker object
			/// </summary>
			private static readonly object Locker = new object();

			/// <summary>
			/// Gets the names of all loaded assemblies.
			/// </summary>
			/// <returns>A list of assembly names.</returns>
			public static string[] GetAssemblyNames()
			{
				var assemblies = new List<string>();

				// check if the App_Code directory exists and has any files
				var appCode = new DirectoryInfo(IOHelper.MapPath("~/App_Code"));
				if (appCode.Exists && appCode.GetFiles().Length > 0)
				{
					assemblies.Add(appCode.Name);
				}

				// add assemblies from the /bin directory
				assemblies.AddRange(Directory.GetFiles(IOHelper.MapPath("~/bin"), "*.dll").Select(fileName => fileName.Substring(fileName.LastIndexOf('\\') + 1)));

				return assemblies.ToArray();
			}

			/// <summary>
			/// Gets the <see cref="Assembly"/> with the specified name.
			/// </summary>
			/// <remarks>Works in Medium Trust.</remarks>
			/// <param name="assemblyName">The <see cref="Assembly"/> name.</param>
			/// <returns>The <see cref="Assembly"/>.</returns>
			public static Assembly GetAssembly(string assemblyName)
			{
				var appTrust = GlobalSettings.ApplicationTrustLevel;
				if (appTrust == AspNetHostingPermissionLevel.Unrestricted)
				{
					AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnCurrentDomainReflectionOnlyAssemblyResolve;
				}

				if (string.Equals(assemblyName, "App_Code", StringComparison.InvariantCultureIgnoreCase))
				{
					return Assembly.Load(assemblyName);
				}

				var path = HostingEnvironment.MapPath(string.Concat("~/bin/", assemblyName));
				if (!string.IsNullOrEmpty(path))
				{
					if (appTrust == AspNetHostingPermissionLevel.Unrestricted)
					{
						return Assembly.ReflectionOnlyLoadFrom(path);
					}
					else
					{
						// Medium Trust support
						return Assembly.LoadFile(path);
					}
				}

				return null;
			}

			/// <summary>
			/// Gets the assembly informational version.
			/// </summary>
			/// <param name="assemblyName">Name of the assembly.</param>
			/// <returns>The value of the informational version.</returns>
			public static string GetAssemblyInformationalVersion(string assemblyName)
			{
				var assembly = GetAssembly(assemblyName);
				return GetAssemblyInformationalVersion(assembly);
			}

			/// <summary>
			/// Gets the assembly informational version.
			/// </summary>
			/// <param name="assembly">The assembly.</param>
			/// <returns>The value of the informational version.</returns>
			public static string GetAssemblyInformationalVersion(Assembly assembly)
			{
				if (assembly != null)
					return FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

				return string.Empty;
			}

			/// <summary>
			/// Ensures the folder exists.
			/// </summary>
			/// <param name="path">The path.</param>
			/// <returns>The folder info.</returns>
			public static DirectoryInfo EnsureFolderExists(string path)
			{
				if (!Directory.Exists(path))
				{
					lock (Locker)
					{
						if (!Directory.Exists(path))
						{
							var dir = new DirectoryInfo(path);
							dir.Create();
						}
					}
				}

				return new DirectoryInfo(path);
			}

			/// <summary>
			/// Ensures the file exists.
			/// </summary>
			/// <param name="path">The path.</param>
			/// <param name="content">The content.</param>
			/// <returns>The file info.</returns>
			public static FileInfo EnsureFileExists(string path, string content)
			{
				if (!File.Exists(path))
				{
					lock (Locker)
					{
						if (!File.Exists(path))
						{
							using (var writer = new StreamWriter(File.Create(path)))
							{
								writer.Write(content);
							}
						}
					}
				}

				return new FileInfo(path);
			}

			/// <summary>
			/// Called when [current domain assembly is resolved].
			/// </summary>
			/// <param name="sender">The sender.</param>
			/// <param name="args">The <see cref="ResolveEventArgs" /> instance containing the event data.</param>
			/// <returns>The resolved <see cref="Assembly"/>.</returns>
			private static Assembly OnCurrentDomainReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
			{
				return Assembly.ReflectionOnlyLoad(args.Name);
			}
		}
	}
}