// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Diagnostics;
using System.Runtime.Versioning;
using MetaCraft.Localisation;
using InvalidOperationException = System.InvalidOperationException;

namespace MetaCraft.Core.Platform;

public static class PlatformUtil
{
    [SupportedOSPlatformGuard("linux")]
    [SupportedOSPlatformGuard("macos")]
    [SupportedOSPlatformGuard("freebsd")]
    [SupportedOSPlatformGuard("windows")]
    public static bool IsBatchSupported()
    {
        return OperatingSystem.IsLinux()
               || OperatingSystem.IsMacOS()
               || OperatingSystem.IsFreeBSD()
               || OperatingSystem.IsWindows();
    }
    
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("freebsd")]
    [SupportedOSPlatform("windows")]
    public static int ExecuteBatch(string fileName, params string[] args)
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsFreeBSD())
        {
            return ExecuteBatchInternalPosix(fileName, args);
        }

        if (OperatingSystem.IsWindows())
        {
            return ExecuteBatchInternalWindows(fileName, args);
        }
        
        throw new PlatformNotSupportedException();
    }

    [SupportedOSPlatform("windows")]
    private static int ExecuteBatchInternalWindows(string fileName, string[] args)
    { 
        // Opt out of telemetry
        Environment.SetEnvironmentVariable("POWERSHELL_TELEMETRY_OPTOUT", "true");
        
        // Select which PowerShell to start.
        var winPowerShellPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), 
            "system32",
            "WindowsPowerShell",
            "1.0",
            "powershell.exe");

        var pwshPath = FindInPath("pwsh.exe");

        var toStart = pwshPath ?? winPowerShellPath;
        
        // Specify -File: it should work on both old and new PowerShell
        string[] realArgList = ["-File", fileName, ..args];
        
        // Start the real process.
        var startInfo = new ProcessStartInfo(toStart, realArgList)
        {
            UseShellExecute = false
        };
        
        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new InvalidOperationException(Lc.L("Unable to spawn shell process."));
        }
        
        process.WaitForExit();
        return process.ExitCode;
    }

    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("freebsd")]
    private static int ExecuteBatchInternalPosix(string fileName, string[] args)
    {
        string[] realArgList = [fileName, ..args];
        
        // Start the real process.
        var startInfo = new ProcessStartInfo("sh", realArgList)
        {
            UseShellExecute = false
        };
        var process = Process.Start(startInfo);
        
        if (process == null)
        {
            throw new InvalidOperationException(Lc.L("Failed to start shell interpreter process.."));
        }
        
        process.WaitForExit();
        return process.ExitCode;
    }
    
    /// <summary>
    /// Finds the specified file in <c>PATH</c>.
    /// </summary>
    /// <param name="fileName">The file to find. Must contain extensions if any.</param>
    /// <returns>The file found, or <see langword="null"/> if not found.</returns>
    private static string? FindInPath(string fileName)
    {
        var paths = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator);
        return paths?.FirstOrDefault(path => Path.GetFileName(path) == fileName);
    }
}
