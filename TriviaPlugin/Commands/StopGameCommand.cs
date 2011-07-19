using System;
using System.Collections.Generic;
using IrcBot;
using IrcBot.Plugins.Trivia;
using Meebey.SmartIrc4net;

namespace Ircbot.Plugins.Trivia.Commands
{
	public class StopGameCommand : BaseBotCommand
	{
		public TriviaPlugin TriviaPlugin;
		private const string StopKeyword = "stop";

		public StopGameCommand(TriviaPlugin plugin)
		{
			TriviaPlugin = plugin;
			FirstMatchingWord = new List<string> {TriviaPlugin.TriviaKeyword};
			SecondMatchingWord = new List<string> {StopKeyword};
		}

		public override void Execute(IrcEventArgs args)
		{
			var channel = args.Data.Channel;

			if (TriviaPlugin.Games.ContainsKey(channel))
			{
				TriviaPlugin.Games[channel].StopGame(args.Data.Nick);
			}
			else
			{
				throw new Exception(string.Format("No game setup for the current channel {0}", channel));
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"trivia stop - will stop a game of trivia if there is one in progress"};
		}

		public override bool ShouldExecuteCommand(IrcEventArgs args)
		{
			return base.ShouldExecuteCommand(args)
				&& TriviaPlugin.Games.ContainsKey(args.Data.Channel) 
				&& TriviaPlugin.Games[args.Data.Channel].GameStarted;
		}
	}
}
