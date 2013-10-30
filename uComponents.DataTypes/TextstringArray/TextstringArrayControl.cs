using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using uComponents.Core;
using umbraco;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;

[assembly: WebResource("uComponents.DataTypes.TextstringArray.TextstringArray.css", Constants.MediaTypeNames.Text.Css)]
[assembly: WebResource("uComponents.DataTypes.TextstringArray.TextstringArray.js", Constants.MediaTypeNames.Application.JavaScript)]

namespace uComponents.DataTypes.TextstringArray
{
	/// <summary>
	/// The <see cref="TextstringArrayControl"/> handles the presentation of the string arrays.
	/// </summary>
	[ValidationProperty("IsValid")]
	public class TextstringArrayControl : PlaceHolder
	{
		/// <summary>
		/// Field for the list of values.
		/// </summary>
		private List<string[]> values;

		/// <summary>
		/// The HiddenField to store the selected values.
		/// </summary>
		private HiddenField SelectedValues = new HiddenField();

		/// <summary>
		/// Gets or sets the options.
		/// </summary>
		/// <value>The options.</value>
		public TextstringArrayOptions Options { get; set; }

		/// <summary>
		/// Gets the value of IsValid.
		/// </summary>
		/// <value>Returns 'Valid' if valid, otherwise an empty string.</value>
		public string IsValid
		{
			get
			{
				if (!string.IsNullOrEmpty(this.Values))
				{
					return "Valid";
				}

				return string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the values.
		/// </summary>
		/// <value>The values.</value>
		public string Values
		{
			get
			{
				return this.SelectedValues.Value;
			}

			set
			{
				this.SelectedValues.Value = value;
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
		/// Add the resources (sytle/scripts)
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// Adds the client dependencies.
			this.RegisterEmbeddedClientResource("uComponents.DataTypes.Shared.Resources.Scripts.json2.js", ClientDependencyType.Javascript);
			this.RegisterEmbeddedClientResource("uComponents.DataTypes.TextstringArray.TextstringArray.css", ClientDependencyType.Css);
			this.RegisterEmbeddedClientResource("uComponents.DataTypes.TextstringArray.TextstringArray.js", ClientDependencyType.Javascript);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (!string.IsNullOrEmpty(this.Values))
			{
				// if the value is coming from a translation task, it will always be XML
				if (Helper.Xml.CouldItBeXml(this.Values))
				{
					this.values = DeserializeFromXml(this.Values);
				}
				else
				{
					// load the values into a string array/list.
					var deserializer = new JavaScriptSerializer();
					this.values = deserializer.Deserialize<List<string[]>>(this.Values);
				}

				// check the number of items per row
				for (var i = 0; i < this.values.Count; i++)
				{
					var row = this.values[i];
					if (row.Length < this.Options.ItemsPerRow)
					{
						// if cell count is less, then add more cells
						var diff = this.Options.ItemsPerRow - row.Length;
						var newCells = new string(Constants.Common.COMMA, diff - 1).Split(new[] { Constants.Common.COMMA }, StringSplitOptions.None);
						this.values[i] = (row ?? Enumerable.Empty<string>()).Concat(newCells).ToArray();
					}
					else if (row.Length > this.Options.ItemsPerRow)
					{
						// if cell count is greater, then remove extra cells
						var diff = row.Length - this.Options.ItemsPerRow;
						var tmp = new List<string>(row);
						tmp.RemoveRange(this.Options.ItemsPerRow, diff);
						this.values[i] = tmp.ToArray();
					}
				}
			}
			else
			{
				// initalise the string array/list.
				this.values = new List<string[]>();
			}

			var emptyRow = new string(Constants.Common.COMMA, this.Options.ItemsPerRow - 1).Split(new[] { Constants.Common.COMMA }, StringSplitOptions.None);

			// check the minimum number allowed, add extra fields.
			if (this.values.Count < this.Options.MinimumRows && this.Options.MinimumRows > 1)
			{
				for (int i = this.values.Count; i < this.Options.MinimumRows; i++)
				{
					this.values.Add(emptyRow);
				}
			}

			// check the maxmimum number allowed, remove the excess.
			if (this.values.Count > this.Options.MaximumRows && this.Options.MaximumRows > 0)
			{
				this.values.RemoveRange(this.Options.MaximumRows, this.values.Count - this.Options.MaximumRows);
			}

			// if there are no selected values...
			if (this.values.Count == 0)
			{
				// ... then add an empty string to display a single textstring row.
				this.values.Add(emptyRow);
			}
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.EnsureChildControls();

			// populate the control's attributes.
			this.SelectedValues.ID = this.SelectedValues.ClientID;

			// add the controls.
			this.Controls.Add(this.SelectedValues);
		}

		/// <summary>
		/// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "TextstringArray");
			writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			// render the header row
			if (this.Options.ShowColumnLabels)
			{
				var labels = this.Options.ColumnLabels.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

				if (labels != null && labels.Length > 0)
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Class, "textstring-header-row");
					writer.RenderBeginTag(HtmlTextWriterTag.Div);

					foreach (string label in labels)
					{
						var displayLabel = label;
						if (displayLabel.Length > 3 && displayLabel.StartsWith("[#") && displayLabel.EndsWith("]"))
						{
							var key = displayLabel.Substring(2, label.Length - 3);
							displayLabel = uQuery.GetDictionaryItem(key, key);
						}

						writer.AddAttribute(HtmlTextWriterAttribute.Class, "textstring-header-row-col");
						writer.RenderBeginTag(HtmlTextWriterTag.Div);
						writer.WriteLine(displayLabel);
						writer.RenderEndTag(); // </div> .textstring-header-row-col
					}

					writer.RenderEndTag();
				}
			}

			// loop through each value
			foreach (string[] row in this.values)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "textstring-row");
				writer.RenderBeginTag(HtmlTextWriterTag.Div);

