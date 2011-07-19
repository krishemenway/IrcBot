using System.Collections.Generic;
using System.Linq;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.TaskList.Commands
{
	public class StatusCommand : BaseBotCommand
	{
		public TaskListPlugin Plugin;

		public StatusCommand(TaskListPlugin plugin)
		{
			Plugin = plugin;
			FirstMatchingWord = new List<string> { "status" };
		}

		public override void Execute(IrcEventArgs args)
		{
			var channel = args.Data.Channel;
			List<TaskList> lists = Plugin.Repository.GetTaskLists(channel);

			Plugin.SendMessage("Current Unclaimed Tasks:",channel);

			foreach (var task in from list in lists from task in list where !task.Complete select task)
			{
				Plugin.SendMessage(task,channel);
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return  new List<string> {"status - gets all current unclaimed tasks for a channel"};
		}
	}
}
