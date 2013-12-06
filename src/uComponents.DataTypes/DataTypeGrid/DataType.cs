// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 11.08.2011 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using umbraco.editorControls;
using umbraco.interfaces;

namespace uComponents.DataTypes.DataTypeGrid
{
    /// <summary>
    /// The dtg data type.
    /// </summary>
    public class DataType : umbraco.cms.businesslogic.datatype.BaseDataType, IDataType
    {
        #region Declarations

        /// <summary>
        /// The m_ data.
        /// </summary>
        private IData m_Data;

        /// <summary>
        /// The Data Editor for the data-type.
        /// </summary>
        private IDataEditor m_DataEditor;

        /// <summary>
        /// The PreValue Editor for the datatype.
        /// </summary>
        private PrevalueEditor m_PreValueEditor;

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the id of the data-type.
        /// </summary>
        /// <value>The id of the data-type.</value>
        public override Guid Id
        {
            get
            {
                return new Guid(DataTypeConstants.DataTypeGridId);
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
                return "uComponents: DataType Grid";
            }
        }

        /// <summary>
        /// Gets the prevalue editor.
        /// </summary>
        /// <value>The prevalue editor.</value>
        public override IDataPrevalue PrevalueEditor
        {
            get
            {
                return m_PreValueEditor ?? (m_PreValueEditor = new PrevalueEditor(this));
            }
        }

        /// <summary>
        /// Lazy load the associated DataEditor, 
        /// this is constructed supplying the data value stored by the PreValueEditor, and also the configuration settings of the PreValueEditor 
        /// </summary>
        public override IDataEditor DataEditor
        {
            get
            {
                return m_DataEditor ?? (m_DataEditor = new DataEditor(this.Data, ((PrevalueEditor)PrevalueEditor).Settings, this.DataTypeDefinitionId, Guid.NewGuid().ToString("N")));
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
                return m_Data ?? (m_Data = new XmlData(this));
            }
        }

        #endregion
    }
}