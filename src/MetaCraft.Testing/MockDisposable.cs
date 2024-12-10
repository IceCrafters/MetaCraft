// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

namespace MetaCraft.Testing;

/// <summary>
/// Provides a pure implementation of <see cref="MockDisposable"/> that does nothing.
/// </summary>
public sealed class MockDisposable : IDisposable
{
    public bool IsDisposed { get; private set; }
    
    public void Dispose()
    {
        // Literally does nothing.
        IsDisposed = true;
    }
}