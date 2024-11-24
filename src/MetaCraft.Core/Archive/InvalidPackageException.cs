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
        return new InvalidPackageException(Strings.PackageArchiveMissingManifest);
    }

    public static InvalidPackageException CreateInvalidManifest()
    {
        return new InvalidPackageException(Strings.PackageArchiveInvalidManifest);
    }

    public static InvalidPackageException CreateManifestNotFile()
    {
        return new InvalidPackageException(Strings.PackageArchiveManifestNotFile);
    }
}
