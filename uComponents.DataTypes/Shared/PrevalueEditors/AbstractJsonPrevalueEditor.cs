using System;
using System.Web.Script.Serialization;
using umbraco.cms.businesslogic.datatype;
using Umbraco.Core.Logging;

namespace uComponents.DataTypes.Shared.PrevalueEditors
{
	/// <summary>
	/// Abstract class for the PreValue Editor.
	/// Specifically designed to serialize/deserialize the options as JSON.
	/// </summary>
	public abstract class AbstractJsonPrevalueEditor : AbstractPrevalueEditor
	{
		/// <summary>
		/// The underlying base data-type.
		/// </summary>
		protected readonly BaseDataType DataType;

		/// <summary>
		/// An object to temporarily lock writing to the database.
		/// </summary>
		private static readonly object Locker = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractJsonPrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		protected AbstractJsonPrevalueEditor(BaseDataType dataType)
		{
			this.DataType = dataType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractJsonPrevalueEditor"/> class.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		/// <param name="dbType">Type of the database field.</param>
		protected AbstractJsonPrevalueEditor(BaseDataType dataType, DBTypes dbType)
			: base()
		{
			this.DataType = dataType;
			this.DataType.DBType = dbType;
		}

		/// <summary>
		/// Gets the PreValue options for the data-type.
		/// </summary>
		/// <typeparam name="T">The type of the resulting object.</typeparam>
		/// <returns>
		/// Returns the options for the PreValue Editor
		/// </returns>
		public T GetPreValueOptions<T>()
		{
			var prevalues = PreValues.GetPreValues(this.DataType.DataTypeDefinitionId);
			if (prevalues.Count > 0)
			{
				var prevalue = (PreValue)prevalues[0];
				if (!string.IsNullOrEmpty(prevalue.Value))
				{
					try
					{
						// deserialize the options
						var serializer = new JavaScriptSerializer();

						// return the options
						return serializer.Deserialize<T>(prevalue.Value);
					}
					catch (Exception ex)
					{
						LogHelper.Error<T>(string.Concat("uComponents: AbstractJsonPrevalueEditor.GetPreValueOptions<T> Exception.", ex.Message), ex);
					}
				}
			}

			// if all else fails, return default options
			return default(T);
		}

		/// <summary>
		/// Saves the data-type PreValue options.
		/// </summary>
		/// <param name="options">The options.</param>
		public void SaveAsJson(object options)
		{
			// serialize the options into JSON
			var serializer = new JavaScriptSerializer();
			var json = serializer.Serialize(options);

			lock (Locker)
			{
				var prevalues = PreValues.GetPreValues(this.DataType.DataTypeDefinitionId);
				if (prevalues.Count > 0)
				{
					var prevalue = (PreValue)prevalues[0];

					// update
					prevalue.Value = json;
					prevalue.Save();
				}
				else
				{
					// insert
					PreValue.MakeNew(this.DataType.DataTypeDefinitionId, json);
				}
			}
		}
	}
}