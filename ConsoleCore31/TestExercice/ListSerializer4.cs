//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using BackEndTestMd.Models;
//using BackEndTestMd.Services;

//namespace BackEndTestMd.Impl
//{
//	internal class ListSerializer4 : IListSerializer
//	{
//		public async Task Serialize(ListNode head, Stream s)
//		{
//			Stopwatch sw = new Stopwatch();
//			sw.Start();

//			Dictionary<int, ListNode> items = new Dictionary<int, ListNode>();
//			Dictionary<int, int> itemHashCodeItemNumber = new Dictionary<int, int>();
//			Dictionary<int, List<int>> itemHashCodeItemNumbers = new Dictionary<int, List<int>>();
//			Dictionary<int, int> itemNumberRandomItemHashCodes = new Dictionary<int, int>();

//			int c = 0;
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
//							var hash = mainHeadLocal.GetHashCode();

//							rwl.EnterWriteLock();
//							try
//							{
//								items.Add(itemsCount, mainHeadLocal);
//								if (!itemHashCodeItemNumber.TryAdd(hash, itemsCount))
//								{
//									if (itemHashCodeItemNumbers.TryGetValue(hash, out var hashList))
//									{
//										hashList.Add(itemsCount);
//									}
//									else
//									{
//										var list =
//											new List<int>() {itemHashCodeItemNumber[hash], itemsCount };
//										itemHashCodeItemNumbers.Add(hash, list);
//									}

//									itemHashCodeItemNumber[hash] = -1;
//								}

//								s.Seek(Convert.ToInt64(itemsCount) * 8 + 4, SeekOrigin.Begin);
//								s.Write(BitConverter.GetBytes(Encoding.Unicode.GetBytes(mainHeadLocal.Data).Length));
//								if (mainHeadLocal.Random != null)
//								{
//									itemNumberRandomItemHashCodes.Add(itemsCount, mainHeadLocal.Random.GetHashCode());
//								}
//								else
//								{
//									s.Write(minusOne, 0, 4);
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

//			Console.WriteLine($"Elapsed 1st stage = {sw.Elapsed.TotalSeconds}");
//			var el1 = sw.Elapsed;

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
//				await s.WriteAsync(dataBytes, 0, dataSize);
//				offset += dataSize;
//				totalItems++;
//				localHead = localHead.Next;
//			}

//			Console.WriteLine($"Elapsed 2st stage = {(sw.Elapsed - el1).TotalSeconds}");
//			el1 = sw.Elapsed;

//			tasks =
//				Enumerable.Range(0, processorsCount).Select(i =>
//				{
//					return
//						Task.Run(async () =>
//						{
//							int totalItemsLocal = 0;
//							int localItemsChanged = 0;
//							foreach (var key in itemNumberRandomItemHashCodes.Keys)
//							{
//								if (localItemsChanged * processorsCount + i == totalItemsLocal)
//								{
//									rwl.EnterWriteLock();
//									try
//									{
//										var number = itemHashCodeItemNumber[itemNumberRandomItemHashCodes[key]];
//										s.Seek(key * 8 + 8, SeekOrigin.Begin);

//										if (number != -1)
//										{
//											s.Write(BitConverter.GetBytes(number), 0, 4);
//										}
//										else
//										{
//											var numbers = itemHashCodeItemNumbers[itemNumberRandomItemHashCodes[key]];

//											if (numbers.Count == 1)
//											{
//												s.Write(BitConverter.GetBytes(numbers.First()), 0, 4);
//											}
//											else
//											{
//												foreach (var possibleNumber in numbers)
//												{
//													if (items[key].Random == items[possibleNumber])
//													{
//														s.Write(BitConverter.GetBytes(possibleNumber), 0, 4);
//														break;
//													}
//												}
//											}
//										}
										
//									}
//									finally
//									{
//										rwl.ExitWriteLock();
//									}


//									localItemsChanged++;
//								}

//								totalItemsLocal++;
//							}
//						});
//				});

//			await Task.WhenAll(tasks);

//			Console.WriteLine($"Elapsed 3rd stage = {(sw.Elapsed - el1).TotalSeconds}");
//			sw.Stop();
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
			
//			return head;
//		}

