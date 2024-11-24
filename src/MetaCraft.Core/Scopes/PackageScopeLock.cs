// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Buffers.Binary;

namespace MetaCraft.Core.Scopes;

public class PackageScopeLock : IDisposable, IAsyncDisposable
{
    private readonly FileStream _stream;

    private PackageScopeLock(FileStream stream)
    {
        _stream = stream;
    }

    internal static PackageScopeLock Create(string filePath)
    {
        try
        {
            var stream = File.Create(filePath);
            
            // Do some really primitive write, write something meaningful.
            // Implementors: Unix epoch is also ok
            Span<byte> buffer = stackalloc byte[sizeof(long)];
            BinaryPrimitives.WriteInt64LittleEndian(buffer, DateTime.UtcNow.ToFileTimeUtc());
            stream.Write(buffer);

            if (!OperatingSystem.IsFreeBSD()
                && !OperatingSystem.IsMacOS())
            {
                // Additionally lock the file.
                stream.Lock(0, sizeof(long));
            }

            return new PackageScopeLock(stream);
        }
        catch (IOException io)
        {
            throw ScopeLockException.CreateIoError(io);
        }
        catch (UnauthorizedAccessException ua)
        {
            throw ScopeLockException.CreateUnauthorizedAccess(ua);
        }
    }

    public void Dispose()
    {
        if (!OperatingSystem.IsFreeBSD()
            && !OperatingSystem.IsMacOS())
        {
            // Additionally unlock the file.
            _stream.Unlock(0, sizeof(long));
        }
        _stream.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _stream.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
