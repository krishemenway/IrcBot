using System;
using Ircbot.Plugins.Trivia;

namespace TestConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			WriteThisShit("tesT","test2");
			WriteThisShit("tesT", "test3");
			WriteThisShit("tesT", "test4");
			WriteThisShit("tesT", "boobs boobs boobs");
			WriteThisShit("tesT", "boobs boobs");
			WriteThisShit("Sacramento", "boobs");
			WriteThisShit("El Cid", "Los Angelos");
		}

		public static void WriteThisShit(string s, string t)
		{
			s = s.ToLower();
			t = t.ToLower();
			double calc = Convert.ToDouble(s.Length + t.Length) / 2;
			double calc2 = Convert.ToDouble(LevenshteinDistance.Compute(s, t)) / calc;

			Console.WriteLine(string.Format("{0} == {1} : {2}", s, t, calc2));
		}
	}
}
