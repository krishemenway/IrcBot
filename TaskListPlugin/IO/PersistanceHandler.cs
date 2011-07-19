using System.Collections.Generic;

namespace IrcBot.Plugins.TaskList.IO
{
	public abstract class PersistanceHandler
	{
		public TaskListRepository Repository;

		public PersistanceHandler(TaskListRepository repository)
		{
			Repository = repository;
		}

		public abstract void SaveTask(Task taskData);
		public abstract void SaveTaskList(string channel, TaskList taskList);
		public abstract void SaveChannel(string channel, List<TaskList> channelData);
		public abstract void SaveData(Dictionary<string, List<TaskList>> taskData);
		public abstract Dictionary<string, List<TaskList>> LoadData(TaskListRepository repository);
	}
}
