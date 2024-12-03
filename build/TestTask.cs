// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Test;
using Cake.Frosting;

namespace Build;

[TaskName("Test")]
[IsDependentOn(typeof(BuildTask))]
public class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetTest(MConstants.SolutionPath, new DotNetTestSettings
        {
            NoBuild = true
        });
    }
}