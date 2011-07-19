using System;
using System.Collections.Generic;
using System.Linq;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class HelpCommand : BaseBotCommand
	{
		protected const string HelpKeyword = "help";
		public IrcBot Bot;

		public HelpCommand(IrcBot bot)
		{
			Bot = bot;
			EligibleReceiveTypes = new List<ReceiveType> {ReceiveType.QueryMessage};
			FirstMatchingWord = new List<string> {HelpKeyword, "?"};
		}

		public override void Execute(IrcEventArgs args)
		{
			string nick = args.Data.Nick;
			string fullName = args.Data.From;

			if (args.Data.MessageArray.Length == 1)
			{
				PrintCommands(Bot.Commands, nick);

				if (Bot.AdminUserRepository.IsAdminUser(fullName))
					PrintCommands(Bot.AdminCommands, nick);

				return;
			} 
			
			if (args.Data.MessageArray.Length == 2)
			{
				if (!Bot.PluginManager.Plugins.Any(x => string.Equals(x.Name, args.Data.MessageArray[1], StringComparison.CurrentCultureIgnoreCase)))
					throw new Exception(string.Format("Could not find the plugin with the name '{0}'",args.Data.MessageArray[1]));

				BotPlugin plugin = Bot.PluginManager.Plugins.Find(x => string.Equals(x.Name, args.Data.MessageArray[1]));

				PrintCommands(plugin.Commands, nick);

				if (Bot.AdminUserRepository.IsAdminUser(fullName))
					PrintCommands(plugin.AdminCommands, nick);

				return;
			}

			throw new Exception("I'm not 100% sure what you were getting at by feeding me a ton of parameters...i'm just going to choke.");
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			if (args != null)
			{
				if ((args.Data.Type == ReceiveType.QueryMessage || args.Data.Type == ReceiveType.ChannelMessage)
				    && args.Data.MessageArray.Length != 1)
				{
					return new List<string> {"help [command] - displays help information for a command"};
				}
			}

			return new List<string> {"help - displays all commands"};
		}

		public override bool ShouldExecuteCommand(IrcEventArgs args)
		{
			// todo add ? check for commands (ie autoop ?)
			return base.ShouldExecuteCommand(args) || MatchesAtLeastOneCommand(args);
		}

		public bool MatchesAtLeastOneCommand(IrcEventArgs args)
		{
			return Bot.PluginManager.Plugins.SelectMany(plugin => plugin.Commands).Any(command => command.ShouldExecuteCommand(args));
		}

		protected void PrintCommands(List<IBotCommand> commands,string destination)
		{
			if (commands.Count > 0)
			{
				Bot.SendMessage("Loaded Commands:", destination);
			}

			foreach(var command in commands)
			{
				foreach (var helpsyntax in command.GetHelpSyntax(null))
				{
					Bot.SendMessage(" " + helpsyntax, destination);
				}
			}
		}
	}
}
