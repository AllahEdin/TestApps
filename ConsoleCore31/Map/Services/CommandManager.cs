using System.Collections.Generic;
using ConsoleCore31.Map.Commands;

namespace ConsoleCore31.Map.Services
{
	public static class CommandManager
	{
		private static Dictionary<string, ICustomCommand> _commands =
			new Dictionary<string, ICustomCommand>();

		public static void Register(ICustomCommand command)
		{
			if (!_commands.TryGetValue(command.Key, out var res))
			{
				_commands.Add(command.Key,command);
			}
		}

		public static ICustomCommand GetCommand(string key) =>
			_commands[key];
	}
}