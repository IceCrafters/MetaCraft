// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Frosting;

namespace Build;

[TaskName("Build")]
[IsDependentOn(typeof(RestoreTask))]
[IsDependentOn(typeof(RegenerateLanguageTask))]
public class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetBuild(Path.Combine(MConstants.SolutionPath), new DotNetBuildSettings
        {
            Configuration = context.BuildConfiguration,
            NoRestore = true
        });
    }
}