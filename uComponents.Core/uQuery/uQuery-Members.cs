using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using umbraco.cms.businesslogic.member;

namespace uComponents.Core
{
	public static partial class uQuery
	{
		/// <summary>
		/// Get a collection of members from an XPath expression (note XML source is currently a flat structure)
		/// </summary>
		/// <param name="xPath">XPath expression</param>
		/// <returns>collection or empty collection</returns>
		public static List<Member> GetMembersByXPath(string xPath)
		{
			var members = new List<Member>();
			var xmlDocument = uQuery.GetPublishedXml(UmbracoObjectType.Member);
			var xPathNavigator = xmlDocument.CreateNavigator();
			var xPathNodeIterator = xPathNavigator.Select(xPath);

			while (xPathNodeIterator.MoveNext())
			{
				var member = uQuery.GetMember(xPathNodeIterator.Current.Evaluate("string(@id)").ToString());
				if (member != null)
				{
					members.Add(member);
				}
			}

			return members;
		}

		/// <summary>
		/// Get collection of member objects from the supplied CSV of IDs
		/// </summary>
		/// <param name="csv">string csv of IDs</param>
		/// <returns>collection or emtpy collection</returns>
		public static List<Member> GetMembersByCsv(string csv)
		{
			var members = new List<Member>();
			var ids = uQuery.GetCsvIds(csv);

			if (ids != null)
			{
				foreach (string id in ids)
				{
					var member = uQuery.GetMember(id);
					if (member != null)
					{
						members.Add(member);
					}
				}
			}

			return members;
		}

		/// <summary>
		/// Gets the members by XML.
		/// </summary>
		/// <param name="xml">The XML.</param>
		/// <returns></returns>
		public static List<Member> GetMembersByXml(string xml)
		{
			var members = new List<Member>();
			var ids = uQuery.GetXmlIds(xml);

			foreach (int id in ids)
			{
				var member = uQuery.GetMember(id);
				if (member != null)
				{
					members.Add(member);
				}
			}

			return members;
		}

		/// <summary>
		/// Get Members by member type alias
		/// </summary>
		/// <param name="memberTypeAlias">The member type alias</param>
		/// <returns>list of members, or empty list</returns>
		public static List<Member> GetMembersByType(string memberTypeAlias)
		{
			// Both XML schema versions have this attribute
			return uQuery.GetMembersByXPath(string.Concat("descendant::*[@nodeTypeAlias='", memberTypeAlias, "']"));
		}

		// public static Member GetCurrentMember() { }

		/// <summary>
		/// Get member from an ID
		/// </summary>
		/// <param name="memberId">string representation of an umbraco.cms.businesslogic.member.Member object Id</param>
		/// <returns>member or null</returns>
		public static Member GetMember(string memberId)
		{
			int id;
			Member member = null;

			// suppress error if member with supplied id doesn't exist
			if (int.TryParse(memberId, out id))
			{
				member = uQuery.GetMember(id);
			}

			return member;
		}

		/// <summary>
		/// Get member from an ID
		/// </summary>
		/// <param name="id">ID of member item to get</param>
		/// <returns>member or null</returns>
		public static Member GetMember(int id)
		{
			// suppress error if member with supplied id doesn't exist
			try
			{
				return new Member(id);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Extension method on Member collection to return key value pairs of: member.Id / member.loginName
		/// </summary>
		/// <param name="members">generic list of Member objects</param>
		/// <returns>a collection of memberIDs and their login names</returns>
		public static Dictionary<int, string> ToNameIds(this List<Member> members)
		{
			var dictionary = new Dictionary<int, string>();

			foreach (var member in members)
			{
				dictionary.Add(member.Id, member.LoginName);
			}

			return dictionary;
		}
	}
}