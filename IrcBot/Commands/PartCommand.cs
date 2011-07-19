using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class PartCommand : BaseBotCommand
	{
		public IrcBot Bot;

		public PartCommand(IrcBot bot)
		{
			Bot = bot;
			FirstMatchingWord = new List<string> {"leave", "part"};
			EligibleReceiveTypes = new List<ReceiveType> {ReceiveType.QueryMessage};
		}

		public override void Execute(IrcEventArgs args)
		{
			if (args.Data.MessageArray.Length != 2)
			{
				throw new Exception("Must supply only a channel to leave");
			}

			string channel = IrcBot.MakeValidChannel(args.Data.MessageArray[1]);
			Bot.IrcClient.RfcPart(channel);

			foreach (BotPlugin plugin in Bot.PluginManager.Plugins)
			{
				try
				{
					plugin.OnLeaveChannel(channel);
				}
				catch (Exception e)
				{
					Bot.PluginManager.UnloadPlugin(plugin, "LeaveChannel", e);
				}
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"leave [channel] - tells bot to leave a channel"};
		}
	}
}
