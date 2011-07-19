using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace IrcBot
{
	public class PluginManager
	{
		private const string ClassAttributeName = "class";
		public string PluginDirectory;
		public List<BotPlugin> Plugins;
		protected IrcBot Bot;

		public PluginManager(string pluginDirectory, IrcBot bot)
		{
			Plugins = new List<BotPlugin>();
			PluginDirectory = pluginDirectory;
			Bot = bot;
		}

		public void LoadPluginsBlind()
		{
			var filesInPluginDirectory = Directory.GetFiles(PluginDirectory, "*.dll");

			foreach (var file in filesInPluginDirectory)
			{
				LoadPluginsFromFile(file);
			}
		}

		public bool LoadPluginsFromFile(string assemblyFile)
		{
			try
			{
				Assembly assembly = Assembly.LoadFrom(assemblyFile);

				foreach (var type in assembly.GetTypes())
				{
					if (type.BaseType != null
						&& type.BaseType == typeof(BotPlugin))
					{
						BotPlugin plugin = (BotPlugin)assembly.CreateInstance(type.FullName);
						plugin.Initialize(Bot, Bot.Settings.GetPluginSettings(plugin.Name));
						Plugins.Add(plugin);

						Bot.LogInformation(string.Format("Loaded Plugin {0}", plugin.Name));
					}
				}
			}
			catch (Exception e)
			{
				Bot.LogError(string.Format("Could not load Plugin in file {0}. Exception: {1}\n{2}\n{3}", assemblyFile, e.Message, e.StackTrace, e.InnerException));
				return false;
			}

			return true;
		}

		public void UnloadPlugin(BotPlugin plugin, string method, Exception exception)
		{
			if (Bot != null)
			{
				Bot.LogError(string.Format("Error in {2} for plugin {0}. Exception {1}\n{3}", plugin.Name, exception.Message, method,
				                           exception.StackTrace));
			}
			UnloadPlugin(plugin);
		}

		public bool UnloadPlugin(BotPlugin plugin)
		{
			try
			{
				plugin.TearDown();
			}
			catch (Exception e)
			{
				if (Bot != null)
				{
					Bot.LogError(string.Format("Could not teardown plugin {0}. Exception {1}\n{2}", plugin.Name, e.Message,
					                           e.StackTrace));
				}
				return false;
			}
			return Plugins.Remove(plugin);
		}

		public bool UnloadPlugin(XmlNode pluginInfo)
		{
			string className = string.Empty;
			if (pluginInfo.Attributes[ClassAttributeName] != null)
				className = pluginInfo.Attributes[ClassAttributeName].Value;

			return UnloadPlugin(Plugins.Find(x => x.GetType().FullName.ToString() == className));
		}

		public bool UnloadPlugin(string name)
		{
			XmlNode pluginSettings = Bot.Settings.GetPluginSettings(name);

			if (pluginSettings == null)
			{
				throw new Exception(string.Format("Could not find the settings for plugin {0}", name));
			}

			return UnloadPlugin(pluginSettings);
		}

		public void ExecuteMethodForPlugins()
		{
			
		}
	}
}
