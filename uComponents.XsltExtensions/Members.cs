using System;
using System.Web.Security;
using System.Xml;
using System.Xml.XPath;
// using uComponents.Core.Shared.Extensions;
using umbraco;
using umbraco.cms.businesslogic.member;

namespace uComponents.XsltExtensions
{
	/// <summary>
	/// The Members class exposes XSLT extensions to offer extended XML/XSLT functionality.
	/// </summary>
	public class Members
	{
		// TODO: [LK] GetMemberGroups

		/// <summary>
		/// Gets the members by CSV.
		/// </summary>
		/// <param name="csv">The CSV of member IDs.</param>
		/// <returns>
		/// Returns an XPathNodeIterator of the member nodes from the CSV list.
		/// </returns>
		public static XPathNodeIterator GetMembersByCsv(string csv)
		{
			var xd = new XmlDocument();
			xd.LoadXml("<root/>");

			var memberNodes = uQuery.GetMembersByCsv(csv);

			foreach (var memberNode in memberNodes)
			{
				var xn = memberNode.ToXml(xd, false);
				xd.DocumentElement.AppendChild(xn);
			}

			return xd.CreateNavigator().Select("descendant::*[@id]");
		}

		/// <summary>
		/// Gets the usernames of all the members in the specified group.
		/// </summary>
		/// <param name="groupName">Name of the member group.</param>
		/// <returns>Returns a list of all the member names.</returns>
		public static XPathNodeIterator GetMembersByGroupName(string groupName)
		{
			try
			{
				var xd = new XmlDocument();
				xd.LoadXml("<members/>");

				if (!string.IsNullOrEmpty(groupName))
				{
					foreach (var memberName in Roles.GetUsersInRole(groupName))
					{
						var memberNode = xmlHelper.addTextNode(xd, "member", memberName);
						xd.DocumentElement.AppendChild(memberNode);
					}
				}

				return xd.CreateNavigator().Select("/members");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Gets the members by node type alias.
		/// </summary>
		/// <param name="nodeTypeAlias">The node type alias.</param>
		/// <returns>
		/// Returns an XPathNodeIterator of the member nodes from specified node type alias.
		/// </returns>
		public static XPathNodeIterator GetMembersByType(string nodeTypeAlias)
		{
			var xpath = string.Concat("descendant::*[@nodeTypeAlias='", nodeTypeAlias, "']");
			return GetMembersByXPath(xpath);
		}

		/// <summary>
		/// Gets the members by an XPath expression.
		/// </summary>
		/// <param name="xpath">The XPath expression.</param>
		/// <returns>
		/// Returns an XPathNodeIterator of the member nodes from specified XPath expression.
		/// </returns>
		public static XPathNodeIterator GetMembersByXPath(string xpath)
		{
			var xml = uQuery.GetPublishedXml(uQuery.UmbracoObjectType.Member);
			if (xml != null)
			{
				var nav = xml.CreateNavigator();
				return nav.Select(xpath);
			}

			return null;
		}

		/// <summary>
		/// Gets the published Xml.
		/// </summary>
		/// <returns>
		/// Returns an XPathNodeIterator of all the member nodes.
		/// </returns>
		public static XPathNodeIterator GetPublishedXml()
		{
			return GetMembersByXPath("/");
		}

		/// <summary>
		/// Gets the unique id of a member node.
		/// </summary>
		/// <param name="memberId">The member id.</param>
		/// <returns>Returns the unique id of a member node.</returns>
		public static string GetUniqueId(int memberId)
		{
			return Cms.GetUniqueId(memberId);
		}

		/// <summary>
		/// Checks if a member is member of a specific group.
		/// </summary>
		/// <param name="groupName">The name of the group</param>
		/// <param name="memberId">Member's Id</param>
		/// <returns>
		/// Returns true if member is a member of the group.
		/// </returns>
		public static bool IsMemberOfAGroup(string groupName, int memberId)
		{
			// check that the member Id has a value.
			if (memberId > 0)
			{
				// get the member
				var member = new Member(memberId);

				// check that the member exists
				if (member != null)
				{
					// return if user is in the group/role.
					return Roles.IsUserInRole(member.LoginName, groupName);
				}
			}

			return false;
		}

		/// <summary>
		/// Gets a list of group names from the specific member.
		/// </summary>
		/// <param name="memberId">Member's Id</param>
		/// <returns>
		/// A node-set of all the member-groups from the specific member
		/// </returns>
		public static XPathNodeIterator GetGroupsByMemberId(int memberId)
		{
			try
			{
				var xd = new XmlDocument();
				xd.LoadXml("<memberGroups/>");

				var member = new Member(memberId);

				foreach (var group in Roles.GetRolesForUser(member.LoginName))
				{
					var memberGroupNode = umbraco.xmlHelper.addTextNode(xd, "memberGroup", group);
					xd.DocumentElement.AppendChild(memberGroupNode);
				}

				return xd.CreateNavigator().Select("/memberGroups");
			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Performs a basic/quick search against the Examine/Lucene index for Members.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <returns>
		/// Returns an XML structure of the Members search results.
		/// </returns>
		public static XPathNodeIterator Search(string searchText)
		{
			return Search(searchText, true);
		}

		/// <summary>
		/// Performs a basic/quick search against the Examine/Lucene index for Members.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="useWildcards">if set to <c>true</c> [use wildcards].</param>
		/// <returns>
		/// Returns an XML structure of the Members search results.
		/// </returns>
		public static XPathNodeIterator Search(string searchText, bool useWildcards)
		{
			return uComponents.XsltExtensions.Search.QuickSearch(searchText, useWildcards, "InternalMemberSearcher");
		}
	}
}