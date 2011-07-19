using IrcBot;
using Ircbot.Plugins.Trivia;
using Ircbot.Plugins.Trivia.Commands;
using NUnit.Framework;

namespace UnitTests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void Test1()
		{
			LevenshteinDistance.Compute("test", "test2");
		}

		[Test]
		public void Test2()
		{
			
		}
	}
}
