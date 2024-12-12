// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Runtime.InteropServices;

namespace MetaCraft.Core.Platform.Interop;

internal static partial class Kernel32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SecurityAttributes
    {
        public uint Length;
        public IntPtr SecurityDescriptor;
        public int InheritHandle;
    }

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    internal static partial int CreateHardLinkW(string lpFileName,
        string lpExistingFileName,
        SecurityAttributes lpSecurityAttributes);
}