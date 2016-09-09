using System;
using System.Diagnostics;
using System.Threading;

namespace PerformanceCounter001
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        /// <summary>
        /// The performance counter
        /// </summary>
        private static PerformanceCounter _performanceCounter;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        public static void Main()
        {
            // Check if counter category exists
            if (!PerformanceCounterCategory.Exists("Processor"))
            {
                Console.WriteLine("Object Processor does not exist!");
                return;
            }

            // Check if counter in category exists
            if (!PerformanceCounterCategory.CounterExists(@"% Processor Time", "Processor"))
            {
                Console.WriteLine(@"Counter % Processor Time does not exist!");
                return;
            }

            // Create the counter
            _performanceCounter = new PerformanceCounter("Processor", @"% Processor Time", @"_Total");

            //// The raw value of a counter can be set in your applications as shown below
            //// if the object is not read-only
            //try
            //{
            //    _performanceCounter.RawValue = 19;
            //}

            //catch
            //{
            //    Console.WriteLine(@"Processor, % Processor Time, _Total instance is READONLY!");
            //}

            Console.WriteLine(@"Press 'CTRL+C' to quit...");

            while (true)
            {
                try
                {
                    // Get value from counter
                    Console.WriteLine(@"Current value of Processor, %Processor Time, _Total= " + _performanceCounter.NextValue());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }

                Thread.Sleep(1000);
                Console.WriteLine(@"Press 'CTRL+C' to quit...");
            }
        }
    }
}
