using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormFramework48.Services;
using Timer = System.Threading.Timer;

namespace WinFormFramework48
{
	public partial class Form1 : Form
	{
		private int _rowHeight = 30;
		private object locker = new object();
		private RaiseEventService _raiseEventService;
		private Graphics _g;

		public Form1()
		{
			_raiseEventService = new RaiseEventService(); 
			InitializeComponent();
		}

		private void StartButton_Click(object sender, EventArgs e)
		{
			_g = DrawingPanel.CreateGraphics();

			var context =
				SynchronizationContext.Current;

			Dictionary<int, int> threadIdRowMap = new Dictionary<int, int>();
			Dictionary<int, PipelineEventArgs> rowBlockMap = new Dictionary<int, PipelineEventArgs>();
			Dictionary<int, Color> rowColorMap = new Dictionary<int, Color>();

			new TaskFactory().StartNew(async () =>
			{
				var b = new BlockPipelineTest();

				b.OnStatusChangedEvent += args =>
				{
					lock (locker)
					{
						if (!threadIdRowMap.TryGetValue(args.ThreadId, out var val))
						{
							int id = threadIdRowMap.Count;
							threadIdRowMap.Add(args.ThreadId, id);
							rowBlockMap.Add(id, args);
							var r = new Random();
							rowColorMap.Add(id, Color.FromArgb(255, r.Next(255), r.Next(255), r.Next(255)));

							context.Post(cb =>
							{
								int y = DrawingPanel.Location.Y + (id + 1) * _rowHeight;
								Label l = new Label();
								l.Location = new Point( 0,  y);
								l.Text = id.ToString();
								l.BringToFront();
								this.Controls.Add(l);
							}, null);
						}
						else
						{
							rowBlockMap[threadIdRowMap[args.ThreadId]] = args;
						}
					}
				};

				b.Start();
			});


			new TaskFactory().StartNew(async () =>
			{
				Stopwatch sw = new Stopwatch();

				TimeSpan checkSpan = TimeSpan.FromSeconds(0.1);

				double msKoef = 1d / 100;

				sw.Start();

				int start = -1;

				Timer timer = new Timer(st =>
					{
						lock (locker)
						{
							foreach (var row in rowBlockMap)
							{
								if (start == -1)
								{
									start = (int) sw.Elapsed.TotalMilliseconds;
								}

								if (row.Value.Status == PipelineStatus.Start)
								{
									int y = (row.Key + 1) * _rowHeight;
									int x = 30 + (int) ((sw.Elapsed.TotalMilliseconds - start) * msKoef);

									context.Post(cb =>
									{
										_g.FillRectangle(new SolidBrush(rowColorMap[row.Key]),
											x,
											y,
											(int) (checkSpan.TotalMilliseconds * msKoef),
											5);
									}, null);
								}
							}

						}
					},
					null,
					TimeSpan.FromSeconds(1),
					checkSpan);
			});

		}

		private void Raise()
		{
			Console.WriteLine(TaskScheduler.FromCurrentSynchronizationContext().Id);

			var sc = SynchronizationContext.Current;

			_raiseEventService.RaiseEvent += val =>
			{
				sc.Post(_ => TargetTextBox.Text = val, null);
			};

			Task.Factory.StartNew(() => _raiseEventService.RaiseAfter(TimeSpan.FromSeconds(3))).ConfigureAwait(false);
		}

		private async void GetValue()
		{

			Console.WriteLine(SynchronizationContext.Current);

			Task<int> task =
				Task.Factory.StartNew(() =>
				{
					Console.WriteLine(SynchronizationContext.Current);

					Task.Delay(TimeSpan.FromSeconds(2)).Wait();
					return 3;
				});

			var res =
				await task.ConfigureAwait(false);

			Console.WriteLine(SynchronizationContext.Current);

			TargetTextBox.Text = res.ToString();
		}

	}
}

internal class CusomShceduler : TaskScheduler
{
	private List<Task> _tasks = new List<Task>();

	protected override void QueueTask(Task task)
	{
		_tasks.Add(task);
	}

	protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
	{

		while (task.IsCompleted == false)
		{
			
		}

		return true;
	}

	protected override IEnumerable<Task> GetScheduledTasks()
	{
		return _tasks;
	}
}

