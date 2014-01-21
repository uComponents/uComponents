using System;
using System.Collections;
using System.Linq;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.propertytype;
using umbraco.interfaces;

namespace uComponents.DataTypes.Shared.macroRenderings
{
	/// <summary>
	/// A legacy copy of Umbraco's macro-rendering control for listing property-types.
	/// </summary>
	/// <remarks>
	/// This class was copied from the Umbraco v6.2.0 code-base.
	/// It has been marked as `internal sealed` so that it is only used for the uComponents data-types.
	/// </remarks>
	[Obsolete("This control has been deprecated from the Umbraco core v7+. It has only been referenced here for backwards-compatibility.")]
	internal sealed class propertyTypePicker : ListBox, IMacroGuiRendering
	{
		string _value = "";
		bool _multiple = false;

		public bool ShowCaption
		{
			get { return true; }
		}

		public bool Multiple
		{
			set { _multiple = value; }
			get { return _multiple; }
		}

		public string Value
		{
			get
			{
				var retVal = "";
				foreach (ListItem i in base.Items)
				{
					if (i.Selected)
						retVal += i.Value + ",";
				}

				if (retVal != "")
					retVal = retVal.Substring(0, retVal.Length - 1);

				return retVal;
			}
			set
			{
				_value = value;
			}
		}

		public propertyTypePicker()
		{
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.CssClass = "guiInputTextStandard";

			// Check for multiple choises
			if (_multiple)
			{
				this.SelectionMode = System.Web.UI.WebControls.ListSelectionMode.Multiple;
				this.Rows = 5;
				this.Multiple = true;
			}
			else
			{
				this.Rows = 1;
				this.Items.Add(new ListItem("", ""));
				this.SelectionMode = ListSelectionMode.Single;
			}

			var ht = new Hashtable();
			foreach (var pt in PropertyType.GetAll().OrderBy(x => x.Name))
			{
				if (!ht.ContainsKey(pt.Alias))
				{
					var li = new ListItem(pt.Alias);
					if (((string)(", " + _value + ",")).IndexOf(", " + pt.Alias + ",") > -1)
						li.Selected = true;
					ht.Add(pt.Alias, "");

					this.Items.Add(li);
				}
			}
		}
	}
}