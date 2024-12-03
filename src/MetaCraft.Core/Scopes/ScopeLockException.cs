// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Locales;

namespace MetaCraft.Core.Scopes;

public class ScopeLockException : Exception
{
    private ScopeLockException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    public static ScopeLockException CreateIoError(Exception cause)
    {
        return new ScopeLockException(Lc.L("Failed to lock the lock file: {0}", cause.Message), cause);
    }

    public static ScopeLockException CreateUnauthorizedAccess(Exception cause)
    {
        return new ScopeLockException(Lc.L("Unable to gain access for lock file: {0}", cause.Message), cause);
    }
}
