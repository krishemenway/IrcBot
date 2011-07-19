using System;
using System.Xml.Serialization;

namespace IrcBot.Plugins.TaskList
{
	[Serializable, XmlType("Task")]
	public class Task
	{
		public TaskList ParentTaskList;
		private string _Owner;
		private string _Status;
		public bool Complete;

		[XmlAttribute("TaskId")]
		public int TaskId = -1;
		
		[XmlText]
		public string TaskText;

		[XmlAttribute("Creator")]
		public string Creator;
		
		[XmlAttribute("Owner")]
		public string Owner
		{
			get { return string.IsNullOrEmpty(_Owner) ? "Unclaimed" : _Owner; }
			set { _Owner = value;}
		}
		[XmlAttribute("Status")]
		public string Status
		{
			get { return _Status; }
		}

		public Task() : this(string.Empty, string.Empty, string.Empty) { }

		public Task(string taskText, string creator, string status) : this(taskText,creator,-1,status) { }

		public Task(string taskText, string creator, int taskId, string status) : this(taskText,creator, taskId, (TaskList)null) { }

		public Task(string taskText, string creator, int taskId, TaskList parentTaskList)
		{
			Creator = creator;
			TaskText = taskText;
			TaskId = taskId;
			ParentTaskList = parentTaskList;

			UpdateStatus(parentTaskList.DefaultStatus);
		}

		public bool UpdateStatus(string status)
		{
			_Status = status;

			Complete = IsCompleteMessage(status);

			return Complete;
		}

		public void WasTakenBy(string user)
		{
			Owner = user;
		}

		public void QuitTask()
		{
			Owner = string.Empty;
		}

		public override string ToString()
		{
			var status = Complete ? "Completed" : _Status;
			const string messageFormat = "Task#{0} {1} | {2} [{3}]";

			string taskStatus = Complete
			    ? MessageFormatting.MakeLightGray(string.Format(messageFormat, TaskId, status, TaskText, Owner))
			    : string.Format(messageFormat, TaskId, status, TaskText, MessageFormatting.MakeNavyBlue(Owner));

			return taskStatus;
		}

		public bool IsCompleteMessage(string message)
		{
			if (message.StartsWith("complete",StringComparison.CurrentCultureIgnoreCase))
				return true;

			if (message.StartsWith("done",StringComparison.CurrentCultureIgnoreCase))
				return true;

			if (message.StartsWith("finish", StringComparison.CurrentCultureIgnoreCase))
				return true;

			if (message.StartsWith("end", StringComparison.CurrentCultureIgnoreCase))
				return true;

			return false;
		}
	}
}
