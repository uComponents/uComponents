using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using umbraco.interfaces;

namespace uComponents.DataTypes.StyledTextBox
{
    /// <summary>
    /// The DataEditor for the StyledTextBox stat-type.
    /// </summary>
    public class StyledTextBoxDataEditor : TextBox, IDataEditor
    {
        /// <summary>
        /// 
        /// </summary>
        private IData _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="StyledTextBoxDataEditor"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="configuration">The configuration.</param>
        public StyledTextBoxDataEditor(IData data, string configuration)
        {
            _data = data;

            if (string.IsNullOrEmpty(configuration) == false)
            {
                var settings = configuration.Split(Constants.Common.COMMA);

                if (settings.Length == 2)
                {
                    this.Attributes.Add("style", string.Join(new string(Constants.Common.COMMA, 1), settings.Skip(1)));
                    this.Width = int.Parse(settings[0]);
                }
            }
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
        public void Save()
        {
            _data.Value = this.Text;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (_data != null && _data.Value != null)
                Text = _data.Value.ToString();
        }
    }
}
