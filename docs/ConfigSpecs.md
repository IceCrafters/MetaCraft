# Configuration System Specification

## Paths

Package PATH configurations are stored in the package archive and the installed
package information. See [Package Specification](PackageSpecs.md).

### Discovery

Packages MAY contain general and platform specific path configuration files.
The program MUST look for the following configuration files specifically:

- the general path configuration file (`/config/paths/all`)
- the platform specific configuration file (`/config/paths/<platform-id>`)
- the OS specific configuration file (`/config/paths/<os-id>-any`)

### Application

When any transaction is completed, the program SHOULD update the
`<scope-root>/config/paths` with the package paths.

The paths MUST be applied in the following order (files loaded later overrides
those loaded before them), from bottom to top:

- the platform specific configuration file (`/config/paths/<platform-id>`)
- the OS specific configuration file (`/config/paths/<os-id>-any`)
- the general path configuration file (`/config/paths/all`)

And then, combined into a "larger" configuration instance then applied to the
system.

The exact way to apply `PATH` additions varies by platform. MetaCraft should
not automate this, except to create scripts to include in shell profiles.
Under POSIX platforms, the program SHOULD prompt the user to include profile
scripts created by IceCraft in their shell; and under Windows, the program
SHOULD prompt the user to include the `PATH` variable additions required by
packages.

## Triggers

Triggers are stored under this directory structure:

```plain
<scope-root>/config/triggers/<platform>/<pkg-id>-<pkg-version|unitary>/<trigger-name>
```

### Platforms

- On Linux, triggers are executed with `sh` - a POSIX shell, this is usually
  `bash` or `dash`.
- On Windows, triggers are executed with PowerShell. This can mean:
  - `pwsh` if available.
  - Otherwise, uses `powershell` (therefore, triggers SHOULD support Windows PowerShell).

### Execution

Whenever—

- A package has been successfully removed; or,
- A package has been successfully installed,

relevant triggers are executed immediately. The triggers executed are:

- The triggers with the same platform ID as the current platform.
- The triggers with the platform ID of the same OS and `any` architecture.

All triggers MUST be run even if other triggers fail.
