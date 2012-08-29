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

        ///// <summary>
        ///// Containg div
        ///// </summary>
        //private HtmlGenericControl div = new HtmlGenericControl("div");

        /// <summary>
        /// AutoComplete TextBox
        /// </summary>
        private TextBox autoCompleteTextBox = new TextBox();

        /// <summary>
        /// ul to inset the selected items
        /// </summary>
        private HtmlGenericControl ul = new HtmlGenericControl("ul");

        /// <summary>
        /// lists the selected values
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
            // TODO: could set these values here and then share sql autocomplete function between data-type instances
            //this.autoCompleteTextBox.Attributes.Add("data-currentId", uQuery.GetIdFromQueryString());
            //this.autoCompleteTextBox.Attributes.Add("data-dataTypeDefinitionId", this.DataTypeDefinitionId.ToString());

            // containing div so that css styles can be applied
            HtmlGenericControl div = new HtmlGenericControl("div");

            div.Attributes.Add("class", "sql-auto-complete");
            this.ul.Attributes.Add("class", "propertypane");
            this.autoCompleteTextBox.CssClass = "umbEditorTextField";

            div.Controls.Add(this.ul);
            div.Controls.Add(this.autoCompleteTextBox);
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

                        // make selection list sortable
                        jQuery('ul#" + this.ul.ClientID + @"').sortable({
                            axis: 'y',
                            update: function(event, ui) { 


                                // update the hidden field
                                //alert ('sorted');

                            }
                        });
                        
                        jQuery('input#" + this.autoCompleteTextBox.ClientID + @"').autocomplete({
                            
                            minLength: '" + options.LetterCount + @"',
                            
                            source: function(request, response) {
                                jQuery.ajax({
                                        type: 'GET',
                                        dataType: 'json',
                                        url: '/Base/" + DataTypeConstants.SqlAutoCompleteId + "/GetData/" + this.DataTypeDefinitionId.ToString() + "/" + uQuery.GetIdFromQueryString().ToString() + @"/' + encodeURI(request.term),
                                        success: function (data) {                                               
                                            response(data);
                                        }
                                });
                            },

                            open: function () {
                                jQuery('input#" + this.autoCompleteTextBox.ClientID + @"').autocomplete('widget').width(300);
                            },
    
                            autoFocus: true,

                            focus: function (event, ui) {
                                return false; // prevent the autocomplete text box from being populated with the value of the currenly highlighted item
                            },

                            select: function(event, ui) { 
                               
                                // is there an id with a matching data-value attribute ?

                                if(jQuery('ul#" + this.ul.ClientID + @" li[data-value=""' + ui.item.value + '""]').length == 0)
                                {
                                    jQuery('ul#" + this.ul.ClientID + @"')
                                        .append('<li data-value=""' + ui.item.value + '"">' + ui.item.label + '<a class=""delete"" title=""remove"" href=""javascript:void(0);"" onClick=""SqlAutoComplete.removeItem(this)""></a></li>');
                                }
                               
                                // return empty textbox                               
                                event.target.value = '';
                                return false;
                            }
                        });
                       


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
