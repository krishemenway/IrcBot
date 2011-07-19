using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.AutoOp.Commands
{
	public class RemoveAutoOpCommand : BaseBotCommand
	{
		public AutoOpPlugin AutoOpPlugin;

		public RemoveAutoOpCommand(AutoOpPlugin plugin)
		{
			AutoOpPlugin = plugin;
			FirstMatchingWord = new List<string> {"unautoop"};
			EligibleReceiveTypes = new List<ReceiveType> {ReceiveType.ChannelMessage};
		}

		public override void Execute(IrcEventArgs args)
		{
			if (args.Data.MessageArray.Length < 2)
				throw new FormatException("I Require More Informatin, Dave.");

			var channel = args.Data.Channel;
			var user = args.Data.MessageArray[1];

			if (string.Equals(user, "me"))
			{
				user = args.Data.Nick;
			}

			AutoOpPlugin.Repository.RemoveOpUser(channel, user);
			AutoOpPlugin.DeOpCommand.Execute(channel, user);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"unautoop [nick|me] - remove someone or yourself from the autoop list for the current channel"};
		}
	}
}
