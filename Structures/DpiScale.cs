using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowExtensions.Interop
{
    /// <summary>
    /// DPI information for a display device or Window; similar to <see cref="System.Windows.DpiScale"/>.
    /// </summary>
    public readonly struct DpiScale
    {
        /// <summary>
        /// This is the default PPI value used in WPF and Windows.
        /// </summary>
        internal const double DefaultPixelsPerInch = 96.0d;

        private readonly uint dotsPerInchX;
        private readonly uint dotsPerInchY;

        /// <summary>
        /// Initializes a new instance of the <see cref="DpiScale"/> structure.
        /// </summary>
        /// <param name="dotsPerInchX">DPI scale on the X axis.</param>
        /// <param name="dotsPerInchY">DPI scale on the Y axis.</param>
        public DpiScale(uint dotsPerInchX, uint dotsPerInchY)
        {
            this.dotsPerInchX = dotsPerInchX;
            this.dotsPerInchY = dotsPerInchY;
        }

#if NET5_0_OR_GREATER
        /// <summary>
        /// Initializes a new instance of the <see cref="DpiScale"/> structure.
        /// </summary>
        /// <param name="dpiScale">DPI scale on the X axis.</param>
        public DpiScale(System.Windows.DpiScale dpiScale)
        {
            dotsPerInchX = (uint)dpiScale.PixelsPerInchX;
            dotsPerInchY = (uint)dpiScale.PixelsPerInchY;
        }
#endif

        /// <summary>
        /// Gets the DPI scale on the X axis.When DPI is 96, <see cref="DpiScaleX"/> is 1.
        /// </summary>
        /// <remarks>
        /// On Windows Desktop, this value is the same as <see cref="DpiScaleY"/>.
        /// </remarks>
        public readonly double DpiScaleX => dotsPerInchX / DefaultPixelsPerInch;

        /// <summary>
        /// Gets the DPI scale on the Y axis. When DPI is 96, <see cref="DpiScaleY"/> is 1.
        /// </summary>
        /// <remarks>
        /// On Windows Desktop, this value is the same as <see cref="DpiScaleX"/>.
        /// </remarks>
        public readonly double DpiScaleY => dotsPerInchY / DefaultPixelsPerInch;

        /// <summary>
        /// Gets the PixelsPerDip at which the text should be rendered.
        /// </summary>
        public readonly double PixelsPerDip => DpiScaleY;

        /// <summary>
        /// Gets the PPI along X axis.
        /// </summary>
        /// <remarks>
        /// On Windows Desktop, this value is the same as <see cref="PixelsPerInchY"/>.
        /// </remarks>
        public readonly double PixelsPerInchX => dotsPerInchX;

        /// <summary>
        /// Gets the PPI along Y axis.
        /// </summary>
        /// <remarks>
        /// On Windows Desktop, this value is the same as <see cref="PixelsPerInchX"/>.
        /// </remarks>
        public readonly double PixelsPerInchY => dotsPerInchY;
#if NET5_0_OR_GREATER

        public static implicit operator System.Windows.DpiScale(DpiScale x) => new System.Windows.DpiScale(x.dotsPerInchX, x.dotsPerInchY);

        public static implicit operator DpiScale(System.Windows.DpiScale x) => new DpiScale(x);
#endif
    }
}
