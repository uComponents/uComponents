using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.macro;
using umbraco.editorControls.macrocontainer;
using umbraco.editorControls;

namespace uComponents.DataTypes.RenderMacro
{
	/// <summary>
	/// The PreValue Editor for the RenderMacro data-type.
	/// </summary>
	public class RenderMacroPrevalueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
	{
		/// <summary>
		/// Field for the allowed macros list.
		/// </summary>
		private List<string> AllowedMacros;

		/// <summary>
		/// Field for the MacroEditor.
		/// </summary>
		private MacroEditor MacroEditor;

		/// <summary>
		/// The Checkbox control to define whether to show the label for the data-type.
		/// </summary>
		private CheckBox ShowLabel;

		/// <summary>
		/// Field for the overriding CSS styles.
		/// </summary>
		private Literal Styles;

		/// <summary>
		/// Initializes a new instance of the <see cref="RenderMacroPrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public RenderMacroPrevalueEditor(RenderMacroDataType dataType)
			: base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
		{
		}

		/// <summary>
		/// Creates child controls for this control
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// get prevalues, load them into the controls.
			var options = this.GetPreValueOptions<RenderMacroOptions>() ?? new RenderMacroOptions(true);
			this.AllowedMacros = Macro.GetAll().Select(m => m.Alias).ToList();

			// if the value of the macro tag is empty, assign the default value.
			if (string.IsNullOrEmpty(options.MacroTag))
			{
				options.MacroTag = RenderMacroOptions.DEFAULT_MACRO_TAG;
			}

			// set-up child controls
			this.MacroEditor = new MacroEditor(options.MacroTag, this.AllowedMacros) { ID = "MacroEditor" };
			this.ShowLabel = new CheckBox() { ID = "ShowLabel", Checked = options.ShowLabel };
			this.Styles = new Literal() { ID = "Styles", Text = "<style type='text/css'>.macroeditor h4, .macroeditor .macroDelete {display:none;}</style>" };

			// add the child controls
			this.Controls.AddPrevalueControls(this.ShowLabel, this.MacroEditor, this.Styles);
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			// add property fields
			writer.AddPrevalueRow("Show label?", this.ShowLabel);
			writer.AddPrevalueRow("Macro Editor", this.MacroEditor);
			writer.AddPrevalueRow(string.Empty, "Please note: If a document is unpublished or is used on a non-content section (e.g. media, member or custom), then the root content node (usually your homepage) will be used as the context.", this.Styles);
		}

		/// <summary>
		/// Saves the data-type PreValue options.
		/// </summary>
		public override void Save()
		{
			// get the macro tag value
			var macroTag = this.MacroEditor.MacroTag;
			if (string.IsNullOrEmpty(macroTag))
			{
				macroTag = RenderMacroOptions.DEFAULT_MACRO_TAG;
			}

			// set the options
			var options = new RenderMacroOptions()
			{
				MacroTag = macroTag,
				ShowLabel = this.ShowLabel.Checked
			};

			// save the options as JSON
			this.SaveAsJson(options);
		}
	}
}