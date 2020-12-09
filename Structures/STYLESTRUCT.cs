using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
