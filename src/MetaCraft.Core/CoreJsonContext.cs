// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json.Serialization;
using MetaCraft.Archive.References;
using MetaCraft.Common;
using MetaCraft.Common.Platform;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Manifest;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes.Referral;

namespace MetaCraft.Core;

[JsonSerializable(typeof(PlatformIdentifier))]
[JsonSerializable(typeof(AssemblyExportDeclaration))]
[JsonSerializable(typeof(ManifestRuntimeInfo))]
[JsonSerializable(typeof(PackageReference))]
[JsonSerializable(typeof(RangedPackageReference))]
[JsonSerializable(typeof(PackageReferenceDictionary))]
[JsonSerializable(typeof(RangedPackageReferenceDictionary))]
[JsonSerializable(typeof(PackageManifest))]
[JsonSerializable(typeof(Dictionary<string, PackageReference>), TypeInfoPropertyName = "ProvisionMap")]
[JsonSerializable(typeof(SemVersionKey))]
[JsonSerializable(typeof(PackageReferralIndex))]
[JsonSerializable(typeof(PackageReferrerDictionary))]
[JsonSerializable(typeof(ReferralIndexDictionary))]
[JsonSerializable(typeof(Dictionary<SemVersionKey, string>),
    TypeInfoPropertyName = "ReferralPreferenceNode")]
[JsonSerializable(typeof(Dictionary<string, Dictionary<SemVersionKey, string>>), 
    TypeInfoPropertyName = "ReferralPreferenceTree")]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class CoreJsonContext : JsonSerializerContext;