// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using JetBrains.Annotations;
using MetaCraft.Localisation;

namespace MetaCraft.Core.Platform;

internal static class PlatformErrors
{
    internal static IOException CreateAlreadyExists(string path)
    {
        return new IOException($"There is a file or directory already existing at '{path}'.");
    }

    internal static IOException TooManyHardLinks(string path)
    {
        return new IOException($"The amount of hard links of '{path}' exceeded the limit.");
    }

    internal static IOException OutOfSpace(string path)
    {
        return new IOException($"The file system of '{path}' is full and cannot be extended.");
    }

    internal static ArgumentException LinkToDirectory([InvokerParameterName] string linkTarget)
    {
        return new ArgumentException(Lc.L("The link target is a directory, which is not allowed."), linkTarget);
    }

    internal static UnauthorizedAccessException ReadOnlyFileSystem()
    {
        return new UnauthorizedAccessException(Lc.L("The file system is read-only."));
    }

    internal static ArgumentException LinkAcrossVolumes([InvokerParameterName] string linkTarget)
    {
        return new ArgumentException(Lc.L("Linking across values is not permitted."), linkTarget);
    }
}