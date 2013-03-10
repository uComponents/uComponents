using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using umbraco;
using umbraco.NodeFactory;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.member;
using umbraco;
using System.Xml.Linq;

namespace uComponents.DataTypes.XPathSortableList
{
    /// <summary>
    /// Strongly typed obj that's returned from uQuery .GetProperty<XPathCheckBoxList>("alias");
    /// </summary>
    public class XPathSortableList : uQuery.IGetProperty
    {

        public IEnumerable<Node> SelectedNodes { get; private set; }

        public IEnumerable<Media> SelectedMedia { get; private set; }

        public IEnumerable<Member> SelectedMembers { get; private set; }

        void uQuery.IGetProperty.LoadPropertyValue(string value)
        {
            this.SelectedNodes = new Node[] { };
            this.SelectedMedia = new Media[] { };
            this.SelectedMembers = new Member[] { };


            if (!string.IsNullOrWhiteSpace(value))
            {
                /*
                    <XPathSortableList Type="c66ba18e-eaf3-4cff-8a22-41b16d66a972">
                        <Item Value="1" />
                        <Item Value="9" />
                    </XPathSortableList>
                */

                XDocument valueXDocument = XDocument.Load(value);
                IEnumerable<int> values = valueXDocument.Descendants("Item").Attributes("Value").Select(x => int.Parse(x.Value));

                Guid typeGuid;
                if (Guid.TryParse(valueXDocument.Root.Attribute("Type").Value, out typeGuid))
                {
                    switch(uQuery.GetUmbracoObjectType(typeGuid))
                    {
                        case uQuery.UmbracoObjectType.Document:
                            this.SelectedNodes = values.Select(x => new Node(x));
                            break;

                        case uQuery.UmbracoObjectType.Media:
                            this.SelectedMedia = values.Select(x => new Media(x));
                            break;

                        case uQuery.UmbracoObjectType.Member:
                            this.SelectedMembers = values.Select(x => new Member(x));
                            break;
                    }
                }
            }
        }


        //int[] uQuery.IPickerRelations.GetIds(string value)
        //{
        //    /*
        //        <XPathSortableList Type="c66ba18e-eaf3-4cff-8a22-41b16d66a972">
        //            <Item Value="1" />
        //            <Item Value="9" />
        //        </XPathSortableList>
        //    */

        //    XDocument valueXDocument = XDocument.Load(value);
            
        //    return valueXDocument.Descendants("Item").Attributes("Value").Select(x => int.Parse(x.Value));
        //}
    }
}
