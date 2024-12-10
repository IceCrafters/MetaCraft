// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;

namespace MetaCraft.Testing;

/// <summary>
/// Provides an in-memory implementation of <see cref="ISerialed"/> that can be used for testing
/// purposes. 
/// </summary>
public class InMemorySerialFile : ISerialed
{
    private long _value;
    private readonly bool _readOnly;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="InMemorySerialFile"/> class.
    /// </summary>
    /// <param name="value">The serial value to begin with.</param>
    /// <param name="readOnly">If <see langword="true"/>, <see cref="CopySerial"/> will not modify the serial value.</param>
    public InMemorySerialFile(long value, bool readOnly)
    {
        _value = value;
        _readOnly = readOnly;
    }
    
    public bool CompareSerialWith(SerialFile serial)
    {
        return serial.ReadSerial() == _value;
    }

    public void CopySerial(ISerialed from)
    {
        if (!_readOnly)
        {
            _value = from.GetSerial();
        }
    }

    public long GetSerial()
    {
        return _value;
    }
}