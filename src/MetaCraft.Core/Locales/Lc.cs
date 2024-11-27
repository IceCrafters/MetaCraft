// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Globalization;
using GetText;

namespace MetaCraft.Core.Locales;

/// <summary>
/// Shorthand for acquiring application translations.
/// </summary>
internal static class Lc
{
    internal const string LocaleDirectory = "locale";

    internal static readonly ICatalog Catalog = new Catalog("frontend", "locale", CultureInfo.CurrentUICulture);

    /// <summary>
    /// Acquires a string from the translation catalog.
    /// </summary>
    /// <param name="str">The plain string.</param>
    /// <returns>The translated text.</returns>
    internal static string L(string str)
    {
        return Catalog.GetString(str);
    }

    /// <summary>
    /// Acquires a string from the translation catalog.
    /// </summary>
    /// <param name="formattable">The formatted string.</param>
    /// <returns>The translated text.</returns>
    internal static string L(FormattableString formattable)
    {
        return Catalog.GetString(formattable);
    }

    /// <summary>
    /// Acquires a string from the translation catalog.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="args">The args.</param>
    /// <returns>The translated text.</returns>
    internal static string L(string format, params object[] args)
    {
        return Catalog.GetString(format, args);
    }
}
