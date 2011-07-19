using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class DeOpCommand : BaseBotCommand
	{
		public IrcBot Bot;

		public DeOpCommand(IrcBot bot)
		{
			Bot = bot;
			FirstMatchingWord = new List<string> {"deop"};
		}

		public void Execute(string channel, string user)
		{
			Bot.IrcClient.Deop(channel, user);
		}

		public override void Execute(IrcEventArgs args)
		{
			if(args.Data.MessageArray.Length < 3)
			{
				throw new ApplicationException("Need to know what channel and what nick to deop.");
			}

			var channel = IrcBot.MakeValidChannel(args.Data.MessageArray[1]);

			for (int i = 2; i < args.Data.MessageArray.Length; i++ )
			{
				Execute(channel, args.Data.MessageArray[i]);
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"deop [channel] [nick] [nick] ... - will deop user(s) in channel"};
		}
	}
}
