using System;

namespace Xsolla
{
	public class StringHelper
	{
		public StringHelper ()
		{
		}

		public static String DateFormat(DateTime pDate)
		{
			return string.Format("{0:dd.MM.yyyy}", pDate);
		}

		public static String PrepareFormatString(String pInnerString)
		{
			String res = pInnerString;
			int indx = 0;
			while (res.Contains("{{"))
			{
				String replacedPart = res.Substring(res.IndexOf("{{", 0) + 1, res.IndexOf("}}", 0) - res.IndexOf("{{", 0));
				res = res.Replace(replacedPart, indx.ToString());  
				indx ++;
			}
			return res;
		}
	}
}

