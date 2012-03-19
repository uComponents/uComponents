using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using uComponents.Core.Shared.Extensions;
using umbraco;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.language;
using umbraco.cms.businesslogic.macro;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.template;
using umbraco.cms.businesslogic.web;

namespace uComponents.Core.XsltExtensions
{
	/// <summary>
	/// The Cms class exposes XSLT extensions to access data from various parts of the Umbraco framework.
	/// </summary>
	public partial class Cms
	{
		/// <summary>
		/// Returns an XPathNodeIterator of all the languages created in Umbraco.
		/// </summary>
		/// <returns>A node-set of all the languages used within Umbraco</returns>
		public static XPathNodeIterator GetLanguages()
		{
			try
			{
				var xd = new XmlDocument();
				xd.LoadXml("<Languages/>");

				var languages = Language.GetAllAsList();

				foreach (var language in languages)
				{
					var languageNode = language.ToXml(xd);
					xd.DocumentElement.AppendChild(languageNode);
				}

				return xd.CreateNavigator().Select("/Languages");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Gets the language by id.
		/// </summary>
		/// <param name="languageId">The language id.</param>
		/// <returns>A node-set of the specified language.</returns>
		public static XPathNodeIterator GetLanguage(int languageId)
		{
			try
			{
				var xd = new XmlDocument();
				var language = new Language(languageId);

				if (language != null)
				{
					var languageNode = language.ToXml(xd);
					xd.AppendChild(languageNode);
				}

				return xd.CreateNavigator().Select("/Language");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Gets the language by culture code.
		/// </summary>
		/// <param name="cultureCode">The culture code.</param>
		/// <returns>Returns a node-set of the specified language.</returns>
		public static XPathNodeIterator GetLanguageByCultureCode(string cultureCode)
		{
			try
			{
				var xd = new XmlDocument();
				var language = Language.GetByCultureCode(cultureCode);

				if (language != null)
				{
					var languageNode = language.ToXml(xd);
					xd.AppendChild(languageNode);
				}

				return xd.CreateNavigator().Select("/Language");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Gets the language by node id.
		/// </summary>
		/// <param name="nodeId">The node id.</param>
		/// <returns>Returns a node-set of the language by the node id.</returns>
		public static XPathNodeIterator GetLanguageByNodeId(int nodeId)
		{
			// get the language id by node id.
			var languageId = GetLanguageIdByNodeId(nodeId);

			// return the language.
			return GetLanguage(languageId);
		}

		/// <summary>
		/// Gets the language id by node id.
		/// </summary>
		/// <param name="nodeId">The node id.</param>
		/// <returns>Returns the id of the language by the node id.</returns>
		public static int GetLanguageIdByNodeId(int nodeId)
		{
			// get the domains for the node Id.
			var domains = library.GetCurrentDomains(nodeId);

			// check that a domain exists.
			if (domains != null && domains.Length > 0)
			{
				// return the language from the first domain.
				return domains[0].Language.id;
			}

			// otherwise return '1' (for the default language).
			return 1;
		}

		/// <summary>
		/// Returns an XPathNodeIterator of all the document types created in Umbraco.
		/// </summary>
		/// <returns>A node-set of all the document types used within Umbraco.</returns>
		public static XPathNodeIterator GetDocumentTypes()
		{
			return GetDocumentTypes(false, false, false);
		}

		/// <summary>
		/// Returns an XPathNodeIterator of all the document types created in Umbraco.
		/// </summary>
		/// <param name="includeTabs">Boolean to choose whether to include the (virtual) tabs for the document type.</param>
		/// <param name="includePropertyTypes">Boolean to choose whether to include the property types for the document type.</param>
		/// <param name="includeAllowedTemplates">Boolean to choose whether to include the allowed templates for the document type.</param>
		/// <returns>
		/// A node-set of all the document types used within Umbraco.
		/// </returns>
		public static XPathNodeIterator GetDocumentTypes(bool includeTabs, bool includePropertyTypes, bool includeAllowedTemplates)
		{
			try
			{
				var xd = new XmlDocument();
				xd.LoadXml("<DocumentTypes/>");

				var docTypes = DocumentType.GetAllAsList();

				foreach (var docType in docTypes)
				{
					AppendDocumentType(xd, docType, includeTabs, includePropertyTypes, includeAllowedTemplates);
				}

				return xd.CreateNavigator().Select("/DocumentTypes");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Returns an XPathNodeIterator of the specified document-type.
		/// </summary>
		/// <param name="docTypeId">The document-type id.</param>
		/// <param name="includeTabs">Boolean to choose whether to include the (virtual) tabs for the document-type.</param>
		/// <param name="includePropertyTypes">Boolean to choose whether to include the property types for the document-type.</param>
		/// <param name="includeAllowedTemplates">Boolean to choose whether to include the allowed templates for the document-type.</param>
		/// <returns>
		/// A node-set of the specified document-type.
		/// </returns>
		public static XPathNodeIterator GetDocumentType(int docTypeId, bool includeTabs, bool includePropertyTypes, bool includeAllowedTemplates)
		{
			try
			{
				var xd = new XmlDocument();
				var docType = new DocumentType(docTypeId);

				if (docType != null)
				{
					AppendDocumentType(xd, docType, includeTabs, includePropertyTypes, includeAllowedTemplates);
				}

				return xd.CreateNavigator().Select("/DocumentType");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Returns an XPathNodeIterator of all the media types created in Umbraco.
		/// </summary>
		/// <returns>
		/// A node-set of all the media types used within Umbraco.
		/// </returns>
		public static XPathNodeIterator GetMediaTypes()
		{
			return GetMediaTypes(false, false);
		}

		/// <summary>
		/// Returns an XPathNodeIterator of all the media types created in Umbraco.
		/// </summary>
		/// <param name="includeTabs">Boolean to choose whether to include the (virtual) tabs for the media type.</param>
		/// <param name="includePropertyTypes">Boolean to choose whether to include the property types for the media type.</param>
		/// <returns>
		/// A node-set of all the media types used within Umbraco.
		/// </returns>
		public static XPathNodeIterator GetMediaTypes(bool includeTabs, bool includePropertyTypes)
		{
			try
			{
				var xd = new XmlDocument();
				xd.LoadXml("<MediaTypes/>");

				var mediaTypes = MediaType.GetAllAsList();

				foreach (var mediaType in mediaTypes)
				{
					AppendMediaType(xd, mediaType, includeTabs, includePropertyTypes);
				}

				return xd.CreateNavigator().Select("/MediaTypes");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Returns an XPathNodeIterator of the specified media-type.
		/// </summary>
		/// <param name="mediaTypeId">The media-type id.</param>
		/// <param name="includeTabs">if set to <c>true</c> [include tabs].</param>
		/// <param name="includePropertyTypes">if set to <c>true</c> [include property types].</param>
		/// <returns>A node-set of the specified media-type.</returns>
		public static XPathNodeIterator GetMediaType(int mediaTypeId, bool includeTabs, bool includePropertyTypes)
		{
			try
			{
				var xd = new XmlDocument();
				var mediaType = new MediaType(mediaTypeId);

				if (mediaType != null)
				{
					AppendMediaType(xd, mediaType, includeTabs, includePropertyTypes);
				}

				return xd.CreateNavigator().Select("/MediaType");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Returns an XPathNodeIterator of all the member types created in Umbraco.
		/// </summary>
		/// <returns>
		/// A node-set of all the member types used within Umbraco.
		/// </returns>
		public static XPathNodeIterator GetMemberTypes()
		{
			return GetMemberTypes(false, false);
		}

		/// <summary>
		/// Returns an XPathNodeIterator of all the member types created in Umbraco.
		/// </summary>
		/// <param name="includeTabs">Boolean to choose whether to include the (virtual) tabs for the member type.</param>
		/// <param name="includePropertyTypes">Boolean to choose whether to include the property types for the member type.</param>
		/// <returns>
		/// A node-set of all the member types used within Umbraco.
		/// </returns>
		public static XPathNodeIterator GetMemberTypes(bool includeTabs, bool includePropertyTypes)
		{
			try
			{
				var xd = new XmlDocument();
				xd.LoadXml("<MemberTypes/>");

				var memeberTypes = MemberType.GetAll;

				foreach (var memeberType in memeberTypes)
				{
					AppendMemberType(xd, memeberType, includeTabs, includePropertyTypes);
				}

				return xd.CreateNavigator().Select("/MemberTypes");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Returns an XPathNodeIterator of the specified member-type.
		/// </summary>
		/// <param name="memberTypeId">The member type id.</param>
		/// <param name="includeTabs">if set to <c>true</c> [include tabs].</param>
		/// <param name="includePropertyTypes">if set to <c>true</c> [include property types].</param>
		/// <returns>A node-set of the specified member-type.</returns>
		public static XPathNodeIterator GetMemberType(int memberTypeId, bool includeTabs, bool includePropertyTypes)
		{
			try
			{
				var xd = new XmlDocument();
				var memberType = new MemberType(memberTypeId);

				if (memberType != null)
				{
					AppendMemberType(xd, memberType, includeTabs, includePropertyTypes);
				}

				return xd.CreateNavigator().Select("/MemberType");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Returns an XPathNodeIterator of all the macros created in Umbraco.
		/// </summary>
		/// <returns>A node-set of all the macros used within Umbraco.</returns>
		public static XPathNodeIterator GetMacros()
		{
			try
			{
				var xd = new XmlDocument();
				xd.LoadXml("<macros/>");

				var macros = Macro.GetAll();

				foreach (var macro in macros)
				{
					var macroNode = macro.ToXml(xd);
					xd.DocumentElement.AppendChild(macroNode);
				}

				return xd.CreateNavigator().Select("/macros");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Returns an XPathNodeIterator of the specified macro.
		/// </summary>
		/// <param name="alias">The alias of the macro.</param>
		/// <returns>A node-set of the specified macro.</returns>
		public static XPathNodeIterator GetMacro(string alias)
		{
			try
			{
				var xd = new XmlDocument();
				var macro = new Macro(alias);

				if (macro != null)
				{
					var macroNode = macro.ToXml(xd);
					xd.AppendChild(macroNode);
				}

				return xd.CreateNavigator().Select("/macro");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Returns an XPathNodeIterator of all the data types created in Umbraco.
		/// </summary>
		/// <returns>A node-set of all the data types used within Umbraco.</returns>
		public static XPathNodeIterator GetDataTypes()
		{
			try
			{
				var xd = new XmlDocument();
				xd.LoadXml("<DataTypes/>");

				var dataTypes = DataTypeDefinition.GetAll();

				foreach (var dataType in dataTypes)
				{
					var dataTypeNode = dataType.ToXml(xd);
					xd.DocumentElement.AppendChild(dataTypeNode);
				}

				return xd.CreateNavigator().Select("/DataTypes");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Returns an XPathNodeIterator of the specified data-type by guid.
		/// </summary>
		/// <param name="dataTypeGuid">The guid of the data-type.</param>
		/// <returns>A node-set of the specified data-type</returns>
		public static XPathNodeIterator GetDataTypeByGuid(string dataTypeGuid)
		{
			try
			{
				var guid = new Guid(dataTypeGuid);
				var xd = new XmlDocument();
				var dataType = new DataTypeDefinition(guid);

				if (dataType != null)
				{
					var dataTypeNode = dataType.ToXml(xd);
					xd.AppendChild(dataTypeNode);
				}

				return xd.CreateNavigator().Select("/DataType");

			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Returns an XPathNodeIterator of the specified data-type by Id.
		/// </summary>
		/// <param name="dataTypeId">The data-type Id.</param>
		/// <returns>A node-set of the specified data-type</returns>
		public static XPathNodeIterator GetDataTypeById(int dataTypeId)
		{
			try
			{
				var xd = new XmlDocument();
				var dataType = new DataTypeDefinition(dataTypeId);

				if (dataType != null)
				{
					var dataTypeNode = dataType.ToXml(xd);
					xd.AppendChild(dataTypeNode);
				}

				return xd.CreateNavigator().Select("/DataType");

			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Gets the templates.
		/// </summary>
		/// <returns>
		/// Returns an XML structure of all the templates.
		/// </returns>
		public static XPathNodeIterator GetTemplates()
		{
			try
			{
				var xd = new XmlDocument();
				var templates = GetAllTemplates();

				xd.LoadXml("<Templates/>");

				if (templates != null)
				{
					foreach (var template in templates)
					{
						var templateNode = template.ToXml(xd);
						xd.DocumentElement.AppendChild(templateNode);
					}
				}

				return xd.CreateNavigator().Select("/Templates");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Gets the template alias.
		/// </summary>
		/// <param name="templateId">The template id.</param>
		/// <returns>Returns the alias name of the template.</returns>
		public static string GetTemplateAlias(int templateId)
		{
			if (templateId > 0)
			{
				try
				{
					var template = Template.GetTemplate(templateId);

					if (template != null)
					{
						return template.Alias;
					}
				}
				catch
				{
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// Gets the template by id.
		/// </summary>
		/// <param name="templateId">The template id.</param>
		/// <returns>
		/// Returns an XML structure of the template.
		/// </returns>
		public static XPathNodeIterator GetTemplateById(int templateId)
		{
			try
			{
				var xd = new XmlDocument();
				var template = Template.GetTemplate(templateId);

				if (template != null)
				{
					var templateNode = template.ToXml(xd);
					xd.AppendChild(templateNode);
				}

				return xd.CreateNavigator().Select("/Template");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Gets the template by alias.
		/// </summary>
		/// <param name="templateAlias">The template alias.</param>
		/// <returns>
		/// Returns an XML structure of the template.
		/// </returns>
		public static XPathNodeIterator GetTemplateByAlias(string templateAlias)
		{
			try
			{
				var templateId = GetTemplateIdByAlias(templateAlias);
				return GetTemplateById(templateId);
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Gets the template id by alias.
		/// </summary>
		/// <param name="templateAlias">The template alias.</param>
		/// <returns>Returns the template id.</returns>
		public static int GetTemplateIdByAlias(string templateAlias)
		{
			return Template.GetTemplateIdFromAlias(templateAlias.ToLower());
		}

		/// <summary>
		/// Gets the dictionary item for the specified language id.
		/// </summary>
		/// <param name="key">The key for the dictionary item.</param>
		/// <param name="languageId">The language id.</param>
		/// <returns>
		/// Returns the string value of the dictionary item for the specified language id.
		/// </returns>
		/// <remarks>
		/// This is an overloaded method for <c>umbraco.library.GetDictionaryItem(string Key)</c> to accept the language id parameter.
		/// </remarks>
		public static string GetDictionaryItem(string key, int languageId)
		{
			return GetDictionaryItem(key, languageId, string.Empty);
		}

		/// <summary>
		/// Gets the dictionary item for the specified language id, with a fall-back default value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="languageId">The language id.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>Returns the string value of the dictionary item for the specified language id.</returns>
		/// <remarks>
		/// This is an overloaded method for <c>umbraco.library.GetDictionaryItem(string Key)</c> to accept the language id and default value parameters.
		/// </remarks>
		public static string GetDictionaryItem(string key, int languageId, string defaultValue)
		{
			return uQuery.GetDictionaryItem(key, defaultValue, languageId);
		}

		/// <summary>
		/// Gets the unique id of a CMSNode.
		/// </summary>
		/// <param name="id">The CMSNode id.</param>
		/// <returns>Returns the unique id of a CMSNode.</returns>
		public static string GetUniqueId(int id)
		{
			try
			{
				var cmsNode = new CMSNode(id);

				if (cmsNode != null)
				{
					return cmsNode.UniqueId.ToString();
				}
			}
			catch
			{
			}

			return string.Empty;
		}

		/// <summary>
		/// Checks the Umbraco XML Schema version in use
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if [is legacy XML schema]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsLegacyXmlSchema()
		{
			return uQuery.IsLegacyXmlSchema();
		}

		/// <summary>
		/// Determines whether Umbraco front-end [is in preview mode].
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if Umbraco front-end [is in preview mode]; otherwise, <c>false</c>.
		/// </returns>
		public static bool InPreviewMode()
		{
			return umbraco.presentation.UmbracoContext.Current.InPreviewMode;
		}
	}
}