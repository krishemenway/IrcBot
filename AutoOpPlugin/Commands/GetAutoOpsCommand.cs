using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.AutoOp.Commands
{
	public class GetAutoOpsCommand : BaseBotCommand
	{
		public AutoOpPlugin AutoOpPlugin;

		public GetAutoOpsCommand(AutoOpPlugin plugin)
		{
			AutoOpPlugin = plugin;
			FirstMatchingWord = new List<string> {"get"};
			SecondMatchingWord = new List<string> {"autoops"};
			EligibleReceiveTypes = new List<ReceiveType> {ReceiveType.ChannelMessage};
		}

		public override void Execute(IrcEventArgs args)
		{
			var channel = args.Data.Channel;
			var autoOps = AutoOpPlugin.Repository.GetUserListForChannel(channel);
			var autoOpsList = string.Join(", ", autoOps);
			var message = string.Format("Current AutoOps in {0}: {1}", channel, autoOpsList);
			AutoOpPlugin.Bot.SendMessage(message, channel);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"get autoops - will list the current AutoOps in the channel"};
		}
	}
}
