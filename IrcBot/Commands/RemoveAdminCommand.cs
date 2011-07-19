using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class RemoveAdminCommand : BaseBotCommand
	{
		public IrcBot Bot;

		public RemoveAdminCommand(IrcBot bot)
		{
			Bot = bot;
			FirstMatchingWord = new List<string> {"remove", "delete"};
			SecondMatchingWord = new List<string> {"admin"};
		}

		public override void Execute(IrcEventArgs args)
		{
			if (args.Data.MessageArray.Length < 3)
				throw new Exception("Not enough parameters supplied. Must supply at least one person to receive admin rights.");

			string adminNicks = string.Empty;
			for (int i = 2; i < args.Data.MessageArray.Length; i++)
			{
				RemoveAdmin(args.Data.MessageArray[i]);
				adminNicks += i > 2 ? ", " : "";
				adminNicks += args.Data.MessageArray[i];
			}

			var plural = args.Data.MessageArray.Length - 1 > 1 ? "s" : "";

			Bot.SendMessage(string.Format("{0} removed as admin{1}", adminNicks, plural), args.Data.Nick);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> { "remove admin [nick] [nick] ... - Removes at least one admin with given nickname in Irc to the list of admins allowed to administrate the bot." };
		}

		public void RemoveAdmin(string nick)
		{
			Bot.AdminUserRepository.RemoveAdmin(nick);
		}
	}
}
