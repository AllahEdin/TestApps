//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BackEndTestMd.Models;
//using BackEndTestMd.Services;

//namespace BackEndTestMd.Impl
//{
//	internal class ListSerializer : IListSerializer
//	{
//		public async Task Serialize(ListNode head, Stream s)
//		{
//			Dictionary<int, ListNode> randoms = new Dictionary<int, ListNode>();
//			Dictionary<int, long> offsets = new Dictionary<int, long>();

//			int c = 0;
//			long total = 0;

//			while (head != null)
//			{
//				//4 number, 1 hasRandom, 4 numberOfRandom, 4 dataSize, dataSize data

//				var num = BitConverter.GetBytes(c);
//				var data = Encoding.ASCII.GetBytes(head.Data);
//				int length = data.Length + (head.Random != null ? 5 : 1);
//				var size = BitConverter.GetBytes(length);
//				bool hasRandom = head.Random != null;

//				s.Seek(total, SeekOrigin.Begin);
//				await s.WriteAsync(num, 0, 4);
//				await s.WriteAsync(size, 0, 4);
//				await s.WriteAsync(BitConverter.GetBytes(hasRandom ? 1 : 0), 0, 1);
//				if (hasRandom)
//				{
//					s.Seek(4, SeekOrigin.Current);
//					randoms.Add(c,head.Random);
//				}
//				await s.WriteAsync(data, 0, data.Length);

//				offsets.Add(c, total);

//				List<int> deleteList =
//					new List<int>();
//				foreach (var random in randoms)
//				{
//					if (head == random.Value)
//					{
//						s.Seek(offsets[random.Key] + 9, SeekOrigin.Begin);
//						await s.WriteAsync(num, 0, 4);
//						deleteList.Add(random.Key);
//					}
//				}
//				foreach (var key in deleteList)
//				{
//					randoms.Remove(key);
//				}

//				total += 8 + length;
//				c++;
//				head = head.Next;
//			}
			
//		}

//		public async Task<ListNode> Deserialize(Stream s)
//		{
//			s.Seek(0, SeekOrigin.Begin);

//			ListNode curLn =
//					null;

//			ListNode head =
//				null;

//			long totalRead = 0;

//			Dictionary<int, ListNode> dictionary =
//				new Dictionary<int, ListNode>();

//			Dictionary<int, int> randomLinks =
//				new Dictionary<int, int>();

//			while (totalRead < s.Length)
//			{
//				byte[] buff = new byte[4];
//				await s.ReadAsync(buff, 0, 4);
//				int num = BitConverter.ToInt32(buff);

//				await s.ReadAsync(buff, 0, 4);
//				int size = BitConverter.ToInt32(buff);

//				byte[] hasRandomBytes = new byte[1];
//				await s.ReadAsync(hasRandomBytes, 0, 1);
//				string data = "";

//				if (hasRandomBytes[0] == 1)
//				{
//					await s.ReadAsync(buff, 0, 4);
//					var randomNum = BitConverter.ToInt32(buff);
//					randomLinks.Add(num, randomNum);

//					byte[] dataBytes =
//						new byte[size - 5];

//					await s.ReadAsync(dataBytes, 0, size - 5);
//					data = Encoding.ASCII.GetString(dataBytes);
//				}
//				else
//				{
//					byte[] dataBytes =
//						new byte[size - 1];

//					await s.ReadAsync(dataBytes, 0, size - 1);
//					data = Encoding.ASCII.GetString(dataBytes);
//				}

//				totalRead += 8 + size;

//				if (head == null)
//				{
//					head = new ListNode()
//					{
//						Data = data,
//					};

//					curLn = head;
//				}
//				else
//				{
//					var ln =
//						new ListNode()
//						{
//							Data = data,
//							Previous = curLn
//						};

//					curLn.Next =
//						ln;

//					curLn = ln;
//				}

//				dictionary.Add(num, curLn);
//			}

//			foreach (var link in randomLinks)
//			{
//				dictionary[link.Key].Random = dictionary[link.Value];
//			}

//			return head;

//		}

//		public Task<ListNode> DeepCopy(ListNode head)
//		{
//			throw new System.NotImplementedException();
//		}
//	}
//}