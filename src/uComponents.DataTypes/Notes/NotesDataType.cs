using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.Notes
{
    /// <summary>
    /// A Notes data-type for Umbraco
    /// </summary>
    public class NotesDataType : AbstractDataEditor
    {
        /// <summary>
        /// The PreValue Editor for the data-type.
        /// </summary>
        private NotesPrevalueEditor m_PreValueEditor;

        /// <summary>
        /// The Data Editor for the data-type.
        /// </summary>
        private NotesDataEditor m_DataEditor;

        /// <summary>
        /// Gets the id of the data-type.
        /// </summary>
        /// <value>The id of the data-type.</value>
        public override Guid Id
        {
            get
            {
                return new Guid(DataTypeConstants.NotesId);
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
                return "uComponents: Notes";
            }
        }

        /// <summary>
        /// Gets the prevalue editor.
        /// </summary>
        /// <value>The prevalue editor.</value>
        public override IDataPrevalue PrevalueEditor
        {
            get { return this.m_PreValueEditor ?? (this.m_PreValueEditor = new NotesPrevalueEditor(this)); }
        }

        /// <summary>
        /// Gets the prevalue editor.
        /// </summary>
        /// <value>The prevalue editor.</value>
        public override IDataEditor DataEditor
        {
            get { return this.m_DataEditor ?? (this.m_DataEditor = new NotesDataEditor(((NotesPrevalueEditor)this.PrevalueEditor).GetPreValueOptions<NotesOptions>())); }
        }
    }
}
