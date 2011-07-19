using System;
using System.Collections.Generic;
using System.Linq;
using Meebey.SmartIrc4net;

namespace IrcBot
{
	public interface IBotCommand
	{
		void Execute(IrcEventArgs args);
		List<string> GetHelpSyntax(IrcEventArgs args);
		bool ShouldExecuteCommand(IrcEventArgs args);
	}

	public abstract class BaseBotCommand : IBotCommand
	{
		public bool IsMatchingCase;
		public List<string> FirstMatchingWord;
		public List<string> SecondMatchingWord;
		public List<ReceiveType> EligibleReceiveTypes;

		public abstract void Execute(IrcEventArgs args);
		public abstract List<string> GetHelpSyntax(IrcEventArgs args);
		
		public virtual bool ShouldExecuteCommand(IrcEventArgs args)
		{
			if(EligibleReceiveTypes != null && EligibleReceiveTypes.Count > 0)
			{
				if(!EligibleReceiveTypes.Contains(args.Data.Type))
				{
					return false;
				}
			}

			if (args.Data.MessageArray.Length > 0 && !WordsetMatches(FirstMatchingWord, args.Data.MessageArray[0], false))
				return false;

			if (args.Data.MessageArray.Length > 1 && !WordsetMatches(SecondMatchingWord, args.Data.MessageArray[1], false))
				return false;

			return true;
		}

		protected bool WordsetMatches(List<string> wordSet, string wordGiven, bool matchingCase)
		{
			if (wordSet != null && wordSet.Count > 0 && !string.IsNullOrEmpty(wordGiven))
			{
				return wordSet.Any(word => string.Equals(word, wordGiven, IsMatchingCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase));
			}

			return true;
		}
	}
}
