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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace WindowExtensions
{
    /// <summary>
    /// Provides extension methods for WPF windows, allowing for the window behavior to be modified.
    /// </summary>
    public static partial class WindowExtensions
    {
        /// <summary>
        /// Get the window handle for this window.
        /// </summary>
        /// <param name="window">Handle source.</param>
        /// <returns>The window handle for this window.</returns>
        public static IntPtr GetHandle(this Window window)
            => new WindowInteropHelper(window).Handle;

        /// <summary>
        /// Disables window activation caused by change in size, position, z-index and mouse events.
        /// Also disables activation on mouse events and on calls to <see cref="Window.Activate"/>.
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

            source.AddHook(SourceHooks.WndActivationProc);
            window.Closed += WindowClosed;
            return true;
        }

        /// <summary>
        /// Reenables window activation, if it has been disabled by <see cref="DisableWindowActivation(Window)"/>.
        /// Call this method to revert window activation behavior to its default state.
        /// </summary>
        /// <param name="window">Window.</param>
        public static void ReenableWindowActivation(this Window window)
        {
            if (window.GetHandleSource() is HwndSource source)
            {
                source.RemoveHook(SourceHooks.WndActivationProc);
            }
        }

        /// <summary>
        /// Retrieve information about the display monitor for this window.
        /// </summary>
        /// <param name="window">Window of interest.</param>
        /// <param name="defaultWindow">Determines the function's return value if the window does not intersect any display monitor.</param>
        /// <param name="monitorInfo">Result. Empty when the method returns false.</param>
        /// <returns>Monitor info for the specified window.</returns>
#if NET451
        public static bool TryGetMonitorInfo(this Window window, DefaultWindow defaultWindow, out MonitorInfo monitorInfo)
#else
        public static bool TryGetMonitorInfo(this Window window, DefaultWindow defaultWindow, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(true)] out MonitorInfo monitorInfo)
#endif
        {
            monitorInfo = default;
            IntPtr monitorHandle = NativeMethods.MonitorFromWindow(window.GetHandle(), defaultWindow);
            if (monitorHandle == IntPtr.Zero)
            {
                return false;
            }

            monitorInfo.Init(); // This MUST be invoked.
            return NativeMethods.GetMonitorInfo(monitorHandle, ref monitorInfo);
        }

        /// <summary>
        /// Gets the scale for the current display.
        /// </summary>
        /// <remarks>
        /// Consider using the <see cref="CompositionTarget.TransformToDevice"/> matrix, provided by <see cref="PresentationSource.CompositionTarget"/> instead.
        /// </remarks>
        /// <param name="window">Window.</param>
        /// <param name="defaultWindow">Determines the function's return value if the window does not intersect any display monitor.</param>
        /// <returns>The device scale factor.</returns>
        public static double? GetScaleFactorForCurrentMonitor(this Window window, DefaultWindow defaultWindow)
        {
            IntPtr monitorHandle = NativeMethods.MonitorFromWindow(window.GetHandle(), defaultWindow);
            int scaleFactor = 1;
            _ = NativeMethods.GetScaleFactorForMonitor(monitorHandle, ref scaleFactor);
            return scaleFactor / 100d;
        }

        /// <summary>
        /// Obtain information on a display monitor associated with this window.
        /// </summary>
        /// <param name="window">Window of interest.</param>
        /// <param name="defaultWindow">Determines the function's return value if the window does not intersect any display monitor.</param>
        /// <param name="displayDevice">Display information for selected display.</param>
        /// <returns>Information about the display device.</returns>
        /// <exception cref="COMException">Win32 error.</exception>
#if NET451
        public static bool TryGetDisplayInfo(this Window window, DefaultWindow defaultWindow, out DisplayDevice displayDevice)
#else
        public static bool TryGetDisplayInfo(this Window window, DefaultWindow defaultWindow, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(true)] out DisplayDevice displayDevice)
#endif
        {
            displayDevice = default;
            if (window.TryGetMonitorInfo(defaultWindow, out MonitorInfo monitorInfo))
            {
                displayDevice.Init();
                return NativeMethods.EnumDisplayDevices(monitorInfo.DeviceName, 0, ref displayDevice, 1);
            }

            return false;
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
    }
}
