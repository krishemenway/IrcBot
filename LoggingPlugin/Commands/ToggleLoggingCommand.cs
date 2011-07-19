using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.Logging.Commands
{
	public class ToggleLoggingCommand : BaseBotCommand
	{
		public LoggingPlugin LoggingPlugin;

		public ToggleLoggingCommand(LoggingPlugin plugin)
		{
			LoggingPlugin = plugin;
			FirstMatchingWord = new List<string> { "logging" };
		}

		public override void Execute(IrcEventArgs args)
		{
			if (args.Data.MessageArray.Length < 2)
				throw new Exception("Must specify what you want to be done to the logging plugin");

			if (args.Data.MessageArray.Length > 2)
				throw new Exception("Far too many parameters, far too ambiguous for me to guess.");

			if (!string.Equals(args.Data.MessageArray[1], "on", StringComparison.CurrentCultureIgnoreCase)
				&& !string.Equals(args.Data.MessageArray[1], "off", StringComparison.CurrentCultureIgnoreCase))
				throw new Exception("Must specify the desired state of the logging plugin");

			if (string.Equals(args.Data.MessageArray[1], "on", StringComparison.CurrentCultureIgnoreCase))
			{
				LoggingPlugin.TurnOnLogging();
				LoggingPlugin.SendMessage("Logging enabled.",args.Data.Nick);
			}
			else if (string.Equals(args.Data.MessageArray[1], "off", StringComparison.CurrentCultureIgnoreCase))
			{
				LoggingPlugin.TurnOffLogging();
				LoggingPlugin.SendMessage("Logging disabled.", args.Data.Nick);
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string>{"logging [on|off] - turn logging on or off"};
		}
	}
}
