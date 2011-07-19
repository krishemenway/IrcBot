using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class AddAdminCommand : BaseBotCommand
	{
		public IrcBot Bot;

		public AddAdminCommand(IrcBot bot)
		{
			Bot = bot;
			FirstMatchingWord = new List<string> {"add", "new"};
			SecondMatchingWord = new List<string> {"admin"};
		}

		public void AddNewAdmin(string nick)
		{
			Bot.AdminUserRepository.AddAdmin(nick);
			Bot.SendMessage("*Authorized*", nick);
		}

		public override void Execute(IrcEventArgs args)
		{
			if(args.Data.MessageArray.Length < 3)
				throw new Exception("Not enough parameters supplied. Must supply at least one person to receive admin rights.");

			string adminNicks = string.Empty;
			for(int i = 2;i < args.Data.MessageArray.Length;i++)
			{
				Bot.AdminUserRepository.AddAdmin(args.Data.MessageArray[i]);
				adminNicks += i > 2 ? ", " : "";
				adminNicks += args.Data.MessageArray[i];
			}

			string plural = args.Data.MessageArray.Length > 3 ? "s" : "";
			Bot.SendMessage(string.Format("Admin{0} {1} added.", plural, adminNicks), args.Data.Nick);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"add admin [nick] [nick] ... - Adds at least one admin with given nickname in Irc to the list of admins allowed to administrate the bot."};
		}
	}
}
