using Microsoft.VisualStudio.TestTools.UnitTesting;
using uComponents.Core.XsltExtensions;

namespace uComponents.Core.UnitTests.XsltExtensions
{
	[TestClass]
	public class NodesTest
	{
		[TestMethod]
		public void GetNodeIdByPathLevelTest()
		{
			var path = "-1,1111,2222,3333,4444";
			var items = path.Split(',');

			for (int i = 0; i < items.Length; i++)
			{
				var nodeId = Nodes.GetNodeIdByPathLevel(path, i);

				Assert.AreEqual(items[i], nodeId);

				i++;
			}
		}
	}
}