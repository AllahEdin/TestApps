using System;
using ConsoleCore31.Map.Commands;
using ConsoleCore31.Map.Contracts;
using ConsoleCore31.Map.Map;
using ConsoleCore31.Map.Services;
using ConsoleCore31.Map.Utility;

namespace ConsoleCore31.Map.Impl
{
	public abstract class MovableBase : MapObject, IMovable
	{
		public MovableBase(Vector2<float> coords, Vector2<float> direction)
		{
			Coords = coords;
			Direction = direction;
		}

		public Vector2<float> Coords { get; set; }

		public Vector2<float> Direction { get; set; }

		public void ChangeDirection(float angle)
			=> CommandManager.GetCommand("RotateCommand").Execute(angle);
		
		public bool CanMove(float distance)
			=> CommandManager.GetCommand("MoveCommand").CanExecute();

		public void Move(float distance)
			=> CommandManager.GetCommand("MoveCommand").Execute(distance);
	}

	public class RotateCommand : ICustomCommand<RotateCommand.RotateCommandInputModel, object>
	{
		public class RotateCommandInputModel
		{
			public ICanChangeDirection RotatableObject { get; set; }

			public float Angle { get; set; }
		}

		public string Key => nameof(RotateCommand);

		public object Execute(RotateCommandInputModel args)
		{
			var norm = args.RotatableObject.Direction.Normalized();

			args.RotatableObject.Direction = new Vector2<float>()
			{
				X = (float)(Math.Cos(args.Angle) * norm.X - Math.Sin(args.Angle) * norm.Y),
				Y = (float)(Math.Sin(args.Angle) * norm.X + Math.Cos(args.Angle) * norm.Y),
			};

			return true;
		}

		
		public bool CanExecute()
		{
			return true;
		}

		public void Execute(object args)
			=> Execute(args as RotateCommandInputModel);
	}

	public class MoveCommand : ICustomCommand<MoveCommand.MoveCommandInputModel, object>
	{
		public class MoveCommandInputModel
		{
			public IMovable Movable { get; set; }

			public float Distance { get; set; }
		}

		public string Key => nameof(MoveCommand);

		public object Execute(MoveCommand.MoveCommandInputModel args)
		{
			var norm = args.Movable.Direction.Normalized();

			args.Movable.Coords = new Vector2<float>()
			{
				X = args.Movable.Coords.X + norm.X * args.Distance,
				Y = args.Movable.Coords.Y + norm.Y * args.Distance
			};

			return true;
		}


		public bool CanExecute()
		{
			return true;
		}

		public void Execute(object args)
			=> Execute(args as MoveCommand.MoveCommandInputModel);
	}


}