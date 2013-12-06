using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using Umbraco.Core.IO;

namespace uComponents.DataTypes.FileDropDownList
{
	/// <summary>
	/// The control for the File DropDownList data-type.
	/// </summary>
	[ValidationProperty("IsValid")]
	public class FileDropDownListControl : PlaceHolder
	{
		/// <summary>
		/// The ListControl control for the File DropDownList.
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
		public FileDropDownListOptions Options { get; set; }

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

			try
			{
				// create the controls
				this.m_ListControl = new DropDownList();

				// clear all the list items
				this.m_ListControl.Items.Clear();
				this.m_ListControl.Items.Add(new ListItem(string.Concat(ui.Text("choose"), "..."), string.Empty));

				// get the files (or sub-directories)
				string[] files;

				if (this.Options.UseDirectories)
				{
					files = Directory.GetDirectories(IOHelper.MapPath(this.Options.Directory), this.Options.SearchPattern);
				}
				else
				{
					files = Directory.GetFiles(IOHelper.MapPath(this.Options.Directory), this.Options.SearchPattern);
				}

				// check there are files
				if (files != null && files.Length > 0)
				{
					// loop through each of the files
					foreach (var file in files)
					{
						// add the file to the dropdown list
						this.m_ListControl.Items.Add(new ListItem(Path.GetFileName(file)));
					}
				}

				// add the list-control to the placeholder
				this.Controls.Add(this.m_ListControl);

				// set the value of the list control.
				this.m_ListControl.SelectedValue = this.selectedValue;
			}
			catch (Exception ex)
			{
				// display an error message.
				this.Controls.Add(new LiteralControl("<em>The selected directory does not exist.</em>"));
				this.Controls.Add(new LiteralControl(string.Concat("<div style='display:none;white-space:pre;width:500px;'><small>", ex, "</small></div>")));
			}
		}
	}
}
