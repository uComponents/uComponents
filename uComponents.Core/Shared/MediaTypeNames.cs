using System;
namespace uComponents.Core.Shared
{
	/// <summary>
	/// Container class for MIME types.
	/// </summary>
	/// <remarks>Idea based on the <c>System.Net.Mime.MediaTypeNames</c> class.</remarks>
	public static class MediaTypeNames
	{
		/// <summary>
		/// Class containing MIME type constants for application files.
		/// </summary>
		public static class Application
		{
			/// <summary>
			/// MIME type for JavaScript files/scripts.
			/// </summary>
			public const string JavaScript = "application/x-javascript";

			/// <summary>
			/// MIME type for JSON text.
			/// </summary>
			public const string Json = "application/json";
		}

		/// <summary>
		/// Class containing MIME type constants for image files.
		/// </summary>
		public static class Image
		{
			/// <summary>
			/// MIME type for GIF images.
			/// </summary>
			public const string Gif = System.Net.Mime.MediaTypeNames.Image.Gif;

			/// <summary>
			/// MIME type for JPEG images.
			/// </summary>
			public const string Jpeg = System.Net.Mime.MediaTypeNames.Image.Jpeg;

			/// <summary>
			/// MIME type for PNG images.
			/// </summary>
			public const string Png = "image/png";
		}

		/// <summary>
		/// Class containing MIME type constants for text files.
		/// </summary>
		public static class Text
		{
			/// <summary>
			/// MIME type for Cascading StyleSheet files.
			/// </summary>
			public const string Css = "text/css";

			/// <summary>
			/// MIME type for JavaScript files.
			/// </summary>
			[Obsolete("Please use uComponents.Core.Shared.MediaTypeNames.Application.JavaScript", true)]
			public const string JavaScript = "text/javascript";
		}
	}
}