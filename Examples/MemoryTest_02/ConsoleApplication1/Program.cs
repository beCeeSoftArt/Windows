using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
    class Program
    {
        [DllImport("Win32Dll.dll", EntryPoint = "GetData", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetData(IntPtr sourceArray, int size);


        [DllImport("Win32Dll.dll", EntryPoint = "GetDataArray", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetDataArray(IntPtr[] sourceArray, int arraySize, int size);


        private static double[] darray;

        static void Main(string[] args)
        {
            int size = 524288;
            int size2 = 100;

            var stopWatch = new Stopwatch();
            for (int i = 0; i < 1000000; i++)
            {
                stopWatch.Reset();
                stopWatch.Start();

                //TestSingleArray(size);
                TestMultiArray(size2, size);

                stopWatch.Stop();
                Console.WriteLine("{0} MB, GetDataArray {1} - {2}ms", (size * size2 * sizeof(double)) / 1024 / 1024, i, stopWatch.ElapsedMilliseconds);
            }
            Console.ReadLine();
        }

        private static double[] TestSingleArray(int arraySize)
        {
            var array = new double[arraySize];
            var size = Marshal.SizeOf(typeof(double));
            var ptr = IntPtr.Zero;

            try
            {
                ptr = Marshal.AllocHGlobal(size * arraySize);

                var retval = GetData(ptr, arraySize);
                if (retval == arraySize)
                    Marshal.Copy(ptr, array, 0, arraySize);
                else
                    Console.Write("GetData failed. {0}", retval);
            }
            finally
            {
                if (ptr != IntPtr.Zero) Marshal.FreeHGlobal(ptr);
            }

            return array;
        }

        private static List<double[]> TestMultiArray(int size, int arraySize)
        {
            var sizeOfType = Marshal.SizeOf(typeof(double));
            var ptr = new IntPtr[size];

            var list = new List<double[]>();
            try
            {
                for (var i = 0; i < size; i++)
                    ptr[i] = Marshal.AllocHGlobal(sizeOfType * arraySize);

                var retval = GetDataArray(ptr, size, arraySize);
                if (retval == arraySize)
                {
                    for (var i = 0; i < size; i++)
                    {
                        var array = new double[arraySize];
                        Marshal.Copy(ptr[i], array, 0, arraySize);

                        list.Add(array);
                    }
                }
                else
                    Console.Write("GetData failed. {0}", retval);
            }
            finally
            {
                for (var i = 0; i < size; i++)
                    if (ptr[i] != IntPtr.Zero)
                        Marshal.FreeHGlobal(ptr[i]);
            }

            return list;
        }
    }
}
