// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json.Serialization;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Manifest;
using MetaCraft.Core.Platform;

namespace MetaCraft.Core;

[JsonSerializable(typeof(PlatformIdentifier))]
[JsonSerializable(typeof(AssemblyExportDeclaration))]
[JsonSerializable(typeof(ManifestRuntimeInfo))]
[JsonSerializable(typeof(PackageReference))]
[JsonSerializable(typeof(ProvisionReference))]
[JsonSerializable(typeof(PackageReferenceDictionary))]
[JsonSerializable(typeof(ProvisionReferenceDictionary))]
[JsonSerializable(typeof(PackageManifest))]
[JsonSerializable(typeof(Dictionary<string, PackageReference>), TypeInfoPropertyName = "ProvisionMap")]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class CoreJsonContext : JsonSerializerContext;