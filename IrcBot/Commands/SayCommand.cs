using System;
using System.Collections.Generic;
using System.Text;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class SayCommand : BaseBotCommand
	{
		public IrcBot Bot;

		public SayCommand(IrcBot bot)
		{
			Bot = bot;
			EligibleReceiveTypes = new List<ReceiveType> {ReceiveType.QueryMessage};
			FirstMatchingWord = new List<string> {"say"};
		}

		public override void Execute(IrcEventArgs args)
		{
			if(args.Data.MessageArray.Length < 3)
			{
				throw new FormatException("Need more info sucka");
			}

			StringBuilder newMessage = new StringBuilder();
			for (int i = 2; i < args.Data.MessageArray.Length; i++)
			{
				if (i != 2) newMessage.Append(" ");
				newMessage.Append(args.Data.MessageArray[i]);
			}

			var destination = args.Data.MessageArray[1];

			Bot.SendMessage(newMessage.ToString(),destination);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"say [channel or nick] [message]"};
		}
	}
}
