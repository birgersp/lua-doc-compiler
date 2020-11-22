using System.Text;
using System.Text.RegularExpressions;

namespace LDC
{
	public class HtmlBuilder
	{
		public int Level = 0;
		readonly StringBuilder data = new StringBuilder();
		bool addedLiteral = false;

		private static string ToLiteral(string text)
		{
			return text
				.Replace("<", "&lt;")
				.Replace(">", "&gt;");
		}

		public void AddLiteral(string text)
		{
			var translated = ToLiteral(text);
			data.Append(translated);
			addedLiteral = true;
		}

		public void Add(params string[] entries)
		{
			var index = 0;
			while (index < entries.Length - 1)
			{
				Open(entries[index++]);
			}
			AddLiteral(entries[index--]);
			while (index >= 0)
			{
				var tag = Regex.Replace(entries[index--], " .*", "");
				Close(tag);
			}
		}

		public void Open(params string[] tags)
		{
			foreach (var tag in tags)
			{
				if (data.Length > 0)
				{
					data.Append("\n");
				}
				for (int i = 0; i < Level; i++)
				{
					data.Append("\t");
				}
				data.Append($"<{tag}>");
				Level++;
			}
		}

		public void Close(params string[] tags)
		{
			foreach (var tag in tags)
			{
				Level--;
				if (!addedLiteral)
				{
					data.Append("\n");
					for (int i = 0; i < Level; i++)
					{
						data.Append("\t");
					}
				}
				data.Append($"</{tag}>");
				addedLiteral = false;
			}
		}

		public override string ToString()
		{
			return data.ToString();
		}
	}
}
