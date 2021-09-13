using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace WindowExtensions
{
    /// <summary>
    /// A value indicating the transparency level of the window.
    /// </summary>
    public enum WindowBlurLevel
    {
        /// <summary>
        /// The window background is Black where nothing is drawn in the window.
        /// </summary>
        None,

        /// <summary>
        /// The window background is Transparent where nothing is drawn in the window.
        /// </summary>
        Transparent,

        /// <summary>
        /// The window background is a blur-behind where nothing is drawn in the window.
        /// </summary>
        Blur,

        /// <summary>
        /// The window background is a blur-behind with a high blur radius. This level may fallback to Blur.
        /// </summary>
        AcrylicBlur,
    }

    /// <summary>
    /// Provides methods for adding a blur effect to a window.
    /// </summary>
    public static partial class WindowExtensions
    {
        private const int ARGBAlphaShift = 24;
        private const int ARGBRedShift = 0;
        private const int ARGBGreenShift = 8;
        private const int ARGBBlueShift = 16;

        private static Version? windowsVersion;

        private static Version WindowsVersion
            => windowsVersion ??= NativeMethods.RtlGetVersion();

        /// <summary>
        /// Sets the transparency level of the window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="transparencyLevel">The level of transparency to use. <see cref="WindowBlurLevel.AcrylicBlur"/> is only supported on build >= 19628.</param>
        /// <param name="a">Blur background opacity.</param>
        /// <param name="r">Blur background red.</param>
        /// <param name="g">Blur background green.</param>
        /// <param name="b">Blur background blue.</param>
        /// <returns>The chosen blur level, or a fallback if the chosen blur level is unsupported on the target platform.</returns>
        /// <remarks>
        /// This utilizes an undocumented API! Use with caution.<br />
        /// <paramref name="a"/>, <paramref name="r"/>, <paramref name="g"/> and <paramref name="b"/> only work on Windows 10 when using <see cref="WindowBlurLevel.Blur"/> or <see cref="WindowBlurLevel.AcrylicBlur"/>.
        /// </remarks>
        public static WindowBlurLevel EnableBlur(this Window window, WindowBlurLevel transparencyLevel, byte a = 0x01, byte r = 0x0, byte g = 0x0, byte b = 0x0)
        {
            if (WindowsVersion.Major >= 6)
            {
                if (NativeMethods.DwmIsCompositionEnabled(out var compositionEnabled) != 0 || !compositionEnabled)
                {
                    return WindowBlurLevel.None;
                }
                else if (WindowsVersion.Major >= 10)
                {
                    return Win10EnableBlur(window.GetHandle().AssertNotZero(), transparencyLevel, unchecked((int)MakeAbgr(a, r, g, b)));
                }
                else if (WindowsVersion.Minor >= 2)
                {
                    return Win8xEnableBlur(window.GetHandle().AssertNotZero(), transparencyLevel);
                }
                else
                {
                    return Win7EnableBlur(window.GetHandle().AssertNotZero(), transparencyLevel);
                }
            }
            else
            {
                return WindowBlurLevel.None;
            }
        }

        private static WindowBlurLevel Win7EnableBlur(IntPtr handle, WindowBlurLevel transparencyLevel)
        {
            if (transparencyLevel == WindowBlurLevel.AcrylicBlur)
            {
                transparencyLevel = WindowBlurLevel.Blur;
            }

            var blurInfo = new DWM_BLURBEHIND(false);

            if (transparencyLevel == WindowBlurLevel.Blur)
            {
                blurInfo = new DWM_BLURBEHIND(true);
            }

            NativeMethods.DwmEnableBlurBehindWindow(handle, ref blurInfo);

            if (transparencyLevel == WindowBlurLevel.Transparent)
            {
                return WindowBlurLevel.None;
            }
            else
            {
                return transparencyLevel;
            }
        }

        private static WindowBlurLevel Win8xEnableBlur(IntPtr handle, WindowBlurLevel transparencyLevel)
        {
            AccentPolicy accent = default;
            var accentStructSize = Marshal.SizeOf(accent);

            if (transparencyLevel == WindowBlurLevel.AcrylicBlur)
            {
                transparencyLevel = WindowBlurLevel.Blur;
            }

            accent.AccentState = transparencyLevel == WindowBlurLevel.Transparent
                ? AccentState.ACCENT_ENABLE_BLURBEHIND
                : AccentState.ACCENT_DISABLED;

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            WindowCompositionAttributeData data = default;
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            NativeMethods.SetWindowCompositionAttribute(handle, ref data);

            Marshal.FreeHGlobal(accentPtr);

            if (transparencyLevel >= WindowBlurLevel.Blur)
            {
                Win7EnableBlur(handle, transparencyLevel);
            }

            return transparencyLevel;
        }

        private static WindowBlurLevel Win10EnableBlur(IntPtr handle, WindowBlurLevel transparencyLevel, int color)
        {
            bool canUseAcrylic = WindowsVersion.Major > 10 || WindowsVersion.Build >= 19628;

            AccentPolicy accent = default;
            var accentStructSize = Marshal.SizeOf(accent);

            if (transparencyLevel == WindowBlurLevel.AcrylicBlur && !canUseAcrylic)
            {
                transparencyLevel = WindowBlurLevel.Blur;
            }

            switch (transparencyLevel)
            {
                default:
                case WindowBlurLevel.None:
                    accent.AccentState = AccentState.ACCENT_DISABLED;
                    break;

                case WindowBlurLevel.Transparent:
                    accent.AccentState = AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT;
                    break;

                case WindowBlurLevel.Blur:
                    accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
                    break;

                case WindowBlurLevel.AcrylicBlur:
                case (WindowBlurLevel.AcrylicBlur + 1): // hack-force acrylic.
                    accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;
                    transparencyLevel = WindowBlurLevel.AcrylicBlur;
                    break;
            }

            accent.AccentFlags = 2;

            // Color recipe: (<bOpacity> << 24) | (<BlueGreenRed> & 0xFFFFFF)
            accent.GradientColor = color;

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            WindowCompositionAttributeData data = default;
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            NativeMethods.SetWindowCompositionAttribute(handle, ref data);

            Marshal.FreeHGlobal(accentPtr);

            return transparencyLevel;
        }

        private static long MakeAbgr(byte alpha, byte red, byte green, byte blue)
        {
            return (long)unchecked((uint)(red << ARGBRedShift |
                         green << ARGBGreenShift |
                         blue << ARGBBlueShift |
                         alpha << ARGBAlphaShift)) & 0xffffffff;
        }
    }
}
