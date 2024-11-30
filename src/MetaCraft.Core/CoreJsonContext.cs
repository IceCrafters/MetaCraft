// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json.Serialization;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Manifest;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes.Referral;
using MetaCraft.Core.Serialization;

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
[JsonSerializable(typeof(SemVersionKey))]
[JsonSerializable(typeof(PackageReferralIndex))]
[JsonSerializable(typeof(PackageReferrerDictionary))]
[JsonSerializable(typeof(ReferralIndexDictionary))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class CoreJsonContext : JsonSerializerContext;