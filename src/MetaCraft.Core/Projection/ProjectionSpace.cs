// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive;
using MetaCraft.Core.Platform;
using MetaCraft.Localisation;

namespace MetaCraft.Core.Projection;

public class ProjectionSpace : IProjectionSpace
{
    private readonly string _rootPath;

    public ProjectionSpace(string rootPath)
    {
        _rootPath = rootPath;
    }

    public void Insert(AssemblyExportDeclaration declaration,
        string fromFile, 
        bool overwrite)
    {
        var fullPath = Path.Combine(_rootPath, GetRelativePathOf(declaration));
        if (File.Exists(fullPath) && !overwrite)
        {
            throw new InvalidOperationException(Lc.L("Assembly '{0}' ({1}) already exists.",
                declaration.Name,
                declaration.Version));
        }
        
        var parent = Path.GetDirectoryName(fullPath);
        
        Directory.CreateDirectory(parent!);
        Project(fromFile, fullPath, overwrite);
    }

    private static void Project(string from, string to, bool overwrite)
    {
        if (overwrite && File.Exists(to))
        {
            File.Delete(to);
        }
        
        try
        {
            PlatformUtil.CreateHardLink(from, to);
        }
        catch
        {
            File.Copy(from, to);
        }
    }
    
    public static string GetRelativePathOf(AssemblyExportDeclaration declaration)
    {
        // shorthand
        var dsc = Path.DirectorySeparatorChar;
        var lowerName = declaration.Name.ToLowerInvariant();
        
        var realPath = declaration.Path.Replace('/', dsc).ToLowerInvariant();
        return $"{lowerName}{dsc}{declaration.Version}{dsc}{realPath}{dsc}{declaration.Name}.dll";
    }
}