using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.TaskList.Commands
{
	public class TakeItemCommand : BaseBotCommand
	{
		public TaskListPlugin TaskListPlugin;

		public TakeItemCommand(TaskListPlugin plugin)
		{
			TaskListPlugin = plugin;
			FirstMatchingWord = new List<string> { "taketask" };
		}

		public override void Execute(IrcEventArgs args)
		{
			string channel = args.Data.Channel;
			string nick = args.Data.Nick;

			if(args.Data.MessageArray.Length < 2)
			{
				throw new Exception("Not enough arguments supplied.");
			}

			for (int i = 1; i <= args.Data.MessageArray.Length; i++)
			{
				int taskId = Convert.ToInt32(args.Data.MessageArray[i]);
				Task task = TaskListPlugin.Repository.GetTask(taskId);

				if(task != null)
				{
					task.WasTakenBy(nick);
					TaskListPlugin.SendMessage(string.Format("Task {0} was taken by {1}",task.TaskText,nick),channel);
				}
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return  new List<string> {"taketask [task#] [[task#] [task#] ...] - assigns a task to yourself. To get a task number, say gettasks"};
		}
	}
}
