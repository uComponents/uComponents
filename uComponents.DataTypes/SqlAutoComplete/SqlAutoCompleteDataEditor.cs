using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using uComponents.Core;
using umbraco;
using umbraco.interfaces;
using System.Web;
using umbraco.editorControls;
using umbraco.cms.businesslogic.datatype;
using System.Web.UI.HtmlControls;
using DefaultData = umbraco.cms.businesslogic.datatype.DefaultData;

[assembly: WebResource("uComponents.DataTypes.SqlAutoComplete.SqlAutoComplete.css", Constants.MediaTypeNames.Text.Css)]
[assembly: WebResource("uComponents.DataTypes.SqlAutoComplete.SqlAutoComplete.js", Constants.MediaTypeNames.Application.JavaScript)]
namespace uComponents.DataTypes.SqlAutoComplete 
{
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Javascript, "ui/jqueryui.js", "UmbracoClient")]
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Css, "ui/ui-lightness/jquery-ui.custom.css", "UmbracoClient")]
    public class SqlAutoCompleteDataEditor : CompositeControl, IDataEditor
    {
        /// <summary>
        /// Field for the data.
        /// </summary>
        private IData data;

        /// <summary>
        /// Field for the options.
        /// </summary>
        private SqlAutoCompleteOptions options;

        /// <summary>
        /// Containing div that wraps all the other controls - hence only need this id to find the others
        /// </summary>
        private HtmlGenericControl div = new HtmlGenericControl("div");

        /// <summary>
        /// Stores the selected values
        /// </summary>
        private HiddenField selectedValuesHiddenField = new HiddenField();

        /// <summary>
        /// Initializes a new instance of SqlAutoCompleteDataEditor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="options"></param>
        internal SqlAutoCompleteDataEditor(IData data, SqlAutoCompleteOptions options)
        {
            this.data = data;
            this.options = options;
        }

        /// <summary>
        /// Gets the property / datatypedefinition id - used to identify the current instance
        /// </summary>
        private int DataTypeDefinitionId
        {
            get
            {
                if (this.data is XmlData)
                {
                    return ((XmlData)this.data).DataTypeDefinitionId;
                }
                else //if (this.data is DefaultData)
                {
                    return ((DefaultData)this.data).DataTypeDefinitionId;
                }
            }
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
            // ul list for the selected items
            HtmlGenericControl ul = new HtmlGenericControl("ul");

            // textbox to attach the js autocomplete functionality to
            TextBox autoCompleteTextBox = new TextBox();

            this.div.Attributes.Add("class", "sql-auto-complete");
            this.div.Attributes.Add("data-sql-autocomplete-id", DataTypeConstants.SqlAutoCompleteId); 
            this.div.Attributes.Add("data-datatype-definition-id", this.DataTypeDefinitionId.ToString());
            this.div.Attributes.Add("data-current-id", uQuery.GetIdFromQueryString());
            this.div.Attributes.Add("data-min-length", this.options.LetterCount.ToString());

            ul.Attributes.Add("class", "propertypane");
            autoCompleteTextBox.CssClass = "umbEditorTextField";

            div.Controls.Add(ul);
            div.Controls.Add(autoCompleteTextBox);
            div.Controls.Add(this.selectedValuesHiddenField);

            this.Controls.Add(div);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.EnsureChildControls();
            
            this.RegisterEmbeddedClientResource("uComponents.DataTypes.SqlAutoComplete.SqlAutoComplete.css", ClientDependencyType.Css);
            this.RegisterEmbeddedClientResource("uComponents.DataTypes.SqlAutoComplete.SqlAutoComplete.js", ClientDependencyType.Javascript);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(@"                
                <script language='javascript' type='text/javascript'>

                    $(document).ready(function () {
                    
                        SqlAutoComplete.init(jQuery('div#" + this.div.ClientID + @"'));

                    });

                </script>");

            ScriptManager.RegisterStartupScript(this, typeof(SqlAutoCompleteDataEditor), this.ClientID + "_init", stringBuilder.ToString(), false);

            // setup
            if (!this.Page.IsPostBack)
            {
                // put the options obj into cache so that the /base method can request it (where the sql statment is being used)
                HttpContext.Current.Cache[DataTypeConstants.SqlAutoCompleteId + "_" + this.DataTypeDefinitionId.ToString()] = this.options;


            }


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

                //TODO: populate selection list with stored values
			}
		}

		/// <summary>
		/// Called by Umbraco when saving the node
		/// </summary>
		public void Save()
		{
            //// Get all checked item values
            IEnumerable<string> selectedOptions = this.selectedValuesHiddenField.Value.Split(',');
                               
            if (this.options.UseXml)
            {
                this.data.Value = new XElement("SqlAutoComplete",
                    selectedOptions.Select(x => new XElement("value", x.ToString()))).ToString();
            }
            else
            {
                // Save the CSV
                this.data.Value = string.Join(",", selectedOptions.ToArray());
            }
		}    
    }
}
