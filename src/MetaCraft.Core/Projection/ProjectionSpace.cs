// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive;
using MetaCraft.Archive.References;
using MetaCraft.Core.Platform;
using MetaCraft.Localisation;
using Semver;

namespace MetaCraft.Core.Projection;

public class ProjectionSpace : IProjectionSpace
{
    private readonly string _rootPath;

    public ProjectionSpace(string rootPath)
    {
        _rootPath = rootPath;
    }

    public bool Exists(AssemblyExportDeclaration declaration)
    {
        var fullPath = Path.Combine(_rootPath, GetRelativePathOf(declaration));
        return File.Exists(fullPath);
    }

    public void Insert(PackageManifest fromPackage,
        AssemblyExportDeclaration declaration,
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

        var sourcePath = Path.Combine(parent!, "from_package");
        File.WriteAllLines(sourcePath, [ fromPackage.Id, fromPackage.Version.ToString() ]);
    }

    public void Delete(AssemblyExportDeclaration declaration)
    {
        var fullPath = Path.Combine(_rootPath, GetRelativePathOf(declaration));
        File.Delete(fullPath);
    }

    public PackageReference? GetProjectionSource(AssemblyExportDeclaration declaration)
    {
        var fullPath = Path.Combine(_rootPath, GetRelativePathOf(declaration));
        var sourceFile = Path.Combine(Path.GetDirectoryName(fullPath)!,
            "from_package");

        string? packageName = null;
        SemVersion? version = null;
        var ln = 0;
        
        // Parse the file.
        try
        {
            foreach (var line in File.ReadLines(sourceFile))
            {
                if (line.StartsWith('#') || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                ln++;
                if (ln == 1)
                {
                    packageName = line;
                }

                if (ln == 2)
                {
                    version = SemVersion.Parse(line);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            GlobalOutput.Warning(ex, Lc.L("Failed to read from_package file"));
            return null;
        }

        // Either one is null, seem as invalid
        if (packageName == null || version == null)
        {
            return null;
        }

        return new PackageReference(packageName, version);
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