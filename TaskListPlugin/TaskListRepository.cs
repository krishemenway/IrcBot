using System;
using System.Collections.Generic;
using System.Linq;
using IrcBot.Plugins.TaskList.IO;

namespace IrcBot.Plugins.TaskList
{
	public class TaskListRepository
	{
		private Dictionary<string, List<TaskList>> _TaskData;
		private readonly PersistanceHandler _PersistanceHandler;
		internal int NextTaskId;

		public TaskListRepository(string[] channels, string filePath)
		{
			_TaskData = new Dictionary<string, List<TaskList>>(StringComparer.CurrentCultureIgnoreCase);
			_PersistanceHandler = new XmlPersistance(this,filePath);
			Initialize(channels);
		}

		public void Initialize(string[] channels)
		{
			foreach(var channel in channels)
			{
				JoinedChannel(channel);
			}

			_TaskData = _PersistanceHandler.LoadData(this);
		}

		public void JoinedChannel(string channel)
		{
			if (!_TaskData.ContainsKey(channel))
			{
				_TaskData.Add(channel, new List<TaskList>());
			}
		}

		public TaskList CreateNewTaskList(string taskListName, string creator, string channel, string defaultStatus)
		{
			if (!_TaskData.ContainsKey(channel))
			{
				_TaskData.Add(channel, new List<TaskList>());
			}

			if (_TaskData[channel].Find(x =>
				string.Equals(x.TaskName, taskListName, StringComparison.CurrentCultureIgnoreCase)) != null)
			{
				throw new Exception("Already a TaskList with that name in that channel. Remove the list if you need use this name.");
			}

			TaskList newList = new TaskList(creator, taskListName, this, defaultStatus);

			_TaskData[channel].Add(newList);

			return newList;
		}

		public int GetNextTaskId()
		{
			return NextTaskId++;
		}

		public TaskList GetTaskList(string name)
		{
			foreach(var channel in _TaskData.Keys)
			{
				TaskList taskList = _TaskData[channel].Find(x =>
					string.Equals(x.TaskName, name, StringComparison.CurrentCultureIgnoreCase));

				if (taskList != null)
					return taskList;
			}

			return null;
		}

		public List<TaskList> GetTaskLists(string channel)
		{
			if(!_TaskData.ContainsKey(channel))
				_TaskData.Add(channel,new List<TaskList>());

			return _TaskData[channel];
		}

		// todo performance enhancement
		public List<Task> GetOpenTasksForUser(string nick)
		{
			List<Task> tasksForUser = new List<Task>();

			foreach(var channel in _TaskData.Keys)
			{
				foreach(var channelLists in _TaskData[channel])
				{
					tasksForUser.AddRange(channelLists.Tasks.FindAll(x => 
						string.Equals(x.Owner,nick) && !x.Complete));
				}
			}

			return tasksForUser;
		}

		public Task GetTask(int taskId)
		{
			return (from channel in _TaskData.Keys
			        from channelList in _TaskData[channel]
			        select channelList.Tasks.Find(x => x.TaskId == taskId)).FirstOrDefault(task => task != null);
		}

		public void SaveTaskListsForChannel(string channel)
		{
			if (!_TaskData.ContainsKey(channel))
			{
				_TaskData.Add(channel, new List<TaskList>());
			}

			_PersistanceHandler.SaveChannel(channel, _TaskData[channel]);
		}

		public void SaveTaskLists()
		{
			_PersistanceHandler.SaveData(_TaskData);
		}

		public void SetTaskId(int taskId)
		{
			NextTaskId = taskId;
		}

		public List<string> GetChannels()
		{
			return _TaskData.Keys.ToList();
		}
	}
}
