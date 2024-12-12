// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive;

namespace MetaCraft.Core.Manifest;

public sealed class ManifestRuntimeInfo
{
    public bool ManagedOnly { get; init; }
    public IList<AssemblyExportDeclaration>? ExportAssemblies { get; init; }
}
