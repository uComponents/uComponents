using System;
using System.Collections.Generic;
using System.Linq;
using umbraco.cms.businesslogic.datatype;
using uComponents.Core;

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
		private SimilarityPrevalueEditor _similarityPickerPrevalueEditor;

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
                if (this._similarityPickerPrevalueEditor == null)
                {
                    this._similarityPickerPrevalueEditor = new SimilarityPrevalueEditor(this);
                }

                return this._similarityPickerPrevalueEditor;
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="SimilarityDataType"/> class.
		/// </summary>
        public SimilarityDataType()
        {
            this.RenderControl = this._mControl;

            this._mControl.Init += this._mControl_Init;
            this._mControl.PreRender += new EventHandler(this._mControl_PreRender);

            this.DataEditorControl.OnSave += new AbstractDataEditorControl.SaveEventHandler(this.DataEditorControl_OnSave);
        }

		/// <summary>
		/// Handles the PreRender event of the _mControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _mControl_PreRender(object sender, EventArgs e)
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
        private void DataEditorControl_OnSave(EventArgs e)
        {
            base.Data.Value = _mControl.SelectedNodes;
            this._mControl.DataBind();
        }

		/// <summary>
		/// Handles the Init event of the _mControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _mControl_Init(object sender, EventArgs e)
        {
            this._mControl.IndexToSearch = ((SimilarityPrevalueEditor) PrevalueEditor).SelectedIndex;
            this._mControl.MaxResults = ((SimilarityPrevalueEditor)PrevalueEditor).MaxNoResults;

            var properties = ((SimilarityPrevalueEditor)PrevalueEditor).SelectedProperties.Split(new[] { Constants.Common.COMMA }).ToList();
            this._mControl.PropertiesToSearch = properties;
            
        }
    }
}