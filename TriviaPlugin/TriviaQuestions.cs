using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace IrcBot.Plugins.Trivia
{
	public class QuestionSet
	{
		protected const string QuestionNodeName = "Question";

		public string QuestionSetName;
		public List<Question> Questions;

		public QuestionSet()
		{
			QuestionSetName = "Current";
			Questions = new List<Question>();
		}

		public QuestionSet(string xmlFileName)
			: this(xmlFileName, xmlFileName)
		{
		}

		public QuestionSet(string xmlFileName,string questionSetName)
		{
			XmlDocument triviaQuestionSet = new XmlDocument();
			triviaQuestionSet.Load(xmlFileName);

			QuestionSetName = questionSetName;
			Questions = LoadQuestions(triviaQuestionSet.DocumentElement);
		}

		public List<Question> LoadQuestions(XmlNode parentNode)
		{
			return (from XmlNode question in parentNode 
					where string.Equals(question.Name, QuestionNodeName, StringComparison.CurrentCultureIgnoreCase) 
					select new Question(question)).ToList();
		}
	}

	public class Question
	{
		private const string AnswerNodeName = "Answer";
		private const string QuestionTextAttributeName = "text";

		public List<Answer> Answers;
		public string QuestionText;

		public Question(XmlNode questionNode)
		{
			if (string.IsNullOrEmpty(questionNode.Attributes[QuestionTextAttributeName].Value)) return;

			QuestionText = questionNode.Attributes[QuestionTextAttributeName].Value;
			Answers = GetAnswers(questionNode);

			if (Answers.Where(x => x.IsCorrectAnswer).Count() > 1) 
				throw new Exception("Question cannot have multiple correct answers. Question " + QuestionText);
		}

		public Answer GetCorrectAnswer()
		{
			if (Answers.Count > 1)
				return Answers.Find(x => x.IsCorrectAnswer);
			
			return Answers[0];
		}

		public char GetCorrectAnswerLetter()
		{
			for (int i = 0; i < Answers.Count; i++)
			{
				Answer a = Answers[i];
				if (a.IsCorrectAnswer)
					return Convert.ToChar('a' + i);
			}

			return 'a';
		}

		public List<Answer> GetAnswers(XmlNode questionNode)
		{
			return (from XmlNode answerNode in questionNode
			        where string.Equals(answerNode.Name, AnswerNodeName, StringComparison.CurrentCultureIgnoreCase)
			        select new Answer(answerNode)).ToList();
		}
	}

	public class Answer
	{
		public bool IsCorrectAnswer;
		public string AnswerText;

		public Answer(XmlNode answerNode)
		{
			if (answerNode.Attributes["correct"] != null)
				IsCorrectAnswer = Convert.ToBoolean(answerNode.Attributes["correct"].Value);
			AnswerText = answerNode.InnerText;
		}
	}
}
