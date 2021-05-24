// ***********************************************************************
// Assembly         : bcplanet.NATIVE.Adapter.RpLidar
// Author           : bcare
// Created          : 05-23-2021
//
// Last Modified By : bcare
// Last Modified On : 05-23-2021
// ***********************************************************************
// <copyright file="NativeModuleManager.cs" company="beCee Soft Art">
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace bcplanet.NATIVE.Adapter.RpLidar.PInvoke
{
    /// <summary>
    /// Class NativeModuleManager.
    /// </summary>
    public class NativeModuleManager
    {
        /// <summary>
        /// The native module pointers
        /// </summary>
        private static readonly Dictionary<string, IntPtr> NativeModulePointers = new Dictionary<string, IntPtr>();

        /// <summary>
        /// Gets the image location.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        private static string GetImageLocation()
        {
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(path ?? throw new InvalidOperationException(), Is64BitImageExecuting()
                ? "X64"
                : "X86");
        }

        /// <summary>
        /// Is64s the bit image executing.
        /// </summary>
        /// <returns><c>true</c> if 64 image executing, <c>false</c> otherwise.</returns>
        private static bool Is64BitImageExecuting()
        {
            var a = Assembly.GetExecutingAssembly();
            var m = a.ManifestModule;

            m.GetPEKind(out var peKinds, out _);
            return 0 == (peKinds & PortableExecutableKinds.Required32Bit);
        }

        /// <summary>
        /// The is loaded
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        protected static bool IsLoaded
        {
            get;
            private set;
        }


        /// <summary>
        /// The synchronize root load
        /// </summary>
        private static readonly object SyncRootLoad = new object();

        /// <summary>
        /// Initializes the native module (load).
        /// </summary>
        /// <returns><c>true</c> if successfully, <c>false</c> otherwise.</returns>
        public static bool InitializeNativeModule()
        {
            var result = true;
            try
            {
                // Return true when already loaded
                if (IsLoaded)
                    return IsLoaded;

                lock (SyncRootLoad)
                {
                    var imagePath = GetImageLocation();
                    if (!Directory.Exists(imagePath))
                        return false;

                    foreach (var fileName in NativeModuleNames.NativeModuleList)
                    {
                        var fullPath = Path.Combine(imagePath, fileName);
                        if (File.Exists(fullPath))
                        {
                            bool fileLoadedResult;
                            if (!NativeModulePointers.ContainsKey(fileName))
                            {
                                var modulePointer = LoadLibrary(fullPath);
                                fileLoadedResult = File.Exists(fullPath) && !IntPtr.Zero.Equals(modulePointer);

                                NativeModulePointers.Add(fullPath, fileLoadedResult ? modulePointer : IntPtr.Zero);
                            }
                            else
                                fileLoadedResult = File.Exists(fullPath) &&
                                                   !IntPtr.Zero.Equals(NativeModulePointers[fullPath]);

                            //else
                            //    _NativeModulePointers[fullPath] = fileLoadedResult ? modulePointer : IntPtr.Zero;

                            if (fileLoadedResult)
                                Console.WriteLine($"Native module: File loaded \"{fullPath}\"");

                            result &= fileLoadedResult;
                        }
                        else
                            Console.WriteLine($"Native module: File not found: \"{fullPath}\"");
                    }
                    IsLoaded = result;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Initializes the native module.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns><c>true</c> if successfully, <c>false</c> otherwise.</returns>
        public static bool InitializeNativeModule(string moduleName)
        {
            var result = true;
            try
            {
                // Return false on unknown modules
                if (!NativeModuleNames.NativeModuleList.Contains(moduleName))
                    return false;

                // Return true when already loaded
                if (IsLoaded)
                    return IsLoaded;

                lock (SyncRootLoad)
                {
                    var imagePath = GetImageLocation();
                    if (!Directory.Exists(imagePath))
                        return false;

                    var fullPath = Path.Combine(imagePath, moduleName);
                    if (File.Exists(fullPath))
                    {
                        bool fileLoadedResult;
                        if (!NativeModulePointers.ContainsKey(moduleName))
                        {
                            var modulePointer = LoadLibrary(fullPath);
                            fileLoadedResult = File.Exists(fullPath) && !IntPtr.Zero.Equals(modulePointer);

                            NativeModulePointers.Add(fullPath, fileLoadedResult ? modulePointer : IntPtr.Zero);
                        }
                        else
                            fileLoadedResult = File.Exists(fullPath) &&
                                               !IntPtr.Zero.Equals(NativeModulePointers[fullPath]);

                        if (fileLoadedResult)
                            Console.WriteLine($"Native module: File loaded \"{fullPath}\"");

                        result &= fileLoadedResult;
                    }
                    else
                        Console.WriteLine($"Native module: File not found: \"{fullPath}\"");
                    IsLoaded = result;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Unloads the native module.
        /// </summary>
        /// <returns><c>true</c> if successfully, <c>false</c> otherwise.</returns>
        public static bool UnloadNativeModule()
        {
            var result = true;
            lock (SyncRootLoad)
            {
                foreach (var key in NativeModulePointers.Keys)
                {
                    if (NativeModulePointers[key] != IntPtr.Zero)
                    {
                        var fileUnloadedResult = FreeLibrary(NativeModulePointers[key]);
                        if (fileUnloadedResult)
                            Console.WriteLine($"Native module: File unloaded \"{key}\"");
                        else
                            Console.WriteLine($"Native module: File not unloaded \"{key}\", may still in use.");
                        result &= fileUnloadedResult;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="dllName">Name of the DLL.</param>
        /// <returns>IntPtr.</returns>
        private static IntPtr LoadLibrary(string dllName)
        {

            var pointer = WinAPILoadLibrary(dllName);
            if (!IntPtr.Zero.Equals(pointer))
                return pointer;

            //Console.WriteLine(@"Load library: {0}", dllName);

            const int loadLibrarySearchDllLoadDir = 0x00000100;
            const int loadLibrarySearchDefaultDirs = 0x00001000;
            return NetFxCoreLoadLibrary(dllName, IntPtr.Zero,
                loadLibrarySearchDllLoadDir | loadLibrarySearchDefaultDirs);

        }

        /// <summary>
        /// Nets the fx core load library.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="hFile">The h file.</param>
        /// <param name="dwFlags">The dw flags.</param>
        /// <returns>IntPtr.</returns>
        [DllImport("Kernel32.dll", EntryPoint = "LoadLibraryEx")]
        private static extern IntPtr NetFxCoreLoadLibrary(
            [MarshalAs(UnmanagedType.LPStr)]
            string fileName,
            IntPtr hFile,
            int dwFlags);

        /// <summary>
        /// Load WinApi library.
        /// </summary>
        /// <param name="dllName">Path to the DLL.</param>
        /// <returns>IntPtr.</returns>
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        private static extern IntPtr WinAPILoadLibrary(
            [MarshalAs(UnmanagedType.LPStr)]
            string dllName);

        /// <summary>
        /// Decrements the reference count of the loaded dynamic-link library (DLL). When the reference count reaches zero, the module is unmapped from the address space of the calling process and the handle is no longer valid
        /// </summary>
        /// <param name="handle">The handle to the library</param>
        /// <returns>If the function succeeds, the return value is true. If the function fails, the return value is false.</returns>
        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr handle);

        /// <summary>
        /// Adds a directory to the search path used to locate DLLs for the application
        /// </summary>
        /// <param name="path">The directory to be searched for DLLs</param>
        /// <returns>True if success</returns>
        [DllImport("kernel32.dll")]
        private static extern bool SetDllDirectory(string path);

        /// <summary>
        /// Adds the DLL directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if successfully, <c>false</c> otherwise.</returns>
        [DllImport("kernel32.dll")]
        private static extern IntPtr AddDllDirectory(string path);

    }
}
