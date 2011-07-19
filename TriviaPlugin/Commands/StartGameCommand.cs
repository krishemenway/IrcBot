using System.Collections.Generic;
using IrcBot;
using IrcBot.Plugins.Trivia;
using Meebey.SmartIrc4net;

namespace Ircbot.Plugins.Trivia.Commands
{
	public class StartGameCommand : BaseBotCommand
	{
		private const string StartKeyword = "start";
		protected TriviaPlugin TriviaPlugin;

		public StartGameCommand(TriviaPlugin plugin)
		{
			TriviaPlugin = plugin;
			FirstMatchingWord = new List<string> {TriviaPlugin.TriviaKeyword};
			SecondMatchingWord = new List<string> { StartKeyword };
		}

		public override void Execute(IrcEventArgs args)
		{
			var message = args.Data.MessageArray;
			var channel = args.Data.Channel;
			var nick = args.Data.Nick;

			List<QuestionSet> sets = new List<QuestionSet>();
			
			if(message.Length > 2)
			{
				for(int i = 2;i < message.Length;i++)
				{
					int i1 = i;
					QuestionSet questionSetToFind = TriviaPlugin.QuestionSets.Find(x => x.QuestionSetName.StartsWith(message[i1]));
					if(questionSetToFind != null)
					{
						sets.Add(questionSetToFind);
					}
				}
			}

			if (!TriviaPlugin.Games.ContainsKey(channel))
			{
				TriviaPlugin.Games.Add(channel,
					sets.Count > 0 ? new TriviaGame(TriviaPlugin, channel, sets) : new TriviaGame(TriviaPlugin, channel));
			}

			TriviaPlugin.Games[channel].StartGame(nick);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"trivia start - will start a game of trivia"};
		}
	}
}
