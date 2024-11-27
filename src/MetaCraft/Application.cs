// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using GetText;
using MetaCraft.Core.Scopes;

namespace MetaCraft;

internal static class Application
{
    private const string AppName = "MetaCraft";

    private static readonly string DefaultScopeLocation = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MetaCraft");

    private static readonly string LocaleDirectory = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!,
        "locale");

    internal static readonly ICatalog Catalog = new Catalog("frontend", LocaleDirectory, CultureInfo.CurrentUICulture);

    internal static readonly string BaseName = Path.GetFileName(Environment.ProcessPath)
                                               ?? Assembly.GetExecutingAssembly().GetName().Name
                                               ?? AppName;
    
    public static void PrintError([Localizable(true)] string message)
    {
        Console.Error.WriteLine(@"{0}: {1}", BaseName, message);
    }
    
    public static void PrintError(Exception ex)
    {
#if DEBUG
        Console.Error.WriteLine(Lc.L("{0}: exception occurred", BaseName));
        Console.Error.WriteLine(ex.ToString());
#else
        Console.Error.WriteLine(Lc.L("{0}: exception occurred: {1}", BaseName, ex.Message));
#endif
    }

    public static PackageScope InitializeScope()
    {
        var scopeEnv = Environment.GetEnvironmentVariable("METACRAFT_SCOPE");
        if (scopeEnv != null)
        {
            return new PackageScope(scopeEnv);
        }
        
        return new PackageScope(DefaultScopeLocation);
    }
}
