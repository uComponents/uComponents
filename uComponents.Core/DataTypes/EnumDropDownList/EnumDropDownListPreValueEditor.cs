using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.XPath;
using System.Linq;
using System.Reflection;

using uComponents.Core.Shared;
using uComponents.Core.Shared.PrevalueEditors;

using umbraco.cms.businesslogic.datatype;

namespace uComponents.Core.DataTypes.EnumDropDownList
{
	class EnumDropDownListPreValueEditor : AbstractJsonPrevalueEditor
	{
		/// <summary>
		/// 
		/// </summary>
		private DropDownList assemblyDropDownList = new DropDownList();

		/// <summary>
		///
		/// </summary>
		private DropDownList enumsDropDownList = new DropDownList();

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
		/// Creates all of the controls and assigns all of their properties
		/// </summary>
		protected override void CreateChildControls()
		{
			this.assemblyDropDownList.ID = "assemblyDropDownList";
			this.assemblyDropDownList.AutoPostBack = true;
			this.assemblyDropDownList.SelectedIndexChanged += new EventHandler(this.AssemblyDropDownList_SelectedIndexChanged);

			// find all assemblies (*.dll)
			this.assemblyDropDownList.DataSource = System.IO.Directory.GetFiles(MapPathSecure("~/bin"), "*.dll").Select(fileName => fileName.Substring(fileName.LastIndexOf('\\') + 1));
			//this.assemblyDropDownList.DataSource = System.Threading.Thread.GetDomain().GetAssemblies().Where(assembly => !string.IsNullOrEmpty(assembly.Location) && assembly.Location.StartsWith(MapPathSecure("~/bin")));			
			//this.assemblyDropDownList.DataTextField = "ManifestModule";
			this.assemblyDropDownList.DataBind();

			this.assemblyDropDownList.Items.Insert(0, new ListItem("", "-1"));

			this.enumsDropDownList.ID = "enumsDropDownList";

			this.Controls.Add(this.assemblyDropDownList);
			this.Controls.Add(this.enumsDropDownList);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
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

		private void AssemblyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.SetSourceEnumDropDownList();
		}

		private void SetSourceEnumDropDownList()
		{
			// recreate the SourceEnum dropdownlist....
			if (!string.IsNullOrEmpty(this.assemblyDropDownList.SelectedValue))
			{
				try
				{
					Assembly assembly = Assembly.LoadFile(MapPathSecure("~/bin/" + this.assemblyDropDownList.SelectedValue));
					Type[] assemblyTypes = assembly.GetTypes().Where(type => type.IsEnum).ToArray();

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

				this.SaveAsJson(this.Options);  // Serialize to Umbraco database field
			}
		}

		/// <summary>
		/// Replaces the base class writer and instead uses the shared uComponents extension method, to inject consistant markup
		/// </summary>
		/// <param name="writer"></param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddPrevalueRow("Assembly", this.assemblyDropDownList);
			writer.AddPrevalueRow("Enum", this.enumsDropDownList);
		}

	}
}