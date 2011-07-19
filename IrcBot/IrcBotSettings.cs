using System;
using System.Collections.Generic;
using System.Xml;

namespace IrcBot
{
	public class IrcBotSettings
	{
		protected const string RootNodeName = "IrcBotSettings";
		protected const string HostSettingName = "Host";
		protected const string PortSettingName = "Port";
		protected const string RealBotNameSettingName = "RealBotName";
		protected const string BotNameSettingName = "BotName";
		protected const string AdminPassSettingName = "AdminPass";
		protected const string ChannelXmlNodeName = "Channel";
		protected const string PluginsXmlNodeName = "Plugin";
		protected const string PluginNameAttributeName = "name";
		protected const string SettingNodeName = "Setting";
		protected const string PluginFolderSettingName = "PluginFolder";
		private const string DefaultAdminPass = "NinjaText";
		
		public List<string> Channels;
		public Dictionary<string, string> Settings;
		public XmlDocument SettingsFile = new XmlDocument();
		public XmlNode SettingsHeadNode;
		public string FilePath;
		public IrcBot Bot;

		#region Settings
		public string Host
		{
			get { return GetSetting(HostSettingName); }
			set { SetSetting(HostSettingName, value); }
		}
		public int Port
		{
			get { return Convert.ToInt32(GetSetting(PortSettingName)); }
			set { SetSetting(PortSettingName, value.ToString()); }
		}
		public string RealBotName
		{
			get { return GetSetting(RealBotNameSettingName); }
			set { SetSetting(RealBotNameSettingName,value); }
		}
		public string BotName
		{
			get { return GetSetting(BotNameSettingName); }
			set { SetSetting(BotNameSettingName,value); }
		}
		public string PluginFolder
		{
			get { return GetSetting(PluginFolderSettingName); }
			set {SetSetting(PluginFolderSettingName,value);}
		}
		public string AdminPass
		{
			get
			{
				try
				{
					return GetSetting(AdminPassSettingName);
				} 
				catch(KeyNotFoundException)
				{
					return DefaultAdminPass;
				}
			}
		}
		#endregion

		public IrcBotSettings(string settingsConfigFile, IrcBot bot)
		{
			Bot = bot;
			Settings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
			Channels = new List<string>();
			SettingsFile.Load(settingsConfigFile);

			if (SettingsFile.DocumentElement == null)
				throw new Exception("No Root Document Element defined.");

			SettingsHeadNode = SettingsFile.DocumentElement;

			LoadChannels();
			LoadSettings();
		}

		private void LoadChannels()
		{
			var channelNodes = SettingsHeadNode.SelectNodes(string.Format("/{0}/{1}",RootNodeName, ChannelXmlNodeName));

			if (channelNodes != null)
			{
				foreach (XmlNode channelNode in channelNodes)
				{
					string channel = channelNode.InnerText;

					channel = IrcBot.MakeValidChannel(channel);

					Channels.Add(channel);
				}
			}
		}

		public string GetSetting(string name)
		{
			if (!Settings.ContainsKey(name))
			{
				throw new KeyNotFoundException(string.Format("No {0} setting defined in Settings.xml", name));
			}

			return Settings[name];
		}

		public void SetSetting(string name, string value)
		{
			Settings[name] = value;
			Bot.LogInformation(string.Format("Value for setting {0} is now {1}",name,value));
		}

		public KeyValuePair<string, string> GetSettingFromXml(string name)
		{
			XmlNode settingNode = SettingsHeadNode.SelectSingleNode(string.Format("/{0}/{1}@name={2}",RootNodeName,SettingNodeName,name));

			if(settingNode == null)
			{
				throw new KeyNotFoundException(string.Format("Could not find setting with name {0}",name));
			}

			var settingValue = settingNode.Attributes["value"];
			if(settingValue == null)
			{
				throw new Exception("Setting was found but no value was in it...this is sad news.");
			}

			return new KeyValuePair<string, string>(name, settingValue.Value);
		}

		public void LoadSettings()
		{
			XmlNodeList settings = SettingsHeadNode.SelectNodes(string.Format("/{0}/{1}", RootNodeName, SettingNodeName));

			if (settings != null)
			{
				foreach (XmlNode settingNode in settings)
				{
					var settingName = settingNode.Attributes["name"];
					var settingValue = settingNode.Attributes["value"];

					if (settingName != null && settingValue != null)
					{
						if (!Settings.ContainsKey(settingName.Name))
						{
							Settings.Add(settingName.Value, settingValue.Value);
						}
						else
						{
							Bot.LogWarning(
								string.Format("Tried to add another value for setting with name {0}. Only the first dude got loaded.",
								              settingName.Name));
						}
					}
				}
			}
		}

		public XmlNode GetPluginSettings(string name)
		{
			return SettingsFile.SelectSingleNode(string.Format(@"/{0}/{1}[@{2}=""{3}""]", RootNodeName, PluginsXmlNodeName, PluginNameAttributeName ,name));
		}

		public XmlNodeList GetPluginsNodes()
		{
			return SettingsFile.SelectNodes(string.Format("/{0}/{1}", RootNodeName, PluginsXmlNodeName));
		}
	}
}
