using System;
using System.Collections.Generic;
using System.Xml;
using Ircbot.Plugins.Trivia;
using Ircbot.Plugins.Trivia.Commands;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.Trivia
{
	public class TriviaPlugin : BotPlugin
	{
		protected const string TriviaStartKeyword = "!trivia start";

		public Dictionary<string,TriviaGame> Games;
		public IrcBot Bot;
		public List<QuestionSet> QuestionSets;
		public XmlNode PluginSettings;
		public double PercentageToBeCorrect;
		public const double HighPercentageToBeCorrect = 1.4;

		public const string TriviaKeyword = "trivia";
		protected const string PercentCorrectnessXPath = "WordCorrectness";
		private const double DefaultPercentCorrectness = .8;

		public TriviaPlugin()
		{
			Games = new Dictionary<string, TriviaGame>(StringComparer.CurrentCultureIgnoreCase);
		}

		public override void Initialize(IrcBot ircBot, XmlNode pluginSettings)
		{
			Bot = ircBot;
			PluginSettings = pluginSettings;

			new LoadQuestionsCommand(this).Execute();
			PercentageToBeCorrect = GetPercentCorrectness();

			foreach (var channel in Bot.Settings.Channels)
				Games.Add(channel, new TriviaGame(this,channel));
		}

		private double GetPercentCorrectness()
		{
			var correctnessNode = PluginSettings.SelectSingleNode(PercentCorrectnessXPath);

			if (correctnessNode != null)
			{
				return Convert.ToDouble(correctnessNode.InnerText);
			}

			return DefaultPercentCorrectness;
		}

		public override void TearDown()
		{
			foreach(var game in Games)
				game.Value.StopGame(string.Empty);
		}

		public override void OnJoinChannel(string channel)
		{
			if(!Games.ContainsKey(channel))
				Games.Add(channel,new TriviaGame(this,channel));
		}

		public override void OnLeaveChannel(string channel)
		{
			var game = GetGameForChannel(channel);

			if(game != null && game.GameStarted)
				game.StopGame(string.Empty);
		}

		public override void LoadCommands()
		{
			AdminCommands.Add(new LoadQuestionsCommand(this));

			Commands.Add(new AnswerQuestionCommand(this));
			Commands.Add(new StartGameCommand(this));
			Commands.Add(new StopGameCommand(this));
			Commands.Add(new PauseGameCommand(this));
			Commands.Add(new ResumeGameCommand(this));
		}

		public TriviaGame GetGameForChannel(string channel)
		{
			if (string.IsNullOrEmpty(channel) || Games[channel] == null)
				return null;

			return Games[channel];
		}
	}
}
