using System;

using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.MultiPickerRelations
{
    /// <summary>
    /// This datatype will render a CheckBoxList where the options are defined by an XPath expression,
    /// </summary>
    public class MultiPickerRelationsDataType : BaseDataType, IDataType
    {
		/// <summary>
		/// 
		/// </summary>
        private MultiPickerRelationsPreValueEditor preValueEditor;

		/// <summary>
		/// 
		/// </summary>
		private IDataEditor dataEditor;

		/// <summary>
		/// 
		/// </summary>
		private IData data;

		/// <summary>
		/// Gets the name of the data type.
		/// </summary>
		/// <value>The name of the data type.</value>
		public override string DataTypeName { get { return "uComponents-Legacy: MultiPicker Relations"; } }

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public override Guid Id { get { return new Guid(DataTypeConstants.MultiPickerRelationsId); } }

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
                    this.preValueEditor = new MultiPickerRelationsPreValueEditor(this);
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
                    this.dataEditor = new MultiPickerRelationsDataEditor(this.Data, ((MultiPickerRelationsPreValueEditor)this.PrevalueEditor).Options);
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
                    this.data = new DefaultData(this);
                }
                return this.data;
            }
        }
    }
}