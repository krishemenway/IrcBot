using System;
using System.Collections.Generic;
using Meebey.SmartIrc4net;

namespace IrcBot.Commands
{
	public class UnloadPluginCommand : BaseBotCommand
	{
		public IrcBot Bot;

		public UnloadPluginCommand(IrcBot bot)
		{
			Bot = bot;
			FirstMatchingWord = new List<string> { "UnloadPlugin" };
		}

		public override void Execute(IrcEventArgs args)
		{
			if(args.Data.MessageArray.Length < 2)
				throw new Exception("Must supply at least one Plugin to unload.");

			for (int i = 1; i < args.Data.MessageArray.Length; i++)
			{
				Bot.PluginManager.UnloadPlugin(args.Data.MessageArray[i]);
				Bot.SendMessage(string.Format("Unloaded {0}", args.Data.MessageArray[i]), args.Data.Nick);
			}
		}

		public override List<string> GetHelpSyntax(IrcEventArgs args)
		{
			return new List<string> {"UnloadPlugin [PluginName] [PluginName] ... - Unload at least one plugin by supplying name specified in Settings.xml"};
		}
	}
}
