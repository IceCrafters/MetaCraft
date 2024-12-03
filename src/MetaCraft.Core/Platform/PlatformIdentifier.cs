// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using MetaCraft.Core.Locales;
using MetaCraft.Core.Serialization;

namespace MetaCraft.Core.Platform;

[JsonConverter(typeof(PlatformIdentifierConverter))]
public readonly record struct PlatformIdentifier
{
    [SetsRequiredMembers]
    public PlatformIdentifier(PlatformSystem system, PlatformArchitecture architecture)
    {
        System = system;
        Architecture = architecture;
    }
    
    public required PlatformSystem System { get; init; }
    public required PlatformArchitecture Architecture { get; init; }
    
    public override string ToString()
    {
        var sysStr = System.ToString().ToLowerInvariant();
        var archStr = Architecture.ToString().ToLowerInvariant();
        
        var builder = new StringBuilder(sysStr.Length + archStr.Length + 1);
        builder.Append(sysStr)
            .Append('-')
            .Append(archStr);
        
        return builder.ToString();
    }
    
    public bool IsCoveredBy(PlatformIdentifier other)
    {
        return other.Covers(this);
    }
    
    /// <summary>
    /// Determines whether this platform identifier includes the specified platform identifier.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This platform "includes" the other platform identifier if any of the following conditions are true:
    /// <list type="bullet">
    ///     <item>This and the other platform identifier equals (that is, <see cref="Equals(MetaCraft.Core.Platform.PlatformIdentifier?)"/> returns true).</item>
    ///     <item>Both <see cref="System"/> and <see cref="Architecture"/> properties of this instance are correspondingly <see cref="PlatformSystem.Any"/> and <see cref="PlatformArchitecture.Any"/>.</item>
    ///     <item><see cref="System"/> is <see cref="PlatformSystem.Any"/>, and <see cref="Architecture"/> property on both instances are the same.</item>
    ///     <item><see cref="Architecture"/> is <see cref="PlatformArchitecture.Any"/>, and the <see cref="System"/> property on both instances are the same.</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <param name="other">The platform identifier to compare.</param>
    /// <returns><see langword="true"/> if the other platform identifier is compatible with this; otherwise, <see langword="false"/>.</returns>
    public bool Covers(PlatformIdentifier other)
    {
        if (System == PlatformSystem.Any && Architecture == PlatformArchitecture.Any)
        {
            return true;
        }
        
        if (Equals(other))
        {
            return true;
        }

        if (System == PlatformSystem.Any && other.Architecture == Architecture)
        {
            return true;
        }

        if (other.System == System && Architecture == PlatformArchitecture.Any)
        {
            return true;
        }

        return false;
    }
    
    #region Parsing

    private enum ParseFunctionResult
    {
        Success,
        InvalidFormat,
        InvalidSystem,
        InvalidArchitecture
    }

    private static ParseFunctionResult ParseInternal(string input, out PlatformIdentifier result)
    {
        var inputSpan = input.AsSpan();
        Span<Range> ranges = stackalloc Range[2];
        var split = inputSpan.Split(ranges, '-');

        // Handle when only the OS name is specified
        if (split == 1)
        {
            if (!ParsePlatformSystem(inputSpan, out var sys))
            {
                result = default;
                return ParseFunctionResult.InvalidSystem;
            }

            result = new PlatformIdentifier(sys, PlatformArchitecture.Any);
            return ParseFunctionResult.Success;
        }
        
        // All other cases must be os-arch, otherwise fails
        if (split != 2)
        {
            result = default;
            return ParseFunctionResult.InvalidFormat;
        }

        // Parse system & architecture
        if (!ParsePlatformSystem(inputSpan[ranges[0]], out var system))
        {
            result = default;
            return ParseFunctionResult.InvalidSystem;
        }

        if (!ParsePlatformArchitecture(inputSpan[ranges[1]], out var arch))
        {
            result = default;
            return ParseFunctionResult.InvalidArchitecture;
        }
        
        result = new PlatformIdentifier(system, arch);
        return ParseFunctionResult.Success;
    }

    private static bool ParsePlatformArchitecture(ReadOnlySpan<char> inputSpan, out PlatformArchitecture system)
    {
        return Enum.TryParse(inputSpan, true, out system);
    }
    
    private static bool ParsePlatformSystem(ReadOnlySpan<char> inputSpan, out PlatformSystem system)
    {
        return Enum.TryParse(inputSpan, true, out system);
    }

    private static FormatException CreateException(ParseFunctionResult result)
    {
        var message = result switch
        {
            ParseFunctionResult.InvalidFormat => Lc.L("Invalid platform identifier format."),
            ParseFunctionResult.InvalidSystem => Lc.L("Invalid system identifier."),
            ParseFunctionResult.InvalidArchitecture => Lc.L("Invalid architecture identifier."),
            _ => Lc.L("Unexpected error occurred when parsing the platform identifier: '{0}'", result)
        };

        return new FormatException(message);
    }
    
    public static PlatformIdentifier Parse(string input)
    {
        var status = ParseInternal(input, out var retVal);
        if (status != ParseFunctionResult.Success)
        {
            throw CreateException(status);
        }
        
        return retVal;
    }

    public static bool TryParse(string input, out PlatformIdentifier result)
    {
        var status = ParseInternal(input, out result);
        return status == ParseFunctionResult.Success;
    }
    
    #endregion

    public static PlatformIdentifier Current { get; } = CreateCurrent();
    
    private static PlatformIdentifier CreateCurrent()
    {
        var osId = GetCurrentOsId();
        var archId = GetCurrentArchitecture();

        return new PlatformIdentifier(osId, archId);
    }

    private static PlatformArchitecture GetCurrentArchitecture()
    {
        var realArchValue = (int)RuntimeInformation.ProcessArchitecture + 1;
        return (PlatformArchitecture)realArchValue;
    }
    
    private static PlatformSystem GetCurrentOsId()
    {
        // TODO implement support for musl detection
        if (OperatingSystem.IsLinux())
        {
            return PlatformSystem.Linux;
        }

        if (OperatingSystem.IsFreeBSD())
        {
            return PlatformSystem.FreeBsd;
        }
        
        if (OperatingSystem.IsWindows())
        {
            return PlatformSystem.Windows;
        }

        if (OperatingSystem.IsMacOS())
        {
            return PlatformSystem.Mac;
        }

        return PlatformSystem.Other;
    }
}
