using System;
using System.Threading;
using ConsoleCore31.MultiThreading.UtilityEntities;

namespace ConsoleCore31.MultiThreading
{
	public class MonitorTest
	{
		private readonly KekStruct _kek;
		private readonly KekClass _kek2;


		public MonitorTest()
		{
			_kek =
				new KekStruct()
				{
					Id = 1
				};

			_kek2 =
				new KekClass();

			for (int i = 0; i < 5; i++)
			{
				Thread t = new Thread(() => Start(_kek2));
				t.Start();
			}
		}

		public void Start(IKek kek)
		{
			bool acquiredLock = false;
			try
			{
				Monitor.Enter(kek, ref acquiredLock);
				var x = 1;
				for (int i = 1; i < 9; i++)
				{
					Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: {x}");
					x++;
					Thread.Sleep(1000);
				}
			}
			finally
			{
				if (acquiredLock) Monitor.Exit(kek);
			}
		}
	}
}