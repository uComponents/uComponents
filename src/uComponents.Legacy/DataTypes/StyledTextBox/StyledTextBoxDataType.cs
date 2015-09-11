using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.StyledTextBox
{
    /// <summary>
    /// Data Type for the Styled TextBox
    /// </summary>
    public class StyledTextBoxDataType : BaseDataType, IDataType
    {
        /// <summary>
        /// 
        /// </summary>
        private IDataEditor _editor;

        /// <summary>
        /// 
        /// </summary>
        private IData _baseData;

        /// <summary>
        /// 
        /// </summary>
        private StyledTextBoxPrevalueEditor _prevalueEditor;

        /// <summary>
        /// Gets the data editor.
        /// </summary>
        /// <value>The data editor.</value>
        public override IDataEditor DataEditor
        {
            get
            {
                if (_editor == null)
                    _editor = new StyledTextBoxDataEditor(Data, ((StyledTextBoxPrevalueEditor)PrevalueEditor).Configuration);
                return _editor;
            }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public override IData Data
        {
            get
            {
                if (_baseData == null)
                    _baseData = new umbraco.cms.businesslogic.datatype.DefaultData(this);
                return _baseData;
            }
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        public override Guid Id
        {
            get { return new Guid(DataTypeConstants.StyledTextBoxId); }
        }

        /// <summary>
        /// Gets the name of the data type.
        /// </summary>
        /// <value>The name of the data type.</value>
        public override string DataTypeName
        {
            get { return "uComponents-Legacy: Styled TextBox"; }
        }

        /// <summary>
        /// Gets the prevalue editor.
        /// </summary>
        /// <value>The prevalue editor.</value>
        public override IDataPrevalue PrevalueEditor
        {
            get
            {
                if (_prevalueEditor == null)
                    _prevalueEditor = new StyledTextBoxPrevalueEditor(this);
                return _prevalueEditor;
            }
        }
    }
}
