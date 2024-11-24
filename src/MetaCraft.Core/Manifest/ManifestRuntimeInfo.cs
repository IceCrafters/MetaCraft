// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-LicenseIdentifier: GPL-3.0-or-later

namespace MetaCraft.Core.Manifest;

public sealed class ManifestRuntimeInfo
{
    public bool ManagedOnly { get; init; }
    public IList<AssemblyExportDeclaration>? ExportAssemblies { get; init; }
}