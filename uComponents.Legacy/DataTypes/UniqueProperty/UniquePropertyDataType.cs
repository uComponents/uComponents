using System;
using umbraco.cms.businesslogic.datatype;

namespace uComponents.DataTypes.UniqueProperty
{
	/// <summary>
	/// The UniqueProperty data-type.
	/// </summary>
    public class UniquePropertyDataType : AbstractDataEditor
    {
		/// <summary>
		/// 
		/// </summary>
        private UniquePropertyPreValueEditor _cogUniquePreValueEditor;

		/// <summary>
		/// 
		/// </summary>
		private UniquePropertyDataEditor _mControl=new UniquePropertyDataEditor();

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public override Guid Id
        {
            get
            {
                return new Guid(DataTypeConstants.UniquePropertyId);
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
                return "uComponents (Legacy): Unique Property";
            }
        }

		/// <summary>
		/// Gets the prevalue editor.
		/// </summary>
		/// <value>The prevalue editor.</value>
        public override umbraco.interfaces.IDataPrevalue PrevalueEditor
        {
            get
            {
                if (_cogUniquePreValueEditor == null)
                    _cogUniquePreValueEditor = new UniquePropertyPreValueEditor(this);
                return _cogUniquePreValueEditor;
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="UniquePropertyDataType"/> class.
		/// </summary>
        public UniquePropertyDataType()
        {
            base.RenderControl = _mControl;
            _mControl.Init += MControlInit;
            base.DataEditorControl.OnSave += DataEditorControl_OnSave;
        }

		/// <summary>
		/// Ms the control init.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void MControlInit(object sender,EventArgs e) 
        {
            _mControl.Text = base.Data.Value != null ? base.Data.Value.ToString() : "";
            _mControl.FieldAlias = ((UniquePropertyPreValueEditor) PrevalueEditor).SelectedPropertyAlias;
        }

		/// <summary>
		/// Datas the editor control_ on save.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void DataEditorControl_OnSave(EventArgs e)
        {
            base.Data.Value = _mControl.Text;
        }
    }
}
