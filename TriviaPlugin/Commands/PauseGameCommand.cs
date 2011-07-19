using System;
using System.Collections.Generic;
using IrcBot;
using IrcBot.Plugins.Trivia;
using Meebey.SmartIrc4net;

namespace Ircbot.Plugins.Trivia.Commands
{
	public class PauseGameCommand : BaseBotCommand
	{
		public TriviaPlugin TriviaPlugin;

		public PauseGameCommand(TriviaPlugin plugin)
		{
			TriviaPlugin = plugin;
			FirstMatchingWord = new List<string> {"pause"};
		}

		public override void Execute(IrcEventArgs args)
		{
			TriviaGame channelGame = TriviaPlugin.GetGameForChannel(args.Data.Channel);

			if(channelGame.GameStarted)
			{
				if(channelGame.Timer != null)
					channelGame.Timer.Stop();

				channelGame.CurrentState = GameState.Paused;
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"pause - pauses the game in the current channel"};
		}

		public override bool ShouldExecuteCommand(IrcEventArgs args)
		{
			TriviaGame channelGame = TriviaPlugin.GetGameForChannel(args.Data.Channel);

			if (channelGame == null)
				return false;

			return base.ShouldExecuteCommand(args) && channelGame.GameStarted;
		}
	}
}
