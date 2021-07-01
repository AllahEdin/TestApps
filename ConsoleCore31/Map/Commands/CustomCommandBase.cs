namespace ConsoleCore31.Map.Commands
{
	public abstract class CustomCommandBase<TInput,TOutput> : ICustomCommand<TInput, TOutput>
	{
		public abstract string Key { get; }

		public abstract TOutput Execute(TInput args);

		public abstract bool CanExecute();

		public void Execute(object args)
			=> Execute((TInput)args);
	}

	public abstract class CustomCommandBase : ICustomCommand
	{
		public abstract string Key { get; }

		public abstract bool CanExecute();

		public abstract void Execute(object args);
	}
}