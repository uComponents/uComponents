using System.Collections.Generic;
using System.Xml;
using umbraco;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.propertytype;
using umbraco.cms.businesslogic.template;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;

namespace uComponents.XsltExtensions
{
	/// <summary>
	/// A helper class for the XSLT extensions.
	/// </summary>
	public partial class Cms
	{
		/// <summary>
		/// Appends the type of the content.
		/// </summary>
		/// <param name="xd">The XML document.</param>
		/// <param name="elementName">Name of the element.</param>
		/// <param name="contentType">The content type.</param>
		/// <param name="includeTabs">if set to <c>true</c> [include tabs].</param>
		/// <param name="includePropertyTypes">if set to <c>true</c> [include property types].</param>
		internal static void AppendContentType(XmlDocument xd, string elementName, ContentType contentType, bool includeTabs, bool includePropertyTypes)
		{
			var nodeContentType = xmlHelper.addTextNode(xd, elementName, string.Empty);
			nodeContentType.AppendChild(xmlHelper.addCDataNode(xd, "description", contentType.Description));
			nodeContentType.Attributes.Append(xmlHelper.addAttribute(xd, "id", contentType.Id.ToString()));
			nodeContentType.Attributes.Append(xmlHelper.addAttribute(xd, "name", contentType.Text));
			nodeContentType.Attributes.Append(xmlHelper.addAttribute(xd, "alias", contentType.Alias));
			nodeContentType.Attributes.Append(xmlHelper.addAttribute(xd, "image", contentType.Image));
			nodeContentType.Attributes.Append(xmlHelper.addAttribute(xd, "thumbnail", contentType.Thumbnail));
			//// nodeContentType.Attributes.Append(xmlHelper.addAttribute(xd, "master", contentType.MasterContentType.ToString()));
			nodeContentType.Attributes.Append(xmlHelper.addAttribute(xd, "hasChildren", contentType.HasChildren.ToString()));

			if (includeTabs)
			{
				var tabs = contentType.getVirtualTabs;
				if (tabs != null && tabs.Length > 0)
				{
					var nodeTabs = xmlHelper.addTextNode(xd, "tabs", string.Empty);

					foreach (var tab in tabs)
					{
						var nodeTab = xmlHelper.addTextNode(xd, "tab", string.Empty);
						nodeTab.Attributes.Append(xmlHelper.addAttribute(xd, "id", tab.Id.ToString()));
						nodeTab.Attributes.Append(xmlHelper.addAttribute(xd, "caption", tab.Caption));
						nodeTab.Attributes.Append(xmlHelper.addAttribute(xd, "sortOrder", tab.SortOrder.ToString()));

						nodeTabs.AppendChild(nodeTab);
					}

					nodeContentType.AppendChild(nodeTabs);
				}
			}

			if (includePropertyTypes)
			{
				var propertyTypes = contentType.PropertyTypes;
				if (propertyTypes != null && propertyTypes.Count > 0)
				{
					var nodePropertyTypes = xmlHelper.addTextNode(xd, "propertyTypes", string.Empty);

					foreach (PropertyType propertyType in propertyTypes)
					{
						var nodePropertyType = xmlHelper.addTextNode(xd, "propertyType", string.Empty);
						nodePropertyType.AppendChild(xmlHelper.addCDataNode(xd, "description", propertyType.Description));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "id", propertyType.Id.ToString()));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "name", propertyType.Name));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "alias", propertyType.Alias));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "contentTypeId", propertyType.ContentTypeId.ToString()));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "mandatory", propertyType.Mandatory.ToString()));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "sortOrder", propertyType.SortOrder.ToString()));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "tabId", propertyType.TabId.ToString()));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "regEx", propertyType.ValidationRegExp));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "dataTypeId", propertyType.DataTypeDefinition.Id.ToString()));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "dataTypeGuid", propertyType.DataTypeDefinition.UniqueId.ToString()));

						nodePropertyTypes.AppendChild(nodePropertyType);
					}

					nodeContentType.AppendChild(nodePropertyTypes);
				}
			}

			// add the content-type node to the XmlDocument.
			if (xd.DocumentElement != null)
			{
				xd.DocumentElement.AppendChild(nodeContentType);
			}
			else
			{
				xd.AppendChild(nodeContentType);
			}
		}

		/// <summary>
		/// Appends the type of the document.
		/// </summary>
		/// <param name="xd">The XML document.</param>
		/// <param name="docType">The document type.</param>
		/// <param name="includeTabs">if set to <c>true</c> [include tabs].</param>
		/// <param name="includePropertyTypes">if set to <c>true</c> [include property types].</param>
		/// <param name="includeAllowedTemplates">if set to <c>true</c> [include allowed templates].</param>
		internal static void AppendDocumentType(XmlDocument xd, DocumentType docType, bool includeTabs, bool includePropertyTypes, bool includeAllowedTemplates)
		{
			var nodeDocType = xmlHelper.addTextNode(xd, "DocumentType", string.Empty);
			nodeDocType.AppendChild(xmlHelper.addCDataNode(xd, "description", docType.Description));
			nodeDocType.Attributes.Append(xmlHelper.addAttribute(xd, "id", docType.Id.ToString()));
			nodeDocType.Attributes.Append(xmlHelper.addAttribute(xd, "name", docType.Text));
			nodeDocType.Attributes.Append(xmlHelper.addAttribute(xd, "alias", docType.Alias));
			nodeDocType.Attributes.Append(xmlHelper.addAttribute(xd, "image", docType.Image));
			nodeDocType.Attributes.Append(xmlHelper.addAttribute(xd, "thumbnail", docType.Thumbnail));
			nodeDocType.Attributes.Append(xmlHelper.addAttribute(xd, "master", docType.MasterContentType.ToString()));
			nodeDocType.Attributes.Append(xmlHelper.addAttribute(xd, "hasChildren", docType.HasChildren.ToString()));
			nodeDocType.Attributes.Append(xmlHelper.addAttribute(xd, "defaultTemplate", docType.DefaultTemplate.ToString()));

			if (includeTabs)
			{
				var tabs = docType.getVirtualTabs;
				if (tabs != null && tabs.Length > 0)
				{
					var nodeTabs = xmlHelper.addTextNode(xd, "tabs", string.Empty);

					foreach (var tab in tabs)
					{
						var nodeTab = xmlHelper.addTextNode(xd, "tab", string.Empty);
						nodeTab.Attributes.Append(xmlHelper.addAttribute(xd, "id", tab.Id.ToString()));
						nodeTab.Attributes.Append(xmlHelper.addAttribute(xd, "caption", tab.Caption));
						nodeTab.Attributes.Append(xmlHelper.addAttribute(xd, "sortOrder", tab.SortOrder.ToString()));

						nodeTabs.AppendChild(nodeTab);
					}

					nodeDocType.AppendChild(nodeTabs);
				}
			}

			if (includePropertyTypes)
			{
				var propertyTypes = docType.PropertyTypes;
				if (propertyTypes != null && propertyTypes.Count > 0)
				{
					var nodePropertyTypes = xmlHelper.addTextNode(xd, "propertyTypes", string.Empty);

					foreach (var propertyType in propertyTypes)
					{
						var nodePropertyType = xmlHelper.addTextNode(xd, "propertyType", string.Empty);
						nodePropertyType.AppendChild(xmlHelper.addCDataNode(xd, "description", propertyType.Description));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "id", propertyType.Id.ToString()));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "name", propertyType.Name));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "alias", propertyType.Alias));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "contentTypeId", propertyType.ContentTypeId.ToString()));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "mandatory", propertyType.Mandatory.ToString()));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "sortOrder", propertyType.SortOrder.ToString()));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "tabId", propertyType.TabId.ToString()));
						nodePropertyType.Attributes.Append(xmlHelper.addAttribute(xd, "regEx", propertyType.ValidationRegExp));

						nodePropertyTypes.AppendChild(nodePropertyType);
					}

					nodeDocType.AppendChild(nodePropertyTypes);
				}
			}

			if (includeAllowedTemplates)
			{
				var templates = docType.allowedTemplates;
				if (templates != null && templates.Length > 0)
				{
					var nodeTemplates = xmlHelper.addTextNode(xd, "allowedTemplates", string.Empty);

					foreach (var template in templates)
					{
						var nodeTemplate = xmlHelper.addTextNode(xd, "template", string.Empty);
						nodeTemplate.Attributes.Append(xmlHelper.addAttribute(xd, "id", template.Id.ToString()));
						nodeTemplate.Attributes.Append(xmlHelper.addAttribute(xd, "name", template.Text));
						nodeTemplate.Attributes.Append(xmlHelper.addAttribute(xd, "alias", template.Alias));
						nodeTemplate.Attributes.Append(xmlHelper.addAttribute(xd, "masterPageFile", template.MasterPageFile));
						nodeTemplate.Attributes.Append(xmlHelper.addAttribute(xd, "masterTemplate", template.MasterTemplate.ToString()));
						nodeTemplate.Attributes.Append(xmlHelper.addAttribute(xd, "hasChildren", template.HasChildren.ToString()));
						nodeTemplate.Attributes.Append(xmlHelper.addAttribute(xd, "sortOrder", template.sortOrder.ToString()));
						nodeTemplate.Attributes.Append(xmlHelper.addAttribute(xd, "isDefaultTemplate", (template.Id == docType.DefaultTemplate).ToString()));

						nodeTemplates.AppendChild(nodeTemplate);
					}

					nodeDocType.AppendChild(nodeTemplates);
				}
			}

			if (xd.DocumentElement != null)
			{
				xd.DocumentElement.AppendChild(nodeDocType);
			}
			else
			{
				xd.AppendChild(nodeDocType);
			}
		}

		/// <summary>
		/// Appends the type of the media.
		/// </summary>
		/// <param name="xd">The XML document.</param>
		/// <param name="mediaType">The media type.</param>
		/// <param name="includeTabs">if set to <c>true</c> [include tabs].</param>
		/// <param name="includePropertyTypes">if set to <c>true</c> [include property types].</param>
		internal static void AppendMediaType(XmlDocument xd, MediaType mediaType, bool includeTabs, bool includePropertyTypes)
		{
			AppendContentType(xd, "MediaType", mediaType, includeTabs, includePropertyTypes);
		}

		/// <summary>
		/// Appends the type of the member.
		/// </summary>
		/// <param name="xd">The XML document.</param>
		/// <param name="memberType">Type of the member.</param>
		/// <param name="includeTabs">if set to <c>true</c> [include tabs].</param>
		/// <param name="includePropertyTypes">if set to <c>true</c> [include property types].</param>
		internal static void AppendMemberType(XmlDocument xd, MemberType memberType, bool includeTabs, bool includePropertyTypes)
		{
			AppendContentType(xd, "MemberType", memberType, includeTabs, includePropertyTypes);
		}

		/// <summary>
		/// Appends the property editor.
		/// </summary>
		/// <param name="xd">The XML document.</param>
		/// <param name="propertyEditor">The property editor.</param>
		internal static void AppendPropertyEditor(XmlDocument xd, IDataType propertyEditor)
		{
			var nodePropertyEditor = xmlHelper.addTextNode(xd, "PropertyEditor", string.Empty);

			nodePropertyEditor.Attributes.Append(xmlHelper.addAttribute(xd, "id", propertyEditor.Id.ToString()));
			nodePropertyEditor.Attributes.Append(xmlHelper.addAttribute(xd, "name", propertyEditor.DataTypeName));

			// add the property-editor node to the XmlDocument.
			if (xd.DocumentElement != null)
			{
				xd.DocumentElement.AppendChild(nodePropertyEditor);
			}
			else
			{
				xd.AppendChild(nodePropertyEditor);
			}
		}

		/// <summary>
		/// Gets all templates.
		/// </summary>
		/// <returns>Returns a list of templates.</returns>
		/// <remarks>
		/// This method checks if the template aliases are stored in memory.
		/// If so then a call to <c>umbraco.cms.businesslogic.templates.GetTemplate</c> will attempt to get the object from cache,
		/// rather than hit the database with <c>umbraco.cms.businesslogic.templates.GetAllAsList</c>.
		/// </remarks>
		internal static List<Template> GetAllTemplates()
		{
			var templates = new List<Template>();

			if (Template.TemplateAliases != null && Template.TemplateAliases.Count > 0)
			{
				foreach (var value in Template.TemplateAliases.Values)
				{
					var templateId = (int)value;
					var template = Template.GetTemplate(templateId);
					if (template != null)
					{
						templates.Add(template);
					}
				}

				templates.Sort(delegate(Template t1, Template t2) { return t1.Text.CompareTo(t2.Text); });
			}
			else
			{
				templates = Template.GetAllAsList();
			}

			return templates;
		}
	}
}