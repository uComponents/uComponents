using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Umbraco.Core.Logging;

namespace uComponents.DataTypes.JsonDropdown
{
	/// <summary>
	/// The control for the JSON DropDown data-type.
	/// </summary>
	public class JD_Control : PlaceHolder
	{
		/// <summary>
		/// Gets or sets the options.
		/// </summary>
		/// <value>The options.</value>
		public JD_Options Options { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text value.</value>
		public string Text
		{
			get
			{
				return this.TextBoxControl.Value;
			}

			set
			{
				if (this.TextBoxControl == null)
				{
					this.TextBoxControl = new HiddenField();
				}

				this.TextBoxControl.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the CheckBox control.
		/// </summary>
		/// <value>The CheckBox control.</value>
		protected HiddenField TextBoxControl { get; set; }

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

			if (this.TextBoxControl == null)
			{
				this.TextBoxControl = new HiddenField();
				this.TextBoxControl.Value = string.Empty;
			}

			this.TextBoxControl.ID = this.TextBoxControl.ClientID;

			// add the control
			this.Controls.Add(this.TextBoxControl);
		}

		/// <summary>
		/// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			var jsonData = this.GetData();

			if (!string.IsNullOrEmpty(jsonData))
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Div);

				this.TextBoxControl.RenderControl(writer);

				writer.AddAttribute("id", "dropDown_" + this.ClientID);
				writer.RenderBeginTag(HtmlTextWriterTag.Select);
				writer.RenderBeginTag(HtmlTextWriterTag.Option);
				writer.RenderEndTag(); // </option>
				writer.RenderEndTag(); // </select>

				writer.RenderEndTag(); // </div>

				if (!string.IsNullOrEmpty(this.Options.Expression) && !this.Options.Expression.StartsWith(".") && !this.Options.Expression.StartsWith("["))
				{
					this.Options.Expression = "." + this.Options.Expression;
				}

				var scriptFunction = new StringBuilder();
				string selected = this.TextBoxControl.Value.Split('|')[0];

				scriptFunction.AppendFormat("var data_{0} = {1};", this.ClientID, jsonData);
				scriptFunction.Append("         jQuery.each(data_" + this.ClientID + this.Options.Expression + ", function (i, item) {");
				scriptFunction.Append("if(item." + this.Options.Key + " == '" + selected + "' ) {");
				scriptFunction.Append("             jQuery('#dropDown_" + this.ClientID + "').append($('<option selected=selected></option>').val(item." + this.Options.Key + ").html(item." + this.Options.Value + "));");
				scriptFunction.Append("}else{");
				scriptFunction.Append("             jQuery('#dropDown_" + this.ClientID + "').append($('<option></option>').val(item." + this.Options.Key + ").html(item." + this.Options.Value + "));");
				scriptFunction.Append(" }");
				scriptFunction.Append("         });");

				scriptFunction.Append("jQuery('#dropDown_" + this.ClientID + "').change(function(){");
				scriptFunction.Append("jQuery('#" + this.TextBoxControl.ClientID + "').val(jQuery('#dropDown_" + this.ClientID + " :selected').val() +'|'+ jQuery('#dropDown_" + this.ClientID + " :selected').html());");
				scriptFunction.Append("});");

				var javascript = string.Concat("<script type='text/javascript'>jQuery(window).load(function(){ ", scriptFunction.ToString(), " });</script>");
				writer.WriteLine(javascript);
			}
			else
			{
				writer.Write("<em>The JSON data could not be retrieved from the URL.</em>");
			}
		}

		/// <summary>
		/// Gets the data.
		/// </summary>
		/// <returns>Returns the data.</returns>
		private string GetData()
		{
			string pageResponse = string.Empty;
			string pageURL = this.Options.UrlToJson;

			try
			{
				var request = (HttpWebRequest)WebRequest.Create(pageURL);
				request.Method = "GET";

				using (var response = request.GetResponse())
				using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
				{
					pageResponse = reader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error(typeof(JD_Control), "uComponents.DataTypes.JsonDropdown.", ex);
			}

			return pageResponse;
		}
	}
}