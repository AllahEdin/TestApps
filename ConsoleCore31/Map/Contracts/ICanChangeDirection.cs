namespace ConsoleCore31.Map.Contracts
{
	public interface ICanChangeDirection : IHasDirection
	{
		void ChangeDirection(float angle);
	}
}