// <copyright file="WindowExtensions.cs" company="Flogard Services">
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
using System.Windows;
using System.Windows.Interop;

namespace WindowExtensions
{
    /// <summary>
    /// Provides extension methods for WPF windows, allowing for the window behavior to be modified.
    /// </summary>
    public static class WindowExtensions
    {
#pragma warning disable SA1310 // Field names should not contain underscore
        private const int WA_INACTIVE = 0;
        private const int WA_ACTIVE = 1;
        private const int WA_CLICKACTIVE = 2;
#pragma warning restore SA1310 // Field names should not contain underscore

        /// <summary>
        /// Get the window handle for this window.
        /// </summary>
        /// <param name="window">Handle source.</param>
        /// <returns>The window handle for this window.</returns>
        public static IntPtr GetHandle(this Window window)
            => new WindowInteropHelper(window).Handle;

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

        /// <summary>
        /// Hides the window from the alt-tab menu.
        /// </summary>
        /// <param name="window">Window.</param>
        public static void HideFromAltTab(this Window window)
        {
            IntPtr handle = window.GetHandle();
            WindowStylesEx extendedWindowStyle = (WindowStylesEx)NativeMethods.GetWindowLongPtr(handle, WindowLongParam.GWL_EXSTYLE);
            extendedWindowStyle &= ~WindowStylesEx.WS_EX_APPWINDOW;
            extendedWindowStyle |= WindowStylesEx.WS_EX_TOOLWINDOW;
            NativeMethods.SetWindowLongPtr(new HandleRef(window, handle), WindowLongParam.GWL_EXSTYLE, (IntPtr)extendedWindowStyle);
        }

        /// <summary>
        /// Removes the maximize box from the shell,
        /// and prevents the window from being brought to a maximized state.
        /// </summary>
        /// <param name="window">Window.</param>
        public static void DisableWindowMaximize(this Window window)
        {
            WindowStyles windowStyle = (WindowStyles)NativeMethods.GetWindowLongPtr(window.GetHandle(), WindowLongParam.GWL_STYLE);
            windowStyle &= ~WindowStyles.WS_MAXIMIZEBOX;
            NativeMethods.SetWindowLongPtr(new HandleRef(window, window.GetHandle()), WindowLongParam.GWL_STYLE, (IntPtr)windowStyle);
        }

        /// <summary>
        /// Reenables window activation, if it has been disabled by <see cref="DisableWindowMaximize(Window)"/>.
        /// Call this method to revert window activation behavior to its default state.
        /// </summary>
        /// <param name="window">Window.</param>
        public static void ReenableWindowActivation(this Window window)
        {
            if (window.GetHandleSource() is HwndSource source)
            {
                source.RemoveHook(WndProc);
            }
        }

        /// <summary>
        /// Disables window activation caused by change in size, position, z-index and mouse events.
        /// Also disabled activation on mouse events and on calls to Activate().
        /// </summary>
        /// <param name="window">Window.</param>
        /// <returns>A boolean indicating whether the operation completed successfully or not.</returns>
        public static bool DisableWindowActivation(this Window window)
        {
            var source = window.GetHandleSource();
            if (source is null)
            {
                return false;
            }

            source.AddHook(WndProc);
            window.Closed += WindowClosed;
            return true;
        }

        private static void WindowClosed(object? sender, EventArgs e)
        {
            if (sender is Window window)
            {
                // Remvove handler to prevent leaks.
                window.ReenableWindowActivation();
                window.Closed -= WindowClosed;
            }
        }

        private static HwndSource? GetHandleSource(this Window window)
            => PresentationSource.FromVisual(window) as HwndSource;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "Pointer to obj.")]
        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...
            WM message = (WM)msg;
            IntPtr result = IntPtr.Zero;

            switch (message)
            {
                case WM.ACTIVATE:
                    if ((int)lParam != WA_INACTIVE)
                    {
                        // Discard all activation events
                        handled = true;
                    }

                    break;
                case WM.MOUSEACTIVATE:
                    // Don't activate window on mouse.
                    result = (IntPtr)MouseActivate.MA_NOACTIVATE;
                    handled = true;
                    break;
                case WM.WINDOWPOSCHANGING:
                case WM.WINDOWPOSCHANGED:
                    // Ensure z-index is bottom on all move events.
                    WINDOWPOS windowPos = default;
                    windowPos = Marshal.PtrToStructure<WINDOWPOS>(lParam);
                    windowPos.hwndInsertAfter = HWNDInsertAfter.Bottom;
                    Marshal.StructureToPtr(windowPos, lParam, true);
                    handled = false; // Allow WPF to consume event. This is needed to update Window.Top, Window.Left, etc.
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
