using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using IrcBot;
using IrcBot.Plugins.Trivia;
using Meebey.SmartIrc4net;

namespace Ircbot.Plugins.Trivia.Commands
{
	public class LoadQuestionsCommand : BaseBotCommand
	{
		public TriviaPlugin TriviaPlugin;

		private const string QuestionSetXPath = "QuestionSet";
		private const string QuestionSetNameAttributeName = "name";

		public LoadQuestionsCommand(TriviaPlugin plugin)
		{
			TriviaPlugin = plugin;
			FirstMatchingWord = new List<string> { "loadquestions" };
		}

		public void Execute()
		{
			Execute(null);
		}

		public override void Execute(IrcEventArgs args)
		{
			List<QuestionSet> questionSets = new List<QuestionSet>();
			var questionSetNodes = TriviaPlugin.PluginSettings.SelectNodes(QuestionSetXPath);

			if(questionSetNodes == null)
				throw new Exception("Could not find any question sets in Settings config");
			
			foreach (XmlNode node in questionSetNodes)
			{
				string filePath = TriviaPlugin.Bot.FilePath + node.InnerText;

				if (!string.IsNullOrEmpty(node.InnerText) && File.Exists(filePath))
				{
					string questonSetName;

					if (node.Attributes[QuestionSetNameAttributeName] != null
						&& !string.IsNullOrEmpty(node.Attributes[QuestionSetNameAttributeName].Value))
					{
						questonSetName = node.Attributes[QuestionSetNameAttributeName].Value;
					}
					else
					{
						questonSetName = node.InnerText;
					}

					QuestionSet set = new QuestionSet(filePath) { QuestionSetName = questonSetName };

					questionSets.Add(set);
				}
			}

			TriviaPlugin.QuestionSets = questionSets;
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"loadquestions - Will reload questions from question files defined in Settings.xml"};
		}
	}
}
