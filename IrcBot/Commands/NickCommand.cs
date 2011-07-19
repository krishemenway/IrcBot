using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class NickCommand : BaseBotCommand
	{
		public IrcBot Bot;

		public NickCommand(IrcBot bot)
		{
			Bot = bot;
			FirstMatchingWord = new List<string> { "nick" };
		}

		public override void Execute(IrcEventArgs args)
		{
			if(args.Data.MessageArray.Length < 2)
				throw new Exception("No nick specified");

			if (args.Data.MessageArray.Length > 2)
				throw new Exception("Whoa whoa whoa, what's with all the parameters?");

			Bot.IrcClient.RfcNick(args.Data.MessageArray[1]);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"nick [NewNickName] - Changes the nickname of the bot"};
		}
	}
}
