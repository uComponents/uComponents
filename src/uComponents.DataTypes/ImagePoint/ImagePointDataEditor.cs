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
using umbraco.DataLayer;
using umbraco.editorControls;
using umbraco.interfaces;
using DefaultData = umbraco.cms.businesslogic.datatype.DefaultData;
using Image = System.Web.UI.WebControls.Image;

[assembly: WebResource("uComponents.DataTypes.ImagePoint.ImagePoint.css", Constants.MediaTypeNames.Text.Css)]
[assembly: WebResource("uComponents.DataTypes.ImagePoint.ImagePoint.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.ImagePoint.ImagePointMarker.png", Constants.MediaTypeNames.Image.Png)]
[assembly: WebResource("uComponents.DataTypes.ImagePoint.ImagePointGhost.png", Constants.MediaTypeNames.Image.Png)]
namespace uComponents.DataTypes.ImagePoint
{
    using System.Xml.Linq;
    using umbraco.cms.businesslogic.property;
    using umbraco.cms.businesslogic.media;

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
        private WebControl imagePointDiv = new WebControl(HtmlTextWriterTag.Div);

        /// <summary>
        /// X coordinate
        /// </summary>
        private TextBox xTextBox = new TextBox();

        /// <summary>
        /// Y coordinate
        /// </summary>
        private TextBox yTextBox = new TextBox();

        /// <summary>
        /// div used to append ghost image controls
        /// </summary>
        private WebControl areaDiv = new WebControl(HtmlTextWriterTag.Div);

        /// <summary>
        /// Image tag used to define the x, y area
        /// </summary>
        private Image mainImage = new Image();

