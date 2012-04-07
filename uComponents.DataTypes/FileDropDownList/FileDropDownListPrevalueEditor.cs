using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;

namespace uComponents.DataTypes.FileDropDownList
{
	/// <summary>
	/// The PreValue Editor for the File DropDownList data-type.
	/// </summary>
	public class FileDropDownListPrevalueEditor : AbstractJsonPrevalueEditor
	{
		private CheckBox CheckBoxUseDirectories;

		/// <summary>
		/// The TextBox control for the directory path of the control.
		/// </summary>
		private TextBox TextBoxDirectory;

		/// <summary>
		/// The TextBox control for the file pattern of the control.
		/// </summary>
		private TextBox TextBoxSearchPattern;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileDropDownListPrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public FileDropDownListPrevalueEditor(BaseDataType dataType)
			: base(dataType, DBTypes.Nvarchar)
		{
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public override void Save()
		{
			// check if the 'search pattern' is empty, set it to wildcard.
			var searchPattern = this.TextBoxSearchPattern.Text;
			if (string.IsNullOrEmpty(searchPattern))
			{
				searchPattern = "*";
			}

			// set the options
			var options = new FileDropDownListOptions(true)
			{
				Directory = this.TextBoxDirectory.Text,
				SearchPattern = searchPattern,
				UseDirectories = this.CheckBoxUseDirectories.Checked
			};

			// save the options as JSON
			this.SaveAsJson(options);
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// set-up child controls
			this.TextBoxDirectory = new TextBox() { ID = "Directory", CssClass = "guiInputText guiInputStandardSize" };
			this.TextBoxSearchPattern = new TextBox() { ID = "SearchPattern", CssClass = "guiInputText" };
			this.CheckBoxUseDirectories = new CheckBox() { ID = "UseDirectories" };

			// add the child controls
			this.Controls.AddPrevalueControls(this.TextBoxDirectory, this.TextBoxSearchPattern, this.CheckBoxUseDirectories);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// get PreValues, load them into the controls.
			var options = this.GetPreValueOptions<FileDropDownListOptions>();

			// no options? use the default ones.
			if (options == null)
			{
				options = new FileDropDownListOptions(true);
			}

			// set the values
			this.TextBoxDirectory.Text = options.Directory;
			this.TextBoxSearchPattern.Text = options.SearchPattern;
			this.CheckBoxUseDirectories.Checked = options.UseDirectories;
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			// add property fields
			writer.AddPrevalueRow("Directory:", "The directory to list the files from, relative to the root of the website.", this.TextBoxDirectory);
			writer.AddPrevalueRow("Search Pattern:", "The pattern to match the files. e.g. <code>*.css</code>", this.TextBoxSearchPattern);
			writer.AddPrevalueRow("Use Directories:", "Select this option to display sub-directories instead of files.", this.CheckBoxUseDirectories);
		}
	}
}