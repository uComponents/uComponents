namespace uComponents.DataTypes.DataTypeGrid.Functions
{
    using System;
    using System.Text.RegularExpressions;

    using uComponents.DataTypes.DataTypeGrid.Interfaces;

    /// <summary>
    /// A regular expression validator
    /// </summary>
    public class RegexValidator : IRegexValidator
    {
        /// <summary>
        /// Validates the regex expression.
        /// </summary>
        /// <param name="regex">The regex expression.</param>
        /// <returns><c>true</c> if the expression is valid, <c>false</c> otherwise</returns>
        public bool Validate(string regex)
        {
            var isValid = true;

            if ((regex != null) && (regex.Trim().Length > 0))
            {
                try
                {
                    Regex.Match(string.Empty, regex);
                }
                catch (ArgumentException)
                {
                    // BAD PATTERN: Syntax error
                    isValid = false;
                }
            }
            else
            {
                // BAD PATTERN: Pattern is null or blank
                isValid = false;
            }

            return isValid;
        }
    }
}