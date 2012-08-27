using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using uComponents.Core;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;

[assembly: WebResource("uComponents.Core.Shared.DataTypes.ElasticTextBox.jquery.elastic.js", Constants.MediaTypeNames.Application.JavaScript)]

namespace uComponents.DataTypes.ElasticTextBox
{
	/// <summary>
	/// The PreValue Editor for the Elastic TextBox data-type.
	/// </summary>
	public class ETB_PrevalueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
	{
		/// <summary>
		/// The TextBox control a preview of the data-type.
		/// </summary>
		private TextBox Preview;

		/// <summary>
		/// Dummy text for the preview TextBox control.
		/// </summary>
		private string Sonnet = "From fairest creatures we desire increase,\nThat thereby beauty's rose might never die,\nBut as the riper should by time decease,\nHis tender heir might bear his memory:\nBut thou, contracted to thine own bright eyes,\nFeed'st thy light'st flame with self-substantial fuel,\nMaking a famine where abundance lies,\nThyself thy foe, to thy sweet self too cruel.\nThou that art now the world's fresh ornament\nAnd only herald to the gaudy spring,\nWithin thine own bud buriest thy content\nAnd, tender churl, makest waste in niggarding.\nPity the world, or else this glutton be,\nTo eat the world's due, by the grave and thee.";

		/// <summary>
		/// The TextBox control for the CSS applied to the data-type.
		/// </summary>
		private TextBox TextBoxCss;

		/// <summary>
		/// The TextBox control for the height of the data-type.
		/// </summary>
		private TextBox TextBoxHeight;

		/// <summary>
		/// The TextBox control for the width of the data-type.
		/// </summary>
		private TextBox TextBoxWidth;

		/// <summary>
		/// Initializes a new instance of the <see cref="ETB_PrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public ETB_PrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
			: base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
		{
		}

		/// <summary>
		/// Saves the data-type PreValue options.
		/// </summary>
		public override void Save()
		{
			// set the options
			var options = new ETB_Options(true)
			{
				Css = this.TextBoxCss.Text
			};

			// parse the height
			int height, width;
			if (int.TryParse(this.TextBoxHeight.Text, out height))
			{
				if (height == 0)
				{
					height = 400;
				}

				options.Height = height;
			}

			// parse the width
			if (int.TryParse(this.TextBoxWidth.Text, out width))
			{
				if (width == 0)
				{
					width = 490;
				}

				options.Width = width;
			}

			// save the options as JSON
			this.SaveAsJson(options);

			// re-add the CSS styles (again)
			this.Preview.Height = Unit.Pixel(options.Height);
			this.Preview.Width = Unit.Pixel(options.Width);
			this.Preview.Attributes.Add("style", options.Css);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.EnsureChildControls();

			// Adds the client dependencies.
			this.RegisterEmbeddedClientResource("uComponents.DataTypes.ElasticTextBox.Scripts.jquery.elastic.js", ClientDependencyType.Javascript);
		}

		/// <summary>
		/// Creates child controls for this control
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// set-up child controls
			this.TextBoxCss = new TextBox() { ID = "Css", CssClass = "guiInputText", Width = Unit.Pixel(500) };
			this.TextBoxHeight = new TextBox() { ID = "Height", CssClass = "guiInputText" };
			this.Preview = new TextBox() { ID = "Preview", Text = this.Sonnet };
			this.TextBoxWidth = new TextBox() { ID = "Width", CssClass = "guiInputText" };

			// add the child controls
			this.Controls.AddPrevalueControls(this.TextBoxCss, this.TextBoxHeight, this.Preview, this.TextBoxWidth);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// get PreValues, load them into the controls.
			var options = this.GetPreValueOptions<ETB_Options>();

			// no options? use the default ones.
			if (options == null)
			{
				options = new ETB_Options(true);
			}

			// set the values
			this.TextBoxCss.Text = options.Css;
			this.TextBoxHeight.Text = options.Height.ToString();
			this.Preview.TextMode = TextBoxMode.MultiLine;
			this.Preview.Attributes.Add("style", options.Css);
			this.Preview.Height = Unit.Pixel(options.Height);
			this.Preview.Width = Unit.Pixel(options.Width);
			this.TextBoxWidth.Text = options.Width.ToString();
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			// add property fields
			writer.AddPrevalueRow("Height:", this.TextBoxHeight);
			writer.AddPrevalueRow("Width:", this.TextBoxWidth);
			writer.AddPrevalueRow("CSS:", "You can use any inline CSS to style the Elastic TextBox.<br/>Note: When using the 'font-size' property, for best results, use 'px' values, ('em' and '%' values can render incorrectly).", this.TextBoxCss);

			// wrapper for the Elastic TextBox preview
			HtmlGenericControl div = new HtmlGenericControl("div");
			div.Attributes.Add("class", "ElasticTextBox");
			div.Controls.Add(this.Preview);
			writer.AddPrevalueRow("Preview:", div);

			// add jquery window load event to create the js elastic
			var javascriptDefaults = string.Format("var dimensions = ';height:{0}px;width:{1}px;'", this.TextBoxHeight.Text, this.TextBoxWidth.Text);
			var javascriptLoadElastic = string.Format("jQuery('#{0}').elastic();", this.Preview.ClientID);
			var javascriptLivePreview = string.Format("jQuery('#{1}').keyup(function(){{ jQuery('#{0}').attr('style', jQuery(this).val() + dimensions).elastic(); }});", this.Preview.ClientID, this.TextBoxCss.ClientID);
			var javascript = string.Concat("<script type='text/javascript'>jQuery(window).load(function(){", javascriptDefaults, Environment.NewLine, javascriptLoadElastic, Environment.NewLine, javascriptLivePreview, "});</script>");
			writer.WriteLine(javascript);
		}
	}
}
