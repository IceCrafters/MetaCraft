// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using Cake.Frosting;

namespace Build;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(BuildTask))]
public class DefaultTask : FrostingTask
{
}

[TaskName("Auto")]
[IsDependentOn(typeof(TestTask))]
public class AutoTask : FrostingTask
{
}