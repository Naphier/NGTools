using System;
using System.Collections.Generic;
using System.Linq;

namespace NG.Diagnostics
{
	/// <summary>
	/// A diagnostic counter that maintains the Min, Max, and Average counts fed in to it.
	/// </summary>
	public class Counter
	{
		private List<int> values = new List<int>();
		private List<KeyValuePair<int, int>> sumCountsPairs = new List<KeyValuePair<int, int>>();
		private int bufferSize = int.MaxValue - 1;

		/// <summary>
		/// The maximum count.
		/// </summary>
		public int Max { get; private set; }

		/// <summary>
		/// The minimum count.
		/// </summary>
		public int Min { get; private set; }


		/// <summary>
		/// The average count.
		/// </summary>
		public int Average
		{
			get
			{
				long count = (long)values.Count;

				long sum = 0;
				if (count > 0)
					sum = values.Sum();

				foreach (var item in sumCountsPairs)
				{
					long maxCheck = long.MaxValue - count;
					if ((uint)item.Value < maxCheck)
						count += (uint)item.Value;
					else
						break;
					sum += item.Key;
				}

				if (count > 0)
					return (int)(sum / count);
				else
					return 0;
			}
		}

		/// <summary>
		/// The last count added to this counter.
		/// </summary>
		public int Last
		{
			get
			{
				if (values.Count > 0)
					return values.Last();
				else
					return -1;
			}
		}

		public long Sum { get; private set; }

		/// <summary>
		/// Constructor ensures initial values for Min and Max.
		/// </summary>
		public Counter()
		{
			Max = int.MinValue;
			Min = int.MaxValue;
		}


		/// <summary>
		/// Call this to capture a count.
		/// </summary>
		/// <param name="value">The count to record.</param>
		/// <param name="shouldCaptureZero">Should this capture a count of zero? Default is false.</param>
		public void Record(int value, bool shouldCaptureZero = false)
		{
			if (!shouldCaptureZero && value == 0)
				return;

			Max = Math.Max(value, Max);
			Min = Math.Min(value, Min);

			if (values.Count >= bufferSize)
			{
				sumCountsPairs.Add(new KeyValuePair<int, int>(values.Sum(), values.Count));
				values.Clear();
			}

			values.Add(value);
			Sum += value; 
		}
	}
}
