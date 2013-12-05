using System;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco.editorControls;

namespace uComponents.DataTypes.EnumCheckBoxList
{
	class EnumCheckBoxListPreValueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
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
		/// Store an Xml fragment or a Csv
		/// </summary>
		private RadioButtonList storageTypeRadioButtonList = new RadioButtonList() { RepeatDirection = RepeatDirection.Vertical, RepeatLayout = RepeatLayout.Flow };

		/// <summary>
		/// Data object used to define the configuration status of this PreValueEditor
		/// </summary>
		private EnumCheckBoxListOptions options = null;

		/// <summary>
		/// Gets the options data object that represents the current state of this datatypes configuration
		/// </summary>
		internal EnumCheckBoxListOptions Options
		{
			get
			{
				if (this.options == null)
				{
					// Deserialize any stored settings for this PreValueEditor instance
					this.options = this.GetPreValueOptions<EnumCheckBoxListOptions>();

					// If still null, ie, object couldn't be de-serialized from PreValue[0] string value
					if (this.options == null)
					{
						// Create a new Options data object with the default values
						this.options = new EnumCheckBoxListOptions();
					}
				}
				return this.options;
			}
		}

		/// <summary>
		/// Initialize a new instance of EnumCheckBoxlistPreValueEditor
		/// </summary>
		/// <param name="dataType">EnumCheckBoxListDataType</param>
		public EnumCheckBoxListPreValueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
			: base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Nvarchar)
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
			this.assemblyDropDownList.DataSource = Helper.IO.GetAssemblyNames();
			this.assemblyDropDownList.DataBind();

			this.assemblyDropDownList.Items.Insert(0, new ListItem(string.Empty, "-1"));

			this.enumsDropDownList.ID = "enumsDropDownList";

			this.storageTypeRadioButtonList.ID = "storageTypeRadioButtonList";
			this.storageTypeRadioButtonList.Items.Add(new ListItem("Xml", bool.TrueString));
			this.storageTypeRadioButtonList.Items.Add(new ListItem("Csv", bool.FalseString));

			this.Controls.AddPrevalueControls(
				this.assemblyDropDownList,
				this.enumsDropDownList,
				this.storageTypeRadioButtonList);
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
				this.storageTypeRadioButtonList.SelectedIndex = this.Options.UseXml ? 0 : 1;
			}
		}

		private void AssemblyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.SetSourceEnumDropDownList();
		}

		private void SetSourceEnumDropDownList()
		{
			var value = this.assemblyDropDownList.SelectedValue;

			// recreate the SourceEnum dropdownlist...
			if (!string.IsNullOrEmpty(value))
			{
				try
				{
					var assembly = Helper.IO.GetAssembly(value);
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

				bool useXml;
				if (bool.TryParse(this.storageTypeRadioButtonList.SelectedValue, out useXml))
				{
					this.Options.UseXml = useXml;
				}

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
			writer.AddPrevalueRow("Storage Type", this.storageTypeRadioButtonList);
		}
	}
}