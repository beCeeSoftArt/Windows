// ***********************************************************************
// Assembly         : bcplanet.NATIVE.Adapter.RpLidar
// Author           : bcare
// Created          : 04-04-2021
//
// Last Modified By : bcare
// Last Modified On : 05-23-2021
// ***********************************************************************
// <copyright file="RpLidarInterface.cs" company="VersionManager.AssemblyCompany">
//     Copyright (c) André Spitzner. All rights reserved.
//    
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met :
//
// 1. Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
// this list of conditions and the following disclaimer in the documentation
// and /or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES(INCLUDING, BUT NOT LIMITED TO,
//PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
// OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// </copyright>
// <summary></summary>
// ***********************************************************************
using bcplanet.NATIVE.Adapter.RpLidar.Base;
using bcplanet.NATIVE.Adapter.RpLidar.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using bcplanet.NATIVE.Adapter.RpLidar.PInvoke;

namespace bcplanet.NATIVE.Adapter.RpLidar
{
    /// <summary>
    /// Class RpLidarInterface.
    /// </summary>
    public class RpLidarInterface
    {
        /// <summary>
        /// Initialize driver.
        /// </summary>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarInitializeDriver",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarInitializeDriver();

        /// <summary>
        /// Dispose driver.
        /// </summary>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarDisposeDriver",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarDisposeDriver();

        /// <summary>
        /// Connect to device
        /// </summary>
        /// <param name="comPort">The COM port.</param>
        /// <param name="baudRate">The baud rate.</param>
        /// <param name="flag">The flag.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarConnect",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarConnect(
            [In][MarshalAs(UnmanagedType.LPStr)] string comPort,
            uint baudRate = 115200,
            uint flag = 0);

        /// <summary>
        /// Disconnect from device.
        /// </summary>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarDisconnect",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarDisconnect();

        /// <summary>
        /// Is connected state.
        /// </summary>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarIsConnected",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarIsConnected();

        /// <summary>
        /// Reset the device.
        /// </summary>
        /// <param name="timeOut">The time out.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarReset",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarReset(uint timeOut = 2000);

        /// <summary>
        /// Clear serial cache.
        /// </summary>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarClearSerialCache",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarClearSerialCache();

