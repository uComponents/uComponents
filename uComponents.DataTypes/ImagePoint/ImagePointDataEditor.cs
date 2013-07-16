using System;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.property;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using Umbraco.Web;
using umbraco.editorControls;
using System.Web.UI.HtmlControls;
using DefaultData = umbraco.cms.businesslogic.datatype.DefaultData;

[assembly: WebResource("uComponents.DataTypes.ImagePoint.ImagePoint.js", Constants.MediaTypeNames.Application.JavaScript)]
namespace uComponents.DataTypes.ImagePoint
{
    /// <summary>
    /// Image Point Data Type
    /// </summary>
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
        /// image used as background for setting a point
        /// </summary>
        private Image image = new Image();

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
            string imageUrl = null;

            try
            {
                // walk up tree from current node looking for the first instance of the specified property
                switch (uQuery.GetUmbracoObjectType(this.CurrentContentId))
                {
                    case uQuery.UmbracoObjectType.Document:
                        imageUrl = uQuery.GetCurrentDocument()
                                            .GetAncestorOrSelfDocuments()
                                            .First(x => x.HasProperty(this.options.PropertyAlias))
                                            .GetProperty<string>(this.options.PropertyAlias);
                        break;

                    case uQuery.UmbracoObjectType.Media:
                        imageUrl = uQuery.GetMedia(this.CurrentContentId)
                                            .GetAncestorOrSelfMedia()
                                            .First(x => x.HasProperty(this.options.PropertyAlias))
                                            .GetProperty<string>(this.options.PropertyAlias);
                        break;

                    case uQuery.UmbracoObjectType.Member:
                        imageUrl = uQuery.GetMember(this.CurrentContentId).GetProperty<string>(this.options.PropertyAlias);
                                        
                        break;
                }
            }
            catch
            {
            }

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                this.image.ImageUrl = imageUrl;
            }
            else
            {
                // TODO: alert user that the image can't be found
            }


            this.div.Controls.Add(this.image);

            this.Controls.Add(this.div);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.EnsureChildControls();

            if (!this.Page.IsPostBack && this.data.Value != null)
            {
            }

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
        }
    }
}
