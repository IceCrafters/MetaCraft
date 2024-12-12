// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive;

namespace MetaCraft.Core.Projection;

public interface IProjectionSpace
{
    void Insert(AssemblyExportDeclaration declaration, string fromFile, bool overwrite);
}