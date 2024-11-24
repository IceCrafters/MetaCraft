<!-- SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com> -->
<!-- SPDX-License-Identifier: GPL-3.0-or-later -->

# MetaCraft Scope Specification

## Container

A package container is a directory storing all of its packages, and it MUST
be `<scope-root>/packages`. The container directory is then the container root.

### Locking

Before doing any write operation, the program MUST _create_ a file and lock the
file. If it is unable to create and lock the file, the program MUST abort all
operations and return an error code.

The program is RECOMMENDED to put some sort of data into the lock file (like
a timestamp). However, this data MUST be 8 bytes long, and MUST be located at
the beginning of the file.

If the operating system does not provide an ability to lock a file, then it is
permitted to hold a write access stream / file descriptor open instead.

For reference, a lock is considered open if the lock is currently not held and
may be written into.

The lock file is `<root>/lock` where `<root>` is the root of the scope
directory.

### Package Location

Package location MUST be constructed accordingly:

- Container root
- Package ID
- Package version (or if unitary, `unitary`)

The program MUST deal with all directory and file names that aren't package
contents as if file and directory names are case insensitive, even if the
platform it runs on is case sensitive.

### Insertion

A package is inserted into the scope by extracting all of its files into its
package location.

### Towing

When overwriting a new package into a package location where a package already
resides, the latter SHOULD be "towed away" by moving them into a towing area.

The towing area used by the reference implementation is:

- Container root
- Package ID
- `backup_`(random filename)

Whenever the towing area is inside the "package ID" directory it MUST NOT be a
valid Semantic Version nor be `unitary`.

If any error happens in the overwriting and insertion process, the new package
directory is deleted and the towing area is moved back to where it were
residing at.
