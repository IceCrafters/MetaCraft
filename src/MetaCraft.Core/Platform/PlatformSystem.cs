// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-LicenseIdentifier: GPL-3.0-or-later

namespace MetaCraft.Core.Platform;

public enum PlatformSystem
{
    /// <summary>
    /// Any platform is selectable.
    /// </summary>
    Any,
    /// <summary>
    /// A Windows operating system, or an implementation of the Win32 API.
    /// </summary>
    Windows,
    /// <summary>
    /// A Linux-based operating system with the GNU C Library.
    /// </summary>
    Linux,
    /// <summary>
    /// A Linux-based operating system with the musl libc.
    /// </summary>
    MuslLinux,
    /// <summary>
    /// A FreeBSD operating system.
    /// </summary>
    FreeBsd,
    /// <summary>
    /// A macOS operating system.
    /// </summary>
    Mac,
    /// <summary>
    /// A platform system which IceCraft is unable to natively support. Only <c>any</c> packages
    /// can be installed.
    /// </summary>
    Other,
}