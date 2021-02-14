// <copyright file="STYLESTRUCT.cs" company="Flogard Services">
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
using System.Runtime.InteropServices;

namespace WindowExtensions
{
    [StructLayout(LayoutKind.Sequential)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "win32.")]
    internal struct STYLESTRUCT
    {
        /// <summary>
        /// The previous styles for a window.
        /// </summary>
        public IntPtr styleOld;

        /// <summary>
        /// The new styles for a window.
        /// </summary>
        public IntPtr styleNew;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "win32.")]
    internal struct STYLESTRUCT_WINDOWSTYLES
    {
        /// <summary>
        /// The previous styles for a window.
        /// </summary>
        public WindowStyles styleOld;

        /// <summary>
        /// The new styles for a window.
        /// </summary>
        public WindowStyles styleNew;
    }
}
