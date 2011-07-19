using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class OpCommand : BaseBotCommand
	{
		public IrcBot Bot;

		private const string CurrentUser = "me";
		private const string CommandWord = "op";

		public OpCommand(IrcBot bot)
		{
			Bot = bot;
			FirstMatchingWord = new List<string> {CommandWord};
		}

		public void Execute(string channel, string nick)
		{
			Bot.IrcClient.Op(channel, nick);
		}

		public override void Execute(IrcEventArgs args)
		{
			if (args.Data.MessageArray.Length <= 2)
				throw new Exception("Not enough arguments supplied");

			string channel = IrcBot.MakeValidChannel(args.Data.MessageArray[1]);

			for (int i = 2; i < args.Data.MessageArray.Length; i++)
			{
				var nameToOp = string.Equals(args.Data.MessageArray[i], CurrentUser, StringComparison.CurrentCultureIgnoreCase)
				               	? args.Data.Nick
				               	: args.Data.MessageArray[i];

				Execute(channel, nameToOp);
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> { string.Format("{0} [channel] [user] [user] ... - Supply channel to op user(s) and as many users as you want to op", CommandWord) };
		}
	}
}
