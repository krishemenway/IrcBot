using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace IrcBot.Plugins.TaskList.IO
{
	public sealed class XmlPersistance : PersistanceHandler
	{
		public XDocument XmlData;
		public XElement RootNode;
		private const string XmlFileName = "TaskSaveData.xml";
		private const string TaskCreatorAttributeName = "Creator";
		private const string TaskListCreatorAttributeName = "Creator";
		private readonly string _XmlFileNameWithPath;

		public XmlPersistance(TaskListRepository repository, string filePath) : base(repository)
		{
			Repository = repository;
			_XmlFileNameWithPath = string.Format("{0}\\{1}",filePath,XmlFileName);
		}

		public override void SaveTask(Task taskData)
		{
			
		}

		public override void SaveTaskList(string channel, TaskList taskList)
		{
			XElement newTaskListNode = BuildTaskListNode(taskList);

			XElement currentTaskList = (from curTaskList in RootNode.Elements("Channel").Elements("TaskList")
										where (string)curTaskList.Attribute("Name") == channel
										select curTaskList).FirstOrDefault(x => string.Equals(x.Name, channel));

			if (currentTaskList != null)
			{
				currentTaskList.ReplaceWith(newTaskListNode);
			}

			WriteXmlDataToFile();
		}

		public override void SaveChannel(string channel, List<TaskList> channelData)
		{
			XElement newChannelNode = BuildChannelNode(channel, channelData);

			foreach(XElement channelNode in RootNode.Nodes())
			{
				var channelNameAttribute = channelNode.Attribute("Name");

				if(channelNameAttribute != null)
				{
					if(string.Equals(channelNameAttribute.Value,channel,StringComparison.CurrentCultureIgnoreCase))
					{
						channelNode.ReplaceWith(newChannelNode);
						WriteXmlDataToFile();
						return;
					}
				}
			}

			RootNode.Add(newChannelNode);
			WriteXmlDataToFile();
		}

		private XElement BuildChannelNode(string channel, IEnumerable<TaskList> channelData)
		{
			XElement channelNode = new XElement("Channel");
			channelNode.SetAttributeValue("Name", channel);

			foreach (var taskList in channelData)
			{
				var taskListElement = BuildTaskListNode(taskList);
				channelNode.Add(taskListElement);
			}

			return channelNode;
		}

		private XElement BuildTaskListNode(TaskList taskList)
		{
			XElement taskListElement = new XElement("TaskList");
			
			taskListElement.SetAttributeValue("Name", taskList.TaskName);
			taskListElement.SetAttributeValue("DefaultStatus", taskList.DefaultStatus);
			taskListElement.SetAttributeValue(TaskListCreatorAttributeName, taskList.Creator);

			foreach (var task in taskList)
			{
				XElement taskElement = BuildTaskNode(task);

				taskListElement.Add(taskElement);
			}

			return taskListElement;
		}

		private XElement BuildTaskNode(Task task)
		{
			XElement taskElement = new XElement("Task");
			taskElement.SetValue(task.TaskText);
			taskElement.SetAttributeValue("Status", task.Status);
			taskElement.SetAttributeValue(TaskCreatorAttributeName, task.Creator);
			taskElement.SetAttributeValue("TaskId", task.TaskId);
			taskElement.SetAttributeValue("Owner", task.Owner);
			return taskElement;
		}


		public override void SaveData(Dictionary<string, List<TaskList>> taskData)
		{
			foreach(var channel in taskData.Keys)
			{
				SaveChannel(channel,taskData[channel]);
			}

			WriteXmlDataToFile();
		}

		public override Dictionary<string, List<TaskList>> LoadData(TaskListRepository repository)
		{
			LoadXmlDataFromFile();

			RootNode = XmlData.Root;

			if (RootNode == null)
			{
				RootNode = new XElement("TaskData");
				XmlData.Add(RootNode);
			}

			Dictionary<string, List<TaskList>> taskData = 
				new Dictionary<string, List<TaskList>>(StringComparer.CurrentCultureIgnoreCase);

			foreach (XElement channelNode in RootNode.Nodes())
			{
				var channelNodeAttribute = channelNode.Attribute("Name");
				if (channelNodeAttribute != null)
				{
					List<TaskList> channelLists = new List<TaskList>();
					foreach(XElement taskListNode in channelNode.Nodes())
					{
						var taskListCreator = taskListNode.Attribute(TaskListCreatorAttributeName);
						var taskListName = taskListNode.Attribute("Name");
						var taskListDefaultStatus = taskListNode.Attribute("DefaultStatus");

						if(taskListCreator != null && taskListName != null && taskListDefaultStatus != null)
						{
							TaskList taskList = new TaskList(taskListCreator.Value, taskListName.Value, repository, taskListDefaultStatus.Value);

							foreach(XElement taskNode in taskListNode.Nodes())
							{
								var taskText = taskNode.Value;
								var taskCreatorNode = taskNode.Attribute("Creator");
								var taskOwnerAttribute = taskNode.Attribute("Owner");
								var taskIdAttribute = taskNode.Attribute("TaskId");
								var taskStatus = taskNode.Attribute("Status");

								if (taskCreatorNode != null && taskOwnerAttribute != null
									&& taskIdAttribute != null && taskStatus != null)
								{
									int taskId = Convert.ToInt32(taskIdAttribute.Value);

									if (taskId >= repository.NextTaskId)
										repository.NextTaskId = taskId + 1;

									Task task = new Task(taskText, taskCreatorNode.Value, taskId, taskList);
									task.WasTakenBy(taskOwnerAttribute.Value);
									task.UpdateStatus(taskStatus.Value);
									taskList.AddTask(task);
								}
							}
							channelLists.Add(taskList);
						}
					}

					taskData.Add(channelNodeAttribute.Value, channelLists);
				}
			}

			return taskData;
		}

		private void LoadXmlDataFromFile()
		{
			XmlData = File.Exists(_XmlFileNameWithPath)
				? XDocument.Load(_XmlFileNameWithPath) 
				: new XDocument(new XElement("TaskData"));
		}

		private void WriteXmlDataToFile()
		{
			XmlData.Save(_XmlFileNameWithPath);
		}
	}
}
