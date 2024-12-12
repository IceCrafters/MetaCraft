// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive;
using MetaCraft.Archive.References;
using Semver;

namespace MetaCraft.Core.Projection;

public interface IProjectionSpace
{
    bool Exists(AssemblyExportDeclaration declaration);
    
    void Insert(PackageManifest fromPackage, AssemblyExportDeclaration declaration, string fromFile, bool overwrite);
    
    void Delete(AssemblyExportDeclaration declaration);
    
    PackageReference? GetProjectionSource(AssemblyExportDeclaration declaration);
}