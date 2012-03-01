using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

// TODO: [LK] Replace with uComponents.Core.Shared.Data.XmlData

namespace uComponents.Core.DataTypes.RelatedLinksWithMedia
{
	/// <summary>
	/// Data for the RelatedLinksWithMedia data-type.
	/// </summary>
	public class RelatedLinkswMediaData : umbraco.cms.businesslogic.datatype.DefaultData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RelatedLinkswMediaData"/> class.
		/// </summary>
		/// <param name="DataType">Type of the data.</param>
		public RelatedLinkswMediaData(umbraco.cms.businesslogic.datatype.BaseDataType DataType)
			: base(DataType)
		{ }

		/// <summary>
		/// Converts the data to Xml.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public override XmlNode ToXMl(XmlDocument data)
		{
			if (this.Value != null && !string.IsNullOrEmpty(this.Value.ToString()))
			{
				var xd = new XmlDocument();
				xd.LoadXml(this.Value.ToString());
				return data.ImportNode(xd.DocumentElement, true);
			}
			else
			{
				return base.ToXMl(data);
			}
		}
	}
}
