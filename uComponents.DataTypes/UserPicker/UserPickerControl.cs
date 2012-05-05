using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using umbraco;
using umbraco.BusinessLogic;

namespace uComponents.DataTypes.UserPicker
{
	/// <summary>
	/// The control for the User Picker data-type.
	/// </summary>
	[ValidationProperty("IsValid")]
	public class UserPickerControl : PlaceHolder
	{
		/// <summary>
		/// The ListControl control for the User Picker.
		/// </summary>
		private ListControl m_ListControl;

		/// <summary>
		/// Field for the SelectedValue
		/// </summary>
		private string selectedValue;

		/// <summary>
		/// Gets the value of IsValid.
		/// </summary>
		/// <value>Returns 'Valid' if valid, otherwise an empty string.</value>
		public string IsValid
		{
			get
			{
				if (!string.IsNullOrEmpty(this.SelectedValue) && this.SelectedValue != "-1")
				{
					return "Valid";
				}

				return string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the options.
		/// </summary>
		/// <value>The options.</value>
		public UserPickerOptions Options { get; set; }

		/// <summary>
		/// Gets or sets the selected value.
		/// </summary>
		/// <value>The selected value.</value>
		public string SelectedValue
		{
			get
			{
				if (this.m_ListControl != null)
				{
					return this.m_ListControl.SelectedValue;
				}

				return string.Empty;
			}
			set
			{
				this.selectedValue = value;
			}
		}

		/// <summary>
		/// Initialize the control, make sure children are created
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.EnsureChildControls();
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.EnsureChildControls();

			// create the controls
			if (this.Options.UseDropDown)
			{
				this.m_ListControl = new DropDownList();
			}
			else
			{
				this.m_ListControl = new RadioButtonList();
			}

			// clear all the list items
			this.m_ListControl.Items.Clear();

			if (this.Options.UseDropDown)
			{
				this.m_ListControl.Items.Add(new ListItem(string.Concat(ui.Text("choose"), "..."), "-1"));
			}

			// get all users
			var users = User.getAll();

			// get the selected user types
			var selectedUserTypes = new List<string>(this.Options.SelectedUserTypes);

			// loop through each user
			foreach (User user in users)
			{
				// check if the user is a selected user-type
				if (selectedUserTypes.Contains(user.UserType.Alias))
				{
					// add the user to the dropdown list
					this.m_ListControl.Items.Add(new ListItem(user.Name, user.Id.ToString()));
				}
			}

			// add the list-control to the placeholder
			this.Controls.Add(this.m_ListControl);

			// set the value of the list control.
			this.m_ListControl.SelectedValue = this.selectedValue;
		}
	}
}
