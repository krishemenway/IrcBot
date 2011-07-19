using System;
using System.Collections.Generic;
using System.Xml;
using IrcBot.Plugins.Trivia;

namespace Ircbot.Plugins.Trivia
{
	public class TriviaSettings
	{
		protected const string TriviaXmlName = "Trivia";
		protected const string PercentCorrectnessXmlName = "WordCorrectness";

		public List<QuestionSet> QuestionSets;
		public Double PercentCorrectness;

		public TriviaSettings(string settingsConfigFile)
		{
			QuestionSets = new List<QuestionSet>();
		}

		public void ParseNode(XmlNode node)
		{
			switch (node.Name)
			{
				case TriviaXmlName:
					GetTriviaFilesFromNode(node);
					break;
				case PercentCorrectnessXmlName:
					PercentCorrectness = Convert.ToDouble(node.InnerText);
					break;
			}
		}

		private void GetTriviaFilesFromNode(XmlNode triviaFilesNode)
		{
			foreach (XmlNode questionFileNode in triviaFilesNode)
			{
				QuestionSets.Add(new QuestionSet(questionFileNode.InnerText));
			}
		}
	}
}
