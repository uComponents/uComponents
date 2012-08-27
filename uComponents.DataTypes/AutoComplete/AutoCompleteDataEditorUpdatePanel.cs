using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using uComponents.Core;

using umbraco;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.member;
using umbraco.editorControls;
using umbraco.interfaces;

[assembly: WebResource("uComponents.DataTypes.AutoComplete.Styles.autocomplete.css", Constants.MediaTypeNames.Text.Css, PerformSubstitution = true)]

namespace uComponents.DataTypes.AutoComplete
{
    /// <summary>
    /// this is the original version of the AutoComplete control, it's replacement (AutoCompleteDataEditor) will be JS and won't use an UpdatePanel
    /// </summary>
    internal class AutoCompleteDataEditorUpdatePanel : UpdatePanel, IDataEditor
    {
        private IData data;

        private AutoCompleteOptions options;

        private Dictionary<int, string> sourceItems = new Dictionary<int, string>();

        private Dictionary<int, string> selectedItems = new Dictionary<int, string>();

        private TextBox autoCompleteTextbox = new TextBox();

        private HiddenField selectedIdHiddenField = new HiddenField();

        private PlaceHolder selectedItemsPlaceHolder = new PlaceHolder();

        public virtual bool TreatAsRichTextEditor
        {
            get
            {
                return false;
            }
        }

        public virtual bool ShowLabel
        {
            get
            {
                return true;
            }
        }

        public Control Editor
        {
            get
            {
                return this;
            }
        }

        private uQuery.UmbracoObjectType TypeToPick
        {
            get
            {
                return uQuery.GetUmbracoObjectType(new Guid(this.options.TypeToPick));
            }
        }