        /// <summary>
        /// Get the device health.
        /// </summary>
        /// <param name="health">The health.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarGetHealth",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarGetHealth(
            ref rplidar_response_device_health_t health,
            uint timeout = 2000);

        /// <summary>
        /// Get device information.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarGetDeviceInfo",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarGetDeviceInfo(
            ref rplidar_response_device_info_t info,
            uint timeout = 2000);

        /// <summary>
        /// Set motor PWM.
        /// </summary>
        /// <param name="pwm">The PWM.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarSetMotorPWM",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarSetMotorPWM(
            ushort pwm);

        /// <summary>
        /// Set spin speed.
        /// </summary>
        /// <param name="rpm">The RPM.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarSetSpinSpeed",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarSetSpinSpeed(
            ushort rpm,
            uint timeout = 2000);

        /// <summary>
        /// Start motor.
        /// </summary>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarStartMotor",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarStartMotor();

        /// <summary>
        /// Stop motor.
        /// </summary>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarStopMotor",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarStopMotor();

        /// <summary>
        /// Check if motor support control.
        /// </summary>
        /// <param name="support">if set to <c>true</c> [support].</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarCheckIfMotorSupportControl",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarCheckIfMotorSupportControl(
            [MarshalAs(UnmanagedType.I1)] bool support,
            uint timeout = 2000);

        /// <summary>
        /// Check if is tof device.
        /// </summary>
        /// <param name="isTofLidar">if set to <c>true</c> [is tof lidar].</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarCheckIfIsTofDevice",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarCheckIfIsTofDevice(
            [MarshalAs(UnmanagedType.I1)] bool isTofLidar,
            uint timeout = 2000);

        /// <summary>
        /// Get frequency.
        /// </summary>
        /// <param name="scanMode">The scan mode.</param>
        /// <param name="count">The count.</param>
        /// <param name="frequency">The frequency.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarGetFrequency",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarGetFrequency(
            ref RplidarScanMode scanMode,
            ulong count,
            float frequency);

        /// <summary>
        /// Start normal scan.
        /// </summary>
        /// <param name="force">if set to <c>true</c> [force].</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarStartNormalScan",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarStartNormalScan(
            [MarshalAs(UnmanagedType.I1)] bool force,
            uint timeout = 2000);

        /// <summary>
        /// Start scan.
        /// </summary>
        /// <param name="force">if set to <c>true</c> [force].</param>
        /// <param name="mode">The mode.</param>
        /// <param name="scanMode">The scan mode.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarStartScan",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarStartScan(
            [MarshalAs(UnmanagedType.I1)] bool force,
            [MarshalAs(UnmanagedType.U2)] ushort mode,
            ref RplidarScanMode scanMode,
            uint options = 0);

        /// <summary>
        /// Stop the device.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarStop",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarStop(
            uint timeout = 2000);

        /// <summary>
        /// Sort scan data ascend (by rotation angle).
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarSortScanDataAscend",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarSortScanDataAscend(
            [In][Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] rplidar_response_measurement_node_hq_t[] nodes,
            ulong count);

        /// <summary>
        /// Get scan data with interval hq. (polar coordinates)
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarGetScanDataWithIntervalHq",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarGetScanDataWithIntervalHq(
            [In][Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] rplidar_response_measurement_node_hq_t[] nodes,
            ulong count);

        /// <summary>
        /// Grab current scan data hq in Slamtec format.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="count">The count.</param>
        /// <param name="resultCount">The result count.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
             NativeModuleNames.NativeRpLidar,
             EntryPoint = "LidarGrabScanDataHq",
             CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarGrabScanDataHq(
            [In][Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] rplidar_response_measurement_node_hq_t[] nodes,
            ulong count,
            out UInt64 resultCount,
            uint timeout = 2000);

        /// <summary>
        /// Grab the current scan data in style NMEA string format.
        /// (LIDAR sentence with bcplanet AIMB format extension)
        /// </summary>
        /// <param name="sentences">The sentences.</param>
        /// <param name="inputSize">Size of the input.</param>
        /// <param name="resultSize">Size of the result.</param>
        /// <param name="sensorId">The sensor identifier.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarGetNmea",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarGetNmea(
            [In][Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] sentences,
            ulong inputSize,
            out ulong resultSize,
            uint sensorId,
            uint timeout = 2000);

        /// <summary>
        /// Grab the current scan data in semicolon separated
        /// polar vector data coordinate string format.
        /// Format:
        /// [angle];[radius/distance]
        /// </summary>
        /// <param name="sentences">The sentences.</param>
        /// <param name="inputSize">Size of the input.</param>
        /// <param name="resultSize">Size of the result.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(
            NativeModuleNames.NativeRpLidar,
            EntryPoint = "LidarGetStringData",
            CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int LidarGetStringData(
            [In][Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] sentences,
            ulong inputSize,
            out ulong resultSize,
            uint timeout = 2000);

        /// <summary>
        /// Gets the current scanned data in polar vectors format (PolarVector ).
        /// </summary>
        /// <returns>List&lt;PolarVector&gt;.</returns>
        public static ConcurrentHashSet<PolarVector> GetCurrentVectors()
        {
            var result = new ConcurrentHashSet<PolarVector>();

            var tmpList = new List<string>();
            for (var index = 0; index < 8192; index++)
                tmpList.Add("                                                                                                                                                      ");

            var scanData = tmpList.ToArray();

            var stopWatch1 = new Stopwatch();
            stopWatch1.Start();

            var grabResult = LidarGetStringData(
                scanData,
                (ulong)scanData.Length,
                out var outputSize,
                2000);

            stopWatch1.Stop();
            Console.WriteLine($@"Call {nameof(LidarGetStringData)} duration: {stopWatch1.ElapsedMilliseconds}ms");

            if (grabResult == 0
                && outputSize > 0)
            {
                var stopWatch2 = new Stopwatch();
                stopWatch2.Start();
                var scanDataResult = scanData.Take((int)outputSize);

                Parallel.ForEach(scanDataResult, (dataString, state) =>
                {
                    var payLoad = dataString.Trim();
                    if (string.IsNullOrEmpty(payLoad))
                        return;

                    var dataStringParts = payLoad.Split(';');
                    if (dataStringParts.Length <= 1)
                        return;

                    if (double.TryParse(dataStringParts[0], NumberStyles.Any, CultureInfo.CurrentCulture, out var angle)
                        && double.TryParse(dataStringParts[1], NumberStyles.Any, CultureInfo.CurrentCulture, out var radius))
                    {
                        // Us unit in meter
                        radius = radius / 10.0;

                        if (!(radius > 0)
                            || !(angle >= 0))
                            return;

                        result.Add(new PolarVector(radius, angle));
                    }
                });

                stopWatch2.Stop();
                Console.WriteLine($@"Call {nameof(GetCurrentVectors)} duration: {stopWatch2.ElapsedMilliseconds}ms");
            }

            return new ConcurrentHashSet<PolarVector>(result.OrderBy(n => n.AngleDeg));
        }
    }
}
