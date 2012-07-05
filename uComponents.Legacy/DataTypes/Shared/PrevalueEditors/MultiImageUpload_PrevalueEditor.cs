using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using uComponents.DataTypes.Shared.Extensions;
using umbraco;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using umbraco.IO;
using umbraco.editorControls;

namespace uComponents.DataTypes.Shared.PrevalueEditors
{           
    /// <summary>
    /// The pre-value editor for the RadiobuttonList node tree picker.
    /// </summary>
    public class MultiImageUpload_PrevalueEditor : Control, IDataPrevalue
    {
		/// <summary>
		/// 
		/// </summary>
        private readonly umbraco.cms.businesslogic.datatype.BaseDataType m_DataType;

		/// <summary>
		/// 
		/// </summary>
		private static readonly object m_Locker = new object();

		/// <summary>
		/// 
		/// </summary>
        protected FileUpload UploadControl;

		/// <summary>
		/// 
		/// </summary>
		protected TextBox TextControl;

		/// <summary>
		/// 
		/// </summary>
		protected RegularExpressionValidator UploadRegEx;

		/// <summary>
		/// Gets the prevalues.
		/// </summary>
		/// <value>The prevalues.</value>
		public List<ImageAndAlias> Prevalues
        {
            get
            {
                List<ImageAndAlias> prevalues = new List<ImageAndAlias>();
                foreach (umbraco.cms.businesslogic.datatype.PreValue val in PreValues.GetPreValues(m_DataType.DataTypeDefinitionId).GetValueList())
                {
                    if (string.IsNullOrEmpty(val.Value))
                        continue;

                    prevalues.Add(new ImageAndAlias
                    {
                        Alias = val.Value.Split('|')[0],
                        ImageUrl = val.Value.Split('|')[1]
                    });
                }
                return prevalues;
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiImageUpload_PrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
       public MultiImageUpload_PrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
        {
            m_DataType = dataType;
        }

	   /// <summary>
	   /// Override on init to ensure child controls
	   /// </summary>
	   /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (HttpContext.Current.Request.QueryString["delete"] != null)
                Delete(HttpContext.Current.Request.QueryString["delete"]);

            EnsureChildControls();

			this.RegisterEmbeddedClientResource(Constants.PrevalueEditorCssResourcePath, ClientDependencyType.Css);
        }

		/// <summary>
		/// Ensures the css to render this control is included.
		/// Binds the saved value to the drop down.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);                        
        }

		/// <summary>
		/// Creates child controls for this control
		/// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();            

            UploadControl = new FileUpload();
            UploadControl.ID = "UploadId";

            UploadRegEx = new RegularExpressionValidator();
            UploadRegEx.ID = "regex";
            UploadRegEx.ValidationExpression = @"(.*?)\.(jpg|jpeg|png|gif)$";
            UploadRegEx.ErrorMessage = "Upload Jpegs, Pngs and Gifs only.";
            UploadRegEx.ControlToValidate = "UploadId";
            TextControl = new TextBox();
            TextControl.ID = "TextId";

            this.Controls.Add(UploadControl);
            this.Controls.Add(TextControl);
            this.Controls.Add(UploadRegEx);
        }

