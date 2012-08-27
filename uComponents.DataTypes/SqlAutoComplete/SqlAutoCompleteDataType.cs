using System;
using umbraco.editorControls;
using umbraco.interfaces;


namespace uComponents.DataTypes.SqlAutoComplete
{
    public class SqlAutoCompleteDataType : umbraco.cms.businesslogic.datatype.BaseDataType, IDataType
    {

        private SqlAutoCompletePreValueEditor preValueEditor;

        private IDataEditor dataEditor;

        private IData data;

        private SqlAutoCompleteOptions options;

        private SqlAutoCompleteOptions Options
        {
            get
            {
                if (this.options == null)
                {
                    this.options = ((SqlAutoCompletePreValueEditor)this.PrevalueEditor).Options;
                }

                return this.options;
            }
        }

        public override string DataTypeName
        {
            get
            {
                return "uComponents: SQL AutoComplete";
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
                return new Guid(DataTypeConstants.SqlAutoCompleteId);
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
                    this.preValueEditor = new SqlAutoCompletePreValueEditor(this);
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
                    this.dataEditor = new SqlAutoCompleteDataEditor(this.Data, this.Options);
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
                    if (this.Options.UseXml)
                    {
                        // Storing an Xml fragment
                        this.data = new XmlData(this);
                    }
                    else
                    {
                        // Storing a Csv
                        this.data = new umbraco.cms.businesslogic.datatype.DefaultData(this);
                    }
                }

                return this.data;
            }
        }



    }
}
