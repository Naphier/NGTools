using System.Collections.Generic;
using System.Text;
using System;

namespace NG.Diagnostics
{
	public class DiagnosticRecorder
	{
		private Dictionary<string, StopwatchRecorder> timers = new Dictionary<string, StopwatchRecorder>();
		private Dictionary<string, Counter> counters = new Dictionary<string, Counter>();

		public void StartTimer(string name)
		{
			if (!timers.ContainsKey(name))
				timers.Add(name, new StopwatchRecorder());

			timers[name].Start();
		}

		public void StopTimer(string name)
		{
			if (timers.ContainsKey(name))
				timers[name].Stop();
		}
		
		public void RecordCount(string name, int value, bool shouldCaptureZero = false)
		{
			if (!counters.ContainsKey(name))
				counters.Add(name, new Counter());

			counters[name].Record(value, shouldCaptureZero);
		}

		public StopwatchRecorder GetStopwatch(string name)
		{
			if (timers.ContainsKey(name))
				return timers[name];
			else
				throw new ArgumentOutOfRangeException(
					string.Format("No StopWatchRecorder with name: '{0}' found.", name));
		}

		public Counter GetCounter(string name)
		{
			if (counters.ContainsKey(name))
				return counters[name];
			else
				throw new ArgumentOutOfRangeException(
					string.Format("No Counter with name: '{0}' found.", name));
		}


		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			if (timers.Count > 0)
				sb.AppendLine("Timers (ms)");

			foreach (var item in timers)
			{
				sb.AppendFormat("{0}\nLast: {1}\tMin: {2}\tMax: {3}\tAvg: {4:N1}\tTotal: {5}\n",
					item.Key, item.Value.LastElapsedMilliseconds, 
					item.Value.MinimumElapsedMilliseconds, 
					item.Value.MaximumElapsedMilliseconds, 
					item.Value.AverageElapsedMilliseconds, 
					item.Value.TotalElapsedMilliseconds);
			}

			if (counters.Count > 0)
				sb.AppendLine("Counters");

			foreach (var item in counters)
			{
				sb.AppendFormat("{0}\nLast: {1}\tMin: {2}\tMax: {3}\tAvg: {4}\tSum: {5}\n",
					item.Key, item.Value.Last, item.Value.Min, item.Value.Max, item.Value.Average, item.Value.Sum);
			}

			return sb.ToString();
		}
	}
}
