using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;

using umbraco;
using umbraco.interfaces;
using umbraco.editorControls;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls.relatedlinks;
using umbraco.IO;
using uComponents.Core.Shared;

namespace uComponents.Core.DataTypes.RelatedLinksWithMedia
{
	/// <summary>
	/// The DataEdtior for the RelatedLinksWithMedia data-type.
	/// </summary>
	[ValidationProperty("IsValid")]
	public class RelatedLinkswMediaDataEditor : UpdatePanel, IDataEditor
	{
		/// <summary>
		/// 
		/// </summary>
		private umbraco.interfaces.IData _data;

		/// <summary>
		/// 
		/// </summary>
		string _configuration;

		/// <summary>
		/// 
		/// </summary>
		private ListBox _listboxLinks;

		/// <summary>
		/// 
		/// </summary>
		private Button _buttonUp;

		/// <summary>
		/// 
		/// </summary>
		private Button _buttonDown;

		/// <summary>
		/// 
		/// </summary>
		private Button _buttonEdit;

		/// <summary>
		/// 
		/// </summary>
		private Button _buttonDelete;

		/// <summary>
		/// 
		/// </summary>
		private TextBox _textboxLinkTitle;

		/// <summary>
		/// 
		/// </summary>
		private CheckBox _checkNewWindow;

		/// <summary>
		/// 
		/// </summary>
		private TextBox _textBoxExtUrl;

		/// <summary>
		/// 
		/// </summary>
		private Button _buttonAddExtUrl;

		/// <summary>
		/// 
		/// </summary>
		private Button _buttonAddIntUrlCP;

		/// <summary>
		/// 
		/// </summary>
		private Button _buttonAddMediaCP;

		/// <summary>
		/// 
		/// </summary>
		private Button _buttonEditExtUrl;

		/// <summary>
		/// 
		/// </summary>
		private Button _buttonEditIntUrlCP;

		/// <summary>
		/// 
		/// </summary>
		private Button _buttonEditMediaCP;

		/// <summary>
		/// 
		/// </summary>
		private XmlDocument _xml;

		/// <summary>
		/// 
		/// </summary>
		private pagePicker _pagePicker;

		/// <summary>
		/// 
		/// </summary>
		private PagePickerwMediaDataExtractor _pagePickerExtractor;

		/// <summary>
		/// 
		/// </summary>
		private mediaChooser _mediaChooser;

		/// <summary>
		/// 
		/// </summary>
		private PagePickerwMediaDataExtractor _mediaChooserExtractor;

		/// <summary>
		/// Initializes a new instance of the <see cref="RelatedLinkswMediaDataEditor"/> class.
		/// </summary>
		/// <param name="Data">The data.</param>
		/// <param name="Configuration">The configuration.</param>
		public RelatedLinkswMediaDataEditor(umbraco.interfaces.IData Data, string Configuration)
		{
			_data = Data;
			_configuration = Configuration;
		}

		/// <summary>
		/// Gets a value indicating whether [treat as rich text editor].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [treat as rich text editor]; otherwise, <c>false</c>.
		/// </value>
		public virtual bool TreatAsRichTextEditor
		{
			get { return false; }
		}

		/// <summary>
		/// Internal logic for validation controls to detect whether or not it's valid (has to be public though) 
		/// </summary>
		/// <value>Am I valid?</value>
		public string IsValid
		{
			get
			{
				if (_listboxLinks != null)
				{
					if (_listboxLinks.Items.Count > 0)
						return "Valid";
				}
				return "";
			}
		}

		/// <summary>
		/// Gets a value indicating whether [show label].
		/// </summary>
		/// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
		public bool ShowLabel
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the editor.
		/// </summary>
		/// <value>The editor.</value>
		public Control Editor { get { return this; } }

