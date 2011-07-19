using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class ShowPluginsCommand : BaseBotCommand
	{
		public IrcBot Bot;

		public ShowPluginsCommand(IrcBot bot)
		{
			Bot = bot;
			FirstMatchingWord = new List<string> { "showplugins" };
		}

		public override void Execute(IrcEventArgs args)
		{
			string destination = string.Empty;
			if (args.Data.Type == ReceiveType.ChannelMessage)
			{
				destination = args.Data.Channel;
			} else if (args.Data.Type == ReceiveType.QueryMessage)
			{
				destination = args.Data.Nick;
			}

			if (Bot.PluginManager.Plugins.Count > 0)
			{
				Bot.SendMessage("Currently Loaded Plugins:", destination);
			}

			foreach (var plugin in Bot.PluginManager.Plugins)
			{
				Bot.SendMessage(string.Format(" {0}",plugin.Name), destination);
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"showplugins - lists all loaded plugins"};
		}
	}
}
