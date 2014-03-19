using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;
using umbraco.interfaces;
using DefaultData = umbraco.cms.businesslogic.datatype.DefaultData;
using Image = System.Web.UI.WebControls.Image;

[assembly: WebResource("uComponents.DataTypes.ImagePoint.ImagePoint.css", Constants.MediaTypeNames.Text.Css)]
[assembly: WebResource("uComponents.DataTypes.ImagePoint.ImagePoint.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.ImagePoint.ImagePointMarker.png", Constants.MediaTypeNames.Image.Png)]

namespace uComponents.DataTypes.ImagePoint
{
    using System.Xml.Linq;
    using umbraco.cms.businesslogic.property;

    /// <summary>
    /// Image Point Data Type
    /// </summary>
    [ClientDependency.Core.ClientDependency(ClientDependency.Core.ClientDependencyType.Javascript, "ui/jqueryui.js", "UmbracoClient")]
    public class ImagePointDataEditor : CompositeControl, IDataEditor
    {
        /// <summary>
        /// Field for the data.
        /// </summary>
        private IData data;

        /// <summary>
        /// Field for the options.
        /// </summary>
        private ImagePointOptions options;

        /// <summary>
        /// Wrapping div
        /// </summary>
        private HtmlGenericControl div = new HtmlGenericControl("div");

        /// <summary>
        /// X coordinate
        /// </summary>
        private TextBox xTextBox = new TextBox();

        /// <summary>
        /// Y coordinate
        /// </summary>
        private TextBox yTextBox = new TextBox();

        /// <summary>
        /// Image tag used to define the x, y area
        /// </summary>
        private Image mainImage = new Image();

        /// <summary>
        /// Image used to mark the point
        /// </summary>
        private Image markerImage = new Image();

        /// <summary>
        /// Collection of ghost images to render
        /// </summary>
        private List<Image> ghostImages = new List<Image>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePointDataEditor"/> class. 
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="options">The options.</param>
        internal ImagePointDataEditor(IData data, ImagePointOptions options)
        {
            this.data = data;
            this.options = options;
        }

        /// <summary>
        /// Gets a value indicating whether [treat as rich text editor].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [treat as rich text editor]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool TreatAsRichTextEditor
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [show label].
        /// </summary>
        /// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
        public virtual bool ShowLabel
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the editor.
        /// </summary>
        /// <value>The editor.</value>
        public Control Editor
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets the id of the current (content, media or member) on which this is a property
        /// </summary>
        private int CurrentContentId
        {
            get
            {
                return ((DefaultData)this.data).NodeId;
            }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            /*  
             * 
             *  <div class="image-point">
             *      X <input type="text" class="x" />
             *      Y <input type="text" class="y" />
             *      <div class="area">
             *          <img src="" width="" height="" class="main" />
             *          <img src="" class="marker" />
             * 
             *          (optional)
             *          <img src="" class="ghost" data-x="" data-y="" />
             *          <img src="" class="ghost" />
             *          ...
             * 
             *      </div>
             *  </div>
             * 
             */
            this.div.Attributes.Add("class", "image-point");

            this.xTextBox.ID = "xTextBox";
            this.xTextBox.CssClass = "x";
            this.xTextBox.MaxLength = 4;                        

            this.yTextBox.ID = "yTextBox";
            this.yTextBox.CssClass = "y";
            this.yTextBox.MaxLength = 4;

            WebControl areaDiv = new WebControl(HtmlTextWriterTag.Div);
            areaDiv.CssClass = "area";
            areaDiv.Controls.Add(this.mainImage);
            areaDiv.Controls.Add(this.markerImage);

            this.mainImage.CssClass = "main";           

            this.markerImage.CssClass = "marker";
            this.markerImage.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "uComponents.DataTypes.ImagePoint.ImagePointMarker.png");

            this.div.Controls.Add(new Literal() { Text = "X " });
            this.div.Controls.Add(this.xTextBox);
            this.div.Controls.Add(new Literal() { Text = " Y " });
            this.div.Controls.Add(this.yTextBox);
            this.div.Controls.Add(areaDiv);

            this.Controls.Add(this.div);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.InitializeControls(); // using configuration options, set the class scope vars in preperation to be added to the control collection
            this.EnsureChildControls(); // use the class scope vars to build markup hierarachy

            if (!this.Page.IsPostBack && this.data.Value != null)
            {
                // set the x and y textboxes
                ImagePoint value = new ImagePoint();
                ((uQuery.IGetProperty)value).LoadPropertyValue(this.data.Value.ToString());

                if (value.X.HasValue)
                {
                    this.xTextBox.Text = value.X.ToString();
                }

                if (value.Y.HasValue)
                {
                    this.yTextBox.Text = value.Y.ToString();
                }
            }

            this.RegisterEmbeddedClientResource("uComponents.DataTypes.ImagePoint.ImagePoint.css", ClientDependencyType.Css);
            this.RegisterEmbeddedClientResource("uComponents.DataTypes.ImagePoint.ImagePoint.js", ClientDependencyType.Javascript);

            string startupScript = @"
                <script language='javascript' type='text/javascript'>
                    $(document).ready(function () {
                        ImagePoint.init(jQuery('div#" + this.div.ClientID + @"'));
                    });
                </script>";

            ScriptManager.RegisterStartupScript(this, typeof(ImagePointDataEditor), this.ClientID + "_init", startupScript, false);
        }

