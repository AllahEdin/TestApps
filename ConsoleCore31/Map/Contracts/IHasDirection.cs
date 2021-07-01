using ConsoleCore31.Map.Utility;

namespace ConsoleCore31.Map.Contracts
{
	public interface IHasDirection
	{
		Vector2<float> Direction { get; set; }
	}
}