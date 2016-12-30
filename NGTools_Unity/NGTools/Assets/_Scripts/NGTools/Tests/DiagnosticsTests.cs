using UnityEngine;
using NG.Diagnostics;
using UnityEngine.Assertions;
using System.Collections;

public class DiagnosticsTests : MonoBehaviour
{

	void Start()
	{
		TestCounter();

		StartCoroutine(TestTimer());
	}

	void TestCounter()
	{
		DiagnosticRecorder dr = new DiagnosticRecorder();

		// Test 1
		int iterations = 1000;
		int value = 2;
		for (int i = 0; i < iterations; i++)
		{
			dr.RecordCount("counter1", value);
		}

		int expectedLast = value;
		int expectedSum = iterations * value;
		int expectedAverage = value;
		int expectedMin = value;

		Assert.AreEqual(expectedLast, dr.GetCounter("counter1").Last, "Last fails.");
		Assert.AreEqual(expectedSum, dr.GetCounter("counter1").Sum, "Sum fails.");
		Assert.AreEqual(expectedAverage, dr.GetCounter("counter1").Average, "Average fails.");
		Assert.AreEqual(expectedMin, dr.GetCounter("counter1").Min, "Min fails.");
		Assert.AreEqual(expectedMin, dr.GetCounter("counter1").Max, "Max fails.");


		// Test 2
		dr.RecordCount("counter0", 0);
		dr.RecordCount("counter0", 1);
		Assert.AreNotEqual(0, dr.GetCounter("counter0").Min, "Min included zero and shouldn't have.");


		// Test 3
		dr.RecordCount("counter2", 10);
		dr.RecordCount("counter2", 20);
		expectedLast = 20;
		expectedSum = 20 + 10;
		expectedMin = 10;
		expectedAverage = 15;
		int expectedMax = 20;

		Assert.AreEqual(expectedLast, dr.GetCounter("counter2").Last, "Last fails.");
		Assert.AreEqual(expectedSum, dr.GetCounter("counter2").Sum, "Sum fails.");
		Assert.AreEqual(expectedAverage, dr.GetCounter("counter2").Average, "Average fails.");
		Assert.AreEqual(expectedMin, dr.GetCounter("counter2").Min, "Min fails.");
		Assert.AreEqual(expectedMax, dr.GetCounter("counter2").Max, "Max fails.");

		// Test 4
		dr.RecordCount("counter3", 0, true);
		dr.RecordCount("counter3", 10);
		expectedMin = 0;
		expectedSum = 10;
		expectedAverage = 5;
		Assert.AreEqual(expectedSum, dr.GetCounter("counter3").Sum, "Sum fails.");
		Assert.AreEqual(expectedAverage, dr.GetCounter("counter3").Average, "Average fails.");
		Assert.AreEqual(expectedMin, dr.GetCounter("counter3").Min, "Min fails.");

	}


	IEnumerator TestTimer()
	{
		yield return new WaitForSeconds(1);

		DiagnosticRecorder dr = new DiagnosticRecorder();

		float duration = 10;
		float timer = 0;
		while (timer < duration)
		{
			timer += Time.deltaTime;
			dr.StartTimer("timer1");
			yield return new WaitForEndOfFrame();
			dr.StopTimer("timer1");
		}

		Debug.Log(dr.ToString());
	}
}

