using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.TaskList.Commands
{
	public class NewTaskCommand : BaseBotCommand
	{
		public TaskListPlugin TaskListPlugin;

		public NewTaskCommand(TaskListPlugin taskListPlugin)
		{
			TaskListPlugin = taskListPlugin;
			FirstMatchingWord = new List<string> { "addtask" };
			EligibleReceiveTypes = new List<ReceiveType> { ReceiveType.ChannelMessage };
		}

		public override void Execute(IrcEventArgs args)
		{
			var channel = args.Data.Channel;
			var user = args.Data.Nick;
			string madeNewTaskList = string.Empty;

			if(args.Data.MessageArray.Length < 3)
				throw new Exception("Not enough parameters supplied for command.");

			var taskListName = GetTaskListName(args.Data.MessageArray);

			string taskText = string.Empty;
			for(int i = 2;i < args.Data.MessageArray.Length; i++)
				taskText += string.Format(" {0}", args.Data.MessageArray[i]);

			TaskList list = TaskListPlugin.Repository.GetTaskList(taskListName);
			if(list == null)
			{
				// todo interface out a concept for default status
				list = TaskListPlugin.Repository.CreateNewTaskList(taskListName, user, channel,string.Empty);
				madeNewTaskList = "new";
			}

			Task newTask = list.NewTask(taskText, user);

			TaskListPlugin.SendMessage(string.Format("Added task to {0} TaskList {1} ({2} / {3} completed)",madeNewTaskList, list.TaskName, list.CompletedTasks(),list.Tasks.Count), channel);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"addtask [TaskListName] [task] - adds a task to the tasklist"};
		}

		protected string GetTaskListName(string[] message)
		{
			if (message[1] != null)
				return message[1];

			return string.Empty;
		}

		protected string GetTaskListName()
		{
			// todo make this work
			return string.Empty;
		}
	}
}
