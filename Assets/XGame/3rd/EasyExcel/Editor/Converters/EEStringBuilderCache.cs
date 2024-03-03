using System.Text;
using System.Collections.Generic;

namespace EasyExcel
{
	public static class EEStringBuilderCache
	{
		private static readonly List<StringBuilder> stringBuilders = new List<StringBuilder>();

		public static void Reset()
		{
			stringBuilders.Clear();
		}
		
		public static StringBuilder Borrow()
		{
			if (stringBuilders.Count == 0)
				return new StringBuilder(1024);
			var first = stringBuilders[0];
			stringBuilders.RemoveAt(0);
			//EELog.LogError("get " + stringBuilders.Count.ToString());
			return first;
		}

		public static string Return(StringBuilder strb)
		{
			if (strb == null) return null;
			if (!stringBuilders.Contains(strb))
				stringBuilders.Add(strb);
			var str = strb.ToString();
			strb.Clear();
			//EELog.LogError(stringBuilders.Count.ToString());
			return str;
		}
	}
}