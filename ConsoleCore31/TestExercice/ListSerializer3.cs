//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using BackEndTestMd.Models;
//using BackEndTestMd.Services;

//namespace BackEndTestMd.Impl
//{
//	internal class ListSerializer3 : IListSerializer
//	{
//		public async Task Serialize(ListNode head, Stream s)
//		{
//			Dictionary<int, ListNode> items = new Dictionary<int, ListNode>();
//			Dictionary<int, ListNode> randomLinks = new Dictionary<int, ListNode>();

//			int c = 0;
//			long total = 0;
//			byte[] minusOne = BitConverter.GetBytes(-1);

//			int processorsCount = Environment.ProcessorCount;

//			ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

//			var totalItemsCount = 0;

//			var tasks =
//			Enumerable.Range(0, processorsCount).Select(i =>
//			{
//				ListNode mainHeadLocal = head;
//				int itemsCount = 0;
//				int localItemsChanged = 0;

//				return
//				Task.Run(async () =>
//				{
//					while (mainHeadLocal != null)
//					{
//						if (localItemsChanged * processorsCount + i == itemsCount)
//						{
//							rwl.EnterWriteLock();
//							try
//							{
//								s.Seek(Convert.ToInt64(itemsCount) * 8 + 4, SeekOrigin.Begin);
//								s.Write(BitConverter.GetBytes(Encoding.Unicode.GetBytes(mainHeadLocal.Data).Length));
//								s.Write(minusOne,0, 4);
//								items.Add(itemsCount, mainHeadLocal);
//								if (mainHeadLocal.Random != null)
//								{
//									randomLinks.Add(itemsCount, mainHeadLocal.Random);
//								}
//							}
//							finally
//							{
//								rwl.ExitWriteLock();
//							}

//							localItemsChanged++;
//						}

//						itemsCount++;
//						mainHeadLocal = mainHeadLocal.Next;
//					}

//					if (i == 0)
//					{
//						totalItemsCount = itemsCount;
//					}
//				});
//			});

//			await Task.WhenAll(tasks);

//			//Console.WriteLine("1st stage has completed");

//			s.Seek(0, SeekOrigin.Begin);
//			await s.WriteAsync(BitConverter.GetBytes(totalItemsCount), 0, 4);

//			ListNode localHead = head;
//			int totalItems = 0;
//			long offset = 0;
//			byte[] buff = new byte[4];

//			while (localHead != null)
//			{
//				s.Seek(totalItems * 8 + 4, SeekOrigin.Begin);
//				await s.ReadAsync(buff, 0, 4);
//				var dataSize = BitConverter.ToInt32(buff);
//				s.Seek(totalItemsCount * 8 + offset + 4, SeekOrigin.Begin);
//				var dataBytes = Encoding.Unicode.GetBytes(localHead.Data);
//				if (dataSize > 28)
//				{
//					Console.WriteLine(dataSize);
//				}
//				await s.WriteAsync(dataBytes, 0, dataSize);
//				offset += dataSize;
//				totalItems++;
//				localHead = localHead.Next;
//			}

//			//Console.WriteLine("2nd stage has completed");

//			//tasks =
//			//	Enumerable.Range(0, processorsCount).Select(i =>
//			//	{
//			//		int totalItemsLocal = 0;
//			//		int localItemsChanged = 0;

//			//		return
//			//			Task.Run(async () =>
//			//			{
//			//				foreach (var randomLink in randomLinks)
//			//				{
//			//					if (localItemsChanged * processorsCount + i == totalItemsLocal)
//			//					{
//			//						int val =
//			//							items.First(f => f.Value == randomLink.Value).Key;

//			//						rwl.EnterWriteLock();
//			//						try
//			//						{
//			//							s.Seek(totalItemsLocal * 8 + 8, SeekOrigin.Begin);
//			//							s.Write(BitConverter.GetBytes(val), 0, 4);
//			//						}
//			//						finally
//			//						{
//			//							rwl.ExitWriteLock();
//			//						}

//			//						localItemsChanged++;
//			//					}

//			//					totalItemsLocal++;
//			//				}
//			//			});
//			//	});

