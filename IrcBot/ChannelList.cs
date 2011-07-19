using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IrcBot
{
	[Serializable, XmlType("ChannelList")]
	public class ChannelList : IEnumerable<string>
	{
		private Dictionary<string,object> Channels;

		public ChannelList()
		{
			Channels = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
		}

		public int Count
		{
			get
			{
				return Channels.Count;
			}
		}

		public bool Contains(string channel)
		{
			return Channels.ContainsKey(IrcBot.MakeValidChannel(channel));
		}

		public void AddRange(IEnumerable<string> channels)
		{
			foreach(var channel in channels)
			{
				Add(channel);
			}
		}

		public void Add(string channel)
		{
			var channelCleaned = IrcBot.MakeValidChannel(channel);
			if (!Channels.ContainsKey(channelCleaned))
			{
				Channels.Add(channelCleaned, null);
			}
		}

		public void Remove(string channel)
		{
			var channelCleaned = IrcBot.MakeValidChannel(channel);
			if (Channels.ContainsKey(channelCleaned))
			{
				Channels.Remove(channelCleaned);
			}
		}

		public IEnumerator<string> GetEnumerator()
		{
			return Channels.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
