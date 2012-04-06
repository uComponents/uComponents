// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   12.01.2012 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.Core.DataTypes.DataTypeGrid.DataTypeFunctions
{
	using System;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;

	using umbraco.cms.businesslogic;
	using umbraco.editorControls.tinyMCE3;
	using umbraco.uicontrols;

	/// <summary>
	/// DTG functions for the Richtext Editor DataType
	/// </summary>
	internal class TinyMCE3DataTypeFunctions
	{
		public static void ConfigureForDtg(tinyMCE3dataType dataType, Control container)
		{
			var editor = dataType.DataEditor as TinyMCE;

			if (editor != null)
			{
				var menuContainer = new HtmlGenericControl("div");

				container.PreRender += (sender, args) =>
					{
						var script = new LiteralControl("<script type=\"text/javascript\">ConfigureTinyMceToolbar('umbTinymceMenu_" + editor.ClientID + "');</script>");

						var menu = new ScrollingMenu
						{
							ID = "dtgTinymceMenu_" + editor.ClientID,
							CssClass = "dtgTinymceMenu"
						};
						menu.NewElement("div", "umbTinymceMenu_" + editor.ClientID, "tinymceMenuBar", editor.ExtraMenuWidth);

						container.Controls.Add(script);
						menuContainer.Controls.Add(menu);
					};

				container.Controls.Add(menuContainer);
			}
		}

		/// <summary>
		/// Saves for DTG.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public static void SaveForDtg(tinyMCE3dataType dataType)
		{
			dataType.DataEditor.Save();
		}
	}
}
