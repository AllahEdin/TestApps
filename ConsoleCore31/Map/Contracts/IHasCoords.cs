using ConsoleCore31.Map.Utility;

namespace ConsoleCore31.Map.Contracts
{
	public interface IHasCoords
	{
		Vector2<float> Coords { get; set; }
	}
}