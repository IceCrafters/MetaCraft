// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

namespace MetaCraft.Core.Scopes;

public interface ISerialed
{
    /// <summary>
    /// If implemented, compares the serial file of the current instance to the specified serial
    /// file.
    /// </summary>
    /// <param name="serial">The serial file to compare to.</param>
    /// <returns><see langword="true"/> if the serial data equals; otherwise, <see langword="false"/>.</returns>
    bool CompareSerialWith(SerialFile serial);

    void CopySerial(SerialFile from);
}
