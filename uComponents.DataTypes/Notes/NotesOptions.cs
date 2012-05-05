using umbraco.cms.businesslogic.datatype;

namespace uComponents.DataTypes.Notes
{
    /// <summary>
    /// The options for the Notes data-type.
    /// </summary>
    public class NotesOptions : AbstractOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether [show label].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show label]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowLabel { get; set; } 

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotesOptions"/> class.
        /// </summary>
        public NotesOptions()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotesOptions"/> class.
        /// </summary>
        /// <param name="loadDefaults">if set to <c>true</c> [load defaults].</param>
        public NotesOptions(bool loadDefaults)
			: base(loadDefaults)
		{ }
    }
}
