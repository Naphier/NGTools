using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace NG.Diagnostics
{
	/// <summary>
	/// Derives from System.Diagnostics.Stopwatch
	/// Adds the ability to stop and restart the stopwatch multiple times
	/// and record the total time elapsed, minimum & maximum time elapsed, 
	/// and the average time elapsed from all calls to this stopwatch.
	/// </summary>
	public class StopwatchRecorder : Stopwatch
	{
		private List<long> capturedMilliseconds = new List<long>();
		private List<KeyValuePair<long, int>> sumCountsPairs = new List<KeyValuePair<long, int>>();
		private int bufferSize = int.MaxValue - 1;

		/// <summary>
		/// The total time recorded by this stopwatch even after restarts.
		/// </summary>
		public ulong TotalElapsedMilliseconds { get; private set; }
		
		/// <summary>
		/// This minimum amount of time elapsed during a run of this stopwatch.
		/// </summary>
		public long MinimumElapsedMilliseconds { get; private set; }
		
		/// <summary>
		/// This maximum amount of time elapsed during a run of this stopwatch.
		/// </summary>
		public long MaximumElapsedMilliseconds { get; private set; }

		/// <summary>
		/// The average time elapsed during a run of this stopwatch.
		/// </summary>
		public double AverageElapsedMilliseconds
		{
			get
			{
				ulong count = (uint)capturedMilliseconds.Count;
				double sum = 0;

				if (count > 0)
					sum = capturedMilliseconds.Sum();

				foreach (var item in sumCountsPairs)
				{
					ulong maxCheck = ulong.MaxValue - count;
					if ((uint)item.Value < maxCheck)
						count += (uint)item.Value;
					else
						break;
					sum += item.Key;
				}

				if (count > 0)
					return (sum / count);
				else
					return 0;
			}
		}

		/// <summary>
		/// The amount of time elapsed in the last run of this stopwatch.
		/// </summary>
		public long LastElapsedMilliseconds
		{
			get
			{
				if (capturedMilliseconds.Count > 0)
					return capturedMilliseconds.Last();
				else
					return -1;
			}
		}


		public new void Stop()
		{
			base.Stop();

			if (MinimumElapsedMilliseconds == 0)
				MinimumElapsedMilliseconds = ElapsedMilliseconds;
			else
				MinimumElapsedMilliseconds = Math.Min(ElapsedMilliseconds, MinimumElapsedMilliseconds);

			MaximumElapsedMilliseconds = Math.Max(ElapsedMilliseconds, MaximumElapsedMilliseconds);

			if (capturedMilliseconds.Count >= bufferSize)
			{
				sumCountsPairs.Add(
					new KeyValuePair<long, int>(
						capturedMilliseconds.Sum(), capturedMilliseconds.Count));
				capturedMilliseconds.Clear();
			}
			capturedMilliseconds.Add(ElapsedMilliseconds);
			TotalElapsedMilliseconds += (ulong)ElapsedMilliseconds;

			Reset();
		}
	}
}
