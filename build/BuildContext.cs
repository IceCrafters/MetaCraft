// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using Cake.Common;
using Cake.Core;
using Cake.Frosting;

namespace Build;

public sealed class BuildContext : FrostingContext
{
    public string BuildConfiguration { get; }
    
    public BuildContext(ICakeContext context)
        : base(context)
    {
        BuildConfiguration = context.Argument("configuration", "Release");
    }
}