using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.interfaces;

namespace uComponents.DataTypes.Notes
{
    /// <summary>
    /// The DataEditor for the Notes data-type.
    /// </summary>
    public class NotesDataEditor : Panel, IDataEditor
    {
        /// <summary>
        /// The NotesOptions for the data-type.
        /// </summary>
        private NotesOptions m_options;

        /// <summary>
        /// Gets a value indicating whether to show a label or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if want to show label; otherwise, <c>false</c>.
        /// </value>
        public bool ShowLabel
        {
            get { return m_options.ShowLabel; }
        }

        /// <summary>
        /// Gets a value indicating whether treat as rich text editor.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if treat as rich text editor; otherwise, <c>false</c>.
        /// </value>
        public bool TreatAsRichTextEditor
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the editor.
        /// </summary>
        public Control Editor
        {
            get { return this; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotesDataEditor"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public NotesDataEditor(NotesOptions options)
        {
            m_options = options;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        { }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.Controls.Add(new Literal { Text = m_options.Value });
            this.Style.Add("padding-left", "10px");
            this.Style.Add("padding-right", "10px");
            base.OnInit(e);
        }
    }
}
