// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;

namespace MetaCraft.Core.Scopes.Referral;

public class JsonFileReferralDatabaseStore : IReferralDatabaseStore
{
    public JsonFileReferralDatabaseStore(string root)
    {
        Root = root;
    }

    public string Root { get; }

    public void Clear()
    {
        if (!Directory.Exists(Root))
        {
            return;
        }

        Directory.Delete(Root, true);
        Directory.CreateDirectory(Root);
    }

    public SerialFile GetSerialFile()
    {
        return new SerialFile(Path.Combine(Root, "serial.dat"));
    }

    public ReferralIndexDictionary? ReadFile(string packageName)
    {
        var filePath = GetPathOf(packageName);

        if (!File.Exists(filePath))
        {
            return null;
        }

        ReferralIndexDictionary? data;
        using (var stream = File.OpenRead(filePath))
        {
            data = JsonSerializer.Deserialize(stream, CoreJsonContext.Default.ReferralIndexDictionary);
        }
        
        if (data == null)
        {
            return null;
        }

        return data;
    }

    public void WriteFile(string packageName, ReferralIndexDictionary data)
    {
        var filePath = GetPathOf(packageName);
        using var stream = File.Create(filePath);
        JsonSerializer.Serialize(stream, data, CoreJsonContext.Default.ReferralIndexDictionary);
    }

    private string GetPathOf(string packageName)
    {
        return Path.Combine(Root,
            $"{packageName}.json");
    }
}