				foreach (string value in row)
				{
					// input tag
					writer.AddAttribute(HtmlTextWriterAttribute.Class, "textstring-row-field");
					writer.RenderBeginTag(HtmlTextWriterTag.Div);
					writer.WriteLine("<input type='text' value='{0}' />", value.Replace("'", "&#39;"));
					writer.RenderEndTag(); // </div> .textstring-row-field
				}

				// append the add/remove buttons
				writer.WriteLine("<div class='textstring-row-edit'>");
				writer.WriteLine("<a href='#add' class='textstring-row-add' title='Add a new row'><img src='{0}/images/small_plus.png' /></a>", GlobalSettings.Path);
				writer.WriteLine("<a href='#remove' class='textstring-row-remove' title='Remove this row'><img src='{0}/images/small_minus.png' /></a>", GlobalSettings.Path);
				writer.WriteLine("</div>");

				if (!this.Options.DisableSorting)
					writer.WriteLine("<div class='textstring-row-sort' title='Re-order this row' style='background: url({0}/images/sort.png) no-repeat 0 2px;'></div>", GlobalSettings.Path);

				writer.RenderEndTag(); // </div> .textstring-row
			}

			this.SelectedValues.RenderControl(writer);

			writer.RenderEndTag(); // </div> .TextstringArray

			// add jquery window load event
			var javascriptMethod = string.Concat("new jQuery.textstringArray($('#", this.ClientID, "'), { hiddenId: '#", this.SelectedValues.ClientID, "', minimum: ", this.Options.MinimumRows, ", maximum: ", this.Options.MaximumRows, "});");
			var javascript = string.Concat("<script type='text/javascript'>jQuery(window).load(function(){", javascriptMethod, "});</script>");
			writer.WriteLine(javascript);
		}

		/// <summary>
		/// Deserializes the XML data into the string array.
		/// </summary>
		/// <param name="data">The data as an XML string.</param>
		/// <returns>Returns a string array.</returns>
		private static List<string[]> DeserializeFromXml(string data)
		{
			var list = new List<string[]>();
			var xml = new XmlDocument();
			xml.LoadXml(data);

			var xmlNodeList = xml.SelectNodes("/TextstringArray/values");
			if (xmlNodeList != null)
			{
				foreach (XmlNode node in xmlNodeList)
				{
					var value = new List<string>();
					var selectNodes = node.SelectNodes("value");
					if (selectNodes != null)
					{
						foreach (XmlNode child in selectNodes)
						{
							value.Add(child.InnerText);
						}
					}

					list.Add(value.ToArray());
				}
			}

			return list;
		}
	}
}