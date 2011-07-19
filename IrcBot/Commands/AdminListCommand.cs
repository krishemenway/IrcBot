using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class AdminListCommand : BaseBotCommand
	{
		public IrcBot Bot;

		public AdminListCommand(IrcBot bot)
		{
			Bot = bot;

			FirstMatchingWord = new List<string> {"listadmins", "adminlist"};
		}

		public override void Execute(IrcEventArgs args)
		{
			var adminList = Bot.AdminUserRepository.GetAdmins();

			Bot.SendMessage(string.Join(", ", adminList), args.Data.Nick);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return  new List<string> {"listadmins - Displays the current list of admins for bot"};
		}
	}
}
