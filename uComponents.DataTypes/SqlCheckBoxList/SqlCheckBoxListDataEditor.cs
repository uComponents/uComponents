using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using uComponents.Core;
using umbraco;
using umbraco.DataLayer;
using umbraco.interfaces;

namespace uComponents.DataTypes.SqlCheckBoxList
{
	/// <summary>
	/// DataEditor for the SQL CheckBoxList data-type.
	/// </summary>
	public class SqlCheckBoxListDataEditor : CompositeControl, IDataEditor
	{
		/// <summary>
		/// Field for the data.
		/// </summary>
		private IData data;

		/// <summary>
		/// Field for the options.
		/// </summary>
		private SqlCheckBoxListOptions options;

		/// <summary>
		/// Field for the checkbox list.
		/// </summary>
		private CheckBoxList checkBoxList = new CheckBoxList();

		/// <summary>
		/// Initializes a new instance of SqlCheckBoxListDataEditor
		/// </summary>
		/// <param name="data"></param>
		/// <param name="options"></param>
		internal SqlCheckBoxListDataEditor(IData data, SqlCheckBoxListOptions options)
		{
			this.data = data;
			this.options = options;
		}

		/// <summary>
		/// Gets a value indicating whether [treat as rich text editor].
		/// </summary>
		/// <value>
		///     <c>true</c> if [treat as rich text editor]; otherwise, <c>false</c>.
		/// </value>
		public virtual bool TreatAsRichTextEditor
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether [show label].
		/// </summary>
		/// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
		public virtual bool ShowLabel
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets the editor.
		/// </summary>
		/// <value>The editor.</value>
		public Control Editor
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			using (var sqlHelper = this.options.GetSqlHelper())
			{
				var sql = this.options.Sql;
				var parameters = new List<IParameter>();

				if (sql.Contains("@currentId"))
					parameters.Add(sqlHelper.CreateParameter("@currentId", uQuery.GetIdFromQueryString()));

				using (var dataReader = sqlHelper.ExecuteReader(sql, parameters.ToArray()))
				{
					if (dataReader != null)
					{
						this.checkBoxList.DataSource = dataReader;
						this.checkBoxList.DataTextField = "Text";
						this.checkBoxList.DataValueField = "Value";
						this.checkBoxList.DataBind();
					}
				}
			}

			this.Controls.Add(this.checkBoxList);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.EnsureChildControls();

			if (!this.Page.IsPostBack && this.data.Value != null)
			{
				string value = this.data.Value.ToString();
				List<string> selectedValues = new List<string>();

				if (Helper.Xml.CouldItBeXml(value))
				{
					// build selected values from XML fragment
					foreach (XElement nodeXElement in XElement.Parse(value).Elements())
					{
						selectedValues.Add(nodeXElement.Value);
					}
				}
				else
				{
					// Assume a CSV source
					selectedValues = value.Split(Constants.Common.COMMA).ToList();
				}

				// Find checkboxes where values match the stored values and set to selected
				ListItem checkBoxListItem = null;
				foreach (string selectedValue in selectedValues)
				{
					checkBoxListItem = this.checkBoxList.Items.FindByValue(selectedValue);
					if (checkBoxListItem != null)
					{
						checkBoxListItem.Selected = true;
					}
				}
			}
		}

		/// <summary>
		/// Called by Umbraco when saving the node
		/// </summary>
		public void Save()
		{
			// Get all checked item values
			IEnumerable<string> selectedOptions = from ListItem item in this.checkBoxList.Items
												  where item.Selected
												  select item.Value;

			if (this.options.UseXml)
			{
				this.data.Value = new XElement("SqlCheckBoxList",
					selectedOptions.Select(x => new XElement("value", x.ToString()))).ToString();
			}
			else
			{
				// Save the CSV
				this.data.Value = string.Join(new string(Constants.Common.COMMA, 1), selectedOptions.ToArray());
			}
		}
	}
}
