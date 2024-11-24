// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-LicenseIdentifier: GPL-3.0-or-later

using MetaCraft.Core.Locales;

namespace MetaCraft.Core.Scopes;

public class ScopeLockException : Exception
{
    private ScopeLockException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    public static ScopeLockException CreateIoError(Exception cause)
    {
        return new ScopeLockException(string.Format(Strings.ScopeLockIOError, cause.Message), cause);
    }

    public static ScopeLockException CreateUnauthorizedAccess(Exception cause)
    {
        return new ScopeLockException(string.Format(Strings.ScopeLockUnauthorizedAccess, cause.Message), cause);
    }
}