//		public async Task<ListNode> DeepCopy(ListNode head)
//		{
//			ListNode newListHead = null;
//			ListNode newListCurrentNode = newListHead;

//			Dictionary<int, ListNode> items = new Dictionary<int, ListNode>();
//			Dictionary<int, ListNode> newItems = new Dictionary<int, ListNode>();
//			Dictionary<int, int> itemHashCodeItemNumber = new Dictionary<int, int>();
//			Dictionary<int, List<int>> itemHashCodeItemNumbers = new Dictionary<int, List<int>>();
//			Dictionary<int, int> itemNumberRandomItemHashCodes = new Dictionary<int, int>();

//			int processorsCount = Environment.ProcessorCount;

//			ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

//			var totalItemsCount = 0;

//			var tasks =
//				Enumerable.Range(0, processorsCount).Select(i =>
//				{
//					ListNode mainHeadLocal = head;
//					int itemsCount = 0;
//					int localItemsChanged = 0;

//					return
//						Task.Run(async () =>
//						{
//							while (mainHeadLocal != null)
//							{
//								if (i == 0)
//								{
//									var ln = new ListNode()
//									{
//										Data = mainHeadLocal.Data
//									};

//									if (newListCurrentNode != null)
//									{
//										newListCurrentNode.Next = ln;
//										ln.Previous = newListCurrentNode;
//									}
//									else
//									{
//										newListHead = ln;
//									}

//									newItems.Add(itemsCount, ln);
//									newListCurrentNode = ln;
//								}

//								if (localItemsChanged * processorsCount + i == itemsCount)
//								{
//									rwl.EnterWriteLock();
//									try
//									{
//										items.Add(itemsCount, mainHeadLocal);

//										var hash = mainHeadLocal.GetHashCode();

//										if (!itemHashCodeItemNumber.TryAdd(hash, itemsCount))
//										{
//											if (itemHashCodeItemNumbers.TryGetValue(hash, out var hashList))
//											{
//												hashList.Add(itemsCount);
//											}
//											else
//											{
//												var list =
//													new List<int>() { itemHashCodeItemNumber[hash], itemsCount };
//												itemHashCodeItemNumbers.Add(hash, list);
//											}

//											itemHashCodeItemNumber[hash] = -1;
//										}

//										if (mainHeadLocal.Random != null)
//										{
//											itemNumberRandomItemHashCodes.Add(itemsCount, mainHeadLocal.Random.GetHashCode());
//										}
//									}
//									finally
//									{
//										rwl.ExitWriteLock();
//									}

//									localItemsChanged++;
//								}

//								itemsCount++;
//								mainHeadLocal = mainHeadLocal.Next;
//							}

//							if (i == 0)
//							{
//								totalItemsCount = itemsCount;
//							}
//						});
//				});

//			await Task.WhenAll(tasks);

//			tasks =
//				Enumerable.Range(0, processorsCount).Select(i =>
//				{
//					return
//						Task.Run(async () =>
//						{
//							int totalItemsLocal = 0;
//							int localItemsChanged = 0;
//							foreach (var key in itemNumberRandomItemHashCodes.Keys)
//							{
//								if (localItemsChanged * processorsCount + i == totalItemsLocal)
//								{
//									rwl.EnterWriteLock();
//									try
//									{
//										var number = itemHashCodeItemNumber[itemNumberRandomItemHashCodes[key]];

//										if (number != -1)
//										{
//											newItems[key].Random = newItems[number];
//										}
//										else
//										{
//											var numbers = itemHashCodeItemNumbers[itemNumberRandomItemHashCodes[key]];

//											if (numbers.Count == 1)
//											{
//												newItems[key].Random = newItems[numbers.First()];
//											}
//											else
//											{
//												foreach (var possibleNumber in numbers)
//												{
//													if (items[key].Random == items[possibleNumber])
//													{
//														newItems[key].Random = newItems[possibleNumber];
//														break;
//													}
//												}
//											}
//										}

//									}
//									finally
//									{
//										rwl.ExitWriteLock();
//									}


//									localItemsChanged++;
//								}

//								totalItemsLocal++;
//							}
//						});
//				});

//			await Task.WhenAll(tasks);

//			return newListHead;
//		}
//	}
//}