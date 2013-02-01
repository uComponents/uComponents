using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using uComponents.Core;
using Umbraco.Core;
using uComponents.DataTypes.Shared.WebServices;
using Umbraco.Core.IO;

namespace uComponents.Installer
{
	/// <summary>
	/// The post-install dashboard control.
	/// </summary>
	public partial class uComponentsInstaller : UserControl
	{
		/// <summary>
		/// Gets the logo.
		/// </summary>
		/// <value>The logo.</value>
		protected string Logo
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(typeof(Constants), Constants.IconResourcePath);
			}
		}

		/// <summary>
		/// Handles the initialization event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				switch (assembly.FullName)
				{
					case "uComponents.DataTypes.RazorDataTypeModels":
						this.phRazorModelBinding.Visible = true;
						break;

					case "uComponents.UI":
						this.phUiModules.Visible = true;

						// bind the UI Modules options
						this.cblUiModules.DataSource = Settings.AppKeys_UiModules;
						this.cblUiModules.DataTextField = "Value";
						this.cblUiModules.DataValueField = "Key";
						this.cblUiModules.DataBind();

						break;

					case "uComponents.NotFoundHandlers":
						this.phNotFoundHandlers.Visible = true;

						// find and bind the NotFoundHandlers
						var notFoundHandlersTypes = assembly.GetTypes();
						if (notFoundHandlersTypes.Length > 0)
						{
							var notFoundHandlers =
								notFoundHandlersTypes.Where(
									type => string.Equals(type.Namespace, assembly.FullName) && type.FullName.StartsWith(assembly.FullName))
								                     .ToDictionary(
									                     type => type.FullName.Substring(assembly.FullName.Length + 1), type => type.Name);

							this.cblNotFoundHandlers.DataSource = notFoundHandlers;
							this.cblNotFoundHandlers.DataTextField = "Value";
							this.cblNotFoundHandlers.DataValueField = "Key";
							this.cblNotFoundHandlers.DataBind();
						}

						break;

					case "uComponents.XsltExtensions":
						this.phXsltExtensions.Visible = true;
						break;
				}
			}

			// disable the dashboard control checkbox
			try
			{
				var dashboardXml = XmlHelper.OpenAsXmlDocument(SystemFiles.DashboardConfig);
				if (dashboardXml.SelectSingleNode("//section[@alias = 'uComponentsInstaller']") != null)
				{
					this.Success.Visible = false;
					this.phDashboardControl.Visible = false;
				}
			}
			catch
			{
			}

			// TODO: [LK] Add the uComponents namespace to the Web.config (system.web/compilation/assemblies)
			// TODO: [LK] Add the uComponents.Controls namespace to the Web.config (system.web/pages/controls)
		}

		/// <summary>
		/// Handles the click event of the Activate button.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void btnActivate_Click(object sender, EventArgs e)
		{
			var successes = new List<string>();
			var failures = new List<string>();
			var xml = new XmlDocument();

			// Razor Model Binding
			try
			{
				xml.LoadXml(string.Format("<Action runat=\"install\" undo=\"true\" alias=\"uComponents_AddAppConfigKey\" key=\"{0}\" value=\"{1}\" />", Constants.AppKey_RazorModelBinding, (!this.cbDisableRazorModelBinding.Checked).ToString().ToLower()));
				umbraco.cms.businesslogic.packager.PackageAction.RunPackageAction(this.cbDisableRazorModelBinding.Text, "uComponents_AddAppConfigKey", xml.FirstChild);
				successes.Add(this.cbDisableRazorModelBinding.Text);
			}
			catch (Exception ex)
			{
				failures.Add(string.Concat(this.cbDisableRazorModelBinding.Text, " (", ex.Message, ")"));
			}

			// Not Found Handlers
			foreach (var item in this.cblNotFoundHandlers.Items.Cast<ListItem>().Where(item => item.Selected))
			{
				try
				{
					xml.LoadXml(string.Format("<Action runat=\"install\" undo=\"true\" alias=\"uComponents_Add404Handler\" assembly=\"uComponents.NotFoundHandlers\" type=\"{0}\" />", item.Value));
					umbraco.cms.businesslogic.packager.PackageAction.RunPackageAction(item.Text, "uComponents_Add404Handler", xml.FirstChild);
					successes.Add(item.Text);
				}
				catch (Exception ex)
				{
					failures.Add(string.Concat(item.Text, " (", ex.Message, ")"));
				}
			}

			// UI Modules
			if (this.cbUiModules.Checked)
			{
				try
				{
					xml.LoadXml("<Action runat=\"install\" undo=\"true\" alias=\"uComponents_AddHttpModule\" />");
					umbraco.cms.businesslogic.packager.PackageAction.RunPackageAction(this.cbUiModules.Text, "uComponents_AddHttpModule", xml.FirstChild);
					successes.Add(this.cbUiModules.Text);
				}
				catch (Exception ex)
				{
					failures.Add(string.Concat(this.cbUiModules.Text, " (", ex.Message, ")"));
				}

				// adds the appSettings keys for the UI modules (drag-n-drop and tray-peek)
				foreach (ListItem item in this.cblUiModules.Items)
				{
					try
					{
						xml.LoadXml(string.Format("<Action runat=\"install\" undo=\"true\" alias=\"uComponents_AddAppConfigKey\" key=\"{0}\" value=\"{1}\" />", item.Value, item.Selected.ToString().ToLower()));
						umbraco.cms.businesslogic.packager.PackageAction.RunPackageAction(item.Text, "uComponents_AddAppConfigKey", xml.FirstChild);
						successes.Add(item.Text);
					}
					catch (Exception ex)
					{
						failures.Add(string.Concat(item.Text, " (", ex.Message, ")"));
					}
				}
			}

			// Dashboard control
			if (this.cbDashboardControl.Checked)
			{
				const string title = "Dashboard control";
				xml.LoadXml("<Action runat=\"install\" undo=\"true\" alias=\"addDashboardSection\" dashboardAlias=\"uComponentsInstaller\"><section><areas><area>developer</area></areas><tab caption=\"uComponents: Activator\"><control addPanel=\"true\">/umbraco/plugins/uComponents/uComponentsInstaller.ascx</control></tab></section></Action>");
				umbraco.cms.businesslogic.packager.PackageAction.RunPackageAction(title, "addDashboardSection", xml.FirstChild);
				successes.Add(title);
			}

			// Shared web services
			var serviceFolder = Helper.IO.EnsureFolderExists(Path.Combine(DataTypes.Settings.BaseDir.FullName, "Shared", "WebServices"));
			Helper.IO.EnsureFileExists(Path.Combine(serviceFolder.FullName, "DictionaryService.asmx"), SharedServices.DictionaryService);

			// set the feedback controls to hidden
			this.Failure.Visible = this.Success.Visible = false;

			// display failure messages
			if (failures.Count > 0)
			{
				this.Failure.type = umbraco.uicontrols.Feedback.feedbacktype.error;
				this.Failure.Text = string.Concat("There were errors with the following components:<br />", string.Join("<br />", failures.ToArray()));
				this.Failure.Visible = true;
			}

			// display success messages
			if (successes.Count > 0)
			{
				this.Success.type = umbraco.uicontrols.Feedback.feedbacktype.success;
				this.Success.Text = string.Concat("Successfully installed the following components: ", string.Join(", ", successes.ToArray()));
				this.Success.Visible = true;
			}
		}
	}
}