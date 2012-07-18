using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;

namespace uComponents.DataTypes.CountryPicker
{
	/// <summary>
	/// The PreValue Editor for the UniqueProperty data-type.
	/// </summary>
	public class CountryPickerPreValueEditor : AbstractPrevalueEditor
	{
		/// <summary>
		/// The underlying base data-type.
		/// </summary>
		private readonly umbraco.cms.businesslogic.datatype.BaseDataType m_DataType;

		/// <summary>
		/// An object to temporarily lock writing to the database.
		/// </summary>
		private static readonly object m_Locker = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="CountryPickerPreValueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public CountryPickerPreValueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
		{
			this.m_DataType = dataType;
		}

		/// <summary>
		/// Gets the type of the selected picker.
		/// </summary>
		/// <value>The type of the selected picker.</value>
		public string SelectedPickerType
		{
			get
			{
				var vals = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);

				if (vals.Count > 0)
				{
					return ((PreValue)vals[0]).Value;
				}
				else
				{
					// return default
					return string.Empty;
				}
			}
		}

		/// <summary>
		/// Gets the choose text.
		/// </summary>
		/// <value>The choose text.</value>
		public string ChooseText
		{

			get
			{
				var vals = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);

				if (vals.Count > 1)
				{
					return ((PreValue)vals[1]).Value;
				}
				else
				{
					// return default:
					return ui.Text("general", "choose");
				}
			}
		}

		/// <summary>
		/// Gets or sets the properties dropdown list.
		/// </summary>
		/// <value>The properties dropdown list.</value>
		private DropDownList _pickerType { get; set; }

		/// <summary>
		/// The TextBox control for the 'choose' text.
		/// </summary>
		private TextBox _chooseTextBox;

		/// <summary>
		/// Saves the data for this instance of the PreValue Editor.
		/// </summary>
		public override void Save()
		{
			this.m_DataType.DBType = umbraco.cms.businesslogic.datatype.DBTypes.Ntext;

			lock (m_Locker)
			{
				var vals = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);
				if (vals.Count >= 1)
				{
					// update
					((PreValue)vals[0]).Value = this._pickerType.SelectedValue;

					((PreValue)vals[0]).Save();
				}
				else
				{
					// insert
					PreValue.MakeNew(this.m_DataType.DataTypeDefinitionId, this._pickerType.SelectedValue);
				}

				// store the xpath
				if (vals.Count >= 2)
				{
					// update
					((PreValue)vals[1]).Value = this._chooseTextBox.Text;
					((PreValue)vals[1]).Save();
				}
				else
				{
					// insert
					PreValue.MakeNew(this.m_DataType.DataTypeDefinitionId, this._chooseTextBox.Text);
				}

			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this._pickerType.SelectedValue = this.SelectedPickerType;
			this._chooseTextBox.Text = this.ChooseText;
		}

		/// <summary>
		/// Creates child controls for this control
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// set-up controls
			this._pickerType = new DropDownList() { ID = "PickerType" };

			this._chooseTextBox = new TextBox { ID = "ChooseText", Text = ui.Text("general", "choose") };

			// populate controls
			this._pickerType.Items.Add(new ListItem("DropDownList", "DropDownList"));
			this._pickerType.Items.Add(new ListItem("ListBox", "ListBox"));

			// add controls
			this.Controls.AddPrevalueControls(this._pickerType, this._chooseTextBox);
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddPrevalueRow("Country picker type:", "Please type of picker to render for country list", this._pickerType);
			writer.AddPrevalueRow("Dropdown choose text:", "If dropdown then text to prompt user to choose", this._chooseTextBox);
		}
	}
}