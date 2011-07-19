using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Xml.Serialization;
using Meebey.SmartIrc4net;

namespace IrcBot
{
	public class AdminRepository
	{
		protected AdminList Admins;
		protected Timer WriteToFileTimer;

		private readonly string _AdminPass;
		private readonly string _RootFilePath;

		private const string AdminFileName = "admin.xml";
		private const double TimerInterval = 120000;

		public string AdminConfigFile
		{
			get
			{
				return _RootFilePath + AdminFileName;
			}
		}

		public XmlSerializer Serializer;

		public AdminRepository(string adminPass, string rootFilePath)
		{
			Serializer = new XmlSerializer(typeof(AdminList));
			Admins = new AdminList();
			_AdminPass = adminPass;

			_RootFilePath = rootFilePath;

			WriteToFileTimer = new Timer(TimerInterval);
			WriteToFileTimer.Elapsed += TimerElapsed;
			WriteToFileTimer.Start();
		}

		public void TimerElapsed(object e, ElapsedEventArgs args)
		{
			SaveAdmins();
		}

		public void LoadAdmins()
		{
			if (!File.Exists(AdminConfigFile))
				return;

			Admins = (AdminList)Serializer.Deserialize(new FileStream(AdminConfigFile, FileMode.OpenOrCreate));
		}

		public void SaveAdmins()
		{
			Serializer.Serialize(new FileStream(AdminConfigFile, FileMode.Create), Admins);
		}

		public bool AddUserIfPasswordIsCorrect(IrcEventArgs args)
		{
			if(string.Equals(args.Data.Message, _AdminPass))
			{
				AddAdmin(args.Data.Nick, args.Data.From);
				return true;
			}

			return false;
		}

		public bool IsAdminUser(string fullName)
		{
			return Admins.Contains(fullName);
		}

		public void AddAdmin(string nick)
		{
			AddAdmin(nick, string.Empty);
		}

		public void AddAdmin(string nick, string fullName)
		{
			if (IsAdminUser(fullName))
				return;

			Admin admin = new Admin(nick, fullName);
			
			Admins.Add(admin);
		}

		public void RemoveAdmin(string name)
		{
			Admins.Remove(name);
		}

		public List<Admin> GetAdmins()
		{
			return Admins;
		}
	}

	[Serializable]
	[XmlRoot("Admins")]
	public class AdminList : List<Admin>
	{
		public void Remove(string name)
		{
			foreach(var admin in this)
			{
				if(string.Equals(admin.Nick, name,StringComparison.CurrentCultureIgnoreCase)
					|| string.Equals(admin.FullName, name, StringComparison.CurrentCultureIgnoreCase))
				{
					Remove(admin);
				}
			}
		}

		public bool Contains(string fullName)
		{
			return ContainsFullName(fullName);
		}

		public bool ContainsFullName(string fullName)
		{
			if (this.Any(admin => string.Equals(admin.FullName, fullName, StringComparison.CurrentCultureIgnoreCase)))
			{
				return true;
			}

			return false;
		}

		public bool ContainsNick(string nick)
		{
			if (this.Any(admin => string.Equals(admin.Nick, nick, StringComparison.CurrentCultureIgnoreCase)))
			{
				return true;
			}

			return false;
		}
	}

	[Serializable]
	public class Admin
	{
		[XmlAttribute("Nick")]
		public string Nick;
		[XmlAttribute("FullName")]
		public string FullName;

		public Admin()
		{
			Nick = string.Empty;
			FullName = string.Empty;
		}

		public Admin(string nick, string fullName)
		{
			Nick = nick;
			FullName = fullName;
		}

		public override string ToString()
		{
			return Nick;
		}
	}

	public enum AdminPermissionLevel
	{
		Admin,
		All
	}
}
