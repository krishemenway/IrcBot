using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.TaskList.Commands
{
	public class ShowTaskListsCommand : BaseBotCommand
	{
		public TaskListPlugin TaskListPlugin;

		public ShowTaskListsCommand(TaskListPlugin plugin)
		{
			TaskListPlugin = plugin;
			FirstMatchingWord = new List<string> {"show", "get"};
			SecondMatchingWord = new List<string> {"tasklist"};
		}

		public override void Execute(IrcEventArgs args)
		{
			string destination = string.Empty;
			ChannelList channelList = new ChannelList();
			AddChannelsFromMessage(args.Data.MessageArray, channelList);

			if(args.Data.Type == ReceiveType.QueryMessage)
			{
				if (channelList.Count == 0)
				{
					var channels = TaskListPlugin.Repository.GetChannels();

					channelList.AddRange(channels);
				}

				destination = args.Data.Nick;
			} else if(args.Data.Type == ReceiveType.ChannelMessage)
			{
				var channel = args.Data.Channel;
				if(channelList.Count == 0)
				{
					channelList.Add(channel);
				}

				destination = channel;
			}

			foreach (var channel in channelList)
			{
				ShowTaskListForChannel(channel, destination);
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"show tasklists - display all tasklists for current channel"
									,"show tasklists [channel] - will display all tasklists in a channel"};
		}

		public void AddChannelsFromMessage(string[] message, ChannelList channels)
		{
			for(int i = 2; i < message.Length; i++)
			{
				channels.Add(IrcBot.MakeValidChannel(message[i]));
			}
		}

		public void ShowTaskListForChannel(string channelToShow, string destination)
		{
			var taskLists = TaskListPlugin.Repository.GetTaskLists(channelToShow);

			var message = string.Format("TaskLists in Channel {0}: {1}", channelToShow, string.Join(", ", taskLists));
			TaskListPlugin.SendMessage(message, destination);
		}
	}
}