		/// <summary>
		/// Saves this instance.
		/// </summary>
		/// <remarks>
		/// Creates and saves a xml format of the content of the _listboxLinks
		/// We could adapt the global xml at every adjustment, but this implementation is easier
		/// (and possibly more efficient).
		/// </remarks>
		/// <example>
		///		<links>
		///		   <link type="external" title="google" link="http://google.com" newwindow="1" />
		///		   <link type="internal" title="home" link="1234" newwindow="0" />
		///		</links>
		/// </example>
		public void Save()
		{
			XmlDocument doc = createBaseXmlDocument();
			XmlNode root = doc.DocumentElement;
			foreach (ListItem item in _listboxLinks.Items)
			{
				string value = item.Value;

				XmlNode newNode = doc.CreateElement("link");

				XmlNode titleAttr = doc.CreateNode(XmlNodeType.Attribute, "title", null);
				titleAttr.Value = item.Text;
				newNode.Attributes.SetNamedItem(titleAttr);

				XmlNode linkAttr = doc.CreateNode(XmlNodeType.Attribute, "link", null);
				linkAttr.Value = value.Substring(2);
				newNode.Attributes.SetNamedItem(linkAttr);

				XmlNode typeAttr = doc.CreateNode(XmlNodeType.Attribute, "type", null);
				if (value.Substring(0, 1).Equals("i"))
					typeAttr.Value = "internal";
				else if (value.Substring(0, 1).Equals("m"))
					typeAttr.Value = "media";
				else
					typeAttr.Value = "external";
				newNode.Attributes.SetNamedItem(typeAttr);

				XmlNode windowAttr = doc.CreateNode(XmlNodeType.Attribute, "newwindow", null);
				if (value.Substring(1, 1).Equals("n"))
					windowAttr.Value = "1";
				else
					windowAttr.Value = "0";
				newNode.Attributes.SetNamedItem(windowAttr);

				root.AppendChild(newNode);
			}

			this._data.Value = doc.InnerXml;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		/// <exception cref="T:System.InvalidOperationException">
		/// The <see cref="P:System.Web.UI.UpdatePanel.ContentTemplate"/> property is being defined when the <see cref="P:System.Web.UI.UpdatePanel.ContentTemplateContainer"/> property has already been created.
		/// </exception>
		/// <remarks>
		/// Draws the controls, only gets called for the first drawing of the page, not for each postback
		/// </remarks>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			string editLink = string.Format(@"
                        function {0}_relatedLinkEdit() {{
                            var idx = document.getElementById('{0}links').selectedIndex;
                            if (idx != -1) {{
                                var link = document.getElementById('{0}links')[idx].value;
                                var text = document.getElementById('{0}links')[idx].text;
                                if (link.length > 2) {{
                                    var linkType = link.substring(0,1);
                                    var linkNewWind = link.substring(1,2);
                                    var linkLink = link.substring(2);

                                    {0}_closeLinkContainer();

                                    document.getElementById('{0}linktitle').value = text;
                                    if (linkNewWind == 'n') {{
                                        document.getElementById('{0}checkNewWindow').checked = true;
                                    }}
                                    else {{
                                        document.getElementById('{0}checkNewWindow').checked = false;
                                    }}
                                    switch (linkType)
                                    {{
                                        case 'i':
                                            var e = {{ outVal: linkLink }};
                                            mc_{0}pagePicker.SaveSelection(e);
                                            document.getElementById('{0}_addInternalLinkPanel').style.display='block';
                                            document.getElementById('{0}_editInternalLinkButton').style.display='block';
                                            break;

                                        case 'm':
                                            var e = {{ outVal: linkLink }};
                                            mc_{0}mediaChooser.SaveSelection(e);
                                            document.getElementById('{0}_addMediaLinkPanel').style.display='block';
                                            document.getElementById('{0}_editMediaLinkButton').style.display='block';
                                            break;

                                        case 'e':
                                            document.getElementById('{0}exturl').value = linkLink;
                                            document.getElementById('{0}_addExternalLinkPanel').style.display='block';
                                            document.getElementById('{0}_editExternalLinkButton').style.display='block';
                                            break;
                                    }}
                                    document.getElementById('{0}_addLinkContainer').style.display='block';
                                }}
                            }}
                        }}

                        function {0}_addInternalLink() {{
                            {0}_closeLinkContainer();
                            document.getElementById('{0}linktitle').value = '';
                            document.getElementById('{0}checkNewWindow').checked = false;
                            //{0}pagePicker_clear();
                            mc_{0}pagePicker.ClearSelection();

                            document.getElementById('{0}_addInternalLinkPanel').style.display='block';
                            document.getElementById('{0}_addInternalLinkButton').style.display='block';
                            document.getElementById('{0}_addLinkContainer').style.display='block';
                        }}
                        function {0}_addMediaLink() {{
                            {0}_closeLinkContainer();
                            document.getElementById('{0}linktitle').value = '';
                            document.getElementById('{0}checkNewWindow').checked = false;
                            //{0}mediaChooser_clear();
                            mc_{0}mediaChooser.ClearSelection();

                            document.getElementById('{0}_addMediaLinkPanel').style.display='block';
                            document.getElementById('{0}_addMediaLinkButton').style.display='block';
                            document.getElementById('{0}_addLinkContainer').style.display='block';
                        }}
                        function {0}_addExternalLink() {{
                            {0}_closeLinkContainer();
                            document.getElementById('{0}linktitle').value = '';
                            document.getElementById('{0}checkNewWindow').checked = false;
                            document.getElementById('{0}exturl').value = 'http://';

                            document.getElementById('{0}_addExternalLinkPanel').style.display='block';
                            document.getElementById('{0}_addExternalLinkButton').style.display='block';
                            document.getElementById('{0}_addLinkContainer').style.display='block';
                        }}
                        function {0}_closeLinkContainer() {{
                            document.getElementById('{0}_addInternalLinkPanel').style.display='none';
                            document.getElementById('{0}_addInternalLinkButton').style.display='none';
                            document.getElementById('{0}_editInternalLinkButton').style.display='none';

                            document.getElementById('{0}_addMediaLinkPanel').style.display='none';
                            document.getElementById('{0}_addMediaLinkButton').style.display='none';
                            document.getElementById('{0}_editMediaLinkButton').style.display='none';

                            document.getElementById('{0}_addExternalLinkPanel').style.display='none';
                            document.getElementById('{0}_addExternalLinkButton').style.display='none';
                            document.getElementById('{0}_editExternalLinkButton').style.display='none';

                            document.getElementById('{0}_addLinkContainer').style.display='none';
                        }}
                        ", this.ClientID);
			Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID + "_editLink", editLink, true);

