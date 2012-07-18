using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using uComponents.Core.Shared.Extensions;
using umbraco;
using umbraco.BusinessLogic;

namespace uComponents.Core.XsltExtensions
{
	/// <summary>
	/// A helper class for getting CMS user data.
	/// </summary>
	public partial class Cms
	{
		/// <summary>
		/// Gets all users.
		/// </summary>
		/// <returns></returns>
		public static XPathNodeIterator GetAllUsers()
		{
			var users = User.getAll();
			return ConvertUsersToXml(users);
		}

		/// <summary>
		/// Gets all users by email.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <returns></returns>
		public static XPathNodeIterator GetAllUsersByEmail(string email)
		{
			var users = User.getAllByEmail(email);
			return ConvertUsersToXml(users);
		}

		/// <summary>
		/// Gets the name of all users by login.
		/// </summary>
		/// <param name="loginName">Name of the login.</param>
		/// <returns></returns>
		public static XPathNodeIterator GetAllUsersByLoginName(string loginName)
		{
			return GetAllUsersByLoginName(loginName, false);
		}

		/// <summary>
		/// Gets the name of all users by login.
		/// </summary>
		/// <param name="loginName">Name of the login.</param>
		/// <param name="partialMatch">if set to <c>true</c> [partial match].</param>
		/// <returns></returns>
		public static XPathNodeIterator GetAllUsersByLoginName(string loginName, bool partialMatch)
		{
			var users = User.GetAllByLoginName(loginName, partialMatch).ToArray();
			return ConvertUsersToXml(users);
		}

		/// <summary>
		/// Gets the user.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static XPathNodeIterator GetUser(int userId)
		{
			try
			{
				var user = User.GetUser(userId);
				var xd = new XmlDocument();

				AppendUser(xd, user);

				return xd.CreateNavigator().Select("/");

			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Gets the user email.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static string GetUserEmail(int userId)
		{
			var user = umbraco.BusinessLogic.User.GetUser(userId);
			return user != null ? user.Email : string.Empty;
		}

		/// <summary>
		/// Gets the user language.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static string GetUserLanguage(int userId)
		{
			var user = umbraco.BusinessLogic.User.GetUser(userId);
			return user != null ? user.Language : string.Empty;
		}

		/// <summary>
		/// Gets the name of the user login.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static string GetUserLoginName(int userId)
		{
			var user = umbraco.BusinessLogic.User.GetUser(userId);
			return user != null ? user.LoginName : string.Empty;
		}

		/// <summary>
		/// Gets the name of the user.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static string GetUserName(int userId)
		{
			var user = umbraco.BusinessLogic.User.GetUser(userId);
			return user != null ? user.Name : string.Empty;
		}

		/// <summary>
		/// Gets the user type alias.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static string GetUserTypeAlias(int userId)
		{
			var user = umbraco.BusinessLogic.User.GetUser(userId);
			return user != null && user.UserType != null ? user.UserType.Alias : string.Empty;
		}

		/// <summary>
		/// Converts the users to XML.
		/// </summary>
		/// <param name="users">The users.</param>
		/// <returns></returns>
		internal static XPathNodeIterator ConvertUsersToXml(User[] users)
		{
			try
			{
				var xd = new XmlDocument();
				xd.LoadXml("<Users/>");

				foreach (var user in users)
				{
					AppendUser(xd, user);
				}

				return xd.CreateNavigator().Select("/");

			}
			catch (Exception ex)
			{
				return ex.ToXPathNodeIterator();
			}
		}

		/// <summary>
		/// Appends the user to the XML document.
		/// </summary>
		/// <param name="xd">The XML document.</param>
		/// <param name="user">The user.</param>
		internal static void AppendUser(XmlDocument xd, User user)
		{
			var node = xmlHelper.addTextNode(xd, "User", string.Empty);
			node.Attributes.Append(xmlHelper.addAttribute(xd, "id", user.Id.ToString()));
			node.Attributes.Append(xmlHelper.addAttribute(xd, "name", user.Name));
			node.Attributes.Append(xmlHelper.addAttribute(xd, "loginName", user.LoginName));
			node.Attributes.Append(xmlHelper.addAttribute(xd, "email", user.Email));
			node.Attributes.Append(xmlHelper.addAttribute(xd, "language", user.Language));
			node.Attributes.Append(xmlHelper.addAttribute(xd, "userTypeAlias", user.UserType.Alias));
			node.Attributes.Append(xmlHelper.addAttribute(xd, "isAdmin", user.IsAdmin().ToString()));
			node.Attributes.Append(xmlHelper.addAttribute(xd, "startNodeId", user.StartNodeId.ToString()));
			node.Attributes.Append(xmlHelper.addAttribute(xd, "startMediaId", user.StartMediaId.ToString()));

			// add the user node to the XmlDocument.
			if (xd.DocumentElement != null)
			{
				xd.DocumentElement.AppendChild(node);
			}
			else
			{
				xd.AppendChild(node);
			}
		}
	}
}