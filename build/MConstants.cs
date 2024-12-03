// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System;
using System.IO;

namespace Build;

public static class MConstants
{
    private static readonly string BasePath = Path.GetDirectoryName(Environment.CurrentDirectory)!;
    private static readonly string SourcePath = Path.Combine(BasePath, "src");
    
    public static readonly string I10NPath = Path.Combine(BasePath, "i10n");
    public static readonly string SolutionPath = Path.Combine(SourcePath, "MetaCraft.sln");
}