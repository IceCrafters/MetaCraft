// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;

namespace MetaCraft.Core.Scopes.Referral;

public class JsonFileReferralDatabaseStore : IReferralDatabaseStore
{
    public JsonFileReferralDatabaseStore(string root)
    {
        Root = root;

        Directory.CreateDirectory(Root);
    }

    private string Root { get; }

    public void Clear()
    {
        if (!Directory.Exists(Root))
        {
            return;
        }

        Directory.Delete(Root, true);
        Directory.CreateDirectory(Root);
    }

    public ISerialed GetSerialFile()
    {
        return new SerialFile(Path.Combine(Root, SerialFile.CommonFileName));
    }

    public ReferralIndexDictionary? ReadFile(string packageName)
    {
        var filePath = GetPathOf(packageName);

        if (!File.Exists(filePath))
        {
            return null;
        }
        
        using var stream = File.OpenRead(filePath);
        return JsonSerializer.Deserialize(stream, CoreJsonContext.Default.ReferralIndexDictionary);
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
