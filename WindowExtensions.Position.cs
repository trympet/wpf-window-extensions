using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
