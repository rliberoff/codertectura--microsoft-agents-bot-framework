using Microsoft.SemanticKernel;

using System.ComponentModel;

namespace Demo.Plugins;

/// <summary>
/// Semantic Kernel plugins for date and time.
/// </summary>
public sealed class DateTimePlugin
{
    /// <summary>
    /// Get the current date
    /// </summary>
    /// <remarks>
    /// Example: Sunday, 12 January, 2025
    /// </remarks>
    /// <example>
    /// {{time.date}} => Sunday, 12 January, 2031
    /// </example>
    /// <returns> The current date </returns>
    [KernelFunction, Description(@"Get the current date")]
    public static string Date(IFormatProvider formatProvider) => DateTimeOffset.Now.ToString(@"D", formatProvider);

    /// <summary>
    /// Get the current date
    /// </summary>
    /// <remarks>
    /// Example: Sunday, 12 January, 2025
    /// </remarks>
    /// <example>
    /// {{time.today}} => Sunday, 12 January, 2031
    /// </example>
    /// <returns> The current date </returns>
    [KernelFunction, Description(@"Get the current date")]
    public static string Today(IFormatProvider formatProvider) => Date(formatProvider);

    /// <summary>
    /// Get the current date and time in the local time zone"
    /// </summary>
    /// <remarks>
    /// Example: Sunday, January 12, 2025 9:15 PM
    /// </remarks>
    /// <example>
    /// {{time.now}} => Sunday, January 12, 2025 9:15 PM
    /// </example>
    /// <returns> The current date and time in the local time zone </returns>
    [KernelFunction, Description(@"Get the current date and time in the local time zone")]
    public static string Now(IFormatProvider formatProvider) => DateTimeOffset.Now.ToString(@"f", formatProvider);
}
