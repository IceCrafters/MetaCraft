// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Frosting;

namespace Build;

[TaskName("Restore")]
public class RestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetRestore(MConstants.SolutionPath);
    }
}