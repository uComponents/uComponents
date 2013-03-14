namespace uComponents.DataTypes.DataTypeGrid.Factories
{
    using System.Web.UI;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.cms.businesslogic.member;
    using umbraco.editorControls.memberpicker;
    using umbraco.interfaces;

    /// <summary>
    /// Factory for the <see cref="MemberPickerDataType"/>
    /// </summary>
    [DataTypeFactory(Priority = -1)]
    public class MemberPickerDataTypeFactory : BaseDataTypeFactory<MemberPickerDataType>
    {
        /// <summary>
        /// Method for customizing the way the <see cref="IDataType">datatype</see> value is displayed in the grid.
        /// </summary>
        /// <remarks>Called when the grid displays the cell value for the specified <see cref="IDataType">datatype</see>.</remarks>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The display value.</returns>
        public override string GetDisplayValue(MemberPickerDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            int id;
            int.TryParse(value, out id);

            if (id > 0)
            {
                var m = new Member(id);

                // Return member name
                return string.Format("<a href='editMember.aspx?id={0}' title='Edit content'>{1}</a>", m.Id, m.Text);
            }

            return value;
        }

        /// <summary>
        /// Method for getting the backing object for the specified <see cref="IDataType">datatype</see>.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType">datatype</see> instance.</param>
        /// <returns>The backing object.</returns>
        /// <remarks>Called when the method <see cref="GridCell.GetObject{T}()" /> method is called on a <see cref="GridCell" />.</remarks>
        public override object GetObject(MemberPickerDataType dataType)
        {
            var value = dataType.Data.Value != null ? dataType.Data.Value.ToString() : string.Empty;

            int id;
            int.TryParse(value, out id);

            if (id > 0)
            {
                return new Member(id);
            }

            return dataType.Data.Value;
        }

        /// <summary>
        /// Method for performing special actions <b>before</b> creating the <see cref="IDataType"/> editor.
        /// </summary>
        /// <remarks>Called <b>before</b> the grid creates the editor controls for the specified <see cref="IDataType"/>.</remarks>
        /// <param name="dataType">The <see cref="IDataType"/> instance.</param>
        /// <param name="container">The editor control container.</param>
        public override void Initialize(MemberPickerDataType dataType, Control container)
        {
            if (dataType.Data.Value == null)
            {
                dataType.Data.Value = string.Empty;
            }
        }
    }
}