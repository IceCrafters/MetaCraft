// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using JetBrains.Annotations;

namespace MetaCraft;

/// <summary>
/// Shorthand for acquiring application translations.
/// </summary>
internal static class Lc
{
    /// <summary>
    /// Acquires a string from the translation catalog.
    /// </summary>
    /// <param name="str">The plain string.</param>
    /// <returns>The translated text.</returns>
    internal static string L(string str)
    {
        return Application.Catalog.GetString(str);
    }

    /// <summary>
    /// Acquires a string from the translation catalog.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="args">The args.</param>
    /// <returns>The translated text.</returns>
    [StringFormatMethod(nameof(format))]
    internal static string L(string format, params object[] args)
    {
        return Application.Catalog.GetString(format, args);
    }
}
