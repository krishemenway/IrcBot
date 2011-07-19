using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.AutoOp.Commands
{
	public class AddOpCommand : BaseBotCommand
	{
		public AutoOpPlugin AutoOpPlugin;

		public AddOpCommand(AutoOpPlugin plugin)
		{
			AutoOpPlugin = plugin;
			FirstMatchingWord = new List<string> {"autoop"};
			EligibleReceiveTypes = new List<ReceiveType> {ReceiveType.ChannelMessage};
		}

		public override void Execute(IrcEventArgs args)
		{
			if(args.Data.MessageArray.Length < 2)
				throw new FormatException("I Require More Informatin, Dave.");

			var channel = IrcBot.MakeValidChannel(args.Data.Channel);
			var nick = args.Data.MessageArray[1];
			string fullName = null;

			if(string.Equals(nick,"me"))
			{
				nick = args.Data.Nick;
				fullName = args.Data.From;
			}

			AutoOpPlugin.Repository.AddOpUser(channel, fullName, nick);
			AutoOpPlugin.OpCommand.Execute(channel, nick);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"autoop [nick] - will setup a nick to get Op'ed whenever they join the channel you perform the command in"};
		}
	}
}
