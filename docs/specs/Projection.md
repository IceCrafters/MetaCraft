# Projection

Projection is the process of creating a hard link (or a copy) to an assembly
contained in a package in a projection space that allows .NET to look up for
assemblies that can be references.

This can conserve disk space.

## Projection space format

- **DIR**: (`name`)
  - **DIR**: (`version`)
    - (directories in `path`)
      - **FIL**: (`name`.dll)
      - **FIL**: `from_package`

The projection space is constructed according to the `exportAssemblies` field 
of the [manifest](../ManifestSpecs.md) of each package.

The `from_package` file consists of two lines; the first is the package ID and
the second is the package version. Empty lines and lines starting with `#` are
ignored.

<!-- SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com> -->
<!-- SPDX-License-Identifier: GPL-3.0-or-later -->

## Insertion

Each assembly must be inserted accordingly with the projection space format and
corresponding `from_package` file MUST be created. If a projection without a
`from_package` file is encountered, then that projection is treated as if it
does not belong to any package and MAY be overwritten however the program may
like.

### Conflict

Two exported assemblies conflict if they have the same `name`, `version` and
`path`.

If the package that is being installed (***new package***) is a newer version
of the package that is already inside the package scope (***old package***),
_and_ both new and old package are unitary, then the assembly exported in the
***new package*** MAY overwrite the assembly provided in the ***old package***.

### Overwrite

To overwrite an assembly, the program MUST first try to move the directory of
the old assembly. If access is denied (either due to permission or in use),
the configuration MUST fail.
