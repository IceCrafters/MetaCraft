// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;
using MetaCraft.Common;
using MetaCraft.Core.Platform;
using MetaCraft.Localisation;
using Semver;
using ReferralPreferenceTree = System.Collections.Generic.Dictionary<
    string, 
    System.Collections.Generic.Dictionary<
        MetaCraft.Common.SemVersionKey, 
        string>
>;

namespace MetaCraft.Core.Scopes.Referral;

public class JsonFileReferralPreferenceProvider : IReferralPreferenceProvider
{
    private readonly ReferralPreferenceTree _data;
    private readonly string _fileName;
    
    public JsonFileReferralPreferenceProvider(string fileName)
    {
        _fileName = fileName;
        _data = LoadFile();
    }

    private ReferralPreferenceTree LoadFile()
    {
        if (!File.Exists(_fileName))
        {
            return CreateNewData();
        }

        ReferralPreferenceTree? retVal;
        
        try
        {
            using var stream = File.OpenRead(_fileName);
            retVal = JsonSerializer.Deserialize(stream, 
                CoreJsonContext.Default.ReferralPreferenceTree);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            retVal = CreateNewData();
        }

        return retVal ?? CreateNewData();
    }

    private ReferralPreferenceTree CreateNewData()
    {
        var retVal = new ReferralPreferenceTree();
        SaveData(retVal);
        return retVal;
    }

    private void SaveData(ReferralPreferenceTree data)
    {
        try
        {
            using var stream = File.Create(_fileName);
            JsonSerializer.Serialize(stream, data, CoreJsonContext.Default.ReferralPreferenceTree);
        }
        catch (Exception e)
        {
            GlobalOutput.Warning(e, Lc.L("Failed to save data"));
        }
    }

    public string? GetPreferredId(string name, SemVersion version)
    {
        return _data.GetValueOrDefault(name)
            ?.GetValueOrDefault(new SemVersionKey(version));
    }

    public void SetPreferredId(string clauseId, SemVersion clauseVersion, string preferred)
    {
        var clause = _data.GetOrAdd(clauseId, () => []);
        clause[new SemVersionKey(clauseVersion)] = preferred;
        SaveData(_data);
    }
}