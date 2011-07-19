using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.TaskList.Commands
{
	public class GetMyTasksCommand : BaseBotCommand
	{
		public TaskListPlugin TaskListPlugin;

		public GetMyTasksCommand(TaskListPlugin plugin)
		{
			TaskListPlugin = plugin;
			EligibleReceiveTypes = new List<ReceiveType> {ReceiveType.ChannelMessage};
			FirstMatchingWord = new List<string> { "tasks" };
		}

		public override void Execute(IrcEventArgs args)
		{
			List<Task> tasks = TaskListPlugin.Repository.GetOpenTasksForUser(args.Data.Nick);

			foreach(var task in tasks)
			{
				TaskListPlugin.SendMessage(task,args.Data.Channel);
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string>{"tasks - displays all of your current tasks"};
		}
	}
}
