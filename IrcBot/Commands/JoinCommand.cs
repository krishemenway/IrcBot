using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class JoinCommand : BaseBotCommand
	{
		public IrcBot Bot;

		public JoinCommand(IrcBot bot)
		{
			Bot = bot;
			EligibleReceiveTypes = new List<ReceiveType> {ReceiveType.QueryMessage};
			FirstMatchingWord = new List<string> {"join"};
		}

		public override void Execute(IrcEventArgs args)
		{
			if (args.Data.MessageArray.Length < 2)
				throw new Exception("Must supply a channel name to join.");

			string channel = IrcBot.MakeValidChannel(args.Data.MessageArray[1]);
			Bot.IrcClient.RfcJoin(channel);

			foreach (BotPlugin plugin in Bot.PluginManager.Plugins)
			{
				try
				{
					plugin.OnJoinChannel(channel);
				}
				catch (Exception e)
				{
					Bot.PluginManager.UnloadPlugin(plugin, "JoinChannel", e);
				}
			}

			Bot.SendMessage("Alright, i'm here...so what? You wanna fight about it?",args.Data.Nick);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"join [channel] - tells bot to join a channel"};
		}
	}

}
