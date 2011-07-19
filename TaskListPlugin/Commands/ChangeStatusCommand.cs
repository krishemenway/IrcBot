using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.TaskList.Commands
{
	public class ChangeStatusCommand : BaseBotCommand
	{
		public TaskListPlugin TaskListPlugin;

		public ChangeStatusCommand(TaskListPlugin plugin)
		{
			TaskListPlugin = plugin;

			FirstMatchingWord = new List<string>
				{
					"done",
					"completed",
					"complete",
					"test",
					"bug",
					"updatestatus"
				};
		}

		// todo ugly code
		public override void Execute(IrcEventArgs args)
		{
			string channel = args.Data.Channel;
			string nick = args.Data.Nick;
			List<Task> currentUserTasks = TaskListPlugin.Repository.GetOpenTasksForUser(nick);
			int taskId = -1;
			bool hasTaskId = args.Data.MessageArray.Length > 1
				&& int.TryParse(args.Data.MessageArray[1], out taskId);

			Task taskToUpdate;
			if(hasTaskId)
			{
				taskToUpdate = TaskListPlugin.Repository.GetTask(taskId);
			} else if (currentUserTasks.Count == 1)
			{
				taskToUpdate = currentUserTasks[0];
			} else
			{
				throw new Exception("No taskId was specified and it is far too ambiguous for me to imply which task you meant");
			}

			if(taskToUpdate == null)
				throw new Exception("Could not determine the task that you were inferring");

			string status = string.Empty;
			if (args.Data.MessageArray.Length > 2)
			{
				for (int i = hasTaskId ? 2 : 1; i < args.Data.MessageArray.Length; i++)
					status += args.Data.MessageArray[i] + " ";
			}

			if (string.IsNullOrEmpty(status))
				status = args.Data.MessageArray[0];

			bool taskIsComplete = taskToUpdate.UpdateStatus(status);

			if (taskIsComplete)
			{
				TaskListPlugin.SendMessage(string.Format("Setting task '{0}' to Complete with status message: {1}", taskToUpdate.TaskText, taskToUpdate.Status), channel);
			}
			else
			{
				TaskListPlugin.SendMessage(taskToUpdate, channel);
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return
				new List<string> {"updatestatus [task#] [statusmessage] - Update a task with a status. Item is closed when status is like 'done' or 'complete'"};
		}
	}
}
