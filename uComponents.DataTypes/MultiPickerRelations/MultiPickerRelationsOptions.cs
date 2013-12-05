

namespace uComponents.DataTypes.MultiPickerRelations
{
    /// <summary>
    /// Data Class, used to store the configuration options for the MultiPickerRelationsPreValueEditor
    /// </summary>
    internal class MultiPickerRelationsOptions
    {
        /// <summary>
        /// Alias of the multiNodePickerProperty to get a csv value of IDs from //TODO: a known format for xml fragments would be good too
        /// </summary>
        public string PropertyAlias { get; set; }

        /// <summary>
        /// The Id of the RelationType to use 
        /// </summary>
        public int RelationTypeId { get; set; }

        /// <summary>
        /// only relevant with parent-child 
        /// </summary>
        public bool ReverseIndexing { get; set; }

		/// <summary>
		/// if true then the property is hidden
		/// </summary>
		public bool HideDataEditor { get; set; }

        /// <summary>
        /// Initializes an instance of MultiPickerRelationsOptions
        /// </summary>
        public MultiPickerRelationsOptions()
        {
            // Default values
            this.PropertyAlias = string.Empty;
            this.RelationTypeId = -1;
            this.ReverseIndexing = false;
			this.HideDataEditor = false;
        }
    }
}