        /// <summary>
        /// Image used to mark the point
        /// </summary>
        private Image markerImage = new Image();

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
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.EnsureChildControls();
            this.ConfigureControls(); 

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
                        ImagePoint.init(jQuery('div#" + this.imagePointDiv.ClientID + @"'));
                    });
                </script>";

            ScriptManager.RegisterStartupScript(this, typeof(ImagePointDataEditor), this.ClientID + "_init", startupScript, false);
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
             *          <img src="" class="ghost" style="left:x;top:y;" title="" />
             *          <img src="" class="ghost" style="left:x;top:y;" title="" />
             *          ...
             * 
             *      </div>
             *  </div>
             * 
             */
            this.imagePointDiv.CssClass = "image-point";

            this.xTextBox.ID = "xTextBox";
            this.xTextBox.CssClass = "x";
            this.xTextBox.MaxLength = 4;

            this.yTextBox.ID = "yTextBox";
            this.yTextBox.CssClass = "y";
            this.yTextBox.MaxLength = 4;

            this.mainImage.CssClass = "main";

            this.markerImage.CssClass = "marker";
            this.markerImage.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "uComponents.DataTypes.ImagePoint.ImagePointMarker.png");

            this.areaDiv.CssClass = "area";

            this.areaDiv.Controls.Add(this.mainImage);
            this.areaDiv.Controls.Add(this.markerImage);

            this.imagePointDiv.Controls.Add(new Literal() { Text = "X " });
            this.imagePointDiv.Controls.Add(this.xTextBox);
            this.imagePointDiv.Controls.Add(new Literal() { Text = " Y " });
            this.imagePointDiv.Controls.Add(this.yTextBox);
            this.imagePointDiv.Controls.Add(areaDiv);

            this.Controls.Add(this.imagePointDiv);
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
        private void ConfigureControls()
        {
            int width = 0; // default unknown values
            int height = 0;
            Size imageSize = new Size(0, 0);

            if (!string.IsNullOrWhiteSpace(this.options.ImagePropertyAlias))
            {
                string imageUrl = null;
                int currentDataTypeDefinitionId = ((XmlData)this.data).DataTypeDefinitionId;
                int currentPropertyId = ((XmlData)this.data).PropertyId;

                // determine whether to use the Document / Media or Member api
                switch (uQuery.GetUmbracoObjectType(this.CurrentContentId))
                {
                    case uQuery.UmbracoObjectType.Document:

                        Document imageDocument = uQuery.GetDocument(this.CurrentContentId)
                                                        .GetAncestorOrSelfDocuments()
                                                        .FirstOrDefault(x => x.HasProperty(this.options.ImagePropertyAlias));

                        if (imageDocument != null)
                        {
                            imageUrl = imageDocument.GetProperty<string>(this.options.ImagePropertyAlias);

                            if (this.options.ShowNeighbours)
                            {
                                // get a collection of all documents that could potentially have this imagepoint datatype instance, sharing the same image
                                List<Document> documents = new List<Document>();  
                                
                                // add the document containing the shared main image
                                documents.Add(imageDocument); 

                                // drill down and get all documents, but stop descending a path if an imagePropertyAlias is found - any descendants of that would use a different image
                                documents.AddRange(imageDocument.GetDescendantDocuments(x => !x.HasProperty(this.options.ImagePropertyAlias))); 

                                // for all documents that use this instance of this imagepoint datatype
                                foreach (Document document in documents.Where(x => x.GenericProperties.Select(y => y.PropertyType.DataTypeDefinition.Id).Contains(currentDataTypeDefinitionId)))
                                {
                                    // the document may have multiple properties each using this imagepoint datatype instance
                                    this.CreateGhostImages(document.GenericProperties.Where(x => x.PropertyType.DataTypeDefinition.Id == currentDataTypeDefinitionId && x.Id != currentPropertyId));
                                }
                            }
                        }

                        break;

                    case uQuery.UmbracoObjectType.Media:

                        Media imageMedia = uQuery.GetMedia(this.CurrentContentId)
                                                    .GetAncestorOrSelfMedia()
                                                    .FirstOrDefault(x => x.HasProperty(this.options.ImagePropertyAlias));
                        if (imageMedia != null)
                        {
                            imageUrl = imageMedia.GetProperty<string>(this.options.ImagePropertyAlias);

                            if (this.options.ShowNeighbours)
                            {
                                List<Media> media = new List<Media>();

                                // add the media item containing the shared main image
                                media.Add(imageMedia);

                                // unfortunately no uQuery extension method, so using a private local
                                media.AddRange(this.GetDescendantMedia(imageMedia, x => !x.HasProperty(this.options.ImagePropertyAlias)));

                                foreach(Media mediaItem in media.Where(x => x.GenericProperties.Select(y => y.PropertyType.DataTypeDefinition.Id).Contains(currentDataTypeDefinitionId)))
                                {
                                    this.CreateGhostImages(mediaItem.GenericProperties.Where(x => x.PropertyType.DataTypeDefinition.Id == currentDataTypeDefinitionId && x.Id != currentPropertyId));
                                }
                            }
                        }

                        break;

                    case uQuery.UmbracoObjectType.Member:

                        imageUrl = uQuery.GetMember(this.CurrentContentId).GetProperty<string>(this.options.ImagePropertyAlias);

                        // show neighbours not relevant

                        break;
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

        /// <summary>
        /// from a collection of document or media properties, create and add any required ghost images to the areaDiv
        /// </summary>
        /// <param name="properties"></param>
        private void CreateGhostImages(IEnumerable<Property> properties)
        {
            foreach(Property property in properties)
            {
                ImagePoint imagePoint = new ImagePoint();
                ((uQuery.IGetProperty)imagePoint).LoadPropertyValue(property.Value.ToString());

                if (imagePoint.HasCoordinate)
                {
                    Image ghostImage = new Image();                    

                    ghostImage.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "uComponents.DataTypes.ImagePoint.ImagePointGhost.png");
                    ghostImage.CssClass = "ghost";

                    // using sql here, else would have to query document and media seperately
                    using(IRecordsReader iRecordsReader = uQuery.SqlHelper.ExecuteReader(@"
                                                                                            SELECT  A.contentNodeId AS 'Id', 
		                                                                                            B.text AS 'Name',
		                                                                                            C.Name AS 'PropertyName'

			                                                                                FROM    cmsPropertyData A
			                                                                                        LEFT OUTER JOIN umbracoNode B ON A.ContentNodeId = B.Id
			                                                                                        LEFT OUTER JOIN cmsPropertyType C ON A.propertytypeid = C.id

			                                                                                WHERE   A.id = @propertyDataId",
                                                                                            uQuery.SqlHelper.CreateParameter("propertyDataId", property.Id)))
                    {
                        iRecordsReader.Read(); // there should only be a single row returned

                        // if the properties are on the same item, then label with property name, otherwise default to labeling with the node / media or member name
                        if (iRecordsReader.Get<int>("Id") == this.CurrentContentId)
                        {
                            ghostImage.ToolTip = iRecordsReader.Get<string>("PropertyName");
                        }
                        else
                        {
                            ghostImage.ToolTip = iRecordsReader.Get<string>("Name");
                        }
                    }

                    ghostImage.Style.Add(HtmlTextWriterStyle.Left, (imagePoint.X - 7).ToString() + "px");
                    ghostImage.Style.Add(HtmlTextWriterStyle.Top, (imagePoint.Y - 7).ToString() + "px");

                    areaDiv.Controls.Add(ghostImage);
                }
            }
        }

        /// <summary>
        /// No idea where to put this method ! ideally should have been in uQuery
        /// </summary>
        /// <param name="media"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private IEnumerable<Media> GetDescendantMedia(Media media, Func<Media, bool> func)
        {
            foreach (Media childMedia in media.Children)
            {
                if (func(childMedia))
                {
                    yield return childMedia;

                    foreach (Media descendantMedia in this.GetDescendantMedia(childMedia, func))
                    {
                        yield return descendantMedia;
                    }
                }
            }
        }
    }
}
