using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace TestPropertyMethodAccessSpeed
{
	class Program
	{
		static void Main(string[] args)
		{
			var testClass = new TestClass();
			var maxCount = 10000000;

			var test1 = new List<long>();
			var test2 = new List<long>();
			var test3 = new List<long>();

			var stopWatch = new Stopwatch();

			for (var runs = 0; runs < 10; runs++)
			{
				Console.WriteLine("Test run: {0}", runs);

				stopWatch.Reset();
				stopWatch.Start();
				for (var i = 0; i < maxCount; i++)
				{
					var temp = testClass.PropertyString;
					temp = i.ToString(CultureInfo.InvariantCulture);
					testClass.PropertyString = temp;
				}
				stopWatch.Stop();
				test1.Add(stopWatch.ElapsedMilliseconds);
				Console.WriteLine("Property: {0}ms", (float)stopWatch.ElapsedMilliseconds / (float)maxCount);

				stopWatch.Reset();
				stopWatch.Start();
				for (var i = 0; i < maxCount; i++)
				{
					var temp = testClass.PropertyStringBackingStore;
					temp = i.ToString(CultureInfo.InvariantCulture);
					testClass.PropertyStringBackingStore = temp;
				}
				stopWatch.Stop();
				test2.Add(stopWatch.ElapsedMilliseconds);
				Console.WriteLine("Property with backingstore: {0}ms", (float)stopWatch.ElapsedMilliseconds / (float)maxCount);

				stopWatch.Reset();
				stopWatch.Start();
				for (var i = 0; i < maxCount; i++)
				{
					var temp = testClass.GetProperty();
					temp = i.ToString(CultureInfo.InvariantCulture);
					testClass.SetProperty(temp);
				}
				stopWatch.Stop();
				test3.Add(stopWatch.ElapsedMilliseconds);
				Console.WriteLine("Method Call: {0}ms\n", (float)stopWatch.ElapsedMilliseconds / (float)maxCount);
			}

			Console.WriteLine("Average Property: {0}ms", test1.Select(n => (float)n / (float)maxCount).Average());
			Console.WriteLine("Average Property with backingstore: {0}ms", test2.Select(n => (float)n / (float)maxCount).Average());
			Console.WriteLine("Average Method Call: {0}ms", test3.Select(n => (float)n / (float)maxCount).Average());

			Console.ReadLine();
		}
	}
}
