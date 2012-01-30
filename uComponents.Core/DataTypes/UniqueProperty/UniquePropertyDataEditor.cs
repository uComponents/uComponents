using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using umbraco;

namespace uComponents.Core.DataTypes.UniqueProperty
{
    /// <summary>
    /// This datatype will render text box however on save it will check if field value is unique 
    /// by searching all content for same field alias to ensure value has not been set elsewhere good for SEO unique page titles
    /// </summary>
    public class UniquePropertyDataEditor : CompositeControl
    {
		/// <summary>
		/// 
		/// </summary>
        private TextBox _txtPropertyValue;

		/// <summary>
		/// 
		/// </summary>
		private CustomValidator _validTextBoxValue;

		/// <summary>
		/// 
		/// </summary>
		private const string DuplicatelegacyFieldXpath = "//node[@id!={0} and data[@alias='{1}']='{2}']";

		/// <summary>
		/// 
		/// </summary>
		private const string DuplicateXpath = "//*[@isDoc and @id != {0} and {1} = '{2}']";

		// TODO: [IM] pull warnings from resource file?

		/// <summary>
		/// 
		/// </summary>
		private const string ErrorMessage = "The value entered has already been entered for another page namely ";

		/// <summary>
		/// 
		/// </summary>
		private const string EndErrorMessage = " please change this value or value on other page.";

		/// <summary>
		/// 
		/// </summary>
		private const string PageLink = "<a href=\"editContent.aspx?id={0}\">{1}</a>";

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _txtPropertyValue.Text = Text;
            _txtPropertyValue.CssClass = "umbEditorTextField";
            _txtPropertyValue.ID = this.ID + "uniqueField";

            _validTextBoxValue = new CustomValidator {ControlToValidate = _txtPropertyValue.ID};
            _validTextBoxValue.ServerValidate += ValidPropertyValueServerValidate;
            Controls.Add(_txtPropertyValue);
            Controls.Add(new LiteralControl("<br/>"));
            Controls.Add(_validTextBoxValue);
        }

		/// <summary>
		/// Queries the XML data store for property.
		/// </summary>
		/// <returns></returns>
        private XmlNode QueryXmlDataStoreForProperty()
        {
			// TODO: [IM] use examine on internal index
            int currentDocId = int.Parse(HttpContext.Current.Request.QueryString["id"]);
            string xpath = string.Empty;

            if(UmbracoSettings.UseLegacyXmlSchema){
                xpath = string.Format(DuplicatelegacyFieldXpath, currentDocId,
                                         FieldAlias,
                                         _txtPropertyValue.Text);
            }
            else
            {
                xpath = string.Format(DuplicateXpath, currentDocId,
                                         FieldAlias,
                                         _txtPropertyValue.Text);
            }
            XmlNode node = content.Instance.XmlContent.SelectSingleNode(xpath);
            return node;
        }

		/// <summary>
		/// Valids the property value server validate.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        protected void ValidPropertyValueServerValidate(object source, ServerValidateEventArgs args)
        {
            //make examine call to check if property value is unique
            XmlNode node = QueryXmlDataStoreForProperty();
            args.IsValid = node == null ? true : false;
            if (node != null)
            {
                _validTextBoxValue.ErrorMessage = ErrorMessage + "\"" +
                                                  string.Format(PageLink, node.Attributes["id"].Value,
                                                                node.Attributes["nodeName"].Value) + "\"" +
                                                  EndErrorMessage;
                ;
            }
        }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
        public string Text
        {
            get { return _txtPropertyValue.Text; }
            set
            {
                if (_txtPropertyValue == null)
                    _txtPropertyValue = new TextBox();
                _txtPropertyValue.Text = value;
            }
        }

		/// <summary>
		/// Gets or sets the field alias.
		/// </summary>
		/// <value>The field alias.</value>
        public string FieldAlias { get; set; }
    }
}