		/// <summary>
		/// render our own custom markup
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
			writer.AddAttribute(HtmlTextWriterAttribute.Class, Constants.ApplicationName);
            writer.RenderBeginTag(HtmlTextWriterTag.Div); //start 'uComponents'

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "row clearfix");
            writer.RenderBeginTag(HtmlTextWriterTag.Div); //start 'row'

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "label");
                writer.RenderBeginTag(HtmlTextWriterTag.Div); //start 'label'
                    writer.Write("image alias:");
                writer.RenderEndTag(); //end 'label'

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "field");
                writer.RenderBeginTag(HtmlTextWriterTag.Div); //start 'field'
                    TextControl.RenderControl(writer);
                writer.RenderEndTag(); //end 'field'
            writer.RenderEndTag(); //end 'row'

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "row clearfix");
            writer.RenderBeginTag(HtmlTextWriterTag.Div); //start 'row'
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "label");
                writer.RenderBeginTag(HtmlTextWriterTag.Div); //start 'label'
                    writer.Write("image file:");
                writer.RenderEndTag(); //end 'label'

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "field");
                writer.RenderBeginTag(HtmlTextWriterTag.Div); //start 'field'
                    UploadControl.RenderControl(writer);
                    UploadRegEx.RenderControl(writer);
                writer.RenderEndTag(); //end 'field'
            writer.RenderEndTag(); //end 'row'                        

            if (PreValues.GetPreValues(m_DataType.DataTypeDefinitionId).GetValueList().Count > 0)
            {                
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.RenderBeginTag(HtmlTextWriterTag.Table);
                        foreach (umbraco.cms.businesslogic.datatype.PreValue val in PreValues.GetPreValues(m_DataType.DataTypeDefinitionId).GetValueList())
                        {                         
                            if (string.IsNullOrEmpty(val.Value))
                            {
                                val.Delete();
                                break;
                            }
                            writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
                            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                                    writer.RenderBeginTag(HtmlTextWriterTag.H2);
                                        writer.WriteLine(val.Value.Split('|')[0]);
                                    writer.RenderEndTag();
                                    writer.AddAttribute(HtmlTextWriterAttribute.Href, "?id=" + m_DataType.DataTypeDefinitionId + "&delete=" + val.Id);
                                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                                    writer.WriteLine(ui.Text("delete"));
                                    writer.RenderEndTag();
                                writer.RenderEndTag();
                                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                                    writer.AddAttribute(HtmlTextWriterAttribute.Src, val.Value.Split('|')[1]);
                                    writer.RenderBeginTag(HtmlTextWriterTag.Img);
                                    writer.RenderEndTag();
                                writer.RenderEndTag();
                            writer.RenderEndTag();
                        }
                    writer.RenderEndTag();
                writer.RenderEndTag();
            }
            writer.RenderEndTag(); //end 'uComponents'            
        }

		/// <summary>
		/// Deletes the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
        private void Delete(string id)
        {                        
            foreach (umbraco.cms.businesslogic.datatype.PreValue val in PreValues.GetPreValues(m_DataType.DataTypeDefinitionId).GetValueList())
            {
                if(val.Id.ToString() == id)
                {                                        
                    FileInfo fi = new FileInfo(IOHelper.MapPath(val.Value.Split('|')[1]));
                    if (fi.Exists)
                    {
                        val.Delete();
                        fi.Delete();
                    }
                    return;
                }
            }
        }

        #region IDataPrevalue Members

		/// <summary>
		/// Gets the editor.
		/// </summary>
		/// <value>The editor.</value>
        public Control Editor
        {
            get { return this; }
        }

		/// <summary>
		/// Saves this instance.
		/// </summary>
        public void Save()
        {            
            m_DataType.DBType = umbraco.cms.businesslogic.datatype.DBTypes.Ntext;
            
            if (UploadControl.HasFile)
            {
                Settings.BaseDir.CreateSubdirectory("MultiImageUploadPrevalues").CreateSubdirectory(m_DataType.DataTypeDefinitionId.ToString());
                string savePath = string.Format("{0}\\MultiImageUploadPrevalues\\{1}\\{2}", Settings.BaseDir.FullName, m_DataType.DataTypeDefinitionId, UploadControl.FileName);
                string virtualPath = string.Format("{0}/MultiImageUploadPrevalues/{1}/{2}", Settings.BaseDirName, m_DataType.DataTypeDefinitionId, UploadControl.FileName);
                UploadControl.SaveAs(savePath);
                PreValue.MakeNew(m_DataType.DataTypeDefinitionId, TextControl.Text + "|" + VirtualPathUtility.ToAbsolute(virtualPath));
            }
            return;                        
        }

        #endregion
    }

	/// <summary>
	/// The Image and Alias object.
	/// </summary>
    public class ImageAndAlias 
    {
		/// <summary>
		/// Gets or sets the alias.
		/// </summary>
		/// <value>The alias.</value>
        public string Alias { get; set; }

		/// <summary>
		/// Gets or sets the image URL.
		/// </summary>
		/// <value>The image URL.</value>
		public string ImageUrl { get; set; }
    }
}
