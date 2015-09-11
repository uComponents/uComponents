
namespace uComponents.DataTypes.RelationLinks
{
    using System.ComponentModel;
    using System.Web.UI.WebControls;
    using umbraco.editorControls;    

    /// <summary>
    /// Options class for the RelatedLinks data-type
    /// </summary>
    public class RelationLinksOptions : AbstractOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationLinksOptions"/> class.
        /// </summary>
        public RelationLinksOptions() : base(true) { }

        /// <summary>
        /// Gets or sets the Id of the RelationType to use 
        /// </summary>
        [DefaultValue(-1)]
        public int RelationTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the direction the list should be rendered in
        /// </summary>
        [DefaultValue(RepeatDirection.Vertical)]
        public RepeatDirection RepeatDirection { get; set; }

        /// <summary>
        /// Gets or sets the macro by alias to use as the rendering mechanism for each item (if empty, then default icon + node name will be used)
        /// </summary>
        [DefaultValue("")]
        public string MacroAlias { get; set; }
    }
}
