// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Locales;

namespace MetaCraft.Core.Archive;

public class InvalidPackageException : Exception
{
    public InvalidPackageException()
    {
    }

    public InvalidPackageException(string? message) : base(message)
    {
    }

    public InvalidPackageException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public static InvalidPackageException CreateNoManifest()
    {
        return new InvalidPackageException(Lc.L("The specified package does not have a manifest."));
    }

    public static InvalidPackageException CreateInvalidManifest()
    {
        return new InvalidPackageException(Lc.L("The manifest file for the specified package is invalid."));
    }

    public static InvalidPackageException CreateManifestNotFile()
    {
        return new InvalidPackageException(Lc.L("The entry with the manifest filename in the specified package is not a file."));
    }
}
