using System.Text.RegularExpressions;

namespace IrcBot
{
	public class MessageFormatting
	{
		public static char ColorEscapeChar = (char) 3;
		public static char BoldEscapeChar = (char) 2;
		public static char UnderlineEscapeChar = (char) 31;
		public static char ItalicEscapeChar = (char) 22;

		public static string MakeBold(string message)
		{
			return string.Format("{0}{1}{2}", BoldEscapeChar, message, BoldEscapeChar);
		}

		public static string MakeHotPink(string message)
		{
			return MakeColoredMessage(Color.HotPink, message);
		}

		public static string MakeRed(string message)
		{
			return MakeColoredMessage(Color.Red, message);
		}

		public static string MakeNavyBlue(string message)
		{
			return MakeColoredMessage(Color.NavyBlue,message);
		}

		public static string MakeLightGray(string message)
		{
			return MakeColoredMessage(Color.LightGray, message);
		}

		public static string MakeColoredMessage(Color color, string message)
		{
			return string.Format("{0}{1}{2}{3}{4}",ColorEscapeChar,(int)color,message,ColorEscapeChar,(int)Color.None);
		}

		public static string ClearFormatting(string message)
		{
			return Regex.Replace(message, ColorEscapeChar + "[0-1]*[0-9]", string.Empty);
		}
	}

	public enum Color
	{
		None,
		Black,
		NavyBlue,
		Green,
		Red,
		Brown,
		Purple,
		Olive,
		Yellow,
		LimeGreen,
		Teal,
		AquaLight,
		RoyalBlue,
		HotPink,
		DarkGray,
		LightGray,
		White
	}
}
