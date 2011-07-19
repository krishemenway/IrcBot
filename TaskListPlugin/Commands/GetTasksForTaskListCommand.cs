using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.TaskList.Commands
{
	public class GetTasksForTaskListCommand : BaseBotCommand
	{
		public TaskListPlugin Plugin;

		public GetTasksForTaskListCommand(TaskListPlugin taskListPlugin)
		{
			Plugin = taskListPlugin;
			FirstMatchingWord = new List<string> { "gettasks" };
			EligibleReceiveTypes = new List<ReceiveType> { ReceiveType.ChannelMessage };
		}

		public override void Execute(IrcEventArgs args)
		{
			string channel = args.Data.Channel;

			if(args.Data.MessageArray.Length < 2)
				throw new Exception("Not enough parameters supplied");

			string taskListName = args.Data.MessageArray[1];

			TaskList taskList = Plugin.Repository.GetTaskList(taskListName);

			if(taskList == null)
				throw new Exception(string.Format("Could not find Task List {0}",taskListName));

			List<Task> completedTasks = new List<Task>();
			foreach (Task task in taskList)
			{
				if(task.Complete)
				{
					completedTasks.Add(task);
					continue;
				}
				Plugin.SendMessage(task, channel);
			}

			foreach(Task task in completedTasks)
			{
				Plugin.SendMessage(task,channel);
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return  new List<string> {"gettasks [tasklistname] - will show all tasks and their status' for a Task List"};
		}
	}
}