//			tasks =
//				Enumerable.Range(0, processorsCount).Select(i =>
//				{
//					ListNode mainHeadLocal = head;
//					ListNode searchLocalHead;
//					int totalItemsLocal = 0;
//					int localItemsChanged = 0;

//					return
//						Task.Run(async () =>
//						{
//							while (mainHeadLocal != null)
//							{
//								if (localItemsChanged * processorsCount + i == totalItemsLocal)
//								{
//									if (mainHeadLocal.Random != null)
//									{
//										int c = 0;
//										searchLocalHead = head;
//										while (searchLocalHead != null)
//										{
//											if (searchLocalHead == mainHeadLocal.Random)
//											{
//												rwl.EnterWriteLock();
//												try
//												{
//													s.Seek(totalItemsLocal * 8 + 8, SeekOrigin.Begin);
//													await s.WriteAsync(BitConverter.GetBytes(c), 0, 4);
//												}
//												finally
//												{
//													rwl.ExitWriteLock();
//												}

//												break;
//											}

//											searchLocalHead = searchLocalHead.Next;
//											c++;
//										}
//									}

//									localItemsChanged++;
//								}

//								totalItemsLocal++;
//								mainHeadLocal = mainHeadLocal.Next;
//							}
//						});
//				});

//			await Task.WhenAll(tasks);

//		}

//		public async Task<ListNode> Deserialize(Stream s)
//		{
//			s.Seek(0, SeekOrigin.Begin);

//			ListNode curLn =
//				null;

//			ListNode head =
//				null;

//			Dictionary<int, int> randomLinks =
//				new Dictionary<int, int>();

//			Dictionary<int, ListNode> items =
//				new Dictionary<int, ListNode>();

//			byte[] buff = new byte[4];
//			await s.ReadAsync(buff, 0, 4);
//			int length = BitConverter.ToInt32(buff);

//			int num = 0;
//			long offset = 0;

//			while (num < length)
//			{
//				s.Seek(num * 8 + 4, SeekOrigin.Begin);
//				await s.ReadAsync(buff, 0, 4);
//				int size = BitConverter.ToInt32(buff);

//				await s.ReadAsync(buff, 0, 4);
//				int rnd = BitConverter.ToInt32(buff);

//				if (rnd > -1)
//				{
//					randomLinks.Add(num, rnd);
//				}

//				s.Seek(length * 8 + 4 + offset, SeekOrigin.Begin);
//				var dataBytes = new byte[size];
//				await s.ReadAsync(dataBytes, 0, size);
//				string data = Encoding.Unicode.GetString(dataBytes);
//				offset += size;

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
//							Previous = curLn,
//							Data = data,
//						};

//					curLn.Next =
//						ln;

//					curLn = ln;
//				}

//				items.Add(num, curLn);

//				num++;
//			}

//			foreach (var randomNodeLink in randomLinks)
//			{
//				items[randomNodeLink.Key].Random =
//					items[randomNodeLink.Value];
//			}

//			items.Clear();
//			randomLinks.Clear();

//			//var sourceArray = randomLinks.Keys.OrderBy(o => o).ToArray();
//			//num = 0;
//			//int curSourceIndex = 0;
//			//curLn = head;
//			//while (curLn != null && curSourceIndex < sourceArray.Length)
//			//{
//			//	if (sourceArray[curSourceIndex] == num)
//			//	{
//			//		var curTargetNode = head;
//			//		int numInt = 0;
//			//		int curTargetIndex = 0;
//			//		var targetArray = randomLinks.Where(w => w.Key == num).Select(s => s.Value).OrderBy(o => o).ToArray();

//			//		while (curTargetNode != null && curTargetIndex < targetArray.Length)
//			//		{
//			//			if (targetArray[curTargetIndex] == numInt)
//			//			{
//			//				curLn.Random = curTargetNode;
//			//				curTargetIndex++;
//			//			}

//			//			curTargetNode =
//			//				curTargetNode.Next;
//			//			numInt++;
//			//		}

//			//		curSourceIndex++;
//			//	}

//			//	curLn =
//			//		curLn.Next;
//			//	num++;
//			//}

//			return head;
//		}

//		public Task<ListNode> DeepCopy(ListNode head)
//		{
//			throw new System.NotImplementedException();
//		}
//	}
//}