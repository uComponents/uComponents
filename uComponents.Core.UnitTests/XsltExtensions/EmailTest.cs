using uComponents.XsltExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace uComponents.Core.UnitTests.XsltExtensions
{
	[TestClass]
	public class EmailTest
	{
		/// <summary>
		/// Determines whether [is valid email test].
		/// http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
		/// </summary>
		[TestMethod]
		public void IsValidEmailTest()
		{
			var emails = new Dictionary<string, bool>()
			{
				{@"NotAnEmail", false},
				{@"@NotAnEmail", false},
				{@"""test\\blah""@example.com", true},
				{@"""test\blah""@example.com", false},
				{"\"test\\\rblah\"@example.com", true},
				{"\"test\rblah\"@example.com", false},
				{@"""test\""blah""@example.com", true},
				{@"""test""blah""@example.com", false},
				{@"customer/department@example.com", true},
				{@"$A12345@example.com", true},
				{@"!def!xyz%abc@example.com", true},
				{@"_Yosemite.Sam@example.com", true},
				{@"~@example.com", true},
				{@".wooly@example.com", false},
				{@"wo..oly@example.com", false},
				{@"pootietang.@example.com", false},
				{@".@example.com", false},
				{@"""Austin@Powers""@example.com", true},
				{@"Ima.Fool@example.com", true},
				{@"""Ima.Fool""@example.com", true},
				{@"""Ima Fool""@example.com", true},
				{@"Ima Fool@example.com", false}
			};

			foreach (var email in emails)
			{
				Assert.AreEqual(email.Value, Email.IsValidEmail(email.Key), string.Format("Problem with '{0}'. Expected {1} but was not that.", email.Key, email.Value));
			}
		}
	}
}