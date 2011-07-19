using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace IrcBot.Plugins.AutoOp
{
	public class OpRepository
	{
		private const string OpUsersFilename = "opusers.xml";
		private readonly string _RootFilePath;
		protected string PathToOpUsers
		{
			get { return string.Format("{0}{1}", _RootFilePath, OpUsersFilename); }
		}
		protected OpUserList OpUserInfo;

		public OpRepository(string rootFilePath)
		{
			_RootFilePath = rootFilePath;
			SetupEmptyOpUserInfo();
		}

		protected void SetupEmptyOpUserInfo()
		{
			OpUserInfo = new OpUserList();
		}

		public void AddOpUser(string channel, string fullName)
		{
			AddOpUser(channel, fullName, null);
		}

		public void AddOpUser(string channel, string fullName, string nick)
		{
			if (!IsOpUser(channel, fullName))
			{
				OpUserInfo.Add(new OpUser(channel, fullName, nick));
			}
		}

		public void RemoveOpUsers(string channel)
		{
			OpUserInfo.RemoveAll(x => string.Equals(x.Channel, channel, StringComparison.CurrentCultureIgnoreCase));
		}

		public void RemoveOpUser(string channel, string nick)
		{
			OpUserInfo.RemoveAll(x => string.Equals(x.Nick, nick, StringComparison.CurrentCultureIgnoreCase)
									&& string.Equals(x.Channel, channel, StringComparison.CurrentCultureIgnoreCase));
		}

		public void SaveUsers()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(OpUserList));
			serializer.Serialize(new FileStream(PathToOpUsers, FileMode.Create), OpUserInfo);
		}

		public void LoadUsers()
		{
			if (!File.Exists(PathToOpUsers))
				return;

			XmlSerializer serializer = new XmlSerializer(typeof(OpUserList));
			OpUserInfo = (OpUserList)serializer.Deserialize(new FileStream(PathToOpUsers, FileMode.OpenOrCreate));
			
		}

		public List<OpUser> GetUserListForChannel(string channel)
		{
			return OpUserInfo.Where(user => string.Equals(user.Channel, channel, StringComparison.CurrentCultureIgnoreCase)).ToList();
		}

		public OpUserList GetOpUsers ()
		{
			return OpUserInfo;
		}

		public bool IsOpUser(string channel, string fullName)
		{
			return OpUserInfo.Any(x => string.Equals(x.Channel, channel, StringComparison.CurrentCultureIgnoreCase)
			                    && string.Equals(x.FullName, fullName, StringComparison.CurrentCultureIgnoreCase));
		}
	}

	[Serializable]
	public class OpUser
	{
		[XmlAttribute("Channel")]
		public string Channel;
		[XmlAttribute("FullName")]
		public string FullName;
		[XmlAttribute("Nick")] 
		public string Nick;

		public OpUser()
		{
			Channel = string.Empty;
			FullName = string.Empty;
			Nick = string.Empty;
		}

		public OpUser(string channel, string fullName, string nick = null)
		{
			Channel = channel;
			FullName = fullName;
			Nick = nick ?? string.Empty;
		}
	}

	[Serializable,XmlRoot("OpUsers")]
	public class OpUserList : List<OpUser>
	{
	}
}
