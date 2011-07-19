using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace IrcBot
{
	public partial class IrcBotService : ServiceBase, IBotInitializer
	{
		private Thread _WorkerThread;
		public IrcBot IrcBot;

		public IrcBotService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			LogInformation("IrcBot Service Started");

			IrcBot = new IrcBot(this);

			ThreadStart threadStarter = IrcBot.Initialize;
			_WorkerThread = new Thread(threadStarter);
			_WorkerThread.Start();
		}

		protected override void OnStop()
		{
			IrcBot.TearDown();
		}

		public void LogError(string message)
		{
			EventLog.WriteEntry(message, EventLogEntryType.Error);
		}

		public void LogWarning(string message)
		{
			EventLog.WriteEntry(message, EventLogEntryType.Warning);
		}

		public void LogInformation(string message)
		{
			EventLog.WriteEntry(message, EventLogEntryType.Information);
		}
	}

	public interface IBotInitializer
	{
		void LogError(string message);
		void LogWarning(string message);
		void LogInformation(string message);
		void Stop();
	}

	public class ConsoleStarter : IBotInitializer
	{
		public IrcBot Bot;

		public ConsoleStarter()
		{
			Bot = new IrcBot(this);
		}

		public void LogError(string message)
		{
			WriteMessage("Error", message);
		}

		public void LogWarning(string message)
		{
			WriteMessage("Warning", message);
		}

		public void LogInformation(string message)
		{
			WriteMessage("Information", message);
		}

		public void WriteMessage(string type, string message)
		{
			Console.WriteLine(string.Format("{0}: {1}", type, message));
		}

		public void Stop()
		{
			Bot.TearDown();
		}

		public void Start()
		{
			Bot.Initialize();
		}
	}

	public class IrcBotConsole
	{
		public IrcBot Bot;

		public static void Main(string[] args)
		{
			ConsoleStarter starter = new ConsoleStarter();
			starter.Start();
		}
	}

	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase[] servicesToRun = new ServiceBase[] 
			    { 
			        new IrcBotService() 
			    };
			ServiceBase.Run(servicesToRun);
		}
	}
}
