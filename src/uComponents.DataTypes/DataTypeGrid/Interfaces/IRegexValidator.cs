namespace uComponents.DataTypes.DataTypeGrid.Interfaces
{
    /// <summary>
    /// Interface for a regular expression validator
    /// </summary>
    public interface IRegexValidator
    {
        /// <summary>
        /// Validates the regular expression.
        /// </summary>
        /// <param name="regex">The regular expression.</param>
        /// <returns><c>true</c> if the expression is valid, <c>false</c> otherwise</returns>
        bool Validate(string regex);
    }
}