// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json.Serialization;
using MetaCraft.Common.Json;
using Semver;

namespace MetaCraft.Archive;

public readonly struct AssemblyExportDeclaration
{
    public AssemblyExportDeclaration(string assemblyName, string path)
    {
        Name = assemblyName;
        Path = path;
    }
    
    /// <summary>
    /// Gets the assembly name of the assembly that is exported.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Gets the path, relative to the root of the package installation directory,
    /// to the assembly that is exported.
    /// </summary>
    public required string Path { get; init; }
    
    /// <summary>
    /// Gets the version of the assembly that is exported.
    /// </summary>
    [JsonConverter(typeof(SemVersionConverter))]
    public required SemVersion Version { get; init; }
}
