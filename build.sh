#!/bin/sh
# SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
# SPDX-License-Identifier: GPL-3.0-or-later

dotnet run --project ./build/Build.csproj -- "$@"
