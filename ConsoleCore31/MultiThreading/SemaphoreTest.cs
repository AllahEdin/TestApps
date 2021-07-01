using System;
using System.Threading;

namespace ConsoleCore31.MultiThreading
{
	public class SemaphoreTest
	{
		private int x = 0;

		public SemaphoreTest()
		{
			SemaphoreSlim sem = new SemaphoreSlim(2, 2);
			object locker = new object();

			for (int i = 0; i < 5; i++)
			{
				Thread t = new Thread(() => Start(sem, locker));
				t.Start();
			}
		}

		public async void Start(SemaphoreSlim sem, object locker)
		{
			
			try
			{
				await sem.WaitAsync();

				for (int i = 1; i < 9; i++)
				{

					lock (locker)
					{
						Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: {x} -> {x + 1}");
						x++;
					}
					Thread.Sleep(1000);
				}

			}
			finally
			{
				sem.Release();
			}
		}
	}
}