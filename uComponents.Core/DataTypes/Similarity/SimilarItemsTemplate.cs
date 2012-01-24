using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using umbraco.controls.Images;
using umbraco.IO;

namespace uComponents.Core.DataTypes.Similarity
{
	/// <summary>
	/// used for left hand repeater to list items found by find similar
	/// </summary>
	internal class SimilarItemsTemplate : ITemplate
	{
		/// <summary>
		/// Creates the template for the repeater item
		/// </summary>
		/// <param name="container"></param>
		public void InstantiateIn(Control container)
		{
			var itemDiv = new HtmlGenericControl("div");
			itemDiv.ID = "Item";
			itemDiv.Attributes.Add("class", "item");

			var infoBtn = new HtmlAnchor();
			infoBtn.ID = "InfoButton";
			infoBtn.HRef = "javascript:void(0);";
			infoBtn.Attributes.Add("class", "info");
			itemDiv.Controls.Add(infoBtn);

			var innerDiv = new HtmlGenericControl("div");
			innerDiv.ID = "InnerItem";
			innerDiv.Attributes.Add("class", "inner");

			innerDiv.Controls.Add(
				new LiteralControl(@"<ul class=""rightNode"">"));

			var liSelectNode = new HtmlGenericControl("li");
			liSelectNode.Attributes.Add("class", "closed");
			liSelectNode.ID = "SelectedNodeListItem";
			innerDiv.Controls.Add(liSelectNode);

			var selectedNodeLink = new HtmlAnchor();
			selectedNodeLink.ID = "SelectedNodeLink";

			innerDiv.Controls.Add(selectedNodeLink);

			var selectedNodeText = new LiteralControl();
			selectedNodeText.ID = "SelectedNodeText";
			innerDiv.Controls.Add(selectedNodeText);

			selectedNodeLink.Controls.Add(new LiteralControl("<div>"));
			selectedNodeLink.Controls.Add(selectedNodeText);
			selectedNodeLink.Controls.Add(new LiteralControl("</div>"));

			liSelectNode.Controls.Add(selectedNodeLink);

			innerDiv.Controls.Add(
				new LiteralControl(@"</ul>"));

			itemDiv.Controls.Add(innerDiv);

			container.Controls.Add(itemDiv);
		}
	}
}