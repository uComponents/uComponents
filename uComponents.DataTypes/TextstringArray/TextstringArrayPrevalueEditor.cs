using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using umbraco.editorControls;
using umbraco.editorControls.MultipleTextstring;

namespace uComponents.DataTypes.TextstringArray
{
	/// <summary>
	/// The PreValue Editor for the Textstring Array data-type.
	/// </summary>
	public class TextstringArrayPrevalueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
	{
		/// <summary>
		/// The TextBox control for the maximum rows value of the control.
		/// </summary>
		private TextBox TextBoxMaximumRows;

		/// <summary>
		/// The TextBox control for the minimum rows value of the control.
		/// </summary>
		private TextBox TextBoxMinimumRows;

		/// <summary>
		/// The CheckBox control for determining whether to show the column labels.
		/// </summary>
		private CheckBox CheckBoxShowColumnLabels;

		/// <summary>
		/// A &lt;div&gt; wrapper/container for the column labels control.
		/// </summary>
		private HtmlGenericControl MtcContainer;

		/// <summary>
		/// A MultipleTextstringControl for defining the column labels.
		/// </summary>
		private MultipleTextstringControl MtcColumnLabels;

		/// <summary>
		/// The CheckBox control for determining whether to disable row sorting.
		/// </summary>
		private CheckBox CheckBoxDisableSorting;

		/// <summary>
		/// Initializes a new instance of the <see cref="TextstringArrayPrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public TextstringArrayPrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
			: base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
		{
		}

		/// <summary>
		/// Gets the documentation URL.
		/// </summary>
		/// <value>
		/// The documentation URL.
		/// </value>
		public override string DocumentationUrl
		{
			get
			{
				return string.Concat(base.DocumentationUrl, "/data-types/textstring-array/");
			}
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public override void Save()
		{
			// set the options
			var options = new TextstringArrayOptions(true);

			// save the column label options
			options.ShowColumnLabels = this.CheckBoxShowColumnLabels.Checked;
			options.ColumnLabels = this.MtcColumnLabels.Values;

			// parse the number of items per row; based on the number of labels.
			int itemsPerRow = options.ColumnLabels.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length;
			if (itemsPerRow == 0)
			{
				itemsPerRow = 2;
			}

			options.ItemsPerRow = itemsPerRow;
			options.DisableSorting = this.CheckBoxDisableSorting.Checked;

			// parse the maximum rows
			int maximumRows;
			if (int.TryParse(this.TextBoxMaximumRows.Text, out maximumRows))
			{
				if (maximumRows == 0)
				{
					maximumRows = -1;
				}

				options.MaximumRows = maximumRows;
			}

			// parse the minimum rows
			int minimumRows;
			if (int.TryParse(this.TextBoxMinimumRows.Text, out minimumRows))
			{
				if (minimumRows == 0)
				{
					minimumRows = -1;
				}

				options.MinimumRows = minimumRows;
			}

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
			this.CheckBoxShowColumnLabels = new CheckBox() { ID = "ShowColumnLabels" };
			this.MtcColumnLabels = new MultipleTextstringControl() { ID = "ColumnLabels" };
			this.TextBoxMaximumRows = new TextBox() { ID = "MaximumRows", CssClass = "guiInputText" };
			this.TextBoxMinimumRows = new TextBox() { ID = "MinimumRows", CssClass = "guiInputText" };
			this.CheckBoxDisableSorting = new CheckBox() { ID = "CheckBoxDisableSorting" };

			// HACK: [LK] The 'MultipleTextstring.js' requires a wrapper container with a class of 'propertyItemContent'
			this.MtcContainer = new HtmlGenericControl("div");
			this.MtcContainer.Attributes.Add("class", "propertyItemContent");
			this.MtcContainer.Attributes.Add("style", "float: left;");
			this.MtcContainer.Controls.Add(this.MtcColumnLabels);

			// add the child controls
			this.Controls.AddPrevalueControls(
				this.CheckBoxShowColumnLabels,
				this.MtcContainer,
				this.TextBoxMaximumRows,
				this.TextBoxMinimumRows,
				this.CheckBoxDisableSorting);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// get PreValues, load them into the controls.
			var options = this.GetPreValueOptions<TextstringArrayOptions>();

			// no options? use the default ones.
			if (options == null)
			{
				options = new TextstringArrayOptions(true);
			}

			// set the values
			this.CheckBoxShowColumnLabels.Checked = options.ShowColumnLabels;
			this.MtcColumnLabels.Options = new MultipleTextstringOptions() { Minimum = options.ItemsPerRow };
			this.MtcColumnLabels.Values = options.ColumnLabels;

			this.TextBoxMaximumRows.Text = options.MaximumRows.ToString();
			this.TextBoxMinimumRows.Text = options.MinimumRows.ToString();
			this.CheckBoxDisableSorting.Checked = options.DisableSorting;
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddPrevalueHeading("Columns");
			writer.AddPrevalueRow("Show Column Labels?", "Enter a label for each column; you may also use dictionary item syntax (e.g. <code>[#dictionaryItem]</code>). (Optional - blank labels are acceptable)", this.CheckBoxShowColumnLabels);
			writer.AddPrevalueRow("Labels:", this.MtcContainer);

			writer.AddPrevalueHeading("Rows");
			writer.AddPrevalueRow("Minimum:", "Minimum number of rows to display.", this.TextBoxMinimumRows);
			writer.AddPrevalueRow("Maximum:", "Maximum number of rows to display. Use -1 for unlimited rows.", this.TextBoxMaximumRows);
			writer.AddPrevalueRow("Disable sorting?", "Disables the ability to sort the rows.", this.CheckBoxDisableSorting);
		}
	}
}