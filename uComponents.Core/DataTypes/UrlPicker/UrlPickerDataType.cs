using System;
using System.Xml.Linq;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uComponents.Core.DataTypes.UrlPicker.Dto;
using System.Web.Script.Serialization;

namespace uComponents.Core.DataTypes.UrlPicker
{
    /// <summary>
    /// Description here:
    /// http://ucomponents.codeplex.com/wikipage?title=UrlPicker
    /// </summary>
    public class UrlPickerDataType : AbstractDataEditor
    {
        /// <summary>
        /// 
        /// </summary>
        private UrlPickerDataEditor m_DataEditor = new UrlPickerDataEditor();

        /// <summary>
        /// 
        /// </summary>
        private UrlPickerPreValueEditor m_PreValueEditor;

        /// <summary>
        /// 
        /// </summary>
        private IData m_Data;

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        public override Guid Id
        {
            get
            {
                return new Guid(DataTypeConstants.UrlPickerId);
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
                return "uComponents: URL Picker";
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
                if (this.m_Data == null)
                {
                    if (Settings.DataFormat == UrlPickerDataFormat.Xml)
                    {
                        this.m_Data = new uComponents.Core.Shared.Data.XmlData(this);
                    }
                    else
                    {
                        this.m_Data = new DefaultData(this);
                    }
                }

                return this.m_Data;
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
                if (m_PreValueEditor == null)
                    m_PreValueEditor = new UrlPickerPreValueEditor(this);
                return m_PreValueEditor;
            }
        }

        /// <summary>
        /// Get prevalue settings
        /// </summary>
        public UrlPickerSettings Settings
        {
            get
            {
                return ((UrlPickerPreValueEditor)PrevalueEditor).Settings;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlPickerDataType"/> class.
        /// </summary>
        public UrlPickerDataType()
        {
            RenderControl = m_DataEditor;

            // Events
            m_DataEditor.Init += new EventHandler(m_DataEditor_Init);
            m_DataEditor.Load += new EventHandler(m_DataEditor_Load);
            DataEditorControl.OnSave += new AbstractDataEditorControl.SaveEventHandler(DataEditorControl_OnSave);
        }

        /// <summary>
        /// Handles the Load event of the m_DataEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void m_DataEditor_Load(object sender, EventArgs e)
        {
            if (!m_DataEditor.Page.IsPostBack && !string.IsNullOrEmpty((string)this.Data.Value))
            {
                m_DataEditor.State = UrlPickerState.Deserialize((string)this.Data.Value);
            }
        }

        /// <summary>
        /// Handles the Init event of the m_DataEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void m_DataEditor_Init(object sender, EventArgs e)
        {
            // Fill DataEditor with the prevalue settings and a unique ID
            var settings = Settings;
            settings.UniquePropertyId = ((DefaultData)this.Data).PropertyId;
            settings.Standalone = false;
            m_DataEditor.Settings = settings;
        }

        /// <summary>
        /// Datas the editor control_ on save.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void DataEditorControl_OnSave(EventArgs e)
        {
            var state = m_DataEditor.State;
            // Must be a valid state
            this.Data.Value = (state == null || !Settings.ValidateState(state))
                ? null 
                : state.Serialize(Settings.DataFormat);
        }
    }

    /// <summary>
    /// The modes this datatype can implement - they refer to how the local/external content is referred.
    /// </summary>
    public enum UrlPickerMode : int
    {
        /// <summary>
        /// URL string
        /// </summary>
        URL = 1,
        /// <summary>
        /// Content node
        /// </summary>
        Content = 2,
        /// <summary>
        /// Media node
        /// </summary>
        Media = 3,
        /// <summary>
        /// Upload a file
        /// </summary>
        Upload = 4
    }

    /// <summary>
    /// Determines in which serialized format the the data is saved to the database
    /// </summary>
    public enum UrlPickerDataFormat
    {
        /// <summary>
        /// Store as XML
        /// </summary>
        Xml,
        /// <summary>
        /// Store as comma delimited (CSV, single line)
        /// </summary>
        Csv,
        /// <summary>
        /// Store as a JSON object, which can be deserialized by .NET or JavaScript
        /// </summary>
        Json
    }
}