// ***********************************************************************
// Assembly         : bcplanet.NATIVE.Adapter.RpLidar
// Author           : bcare
// Created          : 04-04-2021
//
// Last Modified By : bcare
// Last Modified On : 05-23-2021
// ***********************************************************************
// <copyright file="RpLidarExtension.cs" company="VersionManager.AssemblyCompany">
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
using bcplanet.NATIVE.Adapter.RpLidar.Structs;
using System.Collections.Generic;
using System.Linq;

namespace bcplanet.NATIVE.Adapter.RpLidar.Extensions
{
    /// <summary>
    /// Class RpLidarExtension.
    /// </summary>
    public static class RpLidarExtension
    {
        /// <summary>
        /// Determines whether [is contains data] [the specified nodes].
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="areaSize">Size of the area.</param>
        /// <returns><c>true</c> if [is contains data] [the specified nodes]; otherwise, <c>false</c>.</returns>
        public static bool IsContainsData(this rplidar_response_measurement_node_hq_t[] nodes, int areaSize = default)
        {
            if (nodes == null)
                return false;

            // Check only the nodes from index 0 to areaSize
            if (areaSize > 0)
                return nodes
                    .Take(areaSize)
                    .Count(n => n.angle_z_q14 > 0) > 0;

            // Search in hole array
            return nodes.Count(n => n.angle_z_q14 > 0) > 0;
        }


        /// <summary>
        /// Initializes the nodes.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public static void InitializeNodes(this rplidar_response_measurement_node_hq_t[] nodes)
        {
            if (nodes == null)
                return;

            for (var index = 0; index < nodes.Length; index++)
                nodes[index] = new rplidar_response_measurement_node_hq_t
                {
                    angle_z_q14 = 0,
                    dist_mm_q2 = 0,
                    quality = 0,
                    flag = 0
                };
        }

        /// <summary>
        /// Gets all nodes with data.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>rplidar_response_measurement_node_hq_t[].</returns>
        public static rplidar_response_measurement_node_hq_t[] GetAllNodesWithData(this IList<rplidar_response_measurement_node_hq_t> nodes)
        {
            //if (nodes == null)
            //    yield break;

            //float originAngle = 360.0f / nodes.Length;

            //for (var index = 0; index < nodes.Length; index++)
            //{
            //    // Exclude nodes with no distance
            //    if (nodes[index].GetDistance() == 0)
            //        continue;

            //    // Exclude nodes with no or smaller angle than 0°
            //    if (nodes[index].GetAngle() <= 0)
            //        continue;

            //    yield return nodes[index];
            //}

            var count = nodes.Count;
            var incOriginAngle = 360.0f / count;
            var index = 0;

            // Tune head
            for (index = 0; index < count; index++)
            {
                if (nodes[index].GetDistance() == 0)
                    continue;

                while (index != 0)
                {
                    index--;
                    var expectAngle = nodes[index + 1].GetAngle() - incOriginAngle;
                    if (expectAngle < 0.0f)
                        expectAngle = 0.0f;

                    nodes[index].SetAngle(expectAngle);
                }
                break;
            }

            // All the data is invalid
            if (index == count)
                return null;

            // Tune tail
            for (index = count - 1; index >= 0; index--)
            {
                if (nodes[index].GetDistance() == 0)
                    continue;

                while (index != count - 1)
                {
                    index++;
                    var expectAngle = nodes[index - 1].GetAngle() + incOriginAngle;
                    if (expectAngle > 360.0f)
                        expectAngle -= 360.0f;

                    nodes[index].SetAngle(expectAngle);
                }
                break;
            }

            // Fill invalid angles in the scan
            var frontAngle = nodes[0].GetAngle();
            for (index = 1; index < count; index++)
            {
                if (nodes[index].GetDistance() != 0)
                    continue;

                var expectAngle = frontAngle + index * incOriginAngle;
                if (expectAngle > 360.0f)
                    expectAngle -= 360.0f;

                nodes[index].SetAngle(expectAngle);
            }

            var test = nodes.Where(n => n.GetDistance() > 0).ToList();

            test.Sort((n1, n2) =>
            {
                if (n1.GetAngle() < n2.GetAngle())
                    return -1;
                if (n1.GetAngle() > n2.GetAngle())
                    return 1;

                return 0;
            });


            return test.ToArray();
        }

        /// <summary>
        /// Comparisons the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private static int Comparison(rplidar_response_measurement_node_hq_t x, rplidar_response_measurement_node_hq_t y)
        {
            throw new System.NotImplementedException();
        }
    }
}
