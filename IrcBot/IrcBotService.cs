using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IrcBot.Commands;
using Meebey.SmartIrc4net;

namespace IrcBot
{
	public class IrcBot
	{
		public string FilePath;
		public List<IBotCommand> Commands;
		public IBotInitializer Initializer;
		public IrcClient IrcClient;
		public IrcBotSettings Settings;
		public string SecretPassword;
		public List<IBotCommand> AdminCommands;
		public PluginManager PluginManager;

		public AdminRepository AdminUserRepository;

		public IrcBot(IBotInitializer initializer)
		{
			Initializer = initializer;

			FilePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
			if (!FilePath.EndsWith("\\")) FilePath += "\\";
			Settings = new IrcBotSettings(FilePath + "Settings.xml", this);

			IrcClient = new IrcClient
			{
				SendDelay = 50,
				AutoRetry = true,
				ActiveChannelSyncing = true, 
				AutoReconnect = true
			};

			LoadCommands();

			SetupEventHandlers();
		}

		protected void SetupEventHandlers()
		{
			IrcClient.OnConnected += OnConnected;
			IrcClient.OnChannelMessage += OnChannelMessage;
			IrcClient.OnQueryMessage += OnQueryMessage;
			IrcClient.OnChannelAction += OnChannelAction;
			IrcClient.OnChannelNotice += OnChannelNotice;
			IrcClient.OnChannelModeChange += OnChannelModeChange;
			IrcClient.OnWho += OnWho;
		}

		#region Logging

		public void LogInformation(string message)
		{
			Initializer.LogInformation(message);
		}

		public void LogWarning(string message)
		{
			Initializer.LogWarning(message);
		}

		public void LogError(string message)
		{
			Initializer.LogError(message);
		}

		#endregion

		protected void LoadCommands()
		{
			var showPluginsCommand = new ShowPluginsCommand(this);

			Commands = new List<IBotCommand>
			    {
			        new HelpCommand(this),
			        new AdminListCommand(this)
			    };
			AdminCommands = new List<IBotCommand>
			    {
			        new JoinCommand(this),
			        new NickCommand(this),
			        new QuitCommand(this),
			        new PartCommand(this),
			        new OpCommand(this),
			        new UnloadPluginCommand(this),
			        new AddAdminCommand(this),
			        new RemoveAdminCommand(this),
					new SayCommand(this),
			        showPluginsCommand
				};
		}

		#region Event Handlers

		public void OnConnected(object sender, EventArgs eventArgs)
		{

		}

		private void OnChannelAction(object sender, ActionEventArgs eventArgs)
		{
			foreach (BotPlugin plugin in PluginManager.Plugins)
			{
				plugin.OnChannelAction(eventArgs);
			}
		}

		private void OnChannelNotice(object sender, IrcEventArgs ircEventArgs)
		{
			foreach (BotPlugin plugin in PluginManager.Plugins)
			{
				plugin.OnChannelNotice(ircEventArgs);
			}
		}

		public void OnQueryMessage(object sender, IrcEventArgs ircEventArgs)
		{
			if(AdminUserRepository.AddUserIfPasswordIsCorrect(ircEventArgs))
			{
				SendMessage("You now have admin status, dave.", ircEventArgs.Data.Nick);
			}

			foreach (BotPlugin plugin in PluginManager.Plugins)
			{
				try
				{
					plugin.QueryMessageHandler(sender, ircEventArgs);
				} catch(Exception e)
				{
					PluginManager.UnloadPlugin(plugin, "OnQueryMessage", e);
				}
			}

			ExecuteAllCommands(ircEventArgs);
		}

		public void OnChannelMessage(object sender, IrcEventArgs ircEventArgs)
		{
			foreach (BotPlugin plugin in PluginManager.Plugins)
			{
				try
				{
					plugin.ChannelMessageHandler(sender, ircEventArgs);
				} catch(Exception e)
				{
					PluginManager.UnloadPlugin(plugin, "OnChannelMessage", e);
				}
			}

			ExecuteAllCommands(ircEventArgs);
		}

		private void OnWho(object sender, WhoEventArgs args)
		{
			foreach (var plugin in PluginManager.Plugins)
			{
				try
				{
					plugin.OnWho(args);
				} catch (Exception exception)
				{
					PluginManager.UnloadPlugin(plugin, "OnWho", exception);
				}
			}
		}

