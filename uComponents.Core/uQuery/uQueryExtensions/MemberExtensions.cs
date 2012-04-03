using System;
using umbraco.cms.businesslogic.member;

namespace uComponents.Core.uQueryExtensions
{
	/// <summary>
	/// uQuery Member extensions.
	/// </summary>
	public static class MemberExtensions
	{
		/// <summary>
		/// Adds a member the group (by group name).
		/// </summary>
		/// <param name="member">The member.</param>
		/// <param name="groupName">Name of the group.</param>
		public static void AddGroup(this Member member, string groupName)
		{
			if (string.IsNullOrWhiteSpace(groupName))
				return;

			var group = MemberGroup.GetByName(groupName);

			if (group != null)
			{
#pragma warning disable 0618
				member.AddGroup(group.Id);
#pragma warning restore 0618
			}
		}

		/// <summary>
		/// Removes a member the group (by group name).
		/// </summary>
		/// <param name="member">The member.</param>
		/// <param name="groupName">Name of the group.</param>
		public static void RemoveGroup(this Member member, string groupName)
		{
			if (string.IsNullOrWhiteSpace(groupName))
				return;

			var group = MemberGroup.GetByName(groupName);

			if (group != null)
			{
#pragma warning disable 0618
				member.RemoveGroup(group.Id);
#pragma warning restore 0618
			}
		}
	}
}