        private Dictionary<int, string> SelectedItems
        {
            get
            {
                if (ViewState["SelectedItems"] == null)
                {
                    ViewState["SelectedItems"] = this.selectedItems;
                }

                return (Dictionary<int, string>)ViewState["SelectedItems"];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompleteDataEditorUpdatePanel"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="options">The options.</param>
        public AutoCompleteDataEditorUpdatePanel(IData data, AutoCompleteOptions options)
        {
            this.data = data;
            this.options = options;

            string xPath = this.options.XPath;

            switch (this.TypeToPick)
            {
                case uQuery.UmbracoObjectType.Document:
                    this.sourceItems = uQuery.GetNodesByXPath(xPath).ToNameIds();
                    break;
                case uQuery.UmbracoObjectType.Media:
                    this.sourceItems = uQuery.GetMediaByXPath(xPath).ToNameIds();
                    break;
                case uQuery.UmbracoObjectType.Member:
                    this.sourceItems = uQuery.GetMembersByXPath(xPath).ToNameIds();
                    break;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //if initial load of this control, then get selected items from data stored
            if (!this.Page.IsPostBack)
            {
                this.selectedItems = BuildIntStringDictionary(this.data.Value.ToString(), this.TypeToPick);
            }

            //setup the autocomplete textbox and add button
            this.BuildAutoCompleteAdd();

            //build label and delete button for each selected item
            this.BuildSeletedItemsDelete();
        }

        protected override void CreateChildControls()
        {
            //// base.CreateChildControls();
            //// this.EnsureChildControls();
        }

        #region helpers

        private static Dictionary<int, string> BuildIntStringDictionary(
            string csv, uQuery.UmbracoObjectType umbracoObjectType)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();

            BuildIntStringDictionary(csv, umbracoObjectType, ref dictionary);

            return dictionary;
        }

        private static void BuildIntStringDictionary(
            string csv, uQuery.UmbracoObjectType umbracoObjectType, ref Dictionary<int, string> collection)
        {
            //for each id, get content item, and add id and name to dictionary
            if (!string.IsNullOrEmpty(csv))
            {
                switch (umbracoObjectType)
                {
                    case uQuery.UmbracoObjectType.Document:
                        foreach (var node in uQuery.GetNodesByCsv(csv))
                        {
                            collection.Add(node.Id, node.Name);
                        }

                        break;

                    case uQuery.UmbracoObjectType.Media:
                        foreach (Media media in uQuery.GetMediaByCsv(csv))
                        {
                            collection.Add(media.Id, media.Text);
                        }

                        break;

                    case uQuery.UmbracoObjectType.Member:
                        foreach (Member member in uQuery.GetMembersByCsv(csv))
                        {
                            collection.Add(member.Id, member.Text);
                        }

                        break;
                }
            }
        }

        #endregion

        private static string BuildCsv(Dictionary<int, string> dictionary)
        {
            return string.Join(",", dictionary.Keys.Select(i => i.ToString()).ToArray());
        }

        private void BuildAutoCompleteAdd()
        {
            this.autoCompleteTextbox.Text = string.Empty;

            this.autoCompleteTextbox.CssClass = "umbEditorTextField";
            base.ContentTemplateContainer.Controls.Add(this.autoCompleteTextbox);

            this.selectedIdHiddenField.ID = string.Concat(this.ClientID, "SelectedIdHiddenField");
            base.ContentTemplateContainer.Controls.Add(this.selectedIdHiddenField);

            //if (!this.Page.IsPostBack) {BuildAutoCompletePageScript(); }
            BuildAutoCompletePageScript();

            Button addButton = new Button();
            addButton.Text = Helper.Dictionary.GetDictionaryItem("Add", "Add");
            addButton.Click += new EventHandler(this.AddButton_Click);
            base.ContentTemplateContainer.Controls.Add(addButton);

            // Script to attach jquery autocomplete to textbox
            string jsAutocomplete = @"  <script type='text/javascript'>
                                            jQuery(function(){
                                                jQuery('#"
                                    + this.autoCompleteTextbox.ClientID + @"')
                                                    .autocomplete("
                                    + this.ClientID + @"_autocompleteData, 
                                                        {
                                                            mustMatch:true, 
                                                            autoFill:true, 
                                                            formatItem:function(item){ return item.text; } 
                                                        })
                                                    .result(function (event, item)
                                                        { 
                                                            if (item) jQuery('#"
                                    + this.selectedIdHiddenField.ClientID + @"').val(item.id); 
                                                        });
                                            });
                                        </script>";

            ScriptManager.RegisterStartupScript(
                this, typeof(AutoCompleteDataEditorUpdatePanel), this.ClientID + "_autocomplete", jsAutocomplete, false);
        }

        private void BuildAutoCompletePageScript()
        {
            //build autocomplete data
            string delimiter = ", ";
            string autocompleteData = "<script type='text/javascript'>var " + this.ClientID + "_autocompleteData = [";

            foreach (KeyValuePair<int, string> keyValuePair in this.sourceItems)
            {
                if (keyValuePair.Value != null)
                {
                    autocompleteData += "{"; //start "
                    autocompleteData += "text: '"
                                        + keyValuePair.Value.Replace(delimiter, string.Empty).Replace("'", "\\'") + "',";
                    autocompleteData += "id: '" + keyValuePair.Key.ToString() + "'";
                    autocompleteData += "},"; //end ",
                }
            }

            autocompleteData = autocompleteData.TrimEnd(',');
            autocompleteData += "];</script>";
            this.Page.ClientScript.RegisterClientScriptBlock(
                autocompleteData.GetType(), this.ClientID + "_autocompleteData", autocompleteData);

            //add jquery autocomplete library
            //this.Page.ClientScript.RegisterClientScriptInclude("jquery.autocomlete.min", "/umbraco/plugins/AutoComplete/jquery.autocomplete.min.js");

            //updated to use the jquery.autocomplete.js script as found in the default Umbraco install
            this.Page.ClientScript.RegisterClientScriptInclude(
                "jquery.autocomplete", "/umbraco_client/Application/JQuery/jquery.autocomplete.js");

            //register autocomplete css
            //string autocompleteCss = "<link rel='stylesheet' href='/umbraco/plugins/AutoComplete/autocomplete.css'/>";
            //this.Page.ClientScript.RegisterClientScriptBlock(autocompleteCss.GetType(), "autocompleteCss", autocompleteCss);
            this.RegisterEmbeddedClientResource(
                "uComponents.DataTypes.AutoComplete.Styles.autocomplete.css", ClientDependencyType.Css);
        }

        /// <summary>
        /// builds the list of selected item controls, each with a delete button
        /// </summary>
        private void BuildSeletedItemsDelete()
        {
            this.selectedItemsPlaceHolder.Controls.Clear();

            foreach (KeyValuePair<int, string> selectedItem in this.SelectedItems)
            {
                this.BuildSelectedItem(selectedItem);
            }

            base.ContentTemplateContainer.Controls.Remove(this.selectedItemsPlaceHolder);
            base.ContentTemplateContainer.Controls.Add(this.selectedItemsPlaceHolder);
        }

        private void BuildSelectedItem(KeyValuePair<int, string> selectedItem)
        {
            Literal startLiteral = new Literal();
            startLiteral.Text = "<br/>";
            this.selectedItemsPlaceHolder.Controls.Add(startLiteral);

            Button deleteButton = new Button();
            deleteButton.ID = this.ClientID + selectedItem.Key.ToString() + "Button";
            deleteButton.Text = Helper.Dictionary.GetDictionaryItem("Delete", "Delete");
            deleteButton.CommandArgument = selectedItem.Key.ToString();
            deleteButton.Click += new EventHandler(this.DeleteButton_Click);

            this.selectedItemsPlaceHolder.Controls.Add(deleteButton);

            Literal endLiteral = new Literal();
            endLiteral.Text = "&nbsp;" + selectedItem.Value;
            this.selectedItemsPlaceHolder.Controls.Add(endLiteral);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            int id; //command arg of id to remove...
            if (int.TryParse(((System.Web.UI.WebControls.Button)sender).CommandArgument, out id))
            {
                this.DeleteSelectedItem(id);
                this.BuildSeletedItemsDelete();
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            int id; //id to add in hidden field
            if (int.TryParse(this.selectedIdHiddenField.Value, out id))
            {
                this.AddSelectedItem(new KeyValuePair<int, string>(id, this.autoCompleteTextbox.Text));
                this.BuildSeletedItemsDelete();
            }

            this.autoCompleteTextbox.Text = string.Empty;
        }

        private void AddSelectedItem(KeyValuePair<int, string> selectedItem)
        {
            if (!this.SelectedItems.ContainsKey(selectedItem.Key))
            {
                this.SelectedItems.Add(selectedItem.Key, selectedItem.Value);
                this.ViewState["SelectedItems"] = this.SelectedItems;
            }
        }

        private void DeleteSelectedItem(int id)
        {
            if (this.SelectedItems.ContainsKey(id))
            {
                this.SelectedItems.Remove(id);
                this.ViewState["SelectedItems"] = this.SelectedItems;
            }
        }

        public void Save()
        {
            this.data.Value = BuildCsv(this.SelectedItems);
        }
    }
}
