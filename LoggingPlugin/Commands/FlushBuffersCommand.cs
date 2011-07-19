using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.Logging.Commands
{
	public class FlushBuffersCommand : BaseBotCommand
	{
		protected readonly LoggingPlugin LoggingPlugin;

		public FlushBuffersCommand(LoggingPlugin plugin)
		{
			LoggingPlugin = plugin;
			FirstMatchingWord = new List<string> {"flush"};
			SecondMatchingWord = new List<string> {"buffers"};
		}

		public override void Execute(IrcEventArgs args)
		{
			LoggingPlugin.FlushStreamWriters();
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"flush buffers - will flush all of the current logs to file"};
		}
	}
}
