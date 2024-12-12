// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive;
using MetaCraft.Archive.References;
using MetaCraft.Core.Projection;

namespace MetaCraft.Testing;

public sealed class NoOpProjectionSpace : IProjectionSpace
{
    public bool Exists(AssemblyExportDeclaration declaration)
    {
        return false;
    }

    public void Insert(PackageManifest fromPackage, AssemblyExportDeclaration declaration, string fromFile, bool overwrite)
    {
        // do nothing
    }
    
    public void Delete(AssemblyExportDeclaration declaration)
    {
        // do nothing
    }

    public PackageReference? GetProjectionSource(AssemblyExportDeclaration declaration)
    {
        return null;
    }
}