        /// <summary>
        /// Called by Umbraco when saving the node
        /// </summary>
        public void Save()
        {
            // storing the width and height allow the percentage calculation in the property-value-convertor
            this.data.Value = new XElement(
                "ImagePoint",
                new XAttribute("x", this.xTextBox.Text),
                new XAttribute("y", this.yTextBox.Text),
                new XAttribute("width", this.mainImage.Attributes["width"]),
                new XAttribute("height", this.mainImage.Attributes["height"]))
                .ToString();
        }

        /// <summary>
        /// Calculates the width and height and sets these as attributes on the image, and builds a collection of ghost points
        /// if a width or height value has been supplied then these are used in preference to the image dimensions
        /// if a width or height value has been supplied, and one of these = 0, then it needs to be calculated from the image dimensions (so as to keep the aspect ratio)
        /// </summary>
        private void InitializeControls()
        {            
            int width = 0; // default unknown values
            int height = 0;
            Size imageSize = new Size(0, 0);
           
            if (!string.IsNullOrWhiteSpace(this.options.ImagePropertyAlias))
            {
                string imageUrl = null;

                // used large try/catch block to simply checking for nulls on content / media & members
                try
                {
                    // looking for the specified property
                    switch (uQuery.GetUmbracoObjectType(this.CurrentContentId))
                    {
                        case uQuery.UmbracoObjectType.Document:

                            Document imageDocument = uQuery.GetDocument(this.CurrentContentId)
                                                            .GetAncestorOrSelfDocuments()
                                                            .First(x => x.HasProperty(this.options.ImagePropertyAlias));
                            
                            imageUrl = imageDocument.GetProperty<string>(this.options.ImagePropertyAlias);

                            // find all self or descendant documents that have this specifc datatype datatype, but stop if any contain the imagePropertyAlias
                            if (this.options.ShowNeighbours)
                            {
                                List<Document> documents = new List<Document>(); // building a collection as there isn't a .GetDescendantsOrSelf(Func<Document, bool> func) overload
                                documents.Add(imageDocument); // add the top level document
                                documents.AddRange(imageDocument.GetDescendantDocuments(x => !x.HasProperty(this.options.ImagePropertyAlias))); // drill down and get all documents, but stop descending if another the imagePropertyAlias is found

                                Image ghostImage;
                                ImagePoint imagePoint;

                                // for all documents that contain this instance of this datatype
                                foreach (Document document in documents.Where(x => x.GenericProperties.Select(y => y.PropertyType.DataTypeDefinition.Id).Contains(((XmlData)this.data).DataTypeDefinitionId)))
                                {
                                    // the document may contain multiple instance of the ImagePoint datatype, but with different aliases
                                    foreach (Property property in document.GenericProperties.Where(x => x.PropertyType.DataTypeDefinition.Id == ((XmlData)this.data).DataTypeDefinitionId))
                                    {
                                        imagePoint = new ImagePoint();
                                        ((uQuery.IGetProperty)imagePoint).LoadPropertyValue(property.Value.ToString());

                                        if (imagePoint.HasCoordinate)
                                        {
                                            ghostImage = new Image();
                                            ghostImage.Attributes.Add("data-x", imagePoint.X.ToString());
                                            ghostImage.Attributes.Add("data-y", imagePoint.Y.ToString());

                                            this.ghostImages.Add(ghostImage);
                                        }
                                    }
                                }
                            }                                            

                            break;

                        case uQuery.UmbracoObjectType.Media:

                            imageUrl = uQuery.GetMedia(this.CurrentContentId)
                                                .GetAncestorOrSelfMedia()
                                                .First(x => x.HasProperty(this.options.ImagePropertyAlias))
                                                .GetProperty<string>(this.options.ImagePropertyAlias);
                            break;

                        case uQuery.UmbracoObjectType.Member:

                            imageUrl = uQuery.GetMember(this.CurrentContentId).GetProperty<string>(this.options.ImagePropertyAlias);

                            break;
                    }
                }
                catch
                {
                    // node, media or member with specified property couldn't be found
                    // TODO: if debug mode on, then thow exception, else be silent
                }

                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    // attempt to get image from the url
                    string imagePath = HttpContext.Current.Server.MapPath(imageUrl);
                    if (File.Exists(imagePath))
                    {                        
                        using (System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath))
                        {
                            this.mainImage.ImageUrl = imageUrl;

                            imageSize = image.Size;
                        }
                    }
                }
            }
            
            if (this.options.Width == 0 && this.options.Height == 0)
            {
                // neither value set, so use image dimensions
                width = imageSize.Width;
                height = imageSize.Height;
            }            
            else if (this.options.Width > 0 && this.options.Height == 0)
            {
                // width set, so calulate height
                width = this.options.Width;

                if (imageSize.Height > 0)
                {
                    height = (int)(imageSize.Height / ((decimal)(imageSize.Width / this.options.Width)));
                }
            }            
            else if (this.options.Width == 0 && this.options.Height > 0)
            {
                // height set, so calculate width
                if (imageSize.Width > 0)
                {
                    width = (int)(imageSize.Width / ((decimal)(imageSize.Height / this.options.Height)));
                }

                height = this.options.Height;
            }
            else if (this.options.Width > 0 && this.options.Height > 0)
            {
                // width and height set, so stretch image to fit
                width = this.options.Width;
                height = this.options.Height;
            }

            //this.mainImage.Width = width;
            this.mainImage.Attributes["width"] = width.ToString();

            //this.mainImage.Height = height;
            this.mainImage.Attributes["height"] = height.ToString();
        }
    }
}
