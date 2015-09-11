using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Examine;
using uComponents.Core;
using uComponents.DataTypes.Shared.Extensions;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.propertytype;
using umbraco.interfaces;
using umbraco.editorControls;
using umbraco;

namespace uComponents.DataTypes.Similarity
{
    /// <summary>
    /// Prevalue Editor for Similarity data-type
    /// </summary>
    /// <remarks>
    /// TODO: [IM] add validation and pick up label descriptions from resource file
    /// </remarks>
    public class SimilarityPrevalueEditor : Control, IDataPrevalue
    {
        /// <summary>
        /// The underlying base data-type.
        /// </summary>
        private readonly umbraco.cms.businesslogic.datatype.BaseDataType m_DataType;

        /// <summary>
        /// An object to temporarily lock writing to the database.
        /// </summary>
        private static readonly object m_Locker = new object();

        /// <summary>
        /// 
        /// </summary>
        private SortedList m_PreValues = null;

        /// <summary>
        /// 
        /// </summary>
        private ListBox _propertiesToSearchOn;

        /// <summary>
        /// 
        /// </summary>
        private TextBox _txtMaxResults;

        /// <summary>
        /// 
        /// </summary>
        private DropDownList _indexToSearch;

        /// <summary>
        /// 
        /// </summary>
        private Guid[] allowedDataTypes = new[]
        {
            new Guid("EC15C1E5-9D90-422A-AA52-4F7622C63BEA"), // Textstring
            new Guid("67DB8357-EF57-493E-91AC-936D305E0F2A"), // Textbox multiple
            new Guid("5E9B75AE-FACE-41C8-B47E-5F4B0FD82F83") // Richtext editor
        };

        /// <summary>
        /// Initializes a new instance of the <c>SimilarityPrevalueEditor</c> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        public SimilarityPrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
        {
            this.m_DataType = dataType;
        }

        /// <summary>
        /// 
        /// </summary>
        public Control Editor
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxNoResults
        {
            get
            {
                var vals = this.GetPreValues();

                if (vals.Count >= 1)
                {
                    int val1;
                    if (int.TryParse(((PreValue)vals[1]).Value, out val1))
                    {
                        return val1;
                    }
                }

                return 5;
            }
        }

        /// <summary>
        /// csv list of properties to search on
        /// </summary>
        public string SelectedProperties
        {
            get
            {
                var vals = this.GetPreValues();

                if (vals.Count >= 2)
                {
                    return ((PreValue)vals[0]).Value;
                }
                return string.Empty;
            }

        }

        /// <summary>
        /// Gets the index of the selected.
        /// </summary>
        /// <value>The index of the selected.</value>
        public string SelectedIndex
        {
            get
            {
                var vals = this.GetPreValues();

                if (vals.Count >= 3)
                {
                    return ((PreValue)vals[2]).Value;
                }
                return string.Empty;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            this.m_DataType.DBType = umbraco.cms.businesslogic.datatype.DBTypes.Ntext;

            lock (m_Locker)
            {
                var vals = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);
                string selectedValuesAsCsv = GetSelectedPropertyAliases();
                if (vals.Count >= 1)
                {

                    // update
                    ((PreValue)vals[0]).Value = selectedValuesAsCsv;
                    ((PreValue)vals[0]).Save();
                }
                else
                {
                    // insert
                    PreValue.MakeNew(this.m_DataType.DataTypeDefinitionId, selectedValuesAsCsv);
                }

                if (vals.Count >= 2)
                {
                    //update
                    ((PreValue)vals[1]).Value = _txtMaxResults.Text;
                    ((PreValue)vals[1]).Save();

                }
                else
                {
                    //insert
                    PreValue.MakeNew(m_DataType.DataTypeDefinitionId, _txtMaxResults.Text);
                }
                if (vals.Count >= 3)
                {
                    //update
                    ((PreValue)vals[2]).Value = _indexToSearch.SelectedValue;
                    ((PreValue)vals[2]).Save();

                }
                else
                {
                    //insert
                    PreValue.MakeNew(m_DataType.DataTypeDefinitionId, _indexToSearch.SelectedValue);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetSelectedPropertyAliases()
        {
            var selected = new StringBuilder();
            foreach (var l in from ListItem l in this._propertiesToSearchOn.Items where l.Selected select l)
            {
                selected.Append(l.Value);
                selected.Append(",");
            }
            return selected.ToString().TrimEnd(new[] { ',' });
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.EnsureChildControls();
            // Adds the client dependencies.
            this.RegisterEmbeddedClientResource(Constants.PrevalueEditorCssResourcePath, ClientDependencyType.Css);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetSelectedItems();
            this._txtMaxResults.Text = MaxNoResults.ToString();
            this._indexToSearch.SelectedValue = SelectedIndex;
        }

        /// <summary>
        /// Sets the selected items.
        /// </summary>
        private void SetSelectedItems()
        {
            IEnumerable<string> items = SelectedProperties.Split(new[] { ',' }).ToList();
            foreach (var item in items)
            {
                var listItem = _propertiesToSearchOn.Items.FindByValue(item);
                if (listItem != null)
                {
                    listItem.Selected = true;
                }
            }
        }

        /// <summary>
        /// Creates child controls for this control
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            // set-up controls
            this._propertiesToSearchOn = new ListBox() { ID = "searchAbleProperties", SelectionMode = ListSelectionMode.Multiple };
            this._indexToSearch = new DropDownList() { ID = "indexToSearch", AppendDataBoundItems = true };
            this._txtMaxResults = new TextBox { ID = "MaxResults", Text = "5" };

            this.AddIndexesToDropDown();
            this.AddPropertyTypesToDropDown();

            // add controls
            this.Controls.AddPrevalueControls(this._propertiesToSearchOn, this._txtMaxResults, this._indexToSearch);
        }

        /// <summary>
        /// Adds the indexes to drop down.
        /// </summary>
        private void AddIndexesToDropDown()
        {
            this._indexToSearch.DataSource = ExamineManager.Instance.IndexProviderCollection;
            this._indexToSearch.Items.Add(new ListItem(string.Concat(ui.GetText("choose"), "..."), string.Empty));
            this._indexToSearch.DataTextField = "Name";
            this._indexToSearch.DataValueField = "Name";
            this._indexToSearch.DataBind();
        }

        /// <summary>
        /// Adds the property types to drop down.
        /// </summary>
        private void AddPropertyTypesToDropDown()
        {
            var hashtable = new Hashtable();
            foreach (var type in PropertyType.GetAll())
            {
                if (allowedDataTypes.Contains(type.DataTypeDefinition.DataType.Id))
                {
                    if (!hashtable.ContainsKey(type.Alias))
                    {
                        var item = new ListItem(type.Alias, type.Alias);
                        hashtable.Add(type.Alias, string.Empty);
                        this._propertiesToSearchOn.Items.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, Constants.ApplicationName);
            writer.RenderBeginTag(HtmlTextWriterTag.Div); //// start 'uComponents'

            // add property fields
            writer.AddPrevalueRow("Properties to search on :", "Please select properties to search on ", this._propertiesToSearchOn);
            writer.AddPrevalueRow("Maximum number of results to return :", "", this._txtMaxResults);
            writer.AddPrevalueRow("Index to search on", "Please select umbraco index to search, please note this will not work with media index", this._indexToSearch);
            writer.RenderEndTag(); //// end 'uComponents'
        }

        private SortedList GetPreValues()
        {
            if (m_PreValues == null)
            {
                m_PreValues = PreValues.GetPreValues(m_DataType.DataTypeDefinitionId);
            }

            return m_PreValues;
        }
    }
}
