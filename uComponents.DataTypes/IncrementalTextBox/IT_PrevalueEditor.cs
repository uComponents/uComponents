using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.DataTypes.Shared.Extensions;
using uComponents.DataTypes.Shared.PrevalueEditors;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;

namespace uComponents.DataTypes.IncrementalTextBox
{
	/// <summary>
	/// The PrevalueEditor for the IncrementalTextBox data-type.
	/// </summary>
	public class IT_PrevalueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractPrevalueEditor
	{
		/// <summary>
		/// The underlying data-type.
		/// </summary>
		private readonly umbraco.cms.businesslogic.datatype.BaseDataType m_DataType;

		/// <summary>
		/// 
		/// </summary>
		private static readonly object m_Locker = new object();

		/// <summary>
		/// Gets the min value.
		/// </summary>
		/// <value>The min value.</value>
		public int MinValue
		{
			get
			{
				var vals = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);
				if (vals.Count > 0)
				{
					// return ((PreValue)vals[0]).Value == string.Empty ? "0" : ((PreValue)vals[0]).Value;
					int result;
					if (int.TryParse(((PreValue)vals[0]).Value, out result))
					{
						return result;
					}
				}

				// return default
				return 0;
			}
		}

		/// <summary>
		/// Gets the max value.
		/// </summary>
		/// <value>The max value.</value>
		public int MaxValue
		{
			get
			{
				var vals = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);
				if (vals.Count > 1)
				{
					//return int.Parse(((PreValue)vals[1]).Value);
					int result;
					if (int.TryParse(((PreValue)vals[1]).Value, out result))
					{
						return result;
					}
				}

				return 100;
			}
		}

		/// <summary>
		/// Gets the increment value.
		/// </summary>
		/// <value>The increment value.</value>
		public int IncrementValue
		{
			get
			{
				var vals = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);
				if (vals.Count > 2)
				{
					// return int.Parse(((PreValue)vals[2]).Value);
					int result;
					if (int.TryParse(((PreValue)vals[2]).Value, out result))
					{
						return result;
					}
				}

				return 1;
			}
		}

		/// <summary>
		/// The RegularExpressionValidator for the minimum value.
		/// </summary>
		protected RegularExpressionValidator minValNumberRex;

		/// <summary>
		/// The RegularExpressionValidator for the maximum value.
		/// </summary>
		protected RegularExpressionValidator maxValNumberRex;

		/// <summary>
		/// The RegularExpressionValidator for the incremental value.
		/// </summary>
		protected RegularExpressionValidator incrementValNumberRex;

		/// <summary>
		/// The RequiredFieldValidator for the minimum value.
		/// </summary>
		protected RequiredFieldValidator minValNumberReq;

		/// <summary>
		/// The RequiredFieldValidator for the maximum value.
		/// </summary>
		protected RequiredFieldValidator maxValNumberReq;

		/// <summary>
		/// The RequiredFieldValidator for the incremental value.
		/// </summary>
		protected RequiredFieldValidator incrementValNumberReq;

		/// <summary>
		/// The TextBox control for the minimum value.
		/// </summary>
		protected TextBox MinValueTextBox;

		/// <summary>
		/// The TextBox control for the maximum value.
		/// </summary>
		protected TextBox MaxValueTextBox;

		/// <summary>
		/// The TextBox control for the incremental value.
		/// </summary>
		protected TextBox IncrementValueTextBox;

		/// <summary>
		/// Initializes a new instance of the <see cref="IT_PrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		public IT_PrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
		{
			this.m_DataType = dataType;
		}

		/// <summary>
		/// Ensures the css to render this control is included.
		/// Binds the saved value to the drop down.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.MinValueTextBox.Text = this.MinValue.ToString();
			this.MaxValueTextBox.Text = this.MaxValue.ToString();
			this.IncrementValueTextBox.Text = this.IncrementValue.ToString();
		}

		/// <summary>
		/// Creates child controls for this control
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.MinValueTextBox = new TextBox() { ID = "MinValue", CssClass = "guiInputText" };
			this.MaxValueTextBox = new TextBox() { ID = "MaxValue", CssClass = "guiInputText" };
			this.IncrementValueTextBox = new TextBox() { ID = "IncrementValue", CssClass = "guiInputText" };

			this.Controls.AddPrevalueControls(this.MinValueTextBox, this.MaxValueTextBox, this.IncrementValueTextBox);

			// regex validators

			this.minValNumberRex = new RegularExpressionValidator()
			{
				ID = "minValNumberReg",
				ValidationExpression = @"\d+",
				ForeColor = Color.Red,
				ErrorMessage = " Numbers only",
				ControlToValidate = "MinValue"
			};

			this.maxValNumberRex = new RegularExpressionValidator()
			{
				ID = "maxValNumberReg",
				ValidationExpression = @"\d+",
				ForeColor = Color.Red,
				ErrorMessage = " Numbers only",
				ControlToValidate = "MaxValue"
			};

			this.incrementValNumberRex = new RegularExpressionValidator()
			{
				ID = "incrementValNumberReg",
				ValidationExpression = @"\d+",
				ForeColor = Color.Red,
				ErrorMessage = " Numbers only",
				ControlToValidate = "IncrementValue"
			};

			this.Controls.AddPrevalueControls(this.minValNumberRex, this.maxValNumberRex, this.incrementValNumberRex);

			// required validators

			this.minValNumberReq = new RequiredFieldValidator()
			{
				ID = "minValNumberReq",
				ForeColor = Color.Red,
				ErrorMessage = " Empty field not allowed",
				ControlToValidate = "MinValue"
			};

			this.maxValNumberReq = new RequiredFieldValidator()
			{
				ID = "maxValNumberReq",
				ForeColor = Color.Red,
				ErrorMessage = " Empty field not allowed",
				ControlToValidate = "MaxValue"
			};

			this.incrementValNumberReq = new RequiredFieldValidator()
			{
				ID = "incrementValNumberReq",
				ForeColor = Color.Red,
				ErrorMessage = " Empty field not allowed",
				ControlToValidate = "IncrementValue"
			};

			this.Controls.AddPrevalueControls(this.minValNumberReq, this.maxValNumberReq, this.incrementValNumberReq);
		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.AddPrevalueRow("Select min value:", this.MinValueTextBox, this.minValNumberRex, this.minValNumberReq);
			writer.AddPrevalueRow("Select max value:", this.MaxValueTextBox, this.maxValNumberRex, this.maxValNumberReq);
			writer.AddPrevalueRow("Increment value by:", this.IncrementValueTextBox, this.incrementValNumberReq, this.incrementValNumberRex);
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public override void Save()
		{
			// it will always be text since people may save a huge amount of selected nodes and serializing to xml could be large.
			this.m_DataType.DBType = umbraco.cms.businesslogic.datatype.DBTypes.Ntext;

			// need to lock this operation since multiple inserts are happening and if 2 threads reach here at the same time, there could be issues.
			lock (m_Locker)
			{
				var vals = PreValues.GetPreValues(this.m_DataType.DataTypeDefinitionId);
				if (vals.Count > 0)
				{
					// update
					((PreValue)vals[0]).Value = this.MinValueTextBox.Text;
					((PreValue)vals[0]).Save();
				}
				else
				{
					// insert
					PreValue.MakeNew(this.m_DataType.DataTypeDefinitionId, this.MinValueTextBox.Text);
				}

				if (vals.Count > 1)
				{
					// update
					((PreValue)vals[1]).Value = this.MaxValueTextBox.Text;
					((PreValue)vals[1]).Save();
				}
				else
				{
					// insert
					PreValue.MakeNew(this.m_DataType.DataTypeDefinitionId, this.MaxValueTextBox.Text);
				}

				if (vals.Count > 2)
				{
					// update
					((PreValue)vals[2]).Value = this.IncrementValueTextBox.Text;
					((PreValue)vals[2]).Save();
				}
				else
				{
					// insert
					PreValue.MakeNew(this.m_DataType.DataTypeDefinitionId, this.IncrementValueTextBox.Text);
				}
			}
		}
	}
}