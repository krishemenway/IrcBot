using System.Collections.Generic;
using System.Xml;
using Meebey.SmartIrc4net;

namespace IrcBot
{
	public abstract class BotPlugin
	{
		public string Name
		{
			get { return GetType().Name; }
		}

		public abstract void Initialize(IrcBot ircBot, XmlNode pluginSettings);
		public abstract void TearDown();

		public List<IBotCommand> Commands = new List<IBotCommand>();
		public List<IBotCommand> AdminCommands = new List<IBotCommand>();

		public virtual void LoadCommands() {}
		public virtual void QueryMessageHandler(object sender, IrcEventArgs ircEventArgs) { }
		public virtual void ChannelMessageHandler(object sender, IrcEventArgs ircEventArgs) { }
		public virtual void OnJoinChannel(string channel) {}
		public virtual void OnLeaveChannel(string channel) {}
		public virtual void OnSendMessage(string from, string destination, string message) {}
		public virtual void OnChannelAction(ActionEventArgs eventArgs) {}
		public virtual void OnChannelNotice(IrcEventArgs ircEventArgs) {}
		public virtual void OnChannelModeChange(IrcEventArgs ircEventArgs) {}
		public virtual void OnWho(WhoEventArgs whoEventArgs) {}
	}
}
