using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;
using Umbraco.Core.IO;

namespace uComponents.DataTypes.FilePicker
{
	/// <summary>
	/// The PreValue Editor for the FilePicker data-type.
	/// </summary>
	public class FP_PrevalueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractPrevalueEditor
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
        /// Gets the documentation URL.
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return string.Concat(base.DocumentationUrl, "/data-types/file-picker/");
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="FP_PrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public FP_PrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
			: base()
		{
			this.m_DataType = dataType;
		}

		/// <summary>
		/// Gets the selected directory.
		/// </summary>
		/// <value>The selected directory.</value>
		public string SelectedDirectory
		{
			get
			{
				var vals = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);
				if (vals.Count >= 1)
				{
					return ((PreValue)vals[0]).Value;
				}
				else
				{
					return IOHelper.MapPath("~/");
				}
			}
		}

		/// <summary>
		/// Gets or sets the picked value.
		/// </summary>
		/// <value>The picked value.</value>
		protected TextBox RootDirectory { get; set; }

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
					((PreValue)vals[0]).Value = this.RootDirectory.Text;
					((PreValue)vals[0]).Save();
				}
				else
				{
					// insert
					PreValue.MakeNew(this.m_DataType.DataTypeDefinitionId, this.RootDirectory.Text);
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

			this.RootDirectory.Text = this.SelectedDirectory;
		}

		/// <summary>
		/// Creates child controls for this control
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// create controls
			this.RootDirectory = new TextBox() { ID = "SelectedDirectory", CssClass = "guiInputText guiInputStandardSize" };

			// add controls
			this.Controls.AddPrevalueControls(this.RootDirectory);
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddPrevalueRow("Directory:", "The directory to start the folder browser from; must be relative to the root of the website.", this.RootDirectory);
		}
	}
}
