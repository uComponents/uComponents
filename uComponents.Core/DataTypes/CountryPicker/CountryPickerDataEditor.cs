using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using umbraco;
using uComponents.Core.Shared;

namespace uComponents.Core.DataTypes.CountryPicker
{
	/// <summary>
	/// The Data Editor for the Country Picker data-type.
	/// </summary>
	[ValidationProperty("IsValid")]
	public class CountryPickerDataEditor : Panel
	{
		/// <summary>
		/// The DropDownList control for the Country Picker.
		/// </summary>
		private DropDownList _countryDropDown;

		/// <summary>
		/// The ListBox control for the Country Picker.
		/// </summary>
		private ListBox _countryListBox;

		/// <summary>
		/// The ListControl for the Country Picker.
		/// </summary>
		private ListControl _lstControl;

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.InitListControl();
			this.Controls.Add(_lstControl);
		}

		/// <summary>
		/// Inits the list control.
		/// </summary>
		private void InitListControl()
		{
			this._lstControl = GetListControl();
			this.FillWithIsoCountries(_lstControl);
		}

		/// <summary>
		/// Gets the list control.
		/// </summary>
		/// <returns>Returns the list control.</returns>
		private ListControl GetListControl()
		{
			ListControl lst;

			if (this.IsMultiSelect)
			{
				this._countryListBox = new ListBox { CssClass = "umbEditorTextField", SelectionMode = ListSelectionMode.Multiple };
				lst = this._countryListBox;
			}
			else
			{
				this._countryDropDown = new DropDownList { CssClass = "umbEditorTextField" };
				lst = this._countryDropDown;
			}

			return lst;
		}

		/// <summary>
		/// Populates the list control with countries as given by ISO 4217.
		/// </summary>
		/// <param name="ctrl">The list control to populate.</param>
		/// <remarks>
		/// http://www.prolificnotion.co.uk/c-utility-method-to-populate-list-controls-with-all-countries-as-given-in-iso-3166-1/
		/// </remarks>
		public void FillWithIsoCountries(ListControl ctrl)
		{
			var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

			var countries = cultures.Select(cultureInfo => new RegionInfo(cultureInfo.LCID)).Select(regionInfo => regionInfo.DisplayName).ToList();

			var sorted = (from c in countries
						  select c).Distinct().OrderBy(n => n);

			if (!this.IsMultiSelect)
			{
				ctrl.Items.Add(new ListItem(ChooseText, string.Empty));
			}

			foreach (string country in sorted)
			{
				ctrl.Items.Add(new ListItem(country, country));
			}

			this.SetSelectedListItems(SelectedText);
		}

		/// <summary>
		/// Gets the value of IsValid.
		/// </summary>
		/// <value>Returns 'Valid' if valid, otherwise an empty string.</value>
		public string IsValid
		{
			get
			{
				if (!string.IsNullOrEmpty(this.SelectedValues) && this.SelectedValues != this.ChooseText)
				{
					return "Valid";
				}

				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the selected values.
		/// </summary>
		/// <value>The selected values.</value>
		public string SelectedValues
		{
			get
			{
				if (this.IsMultiSelect)
				{
					return this.GetMultiSelected();
				}

				return this._countryDropDown.SelectedItem.Text;
			}
		}

		/// <summary>
		/// Sets the selected list items.
		/// </summary>
		/// <param name="selectedValues">The selected values.</param>
		private void SetSelectedListItems(string selectedValues)
		{
			if (this._lstControl == null)
			{
				this.InitListControl();
			}

			if (this.IsMultiSelect)
			{
				string[] selected = selectedValues.Split(new char[] { ',' });
				foreach (var s in selected)
				{
					this.SetSelected(s, this._countryListBox);
				}
			}
			else
			{
				this.SetSelected(selectedValues, this._countryDropDown);
			}
		}

		/// <summary>
		/// Gets or sets the selected text.
		/// </summary>
		/// <value>The selected text.</value>
		public string SelectedText { get; set; }

		/// <summary>
		/// Gets the multi selected.
		/// </summary>
		/// <returns>Returns the multiple selection.</returns>
		private string GetMultiSelected()
		{
			var sb = new StringBuilder();

			foreach (ListItem li in this._countryListBox.Items.Cast<ListItem>().Where(li => li.Selected))
			{
				sb.Append(li.Text).Append(Settings.COMMA);
			}

			return sb.ToString().TrimEnd(new char[] { Settings.COMMA });
		}

		/// <summary>
		/// Sets the selected.
		/// </summary>
		/// <param name="s">The s.</param>
		/// <param name="lstControl">The LST control.</param>
		private void SetSelected(string s, ListControl lstControl)
		{
			var li = lstControl.Items.FindByText(s);
			if (li != null)
			{
				li.Selected = true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is multi select.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is multi select; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Boolean property for whether the selection is multiple.</remarks>
		public bool IsMultiSelect { get; set; }

		/// <summary>
		/// Gets or sets the choose text.
		/// </summary>
		/// <value>The choose text.</value>
		public string ChooseText { get; set; }
	}
}