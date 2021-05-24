// ***********************************************************************
// Assembly         : bcplanet.NATIVE.Adapter.RpLidar
// Author           : bcare
// Created          : 04-04-2021
//
// Last Modified By : bcare
// Last Modified On : 05-23-2021
// ***********************************************************************
// <copyright file="RpLidarConnector.cs" company="VersionManager.AssemblyCompany">
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
using bcplanet.NATIVE.Adapter.RpLidar.Eums;
using bcplanet.NATIVE.Adapter.RpLidar.EventArguments;
using bcplanet.NATIVE.Adapter.RpLidar.PInvoke;
using bcplanet.NATIVE.Adapter.RpLidar.Structs;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CancellationTokenSource = bcplanet.NATIVE.Adapter.RpLidar.Base.CancellationTokenSource;

namespace bcplanet.NATIVE.Adapter.RpLidar
{
    /// <summary>
    /// Class RpLidarConnector.
    /// </summary>
    public class RpLidarConnector : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// The token source
        /// </summary>
        private CancellationTokenSource _TokenSource;

        /// <summary>
        /// The token
        /// </summary>
        private CancellationToken _Token;

        /// <summary>
        /// The task
        /// </summary>
        private Task _Task;

        /// <summary>
        /// The serial port
        /// </summary>
        private string _SerialPort;

        /// <summary>
        /// The serial port speed
        /// </summary>
        private uint _SerialPortSpeed;

        /// <summary>
        /// The device information
        /// </summary>
        private rplidar_response_device_info_t _DeviceInfo;

        /// <summary>
        /// The device health information
        /// </summary>
        private rplidar_response_device_health_t _DeviceHealthInfo;

        /// <summary>
        /// The lidar scan mode
        /// </summary>
        private RplidarScanMode _RpLidarScanMode;

        /// <summary>
        /// The scan number
        /// </summary>
        private ulong _ScanNumber;
        #endregion

        #region .Ctor

        // Implement if needed

        #endregion

        #region Dispose

