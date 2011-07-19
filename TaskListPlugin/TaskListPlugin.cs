using System.Xml;
using IrcBot.Plugins.TaskList.Commands;

namespace IrcBot.Plugins.TaskList
{
	public class TaskListPlugin : BotPlugin
	{
		public TaskListRepository Repository;
		public IrcBot IrcBot;
		protected int NextTaskId;

		public override void Initialize(IrcBot ircBot, XmlNode pluginSettings)
		{
			IrcBot = ircBot;
			Repository = new TaskListRepository(ircBot.Settings.Channels.ToArray(),IrcBot.FilePath);
		}

		public override void TearDown()
		{
			Repository.SaveTaskLists();
		}

		public override void OnJoinChannel(string channel)
		{
			Repository.JoinedChannel(channel);
		}

		public override void OnLeaveChannel(string channel)
		{
			Repository.SaveTaskListsForChannel(channel);
		}

		public override void LoadCommands()
		{
			Commands.Add(new NewTaskCommand(this));
			Commands.Add(new NewTaskListCommand(this));
			Commands.Add(new GetTasksForTaskListCommand(this));
			Commands.Add(new GetMyTasksCommand(this));
			Commands.Add(new TakeItemCommand(this));
			Commands.Add(new ChangeStatusCommand(this));
			Commands.Add(new StatusCommand(this));
			Commands.Add(new ShowTaskListsCommand(this));
		}

		public void SendMessage(object message, string channel)
		{
			IrcBot.SendMessage(message.ToString(), channel);
		}

		// todo move task lists between channels
	
	}
}
