using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TriviaBot
{
	public interface IBotCommand
	{
		void Execute(string[] args);
		string GetHelpSyntax();
		string GetCommandName();
	}

	public class HelpCommand : IBotCommand
	{
		public void Execute(string[] args)
		{
			
		}

		public string GetHelpSyntax()
		{
			return string.Empty;
		}

		public string GetCommandName()
		{
			return "help";
		}
	}
}
