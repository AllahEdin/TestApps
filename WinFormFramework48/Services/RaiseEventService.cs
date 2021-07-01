using System;
using System.Threading.Tasks;

namespace WinFormFramework48.Services
{
	internal class RaiseEventService
	{
		public Action<string> RaiseEvent;

		public async Task RaiseAfter(TimeSpan time)
		{
			await Task.Delay(time);
			RaiseEvent?.Invoke(new Random().Next(100).ToString());
		}
	}
}