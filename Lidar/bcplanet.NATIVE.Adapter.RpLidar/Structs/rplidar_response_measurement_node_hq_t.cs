// ***********************************************************************
// Assembly         : bcplanet.NATIVE.Adapter.RpLidar
// Author           : bcare
// Created          : 04-04-2021
//
// Last Modified By : bcare
// Last Modified On : 04-04-2021
// ***********************************************************************
// <copyright file="rplidar_response_measurement_node_hq_t.cs" company="VersionManager.AssemblyCompany">
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
using System.Globalization;
using System.Runtime.InteropServices;

namespace bcplanet.NATIVE.Adapter.RpLidar.Structs
{
    /// <summary>
    /// Struct rplidar_response_measurement_node_hq_t
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 2, Size = 10)]
    public struct rplidar_response_measurement_node_hq_t
    {
        /// <summary>
        /// The angle z Q14
        /// </summary>
        [FieldOffset(0)]
        public ushort angle_z_q14;

        /// <summary>
        /// The dist mm q2
        /// </summary>
        [FieldOffset(4)]
        public uint dist_mm_q2;

        /// <summary>
        /// The quality
        /// </summary>
        [FieldOffset(8)]
        [MarshalAs(UnmanagedType.I1)]
        public byte quality;

        /// <summary>
        /// The flag
        /// </summary>
        [FieldOffset(9)]
        [MarshalAs(UnmanagedType.I1)]
        public byte flag;

        /// <summary>
        /// Gets the distance.
        /// </summary>
        /// <returns>System.UInt32.</returns>
        public uint GetDistance()
        {
            return dist_mm_q2;
        }

        /// <summary>
        /// Gets the angle.
        /// </summary>
        /// <returns>System.Single.</returns>
        public float GetAngle()
        {
            //return angle_z_q14 * 90.0f / (1 << 14);
            return angle_z_q14 * 90.0f / 16384.0f;
        }

        /// <summary>
        /// Sets the angle.
        /// </summary>
        /// <param name="y">The y.</param>
        public void SetAngle(float y)
        {
            angle_z_q14 = (ushort)(y * 16384.0f / 90.0f);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            var isSync = flag & (0x1 << 0);
            return $"{ (isSync == 1 ? "S " : "  ")}, " +
                   $"Theta: {Math.Round(GetAngle(), 2).ToString("F2", CultureInfo.InvariantCulture).PadLeft(6, ' ')}, " +
                   $"Distance: {GetDistance()} Quality: {quality}";
        }
    }
}
