using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core.Shared;
using uComponents.Core.Shared.Extensions;
using uComponents.Core.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;

namespace uComponents.Core.DataTypes.EnumDropDownList
{
	/// <summary>
	/// Prevalue Editor for the Enum DropDownList data-type.
	/// </summary>
	public class EnumDropDownListPreValueEditor : AbstractJsonPrevalueEditor
	{
		/// <summary>
		/// DropDownList to list the assmeblies
		/// </summary>
		private DropDownList assemblyDropDownList = new DropDownList();

		/// <summary>
		/// DropDownList for the enumerables
		/// </summary>
		private DropDownList enumsDropDownList = new DropDownList();

        /// <summary>
        /// Option to set if the first data item should be used as the default value, or whether to add a "please select..." item
        /// </summary>
        private CheckBox defaultToFirstItemCheckBox = new CheckBox();

		/// <summary>
		/// Data object used to define the configuration status of this PreValueEditor
		/// </summary>
		private EnumDropDownListOptions options = null;

		/// <summary>
		/// Gets the options data object that represents the current state of this datatypes configuration
		/// </summary>
		internal EnumDropDownListOptions Options
		{
			get
			{
				if (this.options == null)
				{
					// Deserialize any stored settings for this PreValueEditor instance
					this.options = this.GetPreValueOptions<EnumDropDownListOptions>();

					// If still null, ie, object couldn't be de-serialized from PreValue[0] string value
					if (this.options == null)
					{
						// Create a new Options data object with the default values
						this.options = new EnumDropDownListOptions();
					}
				}

				return this.options;
			}
		}

		/// <summary>
		/// Initialize a new instance of EnumCheckBoxlistPreValueEditor
		/// </summary>
		/// <param name="dataType">EnumCheckBoxListDataType</param>
		public EnumDropDownListPreValueEditor(BaseDataType dataType)
			: base(dataType, DBTypes.Nvarchar)
		{
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			this.assemblyDropDownList.ID = "assemblyDropDownList";
			this.assemblyDropDownList.AutoPostBack = true;
			this.assemblyDropDownList.SelectedIndexChanged += new EventHandler(this.AssemblyDropDownList_SelectedIndexChanged);

			// find all assemblies (*.dll)
			this.assemblyDropDownList.DataSource = this.GetAssemblies();
			this.assemblyDropDownList.DataBind();

			this.assemblyDropDownList.Items.Insert(0, new ListItem(string.Empty, "-1"));

			this.enumsDropDownList.ID = "enumsDropDownList";

            this.defaultToFirstItemCheckBox.ID = "defaultToFirstItemCheckBox";

			this.Controls.AddPrevalueControls(this.assemblyDropDownList, this.enumsDropDownList, defaultToFirstItemCheckBox);
		}

		/// <summary>
		/// Gets the assemblies.
		/// </summary>
		/// <returns></returns>
		private string[] GetAssemblies()
		{
			var assemblies = new List<string>();

			// check if the App_Code directory has any files
			if (Directory.GetFiles(this.MapPathSecure("~/App_Code")).Length > 0)
			{
				assemblies.Add("App_Code");
			}

			// add assemblies from the /bin directory
			assemblies.AddRange(Directory.GetFiles(this.MapPathSecure("~/bin"), "*.dll").Select(fileName => fileName.Substring(fileName.LastIndexOf('\\') + 1)));

			return assemblies.ToArray();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!this.Page.IsPostBack)
			{
				// Read in stored configuration values
				this.assemblyDropDownList.SelectedValue = this.Options.Assembly;
				this.SetSourceEnumDropDownList();
				this.enumsDropDownList.SelectedValue = this.Options.Enum;
			}
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the AssemblyDropDownList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void AssemblyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.SetSourceEnumDropDownList();
		}

		/// <summary>
		/// Sets the source enum drop down list.
		/// </summary>
		private void SetSourceEnumDropDownList()
		{
			var value = this.assemblyDropDownList.SelectedValue;

			// recreate the SourceEnum dropdownlist....
			if (!string.IsNullOrEmpty(value))
			{
				try
				{
					Assembly assembly;
					if (string.Equals(value, "App_Code", StringComparison.InvariantCultureIgnoreCase))
					{
						assembly = Assembly.Load(value);
					}
					else
					{
						assembly = Assembly.Load(File.ReadAllBytes(this.MapPathSecure(string.Concat("~/bin/", value))));
					}
					
					var assemblyTypes = assembly.GetTypes().Where(type => type.IsEnum).ToArray();

					this.enumsDropDownList.DataSource = assemblyTypes;
					this.enumsDropDownList.DataBind();
				}
				catch
				{
					this.enumsDropDownList.Items.Clear();
				}
			}
			else
			{
				this.enumsDropDownList.Items.Clear();
			}

		}

		/// <summary>
		/// Saves the pre value data to Umbraco
		/// </summary>
		public override void Save()
		{
			if (this.Page.IsValid)
			{
				this.Options.Assembly = this.assemblyDropDownList.SelectedValue;
				this.Options.Enum = this.enumsDropDownList.SelectedValue;
                this.Options.DefaultToFirstItem = this.defaultToFirstItemCheckBox.Checked;

				this.SaveAsJson(this.Options);  // Serialize to Umbraco database field
			}
		}

		/// <summary>
		/// Replaces the base class writer and instead uses the shared uComponents extension method, to inject consistant markup
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddPrevalueRow("Assembly", this.assemblyDropDownList);
			writer.AddPrevalueRow("Enum", this.enumsDropDownList);
            writer.AddPrevalueRow("Default To First Item", this.defaultToFirstItemCheckBox);
		}
	}
}