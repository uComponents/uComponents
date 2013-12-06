using System;

namespace uComponents.DataTypes.RelatedLinksWithMedia
{
    /// <summary>
    /// The RelatedLinksWithMedia data-type.
    /// </summary>
    public class RelatedLinkswMediaDataType : umbraco.cms.businesslogic.datatype.BaseDataType, umbraco.interfaces.IDataType
    {
        /// <summary>
        /// 
        /// </summary>
        private umbraco.interfaces.IDataEditor _Editor;

        /// <summary>
        /// 
        /// </summary>
        private umbraco.interfaces.IData _baseData;

        /// <summary>
        /// 
        /// </summary>
        private RelatedLinkswMediaPrevalueEditor _prevalueeditor;

        /// <summary>
        /// Gets the data editor.
        /// </summary>
        /// <value>The data editor.</value>
        public override umbraco.interfaces.IDataEditor DataEditor
        {
            get
            {

                if (_Editor == null)
                    _Editor = new RelatedLinkswMediaDataEditor(Data, ((RelatedLinkswMediaPrevalueEditor)PrevalueEditor).Configuration);
                return _Editor;
            }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public override umbraco.interfaces.IData Data
        {
            get
            {
                if (_baseData == null)
                    _baseData = new RelatedLinkswMediaData(this);
                return _baseData;
            }
        }
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        public override Guid Id
        {
            get { return new Guid(DataTypeConstants.RelatedLinksWithMediaId); }
        }

        /// <summary>
        /// Gets the name of the data type.
        /// </summary>
        /// <value>The name of the data type.</value>
        public override string DataTypeName
        {
            get { return "uComponents-Legacy: Related Links with Media"; }
        }

        /// <summary>
        /// Gets the prevalue editor.
        /// </summary>
        /// <value>The prevalue editor.</value>
        public override umbraco.interfaces.IDataPrevalue PrevalueEditor
        {
            get
            {
                if (_prevalueeditor == null)
                    _prevalueeditor = new RelatedLinkswMediaPrevalueEditor(this);
                return _prevalueeditor;
            }
        }
    }
}
