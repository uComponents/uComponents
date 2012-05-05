using System;
using System.Collections.Generic;
using System.Linq;
using umbraco.cms.businesslogic.datatype;

namespace uComponents.DataTypes.Similarity
{
    /// <summary>
    /// 
    /// </summary>
    public class SimilarityDataType : AbstractDataEditor
    {
		/// <summary>
		/// 
		/// </summary>
        private SimilarityDataEditor _mControl = new SimilarityDataEditor();

		/// <summary>
		/// 
		/// </summary>
		private SimilarityPreValueEditor _similarityPickerPreValueEditor;

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public override Guid Id
        {
            get
            {
                return new Guid(DataTypeConstants.SimilarityId);
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
                return "uComponents: Similarity";
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
                if (_similarityPickerPreValueEditor == null)
                    _similarityPickerPreValueEditor = new SimilarityPreValueEditor(this);
                return _similarityPickerPreValueEditor;
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="SimilarityDataType"/> class.
		/// </summary>
        public SimilarityDataType()
        {
            RenderControl = _mControl;
            _mControl.Init += _mControl_Init;
            DataEditorControl.OnSave += DataEditorControl_OnSave;
            _mControl.PreRender += _mControl_PreRender;
        }

		/// <summary>
		/// Handles the PreRender event of the _mControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void _mControl_PreRender(object sender, EventArgs e)
        {
            if (base.Data.Value != null)
            {
                _mControl.SelectedNodes = base.Data.Value.ToString() + ",";
            }
        }

		/// <summary>
		/// Datas the editor control_ on save.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void DataEditorControl_OnSave(EventArgs e)
        {
            base.Data.Value = _mControl.SelectedNodes;
            _mControl.DataBind();
        }

		/// <summary>
		/// Handles the Init event of the _mControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void _mControl_Init(object sender, EventArgs e)
        {
            _mControl.IndexToSearch = ((SimilarityPreValueEditor) PrevalueEditor).SelectedIndex;
            _mControl.MaxResults = ((SimilarityPreValueEditor) PrevalueEditor).MaxNoResults;
            List<string> properties = ((SimilarityPreValueEditor) PrevalueEditor).SelectedProperties.Split(new[] {','}).ToList();
            _mControl.PropertiesToSearch = properties;
            
        }
    }
}