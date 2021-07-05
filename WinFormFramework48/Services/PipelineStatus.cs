using System;

namespace WinFormFramework48.Services
{
	public class PipelineEventArgs
	{
		public int ThreadId { get; set; }

		public int TaskContentId { get; set; }

		public string Description { get; set; }

		public PipelineStatus Status { get; set; }
	}

	public enum PipelineStatus
	{
		Start,
		Finished
	}

}