using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.web;

namespace uComponents.DataTypes.TextImage
{
    internal class DocumentHandlers : ApplicationBase
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "DocumentHandlers" /> class.
        /// </summary>
        public DocumentHandlers()
        {
            Document.BeforeDelete += Document_BeforeDelete;
            Document.BeforePublish += Document_BeforePublish;
        }

        /// <summary>
        ///   Document_s the before delete.
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "umbraco.cms.businesslogic.DeleteEventArgs" /> instance containing the event data.</param>
        private void Document_BeforeDelete(Document sender, DeleteEventArgs e)
        {
            var propertyTypes = sender.ContentType.PropertyTypes;

            foreach (var propertyType in propertyTypes)
            {
                try
                {
                    var property = sender.getProperty(propertyType);

                    if (property.Value == null) continue;

                    var xDocument = XDocument.Parse(property.Value.ToString());

                    // Check if this is TextImage data
                    if (xDocument.Element("TextImage") == null) continue;

                    // Extract image url from xml and delete file
                    var imageUrl = xDocument.Descendants().Elements("Url").First().Value;
                    File.Delete(HttpContext.Current.Server.MapPath(imageUrl));
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Document_s the before publish.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="umbraco.cms.businesslogic.PublishEventArgs"/> instance containing the event data.</param>
        private void Document_BeforePublish(Document sender, PublishEventArgs e)
        {
            var propertyTypes = sender.ContentType.PropertyTypes;

            foreach (var propertyType in propertyTypes)
            {
                try
                {
                    var property = sender.getProperty(propertyType);

                    if (property.Value == null) continue;

                    var xDocument = XDocument.Parse(property.Value.ToString());

                    // Check if this is TextImage data
                    if (xDocument.Element("TextImage") == null) continue;

                    // Extract text from xml and generate an fresh image
                    var text = xDocument.Descendants().Elements("Text").First().Value;

                    if (text == string.Empty) continue;

                    var imageUrl = xDocument.Descendants().Elements("Url").First().Value;
                    var imageFile = HttpContext.Current.Server.MapPath(imageUrl);
                    var imageName = Path.GetFileNameWithoutExtension(imageFile);

                    var parameters = GetParameters(text, property.PropertyType.DataTypeDefinition);

                    // Delete old and Save updated image
                    File.Delete(imageFile);
                    MediaHelper.SaveTextImage(parameters, imageName);
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        ///   Gets the parameters.
        /// </summary>
        /// <returns></returns>
        private static TextImageParameters GetParameters(string text, DataTypeDefinition typeDefinition)
        {
            TextImageParameters parameters;
            try
            {
                var datatype = (TextImageDataType) typeDefinition.DataType;
                var prevalueEditor = (TextImagePrevalueEditor) datatype.PrevalueEditor;
                
                parameters = new TextImageParameters(text,
                                                     prevalueEditor.OutputFormat,
                                                     prevalueEditor.CustomFontPath,
                                                     prevalueEditor.FontName,
                                                     prevalueEditor.FontSize,
                                                     prevalueEditor.FontStyles,
                                                     prevalueEditor.ForegroundColor,
                                                     prevalueEditor.BackgroundColor,
                                                     prevalueEditor.ShadowColor,
                                                     prevalueEditor.HorizontalAlignment,
                                                     prevalueEditor.VerticalAlignment,
                                                     prevalueEditor.ImageHeight,
                                                     prevalueEditor.ImageWidth,
                                                     prevalueEditor.BackgroundMedia);
            }
            catch (Exception ex)
            {
                parameters = new TextImageParameters(ex.Message, OutputFormat.Jpg, string.Empty, "ARIAL", 12,
                                                     new[] { FontStyle.Regular }, "000", "FFF", "transparent",
                                                     HorizontalAlignment.Left, VerticalAlignment.Top, -1, -1, null);
            }

            return parameters;
        }
    }
}