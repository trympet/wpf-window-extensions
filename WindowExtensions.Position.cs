// <copyright file="WindowExtensions.Position.cs" company="Flogard Services">
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
using System.Drawing;
using System.Windows;

namespace WindowExtensions
{
    /// <summary>
    /// Provides extension methods for manipulating the position of a window.
    /// </summary>
    public static partial class WindowExtensions
    {
        /// <summary>
        /// Changes the size and position of the window, in client coordinates.
        /// </summary>
        /// <param name="window">The window of interest.</param>
        /// <param name="position">The position of the window, in pixels, relative to the primary display.</param>
        public static void SetWindowPosition(this Window window, Rectangle position)
        {
            var handle = window.GetHandle();
            NativeMethods.SetWindowPos(handle, IntPtr.Zero, position.X, position.Y, position.Width, position.Height, SetWindowPosFlags.SWP_FRAMECHANGED | SetWindowPosFlags.SWP_NOZORDER);
        }

        /// <summary>
        /// Places the window at the bottom.
        /// </summary>
        /// <param name="window">Window.</param>
        public static void BringToBack(this Window window)
        {
            // ref: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
            IntPtr hWnd = window.GetHandle();
            NativeMethods.SetWindowPos(hWnd, HWNDInsertAfter.Bottom, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_FRAMECHANGED);
        }

        /// <summary>
        /// Sets the window position to the top of the Z-index, making it float over all other windows.
        /// </summary>
        /// <param name="window">Window.</param>
        public static void BringToFront(this Window window)
        {
            IntPtr hWnd = window.GetHandle();
            NativeMethods.SetWindowPos(hWnd, HWNDInsertAfter.TopMost, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_FRAMECHANGED);
        }
    }
}
