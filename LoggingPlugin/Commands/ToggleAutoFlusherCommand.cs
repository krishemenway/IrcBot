using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.Logging.Commands
{
	public class ToggleAutoFlusherCommand : BaseBotCommand
	{
		public LoggingPlugin LoggingPlugin;

		public ToggleAutoFlusherCommand(LoggingPlugin loggingPlugin)
		{
			LoggingPlugin = loggingPlugin;
			FirstMatchingWord = new List<string> { "autoflush" };
		}

		public override void Execute(IrcEventArgs args)
		{
			if (args.Data.MessageArray.Length < 2)
				throw new Exception("Must specify what you want to be done to the autoflusher");

			if(args.Data.MessageArray.Length > 2)
				throw new Exception("Far too many parameters, far too ambiguous for me to guess.");

			if(!string.Equals(args.Data.MessageArray[1], "on", StringComparison.CurrentCultureIgnoreCase)
				&& !string.Equals(args.Data.MessageArray[1], "off", StringComparison.CurrentCultureIgnoreCase))
				throw new Exception("Must specify the desired state of the autoflusher");

			if (string.Equals(args.Data.MessageArray[1], "on", StringComparison.CurrentCultureIgnoreCase))
			{
				LoggingPlugin.TurnOnAutoFlusher();
				LoggingPlugin.SendMessage("Autoflusher enabled.", args.Data.Nick);
			} else if (string.Equals(args.Data.MessageArray[1],"off",StringComparison.CurrentCultureIgnoreCase))
			{
				LoggingPlugin.TurnOffAutoFlusher();
				LoggingPlugin.SendMessage("Autoflusher disabled.", args.Data.Nick);
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"autoflush [on|off] - Automatically force the bot to flush it's buffers on set timer"};
		}
	}
}
