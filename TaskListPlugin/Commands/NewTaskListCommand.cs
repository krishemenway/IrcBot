using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.TaskList.Commands
{
	public class NewTaskListCommand : BaseBotCommand
	{
		public TaskListPlugin TaskListPlugin;

		public NewTaskListCommand(TaskListPlugin taskListPlugin)
		{
			TaskListPlugin = taskListPlugin;
			FirstMatchingWord = new List<string> { "newtasklist" };
		}

		public override void Execute(IrcEventArgs args)
		{
			string channel = args.Data.Channel;
			string creator = args.Data.Nick;
			string taskListName = args.Data.MessageArray[1];

			// todo interface for default status
			TaskListPlugin.Repository.CreateNewTaskList(taskListName, creator, channel, string.Empty);

			TaskListPlugin.SendMessage(string.Format("Created new TaskList {0}", taskListName),args.Data.Channel);
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return  new List<string> {"newtasklist [tasklistname] - creates a new Task List in your channel"};
		}
	}
}
