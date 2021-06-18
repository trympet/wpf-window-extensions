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
    /// Provides extension methods for window styles.
    /// </summary>
    public static partial class WindowExtensions
    {
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
        /// <remarks>
        /// This will prevent WPF from resizing the window if the resolution or display arrangement changes.
        /// </remarks>
        public static void HideFromAltTab(this Window window)
        {
            WindowStylesEx extendedWindowStyle = window.GetExtendedStyle();
            extendedWindowStyle &= ~WindowStylesEx.WS_EX_APPWINDOW;
            extendedWindowStyle |= WindowStylesEx.WS_EX_TOOLWINDOW;
            window.SetExtendedStyle(extendedWindowStyle);
        }

        /// <summary>
        /// Hides the window from the alt-tab menu.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <remarks>
        /// This will prevent WPF from resizing the window if the resolution or display arrangement changes.
        /// </remarks>
        public static void ShowInAltTab(this Window window)
        {
            WindowStylesEx extendedWindowStyle = window.GetExtendedStyle();
            extendedWindowStyle &= ~WindowStylesEx.WS_EX_TOOLWINDOW;
            extendedWindowStyle |= WindowStylesEx.WS_EX_APPWINDOW;
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
        /// Reenables window maximize, if it has been disabled by <see cref="DisableWindowMaximize(Window)"/>.
        /// Call this method to revert window shell to its default state.
        /// </summary>
        /// <param name="window">Window.</param>
        public static void ReenableWindowMaximize(this Window window)
        {
            if (window.GetHandleSource() is HwndSource source)
            {
                source.RemoveHook(SourceHooks.WndCreateResizibilityOverride);
            }
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
    }
}