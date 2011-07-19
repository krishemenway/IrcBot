using System.Collections.Generic;
using System.Timers;
using IrcBot;
using IrcBot.Plugins.Trivia;
using Meebey.SmartIrc4net;

namespace Ircbot.Plugins.Trivia.Commands
{
	public class ResumeGameCommand : BaseBotCommand
	{
		public TriviaPlugin TriviaPlugin;

		public ResumeGameCommand(TriviaPlugin plugin)
		{
			TriviaPlugin = plugin;
			FirstMatchingWord = new List<string> {"resume"};
		}

		public override void Execute(IrcEventArgs args)
		{
			var currentGame = TriviaPlugin.GetGameForChannel(args.Data.Channel);

			if(currentGame == null || currentGame.CurrentState == GameState.NotStarted)
			{
				new StartGameCommand(TriviaPlugin).Execute(args);
			} else
			{
				if(currentGame.Timer == null)
					currentGame.Timer = new Timer(currentGame.TimeToAnswerQuestion);

				currentGame.Timer.Start();

				currentGame.CurrentState = GameState.Started;
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"resume - will restart your Trivia Game"};
		}

		public override bool ShouldExecuteCommand(IrcEventArgs args)
		{
			var currentGame = TriviaPlugin.GetGameForChannel(args.Data.Channel);

			return base.ShouldExecuteCommand(args)
				&& currentGame != null && currentGame.CurrentState != GameState.Started;
		}
	}
}
