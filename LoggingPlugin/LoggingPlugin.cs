using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Xml;
using IrcBot.Plugins.Logging.Commands;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.Logging
{
	public class LoggingPlugin : BotPlugin
	{
		private const string AdminQueryChannelName = "Admin";
		private const string LogFileExtensionName = "Extension";
		private const string SettingsXmlNodeName = "Set";
		private const string SettingAttributeName = "name";
		private const string SettingAttributeValue = "value";
		private const string LogPathSettingName = "LogPath";
		private const string AutoFlushSetting = "AutoFlush";

		public Dictionary<string, string> Settings;
		public Dictionary<string,StreamWriter> StreamWriters;
		public double FlushInterval = 60;
		private string _LogPath;
		private string _Extension;
		private IrcBot _IrcBot;
		private Timer _Timer;
		private bool _LoggingEnabled;

		public override void Initialize(IrcBot ircBot, XmlNode pluginSettings)
		{
			StreamWriters = new Dictionary<string, StreamWriter>(StringComparer.CurrentCultureIgnoreCase);
			_IrcBot = ircBot;

			Settings = GetSettings(pluginSettings);

			_Extension = Settings.ContainsKey(LogFileExtensionName) ? Settings[LogFileExtensionName] : "log";

			if(string.IsNullOrEmpty(Settings[LogPathSettingName]))
				throw new Exception(@"No LogPath defined in settings file. Put this under <Logging>: <Set name=""LogPath"" value=""C:\Logs\""/>");

			_LogPath = Settings[LogPathSettingName];
			FlushInterval = Convert.ToDouble(Settings[AutoFlushSetting]);

			if(!Directory.Exists(_LogPath))
				Directory.CreateDirectory(_LogPath);

			TurnOnLogging();
		}

		public void TurnOnLogging()
		{
			if (!_LoggingEnabled)
			{
				foreach (var channel in _IrcBot.Settings.Channels)
					StreamWriters[channel] = GetStreamWriterForChannel(channel);

				StreamWriters[AdminQueryChannelName] = new StreamWriter(GetFileLocation("AdminLog"), true);
			}

			TurnOnAutoFlusher();
			_LoggingEnabled = true;
		}

		public void TurnOffLogging()
		{
			if (_LoggingEnabled)
			{
				TurnOffAutoFlusher();
				FlushStreamWriters();

				foreach (KeyValuePair<string, StreamWriter> writers in StreamWriters)
				{
					writers.Value.Close();
				}
			}

			_LoggingEnabled = false;
		}

		public override void TearDown()
		{
			TurnOffLogging();
		}

		public override void QueryMessageHandler(object sender, IrcEventArgs ircEventArgs)
		{
			LogMessage(ircEventArgs.Data.Nick, AdminQueryChannelName, ircEventArgs.Data.Message);
		}

		public override void ChannelMessageHandler(object sender, IrcEventArgs ircEventArgs)
		{
			LogMessage(ircEventArgs.Data.Nick,ircEventArgs.Data.Channel,ircEventArgs.Data.Message);
		}

		private void LogMessage(string from, string destination, string message)
		{
			if (_LoggingEnabled)
			{
				var logLocation = IsChannel(destination) ? destination : AdminQueryChannelName;

				DateTime currentDate = DateTime.Now;
				var timestamp = currentDate.ToString("yyyy-MM-dd HH:mm:ss");

				StreamWriters[logLocation].WriteLine(string.Format("{0} ({1}): {2}", from, timestamp, message));
			}
		}

		private static bool IsChannel(string destination)
		{
			return destination.StartsWith("#");
		}

		public override void OnJoinChannel(string channel)
		{
			if(!StreamWriters.ContainsKey(channel))
				StreamWriters.Add(channel,GetStreamWriterForChannel(channel));
		}

		public override void OnLeaveChannel(string channel)
		{
			if (StreamWriters.ContainsKey(channel))
			{
				StreamWriters[channel].Close();
				StreamWriters.Remove(channel);
			}
		}

		public override void LoadCommands()
		{
			AdminCommands.Add(new ToggleAutoFlusherCommand(this));
			AdminCommands.Add(new ToggleLoggingCommand(this));
			AdminCommands.Add(new FlushBuffersCommand(this));
		}

		public void TurnOnAutoFlusher()
		{
			if (_Timer == null || !_Timer.Enabled)
			{
				_Timer = new Timer(1000*FlushInterval) {AutoReset = true};
				_Timer.Elapsed += FlushStreamWriters;
				_Timer.Start();
			}
		}

		public void TurnOffAutoFlusher()
		{
			if (_Timer != null)
			{
				_Timer.Stop();
				_Timer.Close();
				_Timer.Dispose();
			}
		}

		public void FlushStreamWriters(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			FlushStreamWriters();
		}

		public void FlushStreamWriters()
		{
			foreach (var streamWriter in StreamWriters)
				streamWriter.Value.Flush();
		}

		private string GetFileLocation(string file)
		{
			return string.Format("{0}{1}.{2}", _LogPath, file, _Extension);
		}

		public Dictionary<string, string> GetSettings(XmlNode pluginSettings)
		{
			Dictionary<string, string> settings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

			foreach (XmlNode node in pluginSettings.ChildNodes)
			{
				if (string.Equals(node.Name, SettingsXmlNodeName)
					&& node.Attributes[SettingAttributeName] != null
					&& node.Attributes[SettingAttributeValue] != null)
				{
					settings[node.Attributes[SettingAttributeName].Value] = node.Attributes[SettingAttributeValue].Value;
				}
			}

			return settings;
		}

		public StreamWriter GetStreamWriterForChannel(string channel)
		{
			return new StreamWriter(GetFileLocation(channel), true);
		}

		public override void OnSendMessage(string from, string destination, string message)
		{
			message = CleanMessage(message);
			LogMessage(from, destination, message);
		}

		private static string CleanMessage(string message)
		{
			return MessageFormatting.ClearFormatting(message);
		}

		public void SendMessage(string message, string destination)
		{
			_IrcBot.SendMessage(message ,destination);
		}
	}
}
