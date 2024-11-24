// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

namespace MetaCraft.Core.Platform;

public enum PlatformArchitecture
{
    Any,
    /// <summary>An Intel-based 32-bit processor architecture.</summary>
    X86,
    /// <summary>An Intel-based 64-bit processor architecture.</summary>
    X64,
    /// <summary>A 32-bit ARM processor architecture.</summary>
    Arm,
    /// <summary>A 64-bit ARM processor architecture.</summary>
    Arm64,
    /// <summary>The S390x platform architecture.</summary>
    S390X,
    /// <summary>A LoongArch64 processor architecture.</summary>
    LoongArch64,
    /// <summary>A 32-bit ARMv6 processor architecture.</summary>
    ArmV6,
    /// <summary>A PowerPC 64-bit (little-endian) processor architecture.</summary>
    PPc64Le,
}
