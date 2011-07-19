using System.Xml;
using IrcBot.Commands;
using IrcBot.Plugins.AutoOp.Commands;
using Meebey.SmartIrc4net;

namespace IrcBot.Plugins.AutoOp
{
	public class AutoOpPlugin : BotPlugin
	{
		public IrcBot Bot;
		public OpCommand OpCommand;
		public DeOpCommand DeOpCommand;
		public OpRepository Repository;

		public override void Initialize (IrcBot ircBot, XmlNode pluginSettings)
		{
			Bot = ircBot;
			OpCommand = new OpCommand(Bot);
			DeOpCommand = new DeOpCommand(Bot);
			Repository = new OpRepository(Bot.FilePath);

			Repository.LoadUsers();
			OpUsers();
		}

		public override void TearDown()
		{
			Repository.SaveUsers();
		}

		private void OpUsers()
		{
			var users = Repository.GetOpUsers();

			foreach(var user in users)
			{
				OpCommand.Execute(user.Channel,user.FullName);
			}
		}

		public override void LoadCommands()
		{
			AdminCommands.Add(new AddOpCommand(this));
			//AdminCommands.Add(new ClearAutoOpsCommand(this));
			AdminCommands.Add(new RemoveAutoOpCommand(this));
			AdminCommands.Add(new GetAutoOpsCommand(this));
		}

		public override void OnWho(WhoEventArgs whoEventArgs)
		{
			var channel = whoEventArgs.WhoInfo.Channel;
			var fullName = string.Format("{0}!{1}@{2}", whoEventArgs.WhoInfo.Nick, whoEventArgs.WhoInfo.Ident, whoEventArgs.WhoInfo.Host);
			var nick = whoEventArgs.WhoInfo.Nick;

			if(Repository.IsOpUser(channel, fullName))
			{
				OpCommand.Execute(channel, nick);
			}
		}
	}
}
