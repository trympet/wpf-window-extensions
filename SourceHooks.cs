// <copyright file="SourceHooks.cs" company="Flogard Services">
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
    internal static class SourceHooks
    {
#pragma warning disable SA1310 // Field names should not contain underscore
        private const int WA_INACTIVE = 0;
        private const int WA_ACTIVE = 1;
        private const int WA_CLICKACTIVE = 2;
#pragma warning restore SA1310 // Field names should not contain underscore

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "Pointer to obj.")]
        internal static IntPtr WndActivationProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
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

        /// <summary>See <see href="https://referencesource.microsoft.com/#PresentationFramework/src/Framework/System/Windows/Window.cs,4163"/>.</summary>
        internal static IntPtr WndCreateResizibilityOverride(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            WM message = (WM)msg;
            IntPtr result = IntPtr.Zero;

            if (message == WM.STYLECHANGING)
            {
                var styleType = (WindowLongParam)wParam;
                if (styleType == WindowLongParam.GWL_STYLE)
                {
                    var style = Marshal.PtrToStructure<STYLESTRUCT_WINDOWSTYLES>(lParam);
                    style.styleNew &= ~WindowStyles.WS_MAXIMIZEBOX;
                    Marshal.StructureToPtr(style, lParam, true);
                }
            }

            return result;
        }
    }
}
