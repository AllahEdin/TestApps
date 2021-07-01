using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleCore31.Encryption;
using ConsoleCore31.MultiThreading;

namespace ConsoleCore31
{
	interface IStruct
	{
		int Id { get; set; }
	}

	struct MyStruct : IStruct
	{
		public int Id { get; set; }
	}

	class MyClass : IStruct
	{
		public int Id { get; set; }

		public virtual void A()
		{
			Console.WriteLine("A");
		}
	}

	class MyClass2 : MyClass
	{
		public int Id { get; set; }

		public override void A()
		{
			Console.WriteLine("B");
		}
	}


	class Program
	{
		private static MyClass _cl;

		static void Main(string[] args)
		{

			RsaEncryptionTest semaphoreTest = new RsaEncryptionTest();
			
			Console.ReadKey();
		}

	
	}
}
