// ***********************************************************************
// Assembly         : bcplanet.NATIVE.Adapter.RpLidar
// Author           : bcare
// Created          : 05-23-2021
//
// Last Modified By : bcare
// Last Modified On : 05-24-2021
// ***********************************************************************
// <copyright file="PolarVector.cs" company="beCee Soft Art">
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

namespace bcplanet.NATIVE.Adapter.RpLidar.Base
{
    /// <summary>
    /// Class PolarPoint.
    /// </summary>
    public class PolarVector //: Vector2
    {
        /// <summary>
        /// The y
        /// </summary>
        private double _Y;
        /// <summary>
        /// The x
        /// </summary>
        private double _X;

        /// <summary>
        /// Gets or sets the angle in deg.
        /// </summary>
        /// <value>The angle in deg.</value>
        private readonly double _AngleDeg;

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        private readonly double _Radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarVector" /> class.
        /// Notice:
        /// - Angle expressed in degrees
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="angleDeg">The angle in deg.</param>
        /// <exception cref="System.ArgumentException">Radius must be non-negative</exception>
        /// <exception cref="System.ArgumentException">Angle must be in range [0,360)</exception>
        /// <exception cref="ArgumentException">Radius must be non-negative</exception>
        /// <exception cref="ArgumentException">Angle must be in range [0,360)</exception>
        public PolarVector(double radius, double angleDeg)
        {
            if (radius < 0.0)
                throw new ArgumentException("Radius must be non-negative");

            if (angleDeg < 0 || angleDeg >= 360.0)
                throw new ArgumentException("Angle must be in range [0,360)");

            _Radius = radius;
            _AngleDeg = angleDeg;
        }

        /// <summary>
        /// Polar coordinate Angle
        /// </summary>
        /// <value>The polar angle value.</value>
        /// <exception cref="System.NotSupportedException"></exception>
        public double AngleDeg => _AngleDeg;

        /// <summary>
        /// Cartesian coordinate X
        /// </summary>
        /// <value>The X position.</value>
        /// <exception cref="System.NotSupportedException"></exception>
        public double X
        {
            get
            {
                _X = -_Radius * Math.Cos(_AngleDeg * Math.PI / 180.0);
                return _X;
            }
        }

        /// <summary>
        /// Polar coordinate Radius
        /// </summary>
        /// <value>The radius value.</value>
        /// <exception cref="System.NotSupportedException"></exception>
        public double Radius => _Radius;

        /// <summary>
        /// Cartesian coordinate Y
        /// </summary>
        /// <value>The Y position.</value>
        /// <exception cref="System.NotSupportedException"></exception>
        public double Y
        {
            get
            {
                _Y = _Radius * Math.Sin(_AngleDeg * Math.PI / 180.0);
                return _Y;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
