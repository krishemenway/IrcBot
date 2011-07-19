using System;
using System.Collections.Generic;
using System.Timers;
using IrcBot.Plugins.Trivia;

namespace Ircbot.Plugins.Trivia
{
	public class TriviaGame
	{
		public int TimeToAnswerQuestion = 60000;

		public bool GameStarted
		{
			get
			{
				return CurrentState == GameState.Started ? true : false;
			}
		}

		public string Channel;
		public GameState CurrentState;
		public int CurrentQuestionNum;
		public Question CurrentQuestion;
		public QuestionSet CurrentQuestionSet;
		public Timer Timer;
		public bool UseQuestionsOnlyOnce = true;
		public List<string> UsersAttempted;

		public List<QuestionSet> QuestionSet;
		public TriviaPlugin Plugin;

		public TriviaGame(TriviaPlugin plugin, string channel)
			: this(plugin, channel, plugin.QuestionSets)
		{
		}

		public TriviaGame(TriviaPlugin plugin, string channel, List<QuestionSet> questionSets)
		{
			UsersAttempted = new List<string>();
			CurrentQuestionSet = new QuestionSet();
			Channel = channel;
			Plugin = plugin;
			QuestionSet = questionSets;
		}

		public void TimerIsUp(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			if (GameStarted)
			{
				Answer answer = CurrentQuestion.GetCorrectAnswer();
				SendMessage(Channel, string.Format("Time is up! The answer was: {0}", answer.AnswerText));

				GotoNextQuestion();
			}
		}

		public void GotoNextQuestion()
		{
			Timer.Stop();
			UsersAttempted.Clear();
			CurrentQuestion = GetQuestion();
			if (CurrentQuestion != null && GameStarted)
			{
				PrintQuestion(CurrentQuestion);
				Timer.Start();
			}
		}

		public Question GetQuestion()
		{
			if (CurrentQuestionSet.Questions.Count == 0)
			{
				SendMessage(Channel, "Ran out of questions...");
				StopGame("none");
				return null;
			}
			return UseQuestionsOnlyOnce ? GetAndRemoveQuestion() : GetAndLeaveQuestion();
		}

		public Question GetAndRemoveQuestion()
		{
			int randQuestionInt = new Random().Next(CurrentQuestionSet.Questions.Count);
			Question question = CurrentQuestionSet.Questions[randQuestionInt];
			CurrentQuestionSet.Questions.RemoveAt(randQuestionInt);
			CurrentQuestionNum++;
			return question;
		}

		public Question GetAndLeaveQuestion()
		{
			int randQuestionInt = new Random().Next(CurrentQuestionSet.Questions.Count);
			CurrentQuestionNum++;
			return CurrentQuestionSet.Questions[randQuestionInt];
		}

		public void StartGame(string startingPlayer, string questionSetName = "all question sets")
		{
			if (questionSetName.Equals("all question sets"))
			{
				foreach (var questionSet in QuestionSet)
					CurrentQuestionSet.Questions.AddRange(questionSet.Questions);
			}
			else
			{
				QuestionSet set =
					QuestionSet.Find(x => x.QuestionSetName.Equals(questionSetName, StringComparison.CurrentCultureIgnoreCase));

				CurrentQuestionSet.Questions.Clear();
				CurrentQuestionSet.Questions.AddRange(set.Questions);
			}

			CurrentQuestionNum = 0;
			SetupNewTimer();
			CurrentState = GameState.Started;
			SendMessage(Channel, string.Format("Trivia Game Started with {0}", questionSetName));
			GotoNextQuestion();
		}

		private void SetupNewTimer()
		{
			Timer = new Timer(TimeToAnswerQuestion);
			Timer.Elapsed += TimerIsUp;
			Timer.AutoReset = false;
		}

		public void StopGame (string player)
		{
			if (GameStarted)
			{
				CurrentState = GameState.NotStarted;
				if (Timer != null)
				{
					Timer.Stop();
					Timer.Close();
				}
				SendMessage(Channel, "Game stopping...");
			}
		}

		public void PrintQuestion(Question question)
		{
			SendMessage(Channel, string.Format("Question {0}: {1}", CurrentQuestionNum, question.QuestionText));

			if (question.Answers.Count > 1)
			{
				char answerLetter = 'a';
				foreach (var answer in question.Answers)
				{
					SendMessage(Channel, string.Format("{0}. {1}", answerLetter, answer.AnswerText));
					answerLetter++;
				}
			}
		}

		public void SendMessage(string destination, string message)
		{
			Plugin.Bot.SendMessage(message, destination);
		}
	}

	public enum GameState
	{
		NotStarted = 0,
		Started,
		Paused
	}
}