			try
			{
				_xml = new XmlDocument();
				_xml.LoadXml(_data.Value.ToString());

			}
			catch
			{
				_xml = createBaseXmlDocument();
			}

			_listboxLinks = new ListBox();
			_listboxLinks.ID = base.ID + "links";
			_listboxLinks.Attributes.Add("onClick", ClientID + "_closeLinkContainer();");
			_listboxLinks.Width = 400;
			_listboxLinks.Height = 140;
			foreach (XmlNode node in _xml.DocumentElement.ChildNodes)
			{
				string text = node.Attributes["title"].Value.ToString();
				string type;
				if (node.Attributes["type"].Value.ToString().Equals("internal"))
					type = "i";
				else if (node.Attributes["type"].Value.ToString().Equals("media"))
					type = "m";
				else
					type = "e";

				string value = type
					+ (node.Attributes["newwindow"].Value.ToString().Equals("1") ? "n" : "o")
					+ node.Attributes["link"].Value.ToString();
				_listboxLinks.Items.Add(new ListItem(text, value));
			}

			_buttonUp = new Button();
			_buttonUp.ID = base.ID + "btnUp";
			_buttonUp.Text = umbraco.ui.GetText("relatedlinks", "modeUp");
			_buttonUp.Width = 80;
			_buttonUp.Click += new EventHandler(this.buttonUp_Click);


			_buttonDown = new Button();
			_buttonDown.ID = base.ID + "btnDown";
			_buttonDown.Attributes.Add("style", "margin-top: 5px;");
			_buttonDown.Text = umbraco.ui.GetText("relatedlinks", "modeDown");
			_buttonDown.Width = 80;
			_buttonDown.Click += new EventHandler(this.buttonDown_Click);

			_buttonEdit = new Button();
			_buttonEdit.ID = base.ID + "btnEdit";
			_buttonEdit.Text = GetTextWithDefault("relatedlinks", "editLink", "Edit link");
			_buttonEdit.Width = 80;
			_buttonEdit.OnClientClick = this.ClientID + "_relatedLinkEdit(); return false;";

			_buttonDelete = new Button();
			_buttonDelete.ID = base.ID + "btnDel";
			_buttonDelete.Text = umbraco.ui.GetText("relatedlinks", "removeLink");
			_buttonDelete.Width = 80;
			_buttonDelete.Click += new EventHandler(this.buttonDel_Click);

			_textboxLinkTitle = new TextBox();
			_textboxLinkTitle.Width = 400;
			_textboxLinkTitle.ID = base.ID + "linktitle";

