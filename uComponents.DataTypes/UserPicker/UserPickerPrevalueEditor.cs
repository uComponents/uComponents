using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;

namespace uComponents.DataTypes.UserPicker
{
	/// <summary>
	/// The PreValue Editor for the User Picker data-type.
	/// </summary>
	public class UserPickerPrevalueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
	{
		/// <summary>
		/// The RadioButtonList control to select the type of ListControl.
		/// </summary>
		private RadioButtonList ListControlType;

		/// <summary>
		/// The CheckBoxList control to select the types of users.
		/// </summary>
		private CheckBoxList UserTypesList;

        /// <summary>
        /// Gets the documentation URL.
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return string.Concat(base.DocumentationUrl, "/data-types/user-picker/");
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="UserPickerPrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public UserPickerPrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
			: base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Integer)
		{
		}

		/// <summary>
		/// Saves the data-type PreValue options.
		/// </summary>
		public override void Save()
		{
			// set the options
			var options = new UserPickerOptions(true);

			options.UseDropDown = this.ListControlType.SelectedIndex == 0;
			options.SelectedUserTypes = this.UserTypesList.Items.Cast<ListItem>().Where(i => i.Selected == true).Select(i => i.Value).ToArray();

			// save the options as JSON
			this.SaveAsJson(options);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.EnsureChildControls();
		}

		/// <summary>
		/// Creates child controls for this control
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// set-up child controls
			this.ListControlType = new RadioButtonList()
			{
				ID = "ListControlType",
				RepeatDirection = RepeatDirection.Vertical,
				RepeatLayout = RepeatLayout.Flow
			};

			this.UserTypesList = new CheckBoxList()
			{
				ID = "UserTypesList",
				RepeatDirection = RepeatDirection.Vertical,
				RepeatLayout = RepeatLayout.Flow
			};

			// populate the controls
			this.ListControlType.Items.Clear();
			this.ListControlType.Items.Add("DropDownList");
			this.ListControlType.Items.Add("RadioButtonList");

			this.UserTypesList.Items.Clear();
			foreach (var userType in UserType.GetAllUserTypes())
			{
				this.UserTypesList.Items.Add(new ListItem(userType.Name, userType.Alias));
			}

			// add the child controls
			this.Controls.AddPrevalueControls(this.ListControlType, this.UserTypesList);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// get PreValues, load them into the controls.
			var options = this.GetPreValueOptions<UserPickerOptions>();

			// no options? use the default ones.
			if (options == null)
			{
				options = new UserPickerOptions(true);
			}

			// set the values
			this.ListControlType.SelectedIndex = options.UseDropDown ? 0 : 1;

			if (options.SelectedUserTypes != null && options.SelectedUserTypes.Length > 0)
			{
				var selectedUserTypes = new List<string>(options.SelectedUserTypes);
				foreach (ListItem item in this.UserTypesList.Items)
				{
					if (selectedUserTypes.Contains(item.Value))
					{
						item.Selected = true;
					}
				}
			}
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			// add property fields
			writer.AddPrevalueRow("ListControl Type:", this.ListControlType);
			writer.AddPrevalueRow("User Type:", this.UserTypesList);
		}
	}
}
