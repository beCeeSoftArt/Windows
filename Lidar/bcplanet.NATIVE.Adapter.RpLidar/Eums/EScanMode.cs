﻿// ***********************************************************************
// Assembly         : bcplanet.NATIVE.Adapter.RpLidar
// Author           : bcare
// Created          : 04-04-2021
//
// Last Modified By : bcare
// Last Modified On : 05-23-2021
// ***********************************************************************
// <copyright file="EScanMode.cs" company="VersionManager.AssemblyCompany">
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
namespace bcplanet.NATIVE.Adapter.RpLidar.Eums
{
    /// <summary>
    /// Enum EScanMode
    /// </summary>
    public enum EScanMode
    {
        /// <summary>
        /// The standard scan mode
        /// </summary>
        Standard = 0,

        /// <summary>
        /// The express scan mode
        /// </summary>
        Express = 1,

        /// <summary>
        /// The boost scan mode
        /// </summary>
        Boost = 2,

        /// <summary>
        /// The stability scan mode
        /// </summary>
        Stability = 3
    }
}
