using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IrcBot.Plugins.TaskList
{
	[Serializable,XmlType("TaskList")]
	public class TaskList : IEnumerable<Task> 
	{
		public List<Task> Tasks;
		public TaskListRepository Repository;

		[XmlAttribute("Creator")]
		public string Creator;
		
		[XmlAttribute("Name")]
		public string TaskName;
		
		[XmlAttribute("DefaultStatus")]
		public string DefaultStatus;

		public TaskList(string creator, string taskName, TaskListRepository repository, string defaultStatus)
		{
			DefaultStatus = defaultStatus;
			Repository = repository;
			TaskName = taskName;
			Creator = creator;
			Tasks = new List<Task>();
		}

		public IEnumerator<Task> GetEnumerator()
		{
			return Tasks.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Task NewTask(string taskText, string creator)
		{
			return NewTask(taskText, creator, DefaultStatus);
		}

		public Task NewTask(string taskText, string creator, string status)
		{
			return NewTask(taskText, creator, Repository.GetNextTaskId(),status);
		}

		public Task NewTask(string taskText, string creator, int taskId, string status)
		{
			Task newTask = new Task(taskText, creator, taskId, this);
			newTask.UpdateStatus(status);
			AddTask(newTask);
			return newTask;
		}

		public void AddTask(Task task)
		{
			Tasks.Add(task);
		}

		public int UncompleteTasks()
		{
			return Tasks.Count - CompletedTasks();
		}

		public int CompletedTasks()
		{
			return Tasks.FindAll(x => x.Complete).Count;
		}

		public double PercentageComplete()
		{
			int completedTasks = CompletedTasks();
			return ((double)completedTasks) / Tasks.Count;
		}

		public override string ToString()
		{
			return TaskName;
		}
	}
}
