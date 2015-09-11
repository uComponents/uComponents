using System;
using umbraco.editorControls;
using umbraco.interfaces;


namespace uComponents.DataTypes.XPathAutoComplete
{
    /// <summary>
    /// XPath AutoComplete data-type
    /// </summary>
    public class XPathAutoCompleteDataType : umbraco.cms.businesslogic.datatype.BaseDataType, IDataType
    {

        private XPathAutoCompletePreValueEditor preValueEditor;

        private IDataEditor dataEditor;

        private IData data;

        private XPathAutoCompleteOptions options;

        private XPathAutoCompleteOptions Options
        {
            get
            {
                if (this.options == null)
                {
                    this.options = ((XPathAutoCompletePreValueEditor)this.PrevalueEditor).Options;
                }

                return this.options;
            }
        }

        /// <summary>
        /// Gets the name of the data type.
        /// </summary>
        /// <value>The name of the data type.</value>
        public override string DataTypeName
        {
            get
            {
                return "uComponents: XPath AutoComplete";
            }
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        public override Guid Id
        {
            get
            {
                return new Guid(DataTypeConstants.XPathAutoCompleteId);
            }
        }

        /// <summary>
        /// Lazy load the associated PreValueEditor instance, 
        /// this is constructed supplying 'this'
        /// </summary>
        public override IDataPrevalue PrevalueEditor
        {
            get
            {
                if (this.preValueEditor == null)
                {
                    this.preValueEditor = new XPathAutoCompletePreValueEditor(this);
                }

                return this.preValueEditor;
            }
        }

        /// <summary>
        /// Lazy load the assocated DataEditor, 
        /// this is constructed supplying the data value stored by the PreValueEditor, and also the configuration settings of the PreValueEditor 
        /// </summary>
        public override IDataEditor DataEditor
        {
            get
            {
                if (this.dataEditor == null)
                {
                    this.dataEditor = new XPathAutoCompleteDataEditor(this.Data, this.Options);
                }

                return this.dataEditor;
            }
        }

        /// <summary>
        /// Lazy load an empty DefaultData object, this is used to pass data between the PreValueEditor and the DataEditor
        /// </summary>
        public override IData Data
        {
            get
            {
                if (this.data == null)
                {
                    // Storing an Xml fragment
                    this.data = new XmlData(this);
                }

                return this.data;
            }
        }
    }
}
