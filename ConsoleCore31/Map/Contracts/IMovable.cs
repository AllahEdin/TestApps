namespace ConsoleCore31.Map.Contracts
{
	public interface IMovable : IHasCoords, ICanChangeDirection
	{
		bool CanMove(float distance);

		void Move(float distance);
	}
}