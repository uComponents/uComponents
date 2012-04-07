using System;
using System.Drawing;
using System.Xml.Linq;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uComponents.DataTypes.TextImage
{
	/// <summary>
	///   DataType from AbstractDataEditor
	/// </summary>
	public class TextImageDataType : AbstractDataEditor
	{
		#region Fields

		private readonly TextImageDataEditor _textImageDataEditor = new TextImageDataEditor();
		private IData _data;
		private TextImagePrevalueEditor _textImagePrevalueEditor;

		#endregion

		#region Constructors

		/// <summary>
		///   Initializes a new instance of the <see cref = "TextImageDataType" /> class.
		/// </summary>
		public TextImageDataType()
		{
			RenderControl = _textImageDataEditor;
			_textImageDataEditor.PreRender += TextImageDataEditor_PreRender;
			_textImageDataEditor.Delete += TextDataEditor_Delete;
			_textImageDataEditor.TextChanged += TextImageDataEditor_TextChanged;
			DataEditorControl.OnSave += TextImageDataEditor_OnSave;
		}

		#endregion

		#region Event Methods

		/// <summary>
		///   Handles the TextChanged event of the TextImageBox control.
		/// </summary>
		/// <param name = "sender">The source of the event.</param>
		/// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
		private void TextImageDataEditor_TextChanged(object sender, EventArgs e)
		{
			SaveImage();
		}

		/// <summary>
		///   Handles the PreRender event of the TextImageDataEditor control.
		/// </summary>
		/// <param name = "sender">The source of the event.</param>
		/// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
		private void TextImageDataEditor_PreRender(object sender, EventArgs e)
		{
			if (Data.Value == null || string.IsNullOrEmpty(Data.Value.ToString())) return;

			var xDocument = XDocument.Parse(Data.Value.ToString());
			_textImageDataEditor.XmlValue = xDocument;
		}

		/// <summary>
		///   Handles the Save event of the TextImageDataEditor control
		/// </summary>
		/// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
		private void TextImageDataEditor_OnSave(EventArgs e)
		{
			// Generates the text image file from the parameters
			SaveImage();

			// Save the XML to the data
			var xmlValue = _textImageDataEditor.XmlValue;
			Data.Value = xmlValue == null ? null : xmlValue.ToString();
		}

		/// <summary>
		/// Handles the Delete event of the TextImageDataEditor control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void TextDataEditor_Delete(object sender, EventArgs e)
		{
			_textImageDataEditor.ImageUrl = string.Empty;
			Data.Value = null;
		}

		#endregion

		#region Methods

		/// <summary>
		///   Gets the parameters.
		/// </summary>
		/// <returns></returns>
		private TextImageParameters GetParameters()
		{
			TextImageParameters parameters;
			try
			{
				var prevalueEditor = ((TextImagePrevalueEditor)PrevalueEditor);

				parameters = new TextImageParameters(_textImageDataEditor.Text,
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

		/// <summary>
		///   Saves the image.
		/// </summary>
		private void SaveImage()
		{
			// Generates the text image file from the parameters
			var parameters = GetParameters();
			_textImageDataEditor.SaveImage(parameters);
		}

		#endregion

		#region Overrides of AbstractDataEditor

		/// <summary>
		///   Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public override Guid Id
		{
			get { return new Guid(DataTypeConstants.TextImageId); }
		}

		/// <summary>
		///   Gets the name of the data type.
		/// </summary>
		/// <value>The name of the data type.</value>
		public override string DataTypeName
		{
			get { return "uComponents: Text Image"; }
		}

		/// <summary>
		///   Gets the prevalue editor.
		/// </summary>
		/// <value>The prevalue editor.</value>
		public override IDataPrevalue PrevalueEditor
		{
			get { return _textImagePrevalueEditor ?? (_textImagePrevalueEditor = new TextImagePrevalueEditor(this)); }
		}

		/// <summary>
		///   Gets the data.
		/// </summary>
		/// <value>The data.</value>
		public override IData Data
		{
			get { return _data ?? (_data = new Shared.Data.XmlData(this)); }
		}

		#endregion
	}
}