		private void OnChannelModeChange(object sender, IrcEventArgs args)
		{
			foreach (var plugin in PluginManager.Plugins)
			{
				try
				{
					plugin.OnChannelModeChange(args);
				}
				catch (Exception exception)
				{
					PluginManager.UnloadPlugin(plugin, "OnChannelModeChange", exception);
				}
			}
		}

		public void Initialize()
		{
			IrcClient.Connect(Settings.Host, Settings.Port);
			LogInformation(string.Format("IrcBot Connecting to {0}:{1}", Settings.Host, Settings.Port));

			if (IrcClient.IsConnected)
				IrcClient.Login(Settings.BotName, Settings.RealBotName);

			foreach (string channel in Settings.Channels)
			{
				string channelname = MakeValidChannel(channel);
				LogInformation(string.Format("Joining {0}", channelname));
				IrcClient.RfcJoin(channelname);
			}

			PluginManager = new PluginManager(string.Format("{0}{1}", FilePath, Settings.PluginFolder), this);
			PluginManager.LoadPluginsBlind();

			foreach (BotPlugin plugin in PluginManager.Plugins)
				plugin.LoadCommands();

			AdminUserRepository = new AdminRepository(Settings.AdminPass, FilePath);
			AdminUserRepository.LoadAdmins();
			SendMessageToAdmins("IrcBot is online and you are admin.");

			IrcClient.Listen();
		}

		public void TearDown()
		{
			foreach (BotPlugin plugin in PluginManager.Plugins)
				plugin.TearDown();

			AdminUserRepository.SaveAdmins();

			IrcClient.Disconnect();
		}

		public void ExecuteCommands(IrcEventArgs ircEventArgs, List<IBotCommand> commands)
		{
			foreach (IBotCommand command in commands)
			{
				bool shouldExecuteCommand = false;

				try
				{
					shouldExecuteCommand = command.ShouldExecuteCommand(ircEventArgs);
				}
				catch (Exception e)
				{
					SendMessage(string.Format("Command threw and exception: {0}", e.Message), ircEventArgs.Data.Nick);
				}

				if (shouldExecuteCommand)
				{
					try
					{
						command.Execute(ircEventArgs);
					}
					catch (Exception e)
					{
						SendMessage(e.Message, ircEventArgs.Data.Nick);

						PrintHelpSyntaxForCommand(ircEventArgs, command);
					}
				}
			}
		}

		public void ExecuteCommands(List<BotPlugin> plugins, IrcEventArgs args)
		{
			foreach (var plugin in plugins)
			{
				ExecuteCommands(args, plugin.Commands);

				if (AdminUserRepository.IsAdminUser(args.Data.From))
				{
					ExecuteCommands(args, plugin.AdminCommands);
				}
			}
		}

		public void ExecuteAllCommands(IrcEventArgs ircEventArgs)
		{
			ExecuteCommands(ircEventArgs, Commands);

			if (AdminUserRepository.IsAdminUser(ircEventArgs.Data.From))
			{
				ExecuteCommands(ircEventArgs, AdminCommands);
			}

			ExecuteCommands(PluginManager.Plugins, ircEventArgs);
		}

		#endregion

		public void PrintHelpSyntaxForCommand(IrcEventArgs ircEventArgs, IBotCommand command)
		{
			foreach (var helpsyntax in command.GetHelpSyntax(ircEventArgs))
			{
				SendMessage(helpsyntax, ircEventArgs.Data.Nick);
			}
		}

		public void SendMessage(string message, string destination)
		{
			IrcClient.SendMessage(SendType.Message, destination, message);

			foreach(BotPlugin plugin in PluginManager.Plugins)
			{
				plugin.OnSendMessage(Settings.BotName, destination, message);
			}
		}

		public void SendMessage(string message, string[] destinations)
		{
			foreach(var destination in destinations)
			{
				SendMessage(message, destination);
			}
		}

		public void SendMessageToAdmins(string message)
		{
			var adminlist = AdminUserRepository.GetAdmins();
			var admins = new string[adminlist.Count];

			int i = 0;
			foreach(var admin in adminlist)
			{
				admins[i] = admin.Nick;
				i++;
			}

			SendMessage(message, admins);
		}

		public static string MakeValidChannel(string channel)
		{
			return channel.StartsWith("#") ? channel : "#" + channel;
		}
	}
}
