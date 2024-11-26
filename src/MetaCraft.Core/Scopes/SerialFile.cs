// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Buffers.Binary;
using MetaCraft.Core.Locales;

namespace MetaCraft.Core.Scopes;

/// <summary>
/// Provides a shared implementation on the "serial file" concept that indicates whether a pair
/// of data is up-to-date to each other.
/// </summary>
public class SerialFile
{
    public const string CommonFileName = "serial.dat";

    private readonly string _fileName;

    public SerialFile(string fileName)
    {
        _fileName = fileName;
    }

    public bool CompareSerialWith(SerialFile serialFile)
    {
        var serialA = ReadSerial();
        var serialB = serialFile.ReadSerial();

        if (serialA == -1 || serialB == -1)
        {
            return false;
        }

        return serialA == serialB;
    }

    public void WriteSerial(long value)
    {
        try
        {
            using var stream = File.Create(_fileName);

            Span<byte> buffer = stackalloc byte[sizeof(long)];
            BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
        }
        catch (Exception ex)
        {
            Console.WriteLine(Strings.ResourceManager.GetString("ContainerSerialUpdateFailed"));
#if DEBUG
            Console.WriteLine(ex);
#endif
        }        
    }

    public long ReadSerial()
    {
        if (!File.Exists(_fileName))
        {
            return -1;
        }

        try
        {
            using var stream = File.OpenRead(_fileName);

            Span<byte> buffer = stackalloc byte[sizeof(long)];
            if (stream.Read(buffer) != sizeof(long))
            {
                return -1;
            }

            return BinaryPrimitives.ReadInt64LittleEndian(buffer);
        }
        catch (Exception)
        {
            return -1;
        }
    }

    public void Refresh()
    {
        var epoch = (long)Math.Floor((DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds);
        WriteSerial(epoch);
    }
}
