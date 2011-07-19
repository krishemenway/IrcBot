using System.Xml;
using Meebey.SmartIrc4net;

namespace IrcBot
{
	public interface IBotPlugin
	{
		void Initialize(IrcBot ircBot, XmlNode pluginSettings);
		void TearDown();
		void QueryMessageHandler(object sender, IrcEventArgs ircEventArgs);
		void ChannelMessageHandler(object sender, IrcEventArgs ircEventArgs);
		void OnJoinChannel(string channel);
		void OnLeaveChannel(string channel);
	}

	interface IBotSettings
	{
		
	}
}
