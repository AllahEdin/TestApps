using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BackEndTest.Impl;
using BackEndTest.Models;
using BackEndTest.Services;

namespace BackEndTest
{
	class Program
	{
		private static readonly Random Random = new Random();

		static async Task Main(string[] args)
		{
			IListSerializer serializer = new ListSerializer();

			var ln =
				GenerateRandom(10000000,0, 10);

			using (var str =
				new MemoryStream())
			{
				Stopwatch sw = new Stopwatch();

				sw.Start();

				await serializer.Serialize(ln, str);

				var elapsed = sw.Elapsed;
				Console.WriteLine($"Serialization time : {elapsed.TotalSeconds}");

				var res =
					await serializer.Deserialize(str);

				var elapsed2 = sw.Elapsed;
				Console.WriteLine($"Deserialization time : {sw.Elapsed.TotalSeconds - elapsed.TotalSeconds}");


				var deepCopy =
					await serializer.DeepCopy(ln);

				Console.WriteLine($"Copying time : {sw.Elapsed.TotalSeconds - elapsed2.TotalSeconds}");

				Console.WriteLine($"Serialization : {Equals(ln, res)}");
				Console.WriteLine($"Copy : {Equals(ln, deepCopy)}");

				//Out(ln);
				//Out(res);
				//Out(deepCopy);

			}

			Console.ReadKey();
		}

		/// <summary>
		/// Generates list of <see cref="nodeCount"/> nodes with random payload of length between <see cref="minStrLength"/> and <see cref="maxStrLength"/>
		/// </summary>
		/// <returns></returns>
		static ListNode GenerateRandom(int nodeCount, int minStrLength, int maxStrLength)
		{
			maxStrLength++;
			Dictionary<int, ListNode> items =
				new Dictionary<int, ListNode>();

			ListNode head = new ListNode()
			{
				Data = RandomString(Random.Next(minStrLength, maxStrLength)),
			};
			items.Add(0, head);

			ListNode cur = head;

			for (int i = 0; i < nodeCount - 1; i++)
			{
				ListNode ln =
					new ListNode()
					{
						Previous = cur,
						Data = RandomString(Random.Next(minStrLength, maxStrLength)),
					};

				cur.Next = ln;

				cur = ln;
				items.Add(i + 1, cur);
			}

			cur = head;
			int c = 0;
			while (cur != null)
			{
				bool has =
					//false;
					true;
					//c % 3 == 0;
					//random.Next(2) == 1;

				if (has)
				{
					cur.Random = items[Random.Next(nodeCount)];
				}
				cur = cur.Next;
				c++;
			}

			return head;
		}

		static string RandomString(int length)
		{
			const string chars = "АБВГДЕЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[Random.Next(s.Length)]).ToArray());
		}

		static void Out(ListNode head)
		{
			while (head != null)
			{
				Console.WriteLine(head.Data);

				if (head.Random != null)
				{
					Console.WriteLine($"-> {head.Random.Data}");
				}
				

				head = head.Next;
			}
		}
		
		/// <summary>
		/// Checks equality of two  
		/// </summary>
		static bool Equals(ListNode head1, ListNode head2)
		{
			while (head1 != null)
			{
				if (head1.Previous != null)
				{
					if (head2.Previous == null)
					{
						return false;
					}

					if (!head1.Previous.Data.Equals(head2.Previous.Data))
					{
						return false;
					}
				}
				else
				{
					if (head2.Previous != null)
					{
						return false;
					}
				}

				if (!head1.Data.Equals(head2.Data))
				{
					return false;
				}

				if (head1.Random != null)
				{
					if (head2.Random == null)
					{
						return false;
					}

					if (!head1.Random.Data.Equals(head2.Random.Data))
					{
						return false;
					}
				}
				else
				{
					if (head2.Random != null)
					{
						return false;
					}
				}

				head1 = head1.Next;
				head2 = head2.Next;
			}

			return true;
		}
	}
}
