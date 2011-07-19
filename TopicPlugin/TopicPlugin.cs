using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using IrcBot;
using Meebey.SmartIrc4net;

namespace TopicPlugin
{
	public class TopicPlugin : BotPlugin
	{
		public override void Initialize(IrcBot.IrcBot ircBot, XmlNode pluginSettings)
		{
			throw new NotImplementedException();
		}

		public override void TearDown()
		{
			throw new NotImplementedException();
		}

		public override void QueryMessageHandler(object sender, IrcEventArgs ircEventArgs)
		{
			throw new NotImplementedException();
		}

		public override void ChannelMessageHandler(object sender, IrcEventArgs ircEventArgs)
		{
			throw new NotImplementedException();
		}

		public override void OnJoinChannel(string channel)
		{
			throw new NotImplementedException();
		}

		public override void OnLeaveChannel(string channel)
		{
			throw new NotImplementedException();
		}

		public override void LoadCommands()
		{
			throw new NotImplementedException();
		}
	}
}