			_checkNewWindow = new CheckBox();
			_checkNewWindow.ID = base.ID + "checkNewWindow";
			_checkNewWindow.Checked = false;
			_checkNewWindow.Text = umbraco.ui.GetText("relatedlinks", "newWindow");

			_textBoxExtUrl = new TextBox();
			_textBoxExtUrl.Width = 400;
			_textBoxExtUrl.ID = base.ID + "exturl";

			_buttonAddExtUrl = new Button();
			_buttonAddExtUrl.ID = base.ID + "btnAddExtUrl";
			_buttonAddExtUrl.Text = umbraco.ui.GetText("relatedlinks", "addlink");
			_buttonAddExtUrl.Width = 80;
			_buttonAddExtUrl.Click += new EventHandler(this.buttonAddExt_Click);

			_buttonAddIntUrlCP = new Button();
			_buttonAddIntUrlCP.ID = base.ID + "btnAddIntUrl";
			_buttonAddIntUrlCP.Text = umbraco.ui.GetText("relatedlinks", "addlink");
			_buttonAddIntUrlCP.Width = 80;
			_buttonAddIntUrlCP.Click += new EventHandler(this.buttonAddIntCP_Click);

			_buttonAddMediaCP = new Button();
			_buttonAddMediaCP.ID = base.ID + "btnAddMedia";
			_buttonAddMediaCP.Text = umbraco.ui.GetText("relatedlinks", "addlink");
			_buttonAddMediaCP.Width = 80;
			_buttonAddMediaCP.Click += new EventHandler(this.buttonAddMediaCP_Click);

			_buttonEditExtUrl = new Button();
			_buttonEditExtUrl.ID = base.ID + "btnEditExtUrl";
			_buttonEditExtUrl.Text = GetTextWithDefault("relatedlinks", "savelink", "Save");
			_buttonEditExtUrl.Width = 80;
			_buttonEditExtUrl.Click += new EventHandler(this.buttonEditExt_Click);

			_buttonEditIntUrlCP = new Button();
			_buttonEditIntUrlCP.ID = base.ID + "btnEditIntUrl";
			_buttonEditIntUrlCP.Text = GetTextWithDefault("relatedlinks", "savelink", "Save");
			_buttonEditIntUrlCP.Width = 80;
			_buttonEditIntUrlCP.Click += new EventHandler(this.buttonEditIntCP_Click);

			_buttonEditMediaCP = new Button();
			_buttonEditMediaCP.ID = base.ID + "btnEditMedia";
			_buttonEditMediaCP.Text = GetTextWithDefault("relatedlinks", "savelink", "Save");
			_buttonEditMediaCP.Width = 80;
			_buttonEditMediaCP.Click += new EventHandler(this.buttonEditMediaCP_Click);

			_pagePickerExtractor = new PagePickerwMediaDataExtractor();
			_pagePicker = new pagePicker(_pagePickerExtractor);
			_pagePicker.ID = base.ID + "pagePicker";

			_mediaChooserExtractor = new PagePickerwMediaDataExtractor();
			_mediaChooser = new mediaChooser(_mediaChooserExtractor);
			_mediaChooser.ID = base.ID + "mediaChooser";

			ContentTemplateContainer.Controls.Add(new LiteralControl("<div class=\"relatedlinksdatatype\" style=\"text-align: left;  padding: 5px;\"><table><tr><td rowspan=\"2\">"));
			ContentTemplateContainer.Controls.Add(_listboxLinks);
			ContentTemplateContainer.Controls.Add(new LiteralControl("</td><td style=\"vertical-align: top\">"));
			ContentTemplateContainer.Controls.Add(_buttonUp);
			ContentTemplateContainer.Controls.Add(new LiteralControl("<br />"));
			ContentTemplateContainer.Controls.Add(_buttonDown);
			ContentTemplateContainer.Controls.Add(new LiteralControl("</td></tr><tr><td style=\"vertical-align: bottom\">"));
			ContentTemplateContainer.Controls.Add(_buttonEdit);
			ContentTemplateContainer.Controls.Add(new LiteralControl("<br />"));
			ContentTemplateContainer.Controls.Add(_buttonDelete);
			ContentTemplateContainer.Controls.Add(new LiteralControl("<br />"));
			ContentTemplateContainer.Controls.Add(new LiteralControl("</td></tr></table>"));

