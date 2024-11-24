# MetaCraft Package Specification

A MetaCraft package MUST be a `zip` formatted file, that contains a specific 
folder structure.

The package file name SHOULD end with `.metacraft.zip`, however package file
extensions does not affect using MetaCraft to inspect or install the package.

Unless otherwise specified, except for `manifest.json` that defines the
package, all other files and directories are optional.

## File Structure

Items marked with `D:` are directories and those marked with `F:` are single
files.

- F: `manifest.json` - [manifest](ManifestSpecs.md)
- D: `contents` - Package contents
- D: `licences` - LicenceRef files
- D: `config` - MetaCraft configuration system files
  - D: `triggers` - triggers to activate
    - D: (platform-id)
      - F: (trigger-name)
    - D: (os-id)`-any`
      - F: (trigger-name)
  - D: `scripts` - configure scripts
    - D: `install` - Post installation configure scripts
      - F: (platform-id)`-any`
      - F: (os-id)`-any`
    - D: `remove` - Before removal configure scripts
      - F: (platform-id)`-any`
      - F: (os-id)`-any`
  - D: `paths` - path configuration
    - F: `all` - general platform path configuration
    - F: `paths-`(platform-id) - platform path configuration
    - F: `paths-`(os-id)`-any` - OS-specific path configuration

## Versioning

### Cases that Upstream doesn't follow SemVer format

Generally, as long as the rules for the major version are followed, version
numbers can be converted to a three-part format that fits into a Semantic
Version (even if the versioning scheme actually contradicts it).

However, if upstream fails to follow the rules on major version, then
regardless of whether it is a three-part version number, packagers should
review that package carefully to create a SemVer complaint version number with
original version number as a build metadata if required.

Here are the guidances on some specific known version numbers that doesn't
follow SemVer:

- .NET SDKs: Retain the version number as if it is a SemVer version.
- .NET assemblies with legacy versioning: Use first three parts. Add revision
  number to the manifest and update `revision` accordingly.
- JetBrains: Retain the version number.
- PE embedded versioning: Add revision number to manifest and update `revision`
  accordingly.

## Configuration

### Configure scripts

Packages MAY include configure scripts inside the `config/scripts`, either
use the same name as the platform ID to execute on or named `<os>-any` (where
`os` is a valid OS id) for executing on a specific OS regardless of
architecture.

Configure scripts MUST be valid on the specified OS and platform such scripts
are included for.

There exists two types of configure scripts, one ran for post-install tasks and
one ran before removal. After extraction, configure scripts SHOULD be executed;
and before removal, the removal configure scripts MUST be executed if the
respective post-install configure script is ran, and the program MUST abort
the uninstallation process if pre-removal configure script returns nonzero exit
code.

Scripts MUST be passed two arguments:

1. Installation location
2. Version

For the respective platforms, a respective shell interpreter is used for
executing the configure scripts:

| Platform                    | Program    |
|-----------------------------|------------|
| Windows                     | PowerShell |
| Linux, macOS, FreeBSD, etc. | `sh`[^1]   |

### Paths

Packages MAY declare paths in the  `config/paths` file inside the archive. The
file is a UTF-8 text file consisting of a path relative to the package
contents directory.

Packages MAY also include platform path configuration files named
`paths-<platform>` which `<platform>` is the either a full platform ID or 
`paths-<os>-any` (where `os` is a valid OS id). When the exact platform ID one
is found, the any-architecture one is ignored.  Configuration files MUST NOT 
be named `paths-<os>` or any similar IDs that must be resolved to represent 
`paths-<os>-any`.

### Triggers

Packages MAY include trigger scripts that is run when other packages are
installed or removed.

## Invalid Packages

A package that contravenes this specification is not a valid package, and
the program SHOULD reject them, and MUST NOT allow such packages to be
installed into any package scope.

[^1]: The package authors and application bears no responsibility if this `sh`
      is not a POSIX compatible shell.