        /// <summary>
        /// The disposed
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected bool _Disposed;

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        [Browsable(false)]
        [DisplayName("Is Disposed")]
        [Description("Indicates that the object can not longer be used. It disposing progress by the garbage collection.")]
        public bool IsDisposed => _Disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            LidarDispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void LidarDispose(bool disposing)
        {
            if (!_Disposed)
            {

                if (!IsRunning)
                    return;

                try
                {    // Signal exit
                    _TokenSource?.Cancel();
                }
                catch
                {
                }
                try
                {
                    // Wait until task is ended
                    _Task.Wait(5000, _Token);
                }
                catch (OperationCanceledException)
                {
                    // Task tell us it's ended
                }
                catch (AggregateException e)
                {
                    foreach (var v in e.InnerExceptions)
                        Console.WriteLine(v);
                }
                finally
                {
                    // Dispose token source
                    _TokenSource?.Dispose();

                    // Stop motor, disconnect device and dispose driver
                    RpLidarInterface.LidarDisposeDriver();
                    _Disposed = true;
                }
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RpLidarConnector"/> class.
        /// </summary>
        ~RpLidarConnector()
        {
            // Simply call Dispose(false).
            LidarDispose(false);
        }


        #endregion

        #region Event Handler

        /// <summary>
        /// Occurs when [on device connected].
        /// </summary>
        public event EventHandler<EventArgs> OnDeviceConnected;

        /// <summary>
        /// Occurs when [on device disconnected].
        /// </summary>
        public event EventHandler<EventArgs> OnDeviceDisconnected;

        /// <summary>
        /// Occurs when [on scan data received].
        /// </summary>
        public event EventHandler<LidarDataReceivedEventArgs> OnScanDataReceived;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the device information.
        /// </summary>
        /// <value>The device information.</value>
        public rplidar_response_device_info_t DeviceInfo => _DeviceInfo;

        /// <summary>
        /// Gets the device health information.
        /// </summary>
        /// <value>The device health information.</value>
        public rplidar_response_device_health_t DeviceHealthInfo => _DeviceHealthInfo;

        /// <summary>
        /// Gets the lidar scan mode.
        /// </summary>
        /// <value>The rp lidar scan mode.</value>
        public RplidarScanMode RpLidarScanMode => _RpLidarScanMode;

        #endregion

        #region Methods

        /// <summary>
        /// Initialize device connection settings.
        /// </summary>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="serialPortSpeed">The serial port speed.</param>
        /// <returns><c>true</c> if successfully, <c>false</c> otherwise.</returns>
        public bool LidarInitialize(string serialPort, uint serialPortSpeed = 115200)
        {
            if (IsRunning
                || IsDisposed)
                return false;

            _SerialPort = serialPort;
            _SerialPortSpeed = serialPortSpeed;

            return true;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="scanMode">The scan mode.</param>
        /// <returns><c>true</c> if successfully, <c>false</c> otherwise.</returns>
        public bool LidarStart(EScanMode scanMode = EScanMode.Boost)
        {
            try
            {
                if (IsRunning
                    || IsDisposed)
                    return false;

                _ScanNumber = 0;

                // Load driver
                if (!NativeModuleManager.InitializeNativeModule(NativeModuleNames.NativeRpLidar))
                {
                    Console.WriteLine("Can not load lidar native driver library.");
                    return false;
                }

                // Initialize driver
                if (RpLidarInterface.LidarInitializeDriver() != 0)
                {
                    Console.WriteLine("Can not initialize lidar driver.");
                }

                // Connect device
                if (RpLidarInterface.LidarConnect(_SerialPort, _SerialPortSpeed) != 0)
                {
                    Console.WriteLine($"Can not connect to serial port \"{_SerialPort}:{_SerialPortSpeed}\".");
                }

                // Get device info
                _DeviceInfo = new rplidar_response_device_info_t
                {
                    serialNum = new byte[16]
                };
                if (RpLidarInterface.LidarGetDeviceInfo(ref _DeviceInfo) != 0)
                {
                    Console.WriteLine("Can not get device info from device.");
                }

                // Get device health info
                _DeviceHealthInfo = new rplidar_response_device_health_t();
                if (RpLidarInterface.LidarGetHealth(ref _DeviceHealthInfo) != 0)
                {
                    Console.WriteLine($"Can not get device health info from device.");
                }

                // Reset device on error state
                if (_DeviceHealthInfo.Status == (byte)EDeviceStatus.RPLIDAR_STATUS_ERROR)
                {
                    Console.WriteLine($@"Reset device due to error device health status: {_DeviceHealthInfo.Status}");
                    RpLidarInterface.LidarReset();
                }

                // Start spinning motor
                if (RpLidarInterface.LidarStartMotor() != 0)
                {
                    Console.WriteLine($"Can not start device motor.");
                }

                // Start scanning
                _RpLidarScanMode = new RplidarScanMode
                {
                    scan_mode = new char[64]
                };
                if (RpLidarInterface.LidarStartScan(false, (ushort)scanMode, ref _RpLidarScanMode) != 0)
                {
                    Console.WriteLine($"Can not start scan mode.");
                }

                // Initialize task
                _TokenSource = new CancellationTokenSource();
                _Token = _TokenSource.Token;

                _Task = Task.Factory.StartNew(() => LidarTaskDoWork(_Token), _Token);

                if (RpLidarInterface.LidarIsConnected() == 1)
                    return true;
                else
                    return false;
            }
            finally
            {
                if (RpLidarInterface.LidarIsConnected() == 1)
                    OnDeviceConnected?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <returns><c>true</c> if successfully, <c>false</c> otherwise.</returns>
        public bool LidarStop()
        {
            if (!IsRunning
                || IsDisposed)
                return false;

            _TokenSource?.Cancel();

            try
            {
                _Task?.Wait(_Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (AggregateException e)
            {
                foreach (var v in e.InnerExceptions)
                    Console.WriteLine(v);
            }
            finally
            {
                _TokenSource?.Dispose();
            }

            try
            {
                // Stop device and dispose driver
                RpLidarInterface.LidarDisposeDriver();

                if (RpLidarInterface.LidarIsConnected() != 1)
                    return true;
                else
                    return false;
            }
            finally
            {
                if (RpLidarInterface.LidarIsConnected() != 1)
                    OnDeviceDisconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Does the task work.
        /// </summary>
        /// <param name="token">The token.</param>
        private void LidarTaskDoWork(CancellationToken token)
        {
            try
            {
                // Check if on start is canceled
                token.ThrowIfCancellationRequested();

                IsRunning = true;

                while (true)
                {
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    Thread.Sleep(400);

                    if (RpLidarInterface.LidarIsConnected() != 1)
                        return;

                    var scanResult = RpLidarInterface.GetCurrentVectors();
                    OnScanDataReceived?.Invoke(this, new LidarDataReceivedEventArgs(scanResult, ++_ScanNumber));
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (AggregateException e)
            {
                foreach (var v in e.InnerExceptions)
                    Console.WriteLine(v);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                // Stop device and dispose driver
                try
                {
                    // Exception happens when closing an connection 
                    // where someone has reconnect USB cable during usage

                    RpLidarInterface.LidarDisposeDriver();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                IsRunning = false;
            }
        }

        #endregion
    }
}
