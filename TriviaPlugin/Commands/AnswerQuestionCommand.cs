using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IrcBot;
using IrcBot.Plugins.Trivia;
using Meebey.SmartIrc4net;

namespace Ircbot.Plugins.Trivia.Commands
{
	public class AnswerQuestionCommand : BaseBotCommand
	{
		public TriviaPlugin TriviaPlugin;

		public string[] InsignificantWords = { "the", "is", "was", "a", "and", "or", "for", "yet", "so" };

		public AnswerQuestionCommand(TriviaPlugin plugin)
		{
			TriviaPlugin = plugin;
		}

		public override void Execute(IrcEventArgs args)
		{
			string channel = args.Data.Channel;
			var currentGame = TriviaPlugin.GetGameForChannel(channel);

			if (currentGame != null && currentGame.GameStarted)
			{
				if (!currentGame.UsersAttempted.Contains(args.Data.From)
					|| currentGame.CurrentQuestion.Answers.Count == 1)
				{
					if (IsCorrectAnswer(args.Data.Message, currentGame.CurrentQuestion))
					{
						string cAnswer = currentGame.CurrentQuestion.GetCorrectAnswer().AnswerText;
						currentGame.SendMessage(channel, string.Format("Correct! {0} got the answer right! It was: {1}", MessageFormatting.MakeNavyBlue(args.Data.Nick), MessageFormatting.MakeHotPink(cAnswer)));
						//todo give the dude some points
						currentGame.GotoNextQuestion();
					}
					else
					{
						if (!currentGame.UsersAttempted.Contains(args.Data.From))
							currentGame.UsersAttempted.Add(args.Data.From);
					}
				}
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string>();
		}

		public override bool ShouldExecuteCommand(IrcEventArgs args)
		{
			var currentGame = TriviaPlugin.GetGameForChannel(args.Data.Channel);

			if (currentGame == null)
				return false;

			return currentGame.CurrentState == GameState.Started;
		}

		public bool IsCorrectAnswer(string answer, Question question)
		{
			// multi choice
			if (question.Answers.Count > 1)
			{
				char correctLetterAnswer = question.GetCorrectAnswerLetter();
				if (answer.ToLower().StartsWith(correctLetterAnswer + ".")
					|| answer.Split(new[] { ' ' })[0].ToLower().Equals(correctLetterAnswer.ToString(), StringComparison.CurrentCultureIgnoreCase))
				{
					return true;
				}
			}

			string[] answerArray = RemoveInsignificants(answer.ToLower().Split(new[] { ' ' }));
			string[] correctAnswerArray = RemoveInsignificants(question.GetCorrectAnswer().AnswerText.ToLower().Split(new[] { ' ' }));

			List<string> usedWords = new List<string>();
			foreach (var word in answerArray)
			{
				var cleanedUserWord = RemovePunctuation(word);
				foreach (var cword in correctAnswerArray)
				{
					var cleanedRealWord = RemovePunctuation(cword);
					if (cleanedUserWord.Equals(cleanedRealWord) && !usedWords.Contains(cleanedUserWord))
					{
						usedWords.Add(cleanedUserWord);
					}
				}
			}

			if (Convert.ToDouble(usedWords.Count) / correctAnswerArray.Length > TriviaPlugin.PercentageToBeCorrect
				&& Convert.ToDouble(usedWords.Count) / correctAnswerArray.Length < TriviaPlugin.HighPercentageToBeCorrect
				&& answerArray.Length < correctAnswerArray.Length + 5)
				return true;

			string s1 = string.Empty, s2 = string.Empty;
			s1 = answerArray.Aggregate(s1, (current, a) => current + " " + a);
			s2 = correctAnswerArray.Aggregate(s2, (current, a) => current + " " + a);

			double calc = Convert.ToDouble(LevenshteinDistance.Compute(s1, s2)) / Convert.ToDouble(s1.Length + s2.Length);

			return calc <= .5;
		}

		private string[] RemoveInsignificants(IEnumerable<string> words)
		{
			List<string> newWords = new List<string>();
			foreach (string inputWord in words)
			{
				bool found = false;

				foreach (var insignificantWord in InsignificantWords)
				{
					if (insignificantWord.Equals(inputWord, StringComparison.CurrentCultureIgnoreCase))
						found = true;
				}

				if (!found)
					newWords.Add(inputWord);
			}
			return newWords.ToArray();
		}

		private static string RemovePunctuation(string word)
		{
			return Regex.Replace(word, @"[^A-Za-z0-9]", string.Empty);
		}
	}
}
