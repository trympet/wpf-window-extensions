// <copyright file="AccentPolicy.cs" company="Flogard Services">
// The MIT License (MIT)
//
// Copyright (c) 2020 Trym Lund Flogard and contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowExtensions
{
    /// <summary>
    /// Affects the rendering of the background of a window.
    /// </summary>
    internal enum AccentState
    {
        ACCENT_DISABLED = 0, // Default value. Background is black.
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4, // 1703 and above
        ACCENT_ENABLE_HOSTBACKDROP = 5, // RS5 1809
        ACCENT_INVALID_STATE = 6,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        /// <summary>
        /// Brackground effect.
        /// </summary>
        public AccentState AccentState;

        /// <summary>
        /// Set to 2 for gradient color.
        /// </summary>
        public int AccentFlags;

        /// <summary>
        /// Background color.
        /// </summary>
        public int GradientColor;

        public int AnimationId;
    }
}
