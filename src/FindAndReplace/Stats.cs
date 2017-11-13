using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindAndReplace
{
	public enum Status
	{
		Processing,
		Completed,
		Cancelled
	}
	
	public class Stats
	{
		public class StatsFiles
		{
			public int Total { get; set; }

			public int Processed { get; set; }

			public int Binary { get; set; }

			public int WithMatches { get; set; }

			public int WithoutMatches { get; set; }

			public int FailedToRead { get; set; }

			public int FailedToWrite { get; set; }
		}

		public class StatsMatches
		{
			public int Found { get; set; }

			public int Replaced { get; set; }
		}

		public class StatsTime
		{
			public TimeSpan Passed { get; set; }

			public TimeSpan Remaining { get; set; }
		}

		public StatsFiles Files { get; set; }

		public StatsMatches Matches { get; set; }

		public StatsTime Time { get; set; }

		public Stats()
		{
			Files = new StatsFiles();

			Matches = new StatsMatches();

			Time = new StatsTime();
		}

		public void UpdateTime(DateTime startTime, DateTime startTimeProcessingFiles)
		{
			DateTime now = DateTime.Now;
			Time.Passed = now.Subtract(startTime);
			
			//Use startTimeProcessingFiles to figure out remaining time
			TimeSpan passedProcessingFiles = now.Subtract(startTimeProcessingFiles);

			double passedSeconds = passedProcessingFiles.TotalSeconds;

			int remainingFiles = Files.Total - Files.Processed;
			var remainingSeconds = (passedSeconds / Files.Processed) * remainingFiles;

			Time.Remaining = TimeSpan.FromSeconds(remainingSeconds);
		}
	}
}
