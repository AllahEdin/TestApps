using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ConsoleCore31.MultiThreading.DataFlow
{
	public class BlockPipelineTest
	{
		private readonly BufferBlock<int> _bufferBlock;
		private readonly TransformBlock<int, Color> _transformBlock;
		private readonly ActionBlock<Color> _actionBlock;

		public BlockPipelineTest()
		{
			_bufferBlock = new BufferBlock<int>();

			_transformBlock = new TransformBlock<int, Color>(async i =>
			{
				//Console.WriteLine($"start  {i}: Thread: {Thread.CurrentThread.ManagedThreadId.ToString()}");
				await Task.Delay(TimeSpan.FromSeconds(1));
				var c = Color.FromArgb(i, 255, 0, 0);
				Console.WriteLine($"finish {i}: Thread: {Thread.CurrentThread.ManagedThreadId.ToString()}: Input {_transformBlock.InputCount} Output {_transformBlock.OutputCount}");
				return c;
			}, new ExecutionDataflowBlockOptions()
			{
				BoundedCapacity = 10,
				//MaxDegreeOfParallelism = 4,
			});

			_actionBlock = new ActionBlock<Color>(async c =>
			{
				Console.WriteLine($"==================================> {c.A}: Thread: {Thread.CurrentThread.ManagedThreadId.ToString()} Input {_actionBlock.InputCount}");
				//Console.WriteLine($"START  {c.A}: Thread: {Thread.CurrentThread.ManagedThreadId.ToString()}");
				await Task.Delay(TimeSpan.FromSeconds(2));
				
			}, new ExecutionDataflowBlockOptions()
			{
				BoundedCapacity = 5,
				MaxDegreeOfParallelism = 2
			});

			_bufferBlock.LinkTo(_transformBlock, new DataflowLinkOptions());
			_bufferBlock.Completion.ContinueWith(task => _transformBlock.Complete());
			_transformBlock.LinkTo(_actionBlock);
			_transformBlock.Completion.ContinueWith(task => _actionBlock.Complete());
			

			new TaskFactory().StartNew(() =>
			{
				Stopwatch sw = new Stopwatch();
				//sw.Start();
				TimeSpan elapsed = TimeSpan.Zero;
				int count = 0;
				while (count < 100)
				{
					//if (sw.Elapsed - elapsed > TimeSpan.FromSeconds(5))
					{
						elapsed = sw.Elapsed;
						_bufferBlock.Post(count);
						count++;
					}
				}
			});
		}
	}
}