			// Add related links container: for each link type, we add a link that displays the 
			// corresponding div and hides the others
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("<a href=\"javascript:;\" onClick=\"{0}_addInternalLink();\"><strong>{1}</strong></a>", ClientID, umbraco.ui.GetText("relatedlinks", "addInternal"))));
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format(" | <a href=\"javascript:;\" onClick=\"{0}_addMediaLink();\"><strong>{1}</strong></a>", ClientID, GetTextWithDefault("relatedlinks", "addMedia", "Add media link"))));
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format(" | <a href=\"javascript:;\" onClick=\"{0}_addExternalLink();\"><strong>{1}</strong></a>", ClientID, umbraco.ui.GetText("relatedlinks", "addExternal"))));

			// All urls containers
			// Main contaniner: {ClienID}_addLinkContainer
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("<div id=\"{0}_addLinkContainer\" style=\"display: none; padding: 4px; border: 1px solid #ccc; margin-top: 5px;margin-right:10px;\">", ClientID)));
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("<a href=\"javascript:;\" onClick=\"{0}_closeLinkContainer();\" style=\"border: none;\"><img src=\"{1}/images/close.png\" style=\"float: right\" /></a>", ClientID, IOHelper.ResolveUrl(SystemDirectories.Umbraco))));
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("{0}:<br />", umbraco.ui.GetText("relatedlinks", "caption"))));
			ContentTemplateContainer.Controls.Add(_textboxLinkTitle);
			ContentTemplateContainer.Controls.Add(new LiteralControl("<br />"));

			// External Url container: {CliendID}_addExternalLinkPanel
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("<div id=\"{0}_addExternalLinkPanel\" style=\"display: none; margin: 3px 0\">", ClientID)));
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("{0}:<br />", umbraco.ui.GetText("relatedlinks", "linkurl"))));
			ContentTemplateContainer.Controls.Add(_textBoxExtUrl);
			ContentTemplateContainer.Controls.Add(new LiteralControl("</div>"));

			// Internal Link container: {CliendID}_addInternalLinkPanel
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("<div id=\"{0}_addInternalLinkPanel\" style=\"display: none; margin: 3px 0\">", ClientID)));
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("{0}:<br />", umbraco.ui.GetText("relatedlinks", "internalPage"))));
			ContentTemplateContainer.Controls.Add(_pagePicker);
			ContentTemplateContainer.Controls.Add(new LiteralControl("</div>"));

			// Media Link container: {CliendID}_addMediaLinkPanel
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("<div id=\"{0}_addMediaLinkPanel\" style=\"display: none; margin: 3px 0\">", ClientID)));
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("{0}:<br />", GetTextWithDefault("relatedlinks", "media", "Media"))));
			ContentTemplateContainer.Controls.Add(_mediaChooser);
			ContentTemplateContainer.Controls.Add(new LiteralControl("</div>"));

			// Checkbox for new window container
			ContentTemplateContainer.Controls.Add(new LiteralControl("<div style=\"margin: 5px 0\">"));
			ContentTemplateContainer.Controls.Add(_checkNewWindow);
			ContentTemplateContainer.Controls.Add(new LiteralControl("</div>"));

			// Internal Link button: {ClientID}_addInternalLinkButton
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("<div id=\"{0}_addInternalLinkButton\" style=\"display: none;\">", ClientID)));
			ContentTemplateContainer.Controls.Add(_buttonAddIntUrlCP);
			ContentTemplateContainer.Controls.Add(new LiteralControl("</div>"));
			// Internal Link button: {ClientID}_editInternalLinkButton
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("<div id=\"{0}_editInternalLinkButton\" style=\"display: none;\">", ClientID)));
			ContentTemplateContainer.Controls.Add(_buttonEditIntUrlCP);
			ContentTemplateContainer.Controls.Add(new LiteralControl("</div>"));

			// Media Link button: {ClientID}_addMediaLinkButton
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("<div id=\"{0}_addMediaLinkButton\" style=\"display: none;\">", ClientID)));
			ContentTemplateContainer.Controls.Add(_buttonAddMediaCP);
			ContentTemplateContainer.Controls.Add(new LiteralControl("</div>"));
			// Media Link button: {ClientID}_editMediaLinkButton
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("<div id=\"{0}_editMediaLinkButton\" style=\"display: none;\">", ClientID)));
			ContentTemplateContainer.Controls.Add(_buttonEditMediaCP);
			ContentTemplateContainer.Controls.Add(new LiteralControl("</div>"));

			// External Url button: {ClientID}_addExternalLinkButton
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("<div id=\"{0}_addExternalLinkButton\" style=\"display: none;\">", ClientID)));
			ContentTemplateContainer.Controls.Add(_buttonAddExtUrl);
			ContentTemplateContainer.Controls.Add(new LiteralControl("</div>"));
			// External Url button: {ClientID}_editExternalLinkButton
			ContentTemplateContainer.Controls.Add(new LiteralControl(string.Format("<div id=\"{0}_editExternalLinkButton\" style=\"display: none;\">", ClientID)));
			ContentTemplateContainer.Controls.Add(_buttonEditExtUrl);
			ContentTemplateContainer.Controls.Add(new LiteralControl("</div>"));

			ContentTemplateContainer.Controls.Add(new LiteralControl("</div>"));

			ContentTemplateContainer.Controls.Add(new LiteralControl("</div>"));

			resetInputMedia();
		}

		/// <summary>
		/// Gets the text with default.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="key">The key.</param>
		/// <param name="defaultText">The default text.</param>
		/// <returns></returns>
		private string GetTextWithDefault(string area, string key, string defaultText)
		{
			string text = umbraco.ui.GetText(area, key);

			if (text.StartsWith("["))
				return defaultText;
			else
				return text;
		}

		/// <summary>
		/// Creates the base XML document.
		/// </summary>
		/// <returns></returns>
		private XmlDocument createBaseXmlDocument()
		{
			XmlDocument doc = new XmlDocument();
			XmlNode root = doc.CreateElement("links");
			doc.AppendChild(root);
			return doc;
		}

		/// <summary>
		/// Handles the Click event of the buttonUp control.
		/// </summary>
		/// <param name="o">The source of the event.</param>
		/// <param name="ea">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonUp_Click(Object o, EventArgs ea)
		{
			int index = _listboxLinks.SelectedIndex;
			if (index > 0) //not the first item
			{
				ListItem temp = _listboxLinks.SelectedItem;
				_listboxLinks.Items.RemoveAt(index);
				_listboxLinks.Items.Insert(index - 1, temp);
				_listboxLinks.SelectedIndex = index - 1;
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonDown control.
		/// </summary>
		/// <param name="o">The source of the event.</param>
		/// <param name="ea">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonDown_Click(Object o, EventArgs ea)
		{
			int index = _listboxLinks.SelectedIndex;
			if (index > -1 && index < _listboxLinks.Items.Count - 1) //not the last item
			{
				ListItem temp = _listboxLinks.SelectedItem;
				_listboxLinks.Items.RemoveAt(index);
				_listboxLinks.Items.Insert(index + 1, temp);
				_listboxLinks.SelectedIndex = index + 1;
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonDel control.
		/// </summary>
		/// <param name="o">The source of the event.</param>
		/// <param name="ea">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonDel_Click(Object o, EventArgs ea)
		{
			int index = _listboxLinks.SelectedIndex;
			if (index > -1)
			{
				_listboxLinks.Items.RemoveAt(index);
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonAddExt control.
		/// </summary>
		/// <param name="o">The source of the event.</param>
		/// <param name="ea">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonAddExt_Click(Object o, EventArgs ea)
		{
			string url = _textBoxExtUrl.Text.Trim();
			if (url.Length > 0 && _textboxLinkTitle.Text.Length > 0)
			{
				// use default HTTP protocol if no protocol was specified and it is not an absolute url
				if (!url.Contains("://") && !url.StartsWith("/"))
				{
					url = string.Concat(Settings.HTTP, url);
				}

				string value = "e" + (_checkNewWindow.Checked ? "n" : "o") + url;
				_listboxLinks.Items.Add(new ListItem(_textboxLinkTitle.Text, value));
				resetInputMedia();
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonAddIntCP control.
		/// </summary>
		/// <param name="o">The source of the event.</param>
		/// <param name="ea">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonAddIntCP_Click(Object o, EventArgs ea)
		{
			_pagePicker.Save();
			if (!string.IsNullOrEmpty(_textboxLinkTitle.Text)
				&& _pagePickerExtractor.Value != null
				&& _pagePickerExtractor.Value.ToString() != "")
			{
				string value = "i" + (_checkNewWindow.Checked ? "n" : "o") + _pagePickerExtractor.Value.ToString();
				_listboxLinks.Items.Add(new ListItem(_textboxLinkTitle.Text, value));
				resetInputMedia();
				ScriptManager.RegisterClientScriptBlock(_pagePicker, _pagePicker.GetType(), "clearPagePicker", "mc_" + _pagePicker.ClientID + ".ClearSelection();", true);
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonAddMediaCP control.
		/// </summary>
		/// <param name="o">The source of the event.</param>
		/// <param name="ea">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonAddMediaCP_Click(Object o, EventArgs ea)
		{
			_mediaChooser.Save();
			if (!string.IsNullOrEmpty(_textboxLinkTitle.Text)
				&& _mediaChooserExtractor.Value != null
				&& _mediaChooserExtractor.Value.ToString() != "")
			{
				string value = "m" + (_checkNewWindow.Checked ? "n" : "o") + _mediaChooserExtractor.Value.ToString();
				_listboxLinks.Items.Add(new ListItem(_textboxLinkTitle.Text, value));
				resetInputMedia();
				ScriptManager.RegisterClientScriptBlock(_mediaChooser, _mediaChooser.GetType(), "clearMediaChooser", "mc_" + _mediaChooser.ClientID + ".ClearSelection();", true);
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonEditExt control.
		/// </summary>
		/// <param name="o">The source of the event.</param>
		/// <param name="ea">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonEditExt_Click(Object o, EventArgs ea)
		{
			string url = _textBoxExtUrl.Text.Trim();
			if (url.Length > 0 && _textboxLinkTitle.Text.Length > 0)
			{
				// use default HTTP protocol if no protocol was specified and it is not an absolute url
				if (!url.Contains("://") && !url.StartsWith("/"))
				{
					url = string.Concat(Settings.HTTP, url);
				}

				string value = "e" + (_checkNewWindow.Checked ? "n" : "o") + url;
				_listboxLinks.SelectedItem.Text = _textboxLinkTitle.Text;
				_listboxLinks.SelectedItem.Value = value;
				resetInputMedia();
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonEditIntCP control.
		/// </summary>
		/// <param name="o">The source of the event.</param>
		/// <param name="ea">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonEditIntCP_Click(Object o, EventArgs ea)
		{
			_pagePicker.Save();
			if (!string.IsNullOrEmpty(_textboxLinkTitle.Text)
				&& _pagePickerExtractor.Value != null
				&& _pagePickerExtractor.Value.ToString() != "")
			{
				string value = "i" + (_checkNewWindow.Checked ? "n" : "o") + _pagePickerExtractor.Value.ToString();
				_listboxLinks.SelectedItem.Text = _textboxLinkTitle.Text;
				_listboxLinks.SelectedItem.Value = value;
				resetInputMedia();
				ScriptManager.RegisterClientScriptBlock(_pagePicker, _pagePicker.GetType(), "clearPagePicker", _pagePicker.ClientID + "_clear();", true);
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonEditMediaCP control.
		/// </summary>
		/// <param name="o">The source of the event.</param>
		/// <param name="ea">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonEditMediaCP_Click(object o, EventArgs ea)
		{
			_mediaChooser.Save();
			if (!string.IsNullOrEmpty(_textboxLinkTitle.Text)
				&& _mediaChooserExtractor.Value != null
				&& _mediaChooserExtractor.Value.ToString() != "")
			{
				string value = "m" + (_checkNewWindow.Checked ? "n" : "o") + _mediaChooserExtractor.Value.ToString();
				_listboxLinks.SelectedItem.Text = _textboxLinkTitle.Text;
				_listboxLinks.SelectedItem.Value = value;
				resetInputMedia();
				ScriptManager.RegisterClientScriptBlock(_mediaChooser, _mediaChooser.GetType(), "clearMediaChooser", _mediaChooser.ClientID + "_clear();", true);
			}
		}

		/// <summary>
		/// Resets the input media.
		/// </summary>
		private void resetInputMedia()
		{
			_textBoxExtUrl.Text = Settings.HTTP;
			_textboxLinkTitle.Text = string.Empty;
			_pagePickerExtractor.Value = string.Empty;
			_mediaChooserExtractor.Value = string.Empty;
		}
	}
}
