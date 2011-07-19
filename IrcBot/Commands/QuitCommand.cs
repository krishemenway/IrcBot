using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class QuitCommand : BaseBotCommand
	{
		public IrcBot Bot;

		public QuitCommand(IrcBot bot)
		{
			Bot = bot;
			FirstMatchingWord = new List<string> {"quit"};
			EligibleReceiveTypes = new List<ReceiveType> {ReceiveType.QueryMessage};
		}

		public override void Execute(IrcEventArgs args)
		{
			Bot.SendMessage("Aight i'm outta here.",args.Data.Nick);
			Bot.Initializer.Stop();
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"quit - tells the bot to leave the server and stops the bot service"};
		}

		public override bool ShouldExecuteCommand(IrcEventArgs args)
		{
			return base.ShouldExecuteCommand(args) && args.Data.MessageArray.Length == 1;
		}
	}
}
