<!-- SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com> -->
<!-- SPDX-License-Identifier: GPL-3.0-or-later -->

# MetaCraft Manifest Specification

MetaCraft packages contains a `manifest.json` that is read by MetaCraft during
inspection and installation.

## Example

```json
{
    "id": "package-id",
    "version": "0.0.1",
    "platform": "linux-x64",
    "revision": 0,
    "packageTime": "2024-11-20T17:00:00Z",
    "unitary": false,
    "runtime": {
        "managedOnly": true,
        "exportAssemblies": [
            {
                "name": "Package.Info",
                "version": "1.0.0",
                "path": "lib/netstandard2.0"
            }
        ]
    },
    "dependencies": {
      "example-lib": ">=1.0.0"
    },
    "conflictsWith": {
      "old-package": "*"
    },
    "provides": {
      "package": "0.0.1"
    },
    "label": {
        "authors": [
            "Parry <parry@contoso.com>"
        ],
        "maintainers": [
            "Packagers <packagers@example.com>"
        ],
        "description": "Package description",
        "license": "Apache-2.0"
    }
}
```

## Property Types

- DateTime: A valid ISO8601 date and time representation. Can either be local
  or UTC.
- Int32: A 32-bit integer expressed in JSON as a _Number_.
- Package ID: A _String_ containing a valid package ID (that is, must be
  entirely consisting of `a-z`, `A-Z`, `0-9`, dashes and underscores).
- Table: an object with arbitrary keys of the specified type and a matching
  value for each key. Must be specified with (_Key_ to _Value_). In
  deserialisation process, this gets interperted as a Dictionary.
- Version: A _String_ containing a valid version number that complies with the
  Semantic Version 2.0 specification.

## Properties

Unless otherwise specified, fields are REQUIRED and cannot be `null`.

- `id`: _Package ID_ - the ID of this package.
- `version`: _String_ - must be a SemVer 2.0 version number.

  To cause an upgrade in cases of upstream version not changed, `revision`
  MUST be used instead; per SemVer 2.0, build metadata doesn't affect ordering.

  Also check the guidance [when upstream doesn't follow SemVer](/PackageSpecs.md#Versioning-NoSemVer).
- `revision`: _Int32_ - (optional) the package revision number. Defaults to 0.

  Revision affects how packages are sorted for the purposes of rebuilding, etc.
- `platform`: _String_ - must be a valid platform identifier.
- `packageTime`: _DateTime_ - the time when the package was created
- `unitary`: _Boolean_ - if `true`, only one version of this package can exist at the same time.
- `runtime`: _Object_ (optional)
  - `managedOnly`: _Boolean_ - is the package solely consisting of managed code
  - `exportAssemblies`: _List_ of _Object_ (optional)
    - `name`: The assembly name that was exported.
    - `version`: The version of the assembly that was exported.
    - `path`: The path, relative to the package root, of the assembly to export.
- `dependencies`: _Table_ (_Package ID_ to _Version Ranges_, optional) - packages that this package depends on
- `conflictsWith`: _Table_ (_Package ID_ to _Version Ranges_, optional) - packages that this package cannot coexist with
- `provides`: _Table_ (_String_ to _Version_, optional) - provisions that this package provides
- `label`: _Object_ (optional)
  - `authors`: _Array_ of _String_ - must be mailbox addresses. (optional)
  - `maintainers`: _Array_ of _String_ - must be mailbox addresses. (optional)
  - `description`: _String_ - a description that is displayed to the user (optional)
  - `license`: _String_ - SPDX license expression, or a LicenseRef corresponding to a file in the `licences` directory in the archive. (optional)
