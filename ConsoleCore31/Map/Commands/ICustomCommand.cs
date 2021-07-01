namespace ConsoleCore31.Map.Commands
{
	public interface ICustomCommand
	{
		string Key { get; }

		bool CanExecute();

		void Execute(object args);
	}

	public interface ICustomCommand<in TInput, out TOutput> : ICustomCommand
	{
		TOutput Execute(TInput args);
	}
}