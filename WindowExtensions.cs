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
        /// Marks the window as transparent, ignoring all mouse events.
        /// Mouse events are passed to the other windows underneath.
        /// </summary>
        /// <remarks>
        /// This will add the <see cref="WindowStylesEx.WS_EX_LAYERED"/> style to the window, if the window is missing this style.
        /// </remarks>
        /// <param name="window">Window.</param>
        public static void IgnoreMouseEvents(this Window window)
        {
            WindowStylesEx extendedWindowStyle = window.GetExtendedStyle();
            extendedWindowStyle |= WindowStylesEx.WS_EX_LAYERED;
            extendedWindowStyle |= WindowStylesEx.WS_EX_TRANSPARENT;
            window.SetExtendedStyle(extendedWindowStyle);
        }

        /// <summary>
        /// Removes the transparent style attribute from the window,
        /// making it respond to mouse events again.
        /// </summary>
        /// <param name="window">Window.</param>
        public static void StopIgnoreMouseEvents(this Window window)
        {
            WindowStylesEx extendedWindowStyle = window.GetExtendedStyle();
            extendedWindowStyle &= ~WindowStylesEx.WS_EX_TRANSPARENT;
            window.SetExtendedStyle(extendedWindowStyle);
        }

        /// <summary>
        /// Hides the window from the alt-tab menu.
        /// </summary>
        /// <param name="window">Window.</param>
        public static void HideFromAltTab(this Window window)
        {
            WindowStylesEx extendedWindowStyle = window.GetExtendedStyle();
            extendedWindowStyle &= ~WindowStylesEx.WS_EX_APPWINDOW;
            extendedWindowStyle |= WindowStylesEx.WS_EX_TOOLWINDOW;
            window.SetExtendedStyle(extendedWindowStyle);
        }

        /// <summary>
        /// Removes the maximize box from the shell,
        /// and prevents the window from being brought to a maximized state.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <remarks>Setting the <see cref="Window.ResizeMode"/> property to <see cref="ResizeMode.CanResize"/> or <see cref="ResizeMode.CanResizeWithGrip"/> will override this setting. </remarks>
        public static void DisableWindowMaximize(this Window window)
            => DisableWindowMaximize(window, false);

        /// <inheritdoc cref="DisableWindowMaximize(Window)"/>
        /// <param name="overrideWPF">A value indicating whether all WPF attempts to enable maximizebox should be discarded.
        /// Set to true to override WPF.</param>
        public static void DisableWindowMaximize(this Window window, bool overrideWPF)
        {
            WindowStyles windowStyle = window.GetStyle();
            windowStyle &= ~WindowStyles.WS_MAXIMIZEBOX;
            window.SetStyle(windowStyle);
            if (overrideWPF && window.GetHandleSource() is HwndSource source)
            {
                source.AddHook(SourceHooks.WndCreateResizibilityOverride);
            }
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
                source.RemoveHook(SourceHooks.WndActivationProc);
            }
        }

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
        /// Retrieve information about the display monitor for this window.
        /// </summary>
        /// <param name="window">Window of interest.</param>
        /// <param name="defaultWindow">Determines the function's return value if the window does not intersect any display monitor.</param>
        /// <returns>Monitor info for the specified window.</returns>
        public static MonitorInfo GetMonitorInfo(this Window window, DefaultWindow defaultWindow)
        {
            IntPtr monitorHandle = NativeMethods.MonitorFromWindow(window.GetHandle(), defaultWindow);
            MonitorInfo monitorInfo = default;
            monitorInfo.Init(); // This MUST be invoked.
            if (!NativeMethods.GetMonitorInfo(monitorHandle, ref monitorInfo))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            return monitorInfo;
        }

        private static WindowStyles GetStyle(this Window window)
            => GetStyle(window.GetHandle());

        private static WindowStyles GetStyle(IntPtr handle)
            => (WindowStyles)NativeMethods.GetWindowLongPtr(handle, WindowLongParam.GWL_STYLE);

        private static WindowStylesEx GetExtendedStyle(this Window window)
            => GetExtendedStyle(window.GetHandle());

        private static WindowStylesEx GetExtendedStyle(IntPtr handle)
            => (WindowStylesEx)NativeMethods.GetWindowLongPtr(handle, WindowLongParam.GWL_EXSTYLE);

        private static IntPtr SetStyle(this Window window, WindowStyles style)
            => NativeMethods.SetWindowLongPtr(new HandleRef(window, window.GetHandle()), WindowLongParam.GWL_STYLE, (IntPtr)style);

        private static IntPtr SetExtendedStyle(this Window window, WindowStylesEx extendedStyle)
            => NativeMethods.SetWindowLongPtr(new HandleRef(window, window.GetHandle()), WindowLongParam.GWL_EXSTYLE, (IntPtr)extendedStyle);

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
