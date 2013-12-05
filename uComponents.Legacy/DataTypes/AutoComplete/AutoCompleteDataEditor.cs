using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data;
using System.Web;
using System.Xml;
using System.IO;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using umbraco.DataLayer;
using umbraco.BusinessLogic;
using umbraco.presentation.nodeFactory;
using umbraco.cms.businesslogic.relation;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.member;
using uComponents.Core;
using uComponents.Core.Extensions;
using uComponents.uQueryExtensions;

/*
    in the process of being rebuilt - instead of using an update panel, the autocomplete suggestions will be jQuery AJAX calls,
    and the selection list and sorting in client script. 
*/

//[assembly: WebResource("uComponents.Core.DataTypes.AutoComplete.css.autocomplete.css", "text/css", PerformSubstitution = true)]
//[assembly: WebResource("uComponents.Core.Shared.Resources.Styles.jquery-ui-1.8.4.custom.css", "text/css")]
[assembly: WebResource("uComponents.Core.Shared.Resources.Scripts.jquery-ui-1.8.4.custom.min.js", "text/javascript")]
namespace uComponents.DataTypes.AutoComplete
{
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Javascript, "ui/jquery.js", "UmbracoClient")] //ensure 1.4.2
    class AutoCompleteDataEditor : CompositeControl, IDataEditor
    {
        public virtual bool TreatAsRichTextEditor { get { return false; } }
        public virtual bool ShowLabel { get { return true; } }
        public Control Editor { get { return this; } }

        private IData data;
        private AutoCompleteOptions options;

        private Dictionary<int, string> sourceItems = new Dictionary<int, string>();
        private TextBox autoCompleteTextbox = new TextBox();
        private HiddenField selectedItemsHiddenField = new HiddenField();


        /// <summary>
        /// string to identify a particular instance of this datatype (there may be 2 per doc type or media or member for example... so append propertyId)
        /// </summary>
        private string InstanceIdentifier { get { return "[" + ((DefaultData)this.data).PropertyId.ToString() + "]"; } }



        //constructor
        public AutoCompleteDataEditor(IData data, AutoCompleteOptions options)
        {
            this.data = data;
            this.options = options;

            switch (uQuery.GetUmbracoObjectType(new Guid(this.options.TypeToPick)))
            {
                case uQuery.UmbracoObjectType.Document: this.sourceItems = uQuery.GetNodesByXPath(this.options.XPath).ToIntStringDictionary(); break;
                case uQuery.UmbracoObjectType.Media: this.sourceItems = uQuery.GetMediaByXPath(this.options.XPath).ToIntStringDictionary(); break;
                case uQuery.UmbracoObjectType.Member: this.sourceItems = uQuery.GetMembersByXPath(this.options.XPath).ToIntStringDictionary(); break;
            }
        
        }

        protected override void OnInit(EventArgs e) {
            
            base.OnInit(e);
            //this.AddResourceToClientDependency("uComponents.Core.Shared.Resources.Styles.jquery-ui-1.8.4.custom.css", ClientDependency.Core.ClientDependencyType.Css);
            this.AddResourceToClientDependency("uComponents.Core.Shared.Resources.Scripts.jquery-ui-1.8.4.custom.min.js", ClientDependency.Core.ClientDependencyType.Javascript);
        }

        protected override void OnLoad(EventArgs e) 
        { 
            base.OnLoad(e); 
        
            
        }


        protected override void CreateChildControls()
        {
            
            this.autoCompleteTextbox.Text = string.Empty;
            this.autoCompleteTextbox.CssClass = "umbEditorTextField";
            this.Controls.Add(this.autoCompleteTextbox);

            //this.selectedItemsHiddenField.ID = this.ClientID + "SelectedItemsHiddenField";
            this.Controls.Add(this.selectedItemsHiddenField);


            string jsAutocomplete = @"
                <script type='text/javascript'>

                    jQuery(document).ready(function(){
                                          
alert('boo');
var availableTags = ['ActionScript', 'AppleScript', 'Asp'];
$('#tags').autocomplete({
	source: availableTags
});


                        jQuery('#" + this.autoCompleteTextbox.ClientID + @"').autocomplete({ 
                            source: " + this.ScriptAutoCompleteData() + @" 
                        });
                    });
                

                </script>";


            this.Page.ClientScript.RegisterClientScriptBlock(typeof(AutoCompleteDataEditor), this.ClientID + "_autocomplete", jsAutocomplete, false);
        }



          
        private string ScriptAutoCompleteData() //this is just deserializing a collection to JSON
        {
            string delimiter = ", ";
            string autocompleteData = "[";

            foreach (KeyValuePair<int, string> keyValuePair in this.sourceItems)
            {
                if (keyValuePair.Value != null)//hack!
                {
                    autocompleteData += "{"; //start "
                    autocompleteData += "label: '" + keyValuePair.Value.Replace(delimiter, string.Empty) + "',";
                    autocompleteData += "id: '" + keyValuePair.Key.ToString() + "'";
                    autocompleteData += "},";  //end ",
                }
            }

            autocompleteData = autocompleteData.TrimEnd(',');
            autocompleteData += "]";

            return autocompleteData;
        }



        public void Save()
        {
            //get csv value from hidden field

            //this.data.Value = BuildCsv(this.SelectedItems);

     
        }


        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            
            writer.WriteLine("<label for='tags'>Tags: </label><input id='tags' />"); //DEBUG

        }



    }
}
