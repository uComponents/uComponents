// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 23.05.2012 - Created [Ove Andersen]
// 09.02.2013 - Rewritten [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.ServiceLocators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using uComponents.DataTypes.DataTypeGrid.Factories;
    using uComponents.DataTypes.DataTypeGrid.Interfaces;
    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.interfaces;

    internal class DataTypeFactoryServiceLocator : IDataTypeFactoryServiceLocator
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        private static readonly DataTypeFactoryServiceLocator DefaultInstance = new DataTypeFactoryServiceLocator();

        /// <summary>
        /// All registered datatype factories
        /// </summary>
        private IList<Type> dataTypeFactories;

        /// <summary>
        /// Prevents a default instance of the <see cref="DataTypeFactoryServiceLocator"/> class from being created.
        /// </summary>
        private DataTypeFactoryServiceLocator()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static DataTypeFactoryServiceLocator Instance
        {
            get
            {
                return DefaultInstance;
            }
        }

        /// <summary>
        /// Method for customizing the way the <see cref="IDataType" /> value is displayed in the grid.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <returns>The display value.</returns>
        /// <remarks>Called when the grid displays the cell value for the specified <see cref="IDataType" />.</remarks>
        public string GetDisplayValue(IDataType dataType)
        {
            var f = this.GetDataTypeFactory(dataType);

            var v = f.GetType().GetMethod("GetDisplayValue").Invoke(f, new object[] { dataType });

            return v.ToString();
        }

        /// <summary>
        /// Method for getting the backing object for the specified <see cref="IDataType" />.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <returns>The backing object.</returns>
        /// <remarks>Called when the method <see cref="GridCell.GetObject{T}()" /> method is called on a <see cref="GridCell" />.</remarks>
        public object GetObject(IDataType dataType)
        {
            var f = this.GetDataTypeFactory(dataType);

            var v = f.GetType().GetMethod("GetObject").Invoke(f, new object[] { dataType });

            return v;
        }

        /// <summary>
        /// Method for getting the backing object for the specified <see cref="IDataType" />.
        /// </summary>
        /// <typeparam name="TBackingObject">The backing type for the specified <see cref="IDataType" />.</typeparam>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <returns>The backing object.</returns>
        /// <remarks>Called when the method <see cref="GridCell.GetObject{T}()" /> method is called on a <see cref="GridCell" />.</remarks>
        public TBackingObject GetObject<TBackingObject>(IDataType dataType)
        {
            return default(TBackingObject);
        }

        /// <summary>
        /// Method for performing special actions <b>before</b> creating the <see cref="IDataType"/> editor.
        /// </summary>
        /// <remarks>Called <b>before</b> the grid creates the editor controls for the specified <see cref="IDataType"/>.</remarks>
        /// <param name="dataType">The <see cref="IDataType"/> instance.</param>
        /// <param name="container">The editor control container.</param>
        public void Initialize(IDataType dataType, Control container)
        {
            var f = this.GetDataTypeFactory(dataType);

            f.GetType().GetMethod("Initialize").Invoke(f, new object[] { dataType, container });
        }

        /// <summary>
        /// Method for performing special actions <b>after</b> the <see cref="IDataType" /> <see cref="IDataEditor">editor</see> has been loaded.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <param name="container">The editor control container.</param>
        /// <remarks>Called <b>after</b> the grid creates the editor controls for the specified <see cref="IDataType" />.</remarks>
        public void Configure(IDataType dataType, Control container)
        {
            var f = this.GetDataTypeFactory(dataType);

            f.GetType().GetMethod("Configure").Invoke(f, new object[] { dataType, container });
        }

        /// <summary>
        /// Method for executing special actions before saving the editor value to the database.
        /// </summary>
        /// <param name="dataType">The <see cref="IDataType" /> instance.</param>
        /// <remarks>Called when the grid is saved for the specified <see cref="IDataType" />.</remarks>
        public void Save(IDataType dataType)
        {
            var f = this.GetDataTypeFactory(dataType);

            f.GetType().GetMethod("Save").Invoke(f, new object[] { dataType });
        }

        /// <summary>
        /// Gets the factory for the specified <typeparamref name="TDataType">datatype</typeparamref>.
        /// </summary>
        /// <typeparam name="TDataType">The <typeparamref name="TDataType">datatype</typeparamref>.</typeparam>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>A datatype factory.</returns>
        private object GetDataTypeFactory<TDataType>(TDataType dataType) where TDataType : IDataType
        {
            // Get factories implementing IDataTypeFactory and the specified datatype
            var factories =
                this.GetDataTypeFactories()
                    .Where(
                        f =>
                        f.GetInterfaces()
                         .Any(
                             i =>
                             i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataTypeFactory<>)
                             && i.GetGenericArguments().Any(t => t == dataType.GetType())));

            if (factories.Any())
            {
                // Get the factory with the highest priority
                var f = factories.OrderBy(x => this.GetDataTypeFactoryAttribute(x).Priority).FirstOrDefault();

                if (f != null)
                {
                    return Activator.CreateInstance(f);
                }
            }

            return new DefaultDataTypeFactory();
        }

        /// <summary>
        /// Gets the <see cref="DataTypeFactoryAttribute">datatype factory attribute</see> for the specified type.
        /// </summary>
        /// <param name="factory">The <see cref="IDataTypeFactory{T}">datatype factory type</see>.</param>
        /// <returns>The <see cref="DataTypeFactoryAttribute">datatype factory attribute</see>.</returns>
        private DataTypeFactoryAttribute GetDataTypeFactoryAttribute(Type factory)
        {
            var attribute =
                factory.GetCustomAttributes(typeof(DataTypeFactoryAttribute), false)
                    .FirstOrDefault() as DataTypeFactoryAttribute;
            
            return attribute ?? new DataTypeFactoryAttribute();
        }

        /// <summary>
        /// Gets all datatype factories.
        /// </summary>
        /// <returns>A list of datatype factories.</returns>
        private IEnumerable<Type> GetDataTypeFactories()
        {
            if (this.dataTypeFactories == null || !this.dataTypeFactories.Any())
            {
                var factories = new List<Type>();

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in assemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        foreach (var i in type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataTypeFactory<>)))
                        {
                            factories.Add(type);
                        }
                    }
                }

                this.dataTypeFactories = factories;
            }

            return this.dataTypeFactories;
        }
    }
}