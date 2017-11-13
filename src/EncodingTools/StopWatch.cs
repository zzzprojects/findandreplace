using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace FindAndReplace
{
	/// <summary>
	/// Used for streamlining code execution and finding bottlenecks.
	/// </summary>
	/// <remarks>Use XUtil.StopWatches collection to create and start/stop new stopwatches.</remarks>
	/// <example>
	/// <code>
	///	
	///	private void DoStuff(XStopWatch stopWatch)
	///	{
	///		//Code Section #1
	///		...
	///		
	///		stopWatch.Start();
	///		//Code Section #2
	///		...
	///		stopWatch.Stop();
	///		
	///		//Code Section #3
	///		...
	///	}
	///	
	///	// Create a stopwatch to add up the total amount of time 
	///	// it takes to execute code in 'Code Section #2'
	///	XStopWatch stopWatch=new XStopWatch();
	///				
	///	for (int nCount=1; nCount &lt; 1000; nCount++)
	///	{
	///		DoStuff(stopWatch);
	///	}
	///		
	/// </code>
	/// </example>
	[Serializable]
	public class StopWatch
	{
		private static ConcurrentDictionary<string, StopWatch> _stopWatches;
		private DateTime _endTime;
		private DateTime _startTime;

		private bool _started;
		private int _stopCount;
		private TimeSpan _timeSpan;

		/// <summary>Instantiate a new instance of XStopWatch.</summary>
		public StopWatch()
		{
			Reset();
		}

		#region Class Properties

		public TimeSpan ElapsedSpan
		{
			get
			{
				if (!IsStarted())
					throw new InvalidOperationException("Stopwatch needs to be running");

				TimeSpan timeSpan = _timeSpan.Add(DateTime.Now.Subtract(_startTime));
				return timeSpan;
			}
		}

		/// <summary>Total number of milliseconds that elapsed since start.  This does not effect the stopwatch.</summary>
		public int ElapsedMilliseconds
		{
			get
			{
				if (!IsStarted())
					throw new InvalidOperationException("Stopwatch needs to be running");

				TimeSpan timeSpan = _timeSpan.Add(DateTime.Now.Subtract(_startTime));

				return (int)timeSpan.TotalMilliseconds;
			}
		}

		/// <summary>Total number of milliseconds that elapsed between every call to Start() method and corresponding call to Stop() method.</summary>
		public int Milliseconds
		{
			get { return (int)_timeSpan.TotalMilliseconds; }
			set { }
		}

		/// <summary>Divides milliseconds by the number of stops done thus far.</summary>
		/// <remarks>Assumes one stop has been made if none have occurred.</remarks>
		public int AverageMilliseconds
		{
			get
			{
				int stops = _stopCount;
				if (_stopCount < 1)
					stops = 1;
				return (Milliseconds / stops);
			}
			set { }
		}

		/// <summary>Total number of times Stop() has been called.</summary>
		public int StopCount
		{
			get { return _stopCount; }
			set { _stopCount = value; }
		}

		/// <summary>The time span representation of time elapsed on this watch.  Updated when Stop() is called.</summary>
		public TimeSpan Span
		{
			get { return _timeSpan; }
			set { _timeSpan = value; }
		}

		/// <summary>The time span representation of time elapsed on this watch.  Updated when Stop() is called.</summary>
		public static ConcurrentDictionary<string, StopWatch> Collection
		{
			get { return _stopWatches;}
			set { _stopWatches = value; }
		}

		#endregion

		/// <summary>Resets the stopwatch, setting internal elapsed counter to 0.</summary>
		public void Reset()
		{
			_started = false;
			_stopCount = 0;
			_timeSpan = new TimeSpan(0);
		}

		/// <summary>Indicates if stopwatch has been started.</summary>
		/// <returns>True if stopwatch was started and false, otherwise. </returns>
		public bool IsStarted()
		{
			return _started;
		}

		/// <summary>Starts the stopwatch.</summary>
		public void Start()
		{
			_startTime = DateTime.Now;
			_started = true;
		}

		/// <summary>Stops the stopwatch.</summary>
		/// <remarks>Adds elapsed time from calling Start() to the StopWatch internal counter.</remarks>
		public void Stop()
		{
			if (!_started)
				throw new InvalidOperationException("Timer could not be stopped.  Start() function must be called before Stop().");

			_endTime = DateTime.Now;

			_timeSpan = _timeSpan.Add(_endTime.Subtract(_startTime));

			_stopCount++;

			_started = false;
		}

		//Static collection related methods
		public static void Start(string key, bool reset = false)
		{
			if (_stopWatches == null)
				_stopWatches = new ConcurrentDictionary<string, StopWatch>();

			if (!_stopWatches.ContainsKey(key))
				_stopWatches[key] = new StopWatch();

			if (reset)
				_stopWatches[key].Reset();

			_stopWatches[key].Start();
		}

		public static void Stop(string key)
		{
			_stopWatches[key].Stop();
		}

		public static void PrintCollection(int? totalMilliseconds)
		{
			var sb = new StringBuilder();

			PrintCollection(totalMilliseconds, sb);

			Console.Write(sb.ToString());
		}

	

		public static void PrintCollection(int? totalMilliseconds, StringBuilder sb)
		{
			sb.AppendLine("Print Stop Watches: " + DateTime.Now);
			StopWatch stopWatch;
			string line;
			decimal secs;
			decimal percent;

			foreach (string key in _stopWatches.Keys)
			{
				stopWatch = _stopWatches[key];
				secs = Math.Round((((decimal)stopWatch.Milliseconds) / 1000), 1);

				line = key + ": " + secs + " secs, " + stopWatch.Milliseconds + " millisecs";

				decimal avgDuration = Math.Round((((decimal)stopWatch.Milliseconds) / stopWatch.StopCount), 1);
				line += ", " + stopWatch.StopCount + " stops, Avg Duration " + avgDuration + " millisecs";

				if (totalMilliseconds != null)
				{
					percent = Math.Round((stopWatch.Milliseconds / (decimal)totalMilliseconds.Value) * 100, 1);
					line += ", " + percent + "%";
				}
				sb.AppendLine(line);
			}

			sb.AppendLine("Total: " + totalMilliseconds + " millisecs");
		}
	}
}
