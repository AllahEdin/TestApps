using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormFramework48.Services;

namespace WinFormFramework48
{
	public partial class Form1 : Form
	{
		private RaiseEventService _raiseEventService;

		public Form1()
		{
			_raiseEventService = new RaiseEventService(); 
			InitializeComponent();
		}

		private void StartButton_Click(object sender, EventArgs e)
		{
			Console.WriteLine(TaskScheduler.FromCurrentSynchronizationContext().Id);

			var sc = SynchronizationContext.Current;

			_raiseEventService.RaiseEvent += val =>
			{
				sc.Post(_ => TargetTextBox.Text = val, null);
			};

			var task = Task.Factory.StartNew(() =>_raiseEventService.RaiseAfter(TimeSpan.FromSeconds(3)));

			task.Wait();
		}
	}
}
