# Shared Lc classes

The shared `Lc` class found in `src/shared` directory is used by the Frontend
and the Core module.

## Requirements

The `Lc` class requires `JetBrains.Annotations` and `GetText.NET` packages to
compile.

## Usage

To include the `Lc` class, add a link in the project to it - do not copy it and
most certainly do not move it.

Then, add a new `.cs` file somewhere in the project; we'll name it
`Lc.Config.cs` here. This file should appear like this:

```csharp
namespace MetaCraft.Localisation;

internal partial class Lc
{
    private const string LocaleDirectory = "locale";
    private const string LocaleModule = "frontend";
}
```

The only stuff you can modify here is the values of the `LocaleDirectory` and
`LocaleModule` constants. Additionally, you can add `#pragma` stuff to disable
the `namespace` warning.

<!-- SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com> -->
<!-- SPDX-License-Identifier: GPL-3.0-or-later -->
