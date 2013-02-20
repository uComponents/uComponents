namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using uComponents.DataTypes.EnumCheckBoxList;
    using uComponents.DataTypes.EnumDropDownList;

    using umbraco.cms.businesslogic.datatype;
    using umbraco.cms.businesslogic.property;
    using umbraco.cms.businesslogic.propertytype;
    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="EnumCheckBoxListDataType"/> datatype.
    /// </summary>
    public class EnumCheckBoxListDataTypeFactory : BaseDataTypeFactory<EnumCheckBoxListDataType>
    {
        public override string GetDisplayValue(EnumCheckBoxListDataType dataType)
        {
            return base.GetDisplayValue(dataType);
        }

        public override void Initialize(EnumCheckBoxListDataType dataType, System.Web.UI.Control container)
        {
        }
    }
}