using System;
using umbraco.interfaces;

namespace uComponents.DataTypes.TextImage
{
    internal class DataExtractor : IData
    {
        public DataExtractor()
        {
        }

        public DataExtractor(object o)
        {
            Value = o;
        }

        #region IData Members

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void MakeNew(int propertyId)
        {
            throw new NotImplementedException();
        }

        public int PropertyId
        {
            set { throw new NotImplementedException(); }
        }

        public System.Xml.XmlNode ToXMl(System.Xml.XmlDocument d)
        {
            throw new NotImplementedException();
        }

        public object Value { get; set; }

        #endregion
    }
}