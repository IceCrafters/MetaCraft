// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace MetaCraft.Core.Platform.Interop;

[SupportedOSPlatform("linux")]
[SupportedOSPlatform("freebsd")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static partial class LibC
{
    private const string Lib = "libc.so";

    internal const int EPERM = 1;
    internal const int ENOENT = 2;
    internal const int EIO = 5;
    internal const int EEXIST = 17;
    internal const int EXDEV = 18;
    internal const int EACCES = 20;
    internal const int ENOSPC = 28;
    internal const int EROFS = 30;
    internal const int EMLINK = 31;
    internal const int ENAMETOOLONG = 36;
    
    /// <summary>
    /// Makes a new link to the file named by <paramref name="oldName"/>, under the new name
    /// <paramref name="newName"/>. 
    /// </summary>
    /// <param name="oldName">The file to link to.</param>
    /// <param name="newName">The location of the link.</param>
    /// <returns>Returns <c>0</c> on successful and <c>-1</c> on failure.</returns>
    [LibraryImport(Lib, 
        StringMarshalling = StringMarshalling.Utf8, 
        SetLastError = true)]
    internal static partial int link(string oldName, string newName);
}