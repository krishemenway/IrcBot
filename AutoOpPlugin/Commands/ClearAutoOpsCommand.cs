using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.AutoOp.Commands
{
	public class ClearAutoOpsCommand : BaseBotCommand
	{
		public AutoOpPlugin AutoOpPlugin;

		public ClearAutoOpsCommand(AutoOpPlugin plugin)
		{
			AutoOpPlugin = plugin;
			FirstMatchingWord = new List<string> {"clear", "truncate"};
			SecondMatchingWord = new List<string> {"autoops"};
		}

		public override void Execute(IrcEventArgs args)
		{
			var channel = IrcBot.MakeValidChannel(args.Data.Channel);

			if(args.Data.Type == ReceiveType.QueryMessage)
			{

				if(args.Data.MessageArray.Length < 3)
				{
					throw new ApplicationException("Need to specify at least one channel to clear");
				}

				for(int i = 2; i < args.Data.MessageArray.Length; i++)
				{
					Execute(IrcBot.MakeValidChannel(args.Data.MessageArray[i]));
				}

			} else if(args.Data.Type == ReceiveType.ChannelMessage)
			{
				Execute(channel);
			}
		}

		public void Execute(string channel)
		{
			AutoOpPlugin.Repository.RemoveOpUsers(channel);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"clear autoops [channel] [channel] ... - clears autoops for specified channel(s)",
									 "clear autoops - clears autoops for current channel"};
		}
	}
}
