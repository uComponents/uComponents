namespace uComponents.DataTypes.DataTypeGrid.Validators
{
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using uComponents.DataTypes.DataTypeGrid.Model;
    using uComponents.DataTypes.DataTypeGrid.ServiceLocators;

    /// <summary>
    /// Client-side required field validator
    /// </summary>
    public class ClientSideRequiredFieldValidator : CustomValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSideRequiredFieldValidator" /> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="enabled">If the validator is e enabled.</param>
        public ClientSideRequiredFieldValidator(string prefix, StoredValue cell, bool enabled = true)
        {
            this.ID = prefix + "_" + cell.Alias + "_required";
            this.Enabled = enabled;
            this.CssClass = "validator";
            this.ClientValidationFunction = "RequiredFieldValidate";
            this.Display = ValidatorDisplay.Dynamic;
            this.ErrorMessage = cell.Name + " is mandatory";

            // Set control to validate
            this.Attributes["data-controltovalidate"] = DataTypeFactoryServiceLocator.Instance.GetControlToValidate(cell.Value, cell.Value.DataEditor.Editor).ClientID;

            var validationAttribute = (ValidationPropertyAttribute)TypeDescriptor.GetAttributes(cell.Value.DataEditor.Editor)[typeof(ValidationPropertyAttribute)];

            if (validationAttribute != null)
            {
                this.Attributes["data-validationproperty"] = validationAttribute.Name;
            }
        }

        /// <summary>
        /// Checks the properties of the control for valid values.
        /// </summary>
        /// <returns>true if the control properties are valid; otherwise, false.</returns>
        protected override bool ControlPropertiesValid()
        {
            return true;
        }
    }
}