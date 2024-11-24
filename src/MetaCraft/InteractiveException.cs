// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-LicenseIdentifier: GPL-3.0-or-later

namespace MetaCraft;

public class InteractiveException : Exception
{
    public InteractiveException(string message) : base(message)
    {
    }
}