// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Net.Mail;
using System.Text.Json.Serialization;
using MetaCraft.Common.Json;

namespace MetaCraft.Archive;

public sealed class PackageLabel
{
    [JsonConverter(typeof(MailAddressCollectionConverter))]
    public MailAddressCollection? Authors { get; init; }
    
    [JsonConverter(typeof(MailAddressCollectionConverter))]
    public MailAddressCollection? Maintainers { get; init; }
    
    public string? Description { get; init; }
    
    public string? License { get; init; }
}
