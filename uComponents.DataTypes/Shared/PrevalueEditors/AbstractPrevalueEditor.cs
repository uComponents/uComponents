using System;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;
using umbraco.interfaces;

namespace uComponents.DataTypes.Shared.PrevalueEditors
{
	using System.Diagnostics;
	using System.Reflection;

	/// <summary>
	/// Abstract class for the PreValue Editor.
	/// </summary>
	public abstract class AbstractPrevalueEditor : WebControl, IDataPrevalue
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractPrevalueEditor"/> class.
		/// </summary>
		public AbstractPrevalueEditor()
			: base()
		{
		}

		/// <summary>
		/// Gets the editor.
		/// </summary>
		/// <value>The editor.</value>
		public virtual Control Editor
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public virtual void Save()
		{
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
			this.RegisterEmbeddedClientResource(typeof(AbstractPrevalueEditor), Constants.PrevalueEditorCssResourcePath, ClientDependencyType.Css);
		}

		/// <summary>
		/// Renders the HTML opening tag of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			var infoVersion = Helper.IO.GetAssemblyInformationalVersion(Assembly.GetExecutingAssembly());

			writer.AddAttribute(HtmlTextWriterAttribute.Class, Constants.ApplicationName);
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			// Render logo and version info
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "logo");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			writer.AddAttribute(HtmlTextWriterAttribute.Href, string.Concat("http://ucomponents.org/?v=", HttpUtility.UrlEncode(infoVersion)));
			writer.AddAttribute(HtmlTextWriterAttribute.Target, "_blank");
			writer.AddAttribute(HtmlTextWriterAttribute.Title, Helper.Dictionary.GetDictionaryItem("DocumentationForUComponents", "Documentation for uComponents"));
			writer.RenderBeginTag(HtmlTextWriterTag.A);

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "version");
			writer.RenderBeginTag(HtmlTextWriterTag.Span);
			writer.Write("{0} {1}", Helper.Dictionary.GetDictionaryItem("Version", "Version"), infoVersion);
			writer.RenderEndTag(); // span.version

			writer.RenderEndTag(); // a

			writer.RenderEndTag(); // div.logo

			base.RenderBeginTag(writer);
		}

		/// <summary>
		/// Renders the HTML closing tag of the control into the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		public override void RenderEndTag(HtmlTextWriter writer)
		{
			base.RenderEndTag(writer);

			writer.RenderEndTag();
		}
	}
}