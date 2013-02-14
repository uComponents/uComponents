
namespace uComponents.Core
{
	/// <summary>
	/// Constants class for uComponent specific values.
	/// </summary>
	public partial class Constants
	{
		/// <summary>
		/// Constants for Umbraco property aliases.
		/// </summary>
		public partial struct Umbraco
		{
			/// <summary>
			/// Constants for Umbraco Content property aliases.
			/// </summary>
			public partial struct Content
			{
				/// <summary>
				/// Property alias for the Content's Url (internal) redirect.
				/// </summary>
				public const string InternalRedirectId = "umbracoInternalRedirectId";

				/// <summary>
				/// Property alias for the Content's navigational hide, (not actually used in core code).
				/// </summary>
				public const string NaviHide = "umbracoNaviHide";

				/// <summary>
				/// Property alias for the Content's Url redirect.
				/// </summary>
				public const string Redirect = "umbracoRedirect";

				/// <summary>
				/// Property alias for the Content's Url alias.
				/// </summary>
				public const string UrlAlias = "umbracoUrlAlias";

				/// <summary>
				/// Property alias for the Content's Url name.
				/// </summary>
				public const string UrlName = "umbracoUrlName";
			}

			/// <summary>
			/// Constants for Umbraco Media property aliases.
			/// </summary>
			public partial struct Media
			{
				/// <summary>
				/// Property alias for the Media's file name.
				/// </summary>
				public const string File = "umbracoFile";

				/// <summary>
				/// Property alias for the Media's width.
				/// </summary>
				public const string Width = "umbracoWidth";

				/// <summary>
				/// Property alias for the Media's height.
				/// </summary>
				public const string Height = "umbracoHeight";

				/// <summary>
				/// Property alias for the Media's file size (in bytes).
				/// </summary>
				public const string Bytes = "umbracoBytes";

				/// <summary>
				/// Property alias for the Media's file extension.
				/// </summary>
				public const string Extension = "umbracoExtension";
			}

			/// <summary>
			/// Constants for Umbraco ObjectTypes Guids.
			/// </summary>
			public partial struct ObjectTypes
			{
				/// <summary>
				/// Guid for the ContentItemType object-type.
				/// </summary>
				public const string ContentItemType = "7A333C54-6F43-40A4-86A2-18688DC7E532";

				/// <summary>
				/// Guid for the ROOT object-type.
				/// </summary>
				public const string ROOT = "EA7D8624-4CFE-4578-A871-24AA946BF34D";

				/// <summary>
				/// Guid for the Document object-type.
				/// </summary>
				public const string Document = "C66BA18E-EAF3-4CFF-8A22-41B16D66A972";

				/// <summary>
				/// Guid for the Media object-type.
				/// </summary>
				public const string Media = "B796F64C-1F99-4FFB-B886-4BF4BC011A9C";

				/// <summary>
				/// Guid for the MemberType object-type.
				/// </summary>
				public const string MemberType = "9B5416FB-E72F-45A9-A07B-5A9A2709CE43";

				/// <summary>
				/// Guid for the Template object-type.
				/// </summary>
				public const string Template = "6FBDE604-4178-42CE-A10B-8A2600A2F07D";

				/// <summary>
				/// Guid for the MemberGroup object-type.
				/// </summary>
				public const string MemberGroup = "366E63B9-880F-4E13-A61C-98069B029728";

				/// <summary>
				/// Guid for the ContentItem object-type.
				/// </summary>
				public const string ContentItem = "10E2B09F-C28B-476D-B77A-AA686435E44A";

				/// <summary>
				/// Guid for the MediaType object-type.
				/// </summary>
				public const string MediaType = "4EA4382B-2F5A-4C2B-9587-AE9B3CF3602E";

				/// <summary>
				/// Guid for the DocumentType object-type.
				/// </summary>
				public const string DocumentType = "A2CB7800-F571-4787-9638-BC48539A0EFB";

				/// <summary>
				/// Guid for the RecycleBin object-type.
				/// </summary>
				public const string RecycleBin = "01BB7FF2-24DC-4C0C-95A2-C24EF72BBAC8";

				/// <summary>
				/// Guid for the Stylesheet object-type.
				/// </summary>
				public const string Stylesheet = "9F68DA4F-A3A8-44C2-8226-DCBD125E4840";

				/// <summary>
				/// Guid for the Member object-type.
				/// </summary>
				public const string Member = "39EB0F98-B348-42A1-8662-E7EB18487560";

				/// <summary>
				/// Guid for the DataType object-type.
				/// </summary>
				public const string DataType = "30A2A501-1978-4DDB-A57B-F7EFED43BA3C";
			}

			/// <summary>
			/// Constants for Umbraco URLs/Querystrings.
			/// </summary>
			public partial struct Url
			{
				/// <summary>
				/// Querystring parameter name used for Umbraco's alternative template functionality.
				/// </summary>
				public const string AltTemplate = "altTemplate";
			}
		}
	}
}