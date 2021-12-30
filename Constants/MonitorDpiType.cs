﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowExtensions
{
    /// <summary>Identifies the dots per inch (dpi) setting for a monitor.</summary>
    /// <remarks>
    /// <para>All of these settings are affected by the <a href="https://docs.microsoft.com/windows/desktop/api/shellscalingapi/ne-shellscalingapi-process_dpi_awareness">PROCESS_DPI_AWARENESS</a> of your application</para>
    /// <para><see href="https://docs.microsoft.com/windows/win32/api//shellscalingapi/ne-shellscalingapi-monitor_dpi_type#">Read more on docs.microsoft.com</see>.</para>
    /// </remarks>
    internal enum MonitorDpiType
    {
        /// <summary>The effective DPI. This value should be used when determining the correct scale factor for scaling UI elements. This incorporates the scale factor set by the user for this specific display.</summary>
        MDT_EFFECTIVE_DPI = 0,

        /// <summary>The angular DPI. This DPI ensures rendering at a compliant angular resolution on the screen. This does not include the scale factor set by the user for this specific display.</summary>
        MDT_ANGULAR_DPI = 1,

        /// <summary>The raw DPI. This value is the linear DPI of the screen as measured on the screen itself. Use this value when you want to read the pixel density and not the recommended scaling setting. This does not include the scale factor set by the user for this specific display and is not guaranteed to be a supported DPI value.</summary>
        MDT_RAW_DPI = 2,

        /// <summary>The default DPI setting for a monitor is MDT_EFFECTIVE_DPI.</summary>
        MDT_DEFAULT = 0,
    }
}
