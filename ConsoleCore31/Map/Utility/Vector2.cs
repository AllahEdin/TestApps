using System;

namespace ConsoleCore31.Map.Utility
{
	public struct Vector2<T>
	{
		public T X;

		public T Y;

	}


	public static class Vector2Extensions
	{
		public static Vector2<float> Normalized(this Vector2<float> source)
		{
			return new Vector2<float>()
			{
				X = source.X / (float)Math.Sqrt(source.X * source.X + source.Y * source.Y),
				Y = source.Y / (float)Math.Sqrt(source.X * source.X + source.Y * source.Y),
			};
		}
	}


}