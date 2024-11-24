# Dependency Specification

## Declaration

Packages declares dependencies by providing reference entries to the provisions
they require.

Each reference entry contains a `name` and a `version`, and is stored in the
[`dependencies` field](ManifestSpecs.md#properties) as a dictionary of `name`
to `version`.

## Resolution

When resolving dependencies, the program SHOULD search through this order:

- Packages installed alongside
- Existing Packages

If none can be found matching a provision reference, then the transaction MUST
fail.

### Packages installed alongside

When locating dependencies in packages installed alongside with a package,
their manifest is inspected to determine what provisions they provide. If
a matching provision is found, the package with the matching provision MUST be
installed before the installation of the package that depends on it.

### Existing Packages

When locating dependencies in existing local packages, the [provisions database](ProvisionSpecs.md#database)
is searched through first. If it is invalid, the provisions database MUST be
regenerated, and then this process MUST be restarted.

