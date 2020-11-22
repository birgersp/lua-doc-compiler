using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LDC
{
	[TestClass]
	public class HtmlBuilderTest
	{
		[TestMethod]
		public void Add()
		{
			var builder = new HtmlBuilder();
			builder.Add("a", "b");
			Assert.AreEqual("<a>b</a>", builder.ToString());
			builder.Add("c");
			Assert.AreEqual("<a>b</a>c", builder.ToString());
		}

		[TestMethod]
		public void TestIndentAndNewline()
		{
			var builder = new HtmlBuilder();
			builder.Open("a", "b", "c");
			Assert.AreEqual("<a>\n\t<b>\n\t\t<c>", builder.ToString());
			builder.Add("d");
			builder.Close("c");
			builder.Add("c", "d");
			builder.Close("b", "a");
			Assert.AreEqual("<a>\n\t<b>\n\t\t<c>d</c>\n\t\t<c>d</c>\n\t</b>\n</a>", builder.ToString());
		}
	}
}
