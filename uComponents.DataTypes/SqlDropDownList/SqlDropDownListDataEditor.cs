using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.property;
using umbraco.cms.businesslogic.web;
using umbraco.DataLayer;
using umbraco.interfaces;

namespace uComponents.DataTypes.SqlDropDownList
{
	using Umbraco.Web;

	/// <summary>
	/// XPath configurabale DropDownList Data Type
	/// </summary>
	public class SqlDropDownListDataEditor : CompositeControl, IDataEditor
	{
		/// <summary>
		/// Field for the data.
		/// </summary>
		private IData data;

		/// <summary>
		/// Field for the options.
		/// </summary>
		private SqlDropDownListOptions options;

		/// <summary>
		/// Field for the CustomValidator.
		/// </summary>
		private CustomValidator customValidator = new CustomValidator();

		/// <summary>
		/// Field for the DropDownList.
		/// </summary>
		private DropDownList dropDownList = new DropDownList();

		/// <summary>
		/// Gets a value indicating whether [treat as rich text editor].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [treat as rich text editor]; otherwise, <c>false</c>.
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
		/// Initializes a new instance of XPathCheckBoxListDataEditor
		/// </summary>
		/// <param name="data"></param>
		/// <param name="options"></param>
		internal SqlDropDownListDataEditor(IData data, SqlDropDownListOptions options)
		{
			this.data = data;
			this.options = options;
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
						this.dropDownList.DataSource = dataReader;
						this.dropDownList.DataTextField = "Text";
						this.dropDownList.DataValueField = "Value";
						this.dropDownList.DataBind();
					}
				}
			}

			// Add a default please select value
			this.dropDownList.Items.Insert(0, new ListItem(string.Concat(ui.Text("choose"), "..."), string.Empty));

			this.Controls.Add(this.customValidator);
			this.Controls.Add(this.dropDownList);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.EnsureChildControls();

			if (!this.Page.IsPostBack)
			{
				// Get selected items from Node Name or Node Id
				var dropDownListItem = this.dropDownList.Items.FindByValue(this.data.Value.ToString());
				if (dropDownListItem != null)
				{
					dropDownListItem.Selected = true;
				}
			}
		}

		/// <summary>
		/// Called by Umbraco when saving the node
		/// </summary>
		public void Save()
		{
			var property = new Property(((DefaultData)this.data).PropertyId);
			if (property.PropertyType.Mandatory && this.dropDownList.SelectedIndex == 0)
			{
				// Property is mandatory, but no value selected in the DropDownList
				this.customValidator.IsValid = false;

				var documentType = UmbracoContext.Current.Application.Services.ContentTypeService.GetContentType(property.PropertyType.ContentTypeId);
				var tab = documentType.PropertyGroups.FirstOrDefault(x => x.Id == property.PropertyType.PropertyTypeGroup);

				if (tab != null)
				{
					this.customValidator.ErrorMessage = ui.Text("errorHandling", "errorMandatory", new string[] { property.PropertyType.Name, tab.Name }, User.GetCurrent());
				}
			}

			this.data.Value = this.dropDownList.SelectedValue;
		}
	}
}