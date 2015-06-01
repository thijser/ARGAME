//----------------------------------------------------------------------------
// <copyright file="RXSolutionFixer.cs" company="MattRix">
//     Taken from GitHub Gists.
//     
//     Modified to indicate its purpose (documentation was missing in the original).
//     
//     See https://gist.github.com/MattRix/daf67d66227c61501397
// </copyright>
//----------------------------------------------------------------------------
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

/// <summary>
/// Modifies all Visual Studio project files to use .NET framework 4.0 instead of 3.5.
/// </summary>
public class RXSolutionFixer : AssetPostprocessor
{
    /// <summary>
    /// Fixes all .csproj files in the current working directory.
    /// <para>
    /// This method is automatically called by Unity whenever the solution file is updated.
    /// </para>
    /// </summary>
    public static void OnGeneratedCSProjectFiles() // secret method called by unity after it generates the solution
    {
        string currentDir = Directory.GetCurrentDirectory();
        string[] csprojFiles = Directory.GetFiles(currentDir, "*.csproj");

        foreach (var filePath in csprojFiles)
        {
            FixProject(filePath);
        }
    }

    /// <summary>
    /// Fixes the C# project file at the given path.
    /// </summary>
    /// <param name="filePath">The path to the C# project file.</param>
    /// <returns>True if the file was successfully fixed, false otherwise.</returns>
    public static bool FixProject(string filePath)
    {
        string content = File.ReadAllText(filePath);

        string searchString = "<TargetFrameworkVersion>v3.5</TargetFrameworkVersion>";
        string replaceString = "<TargetFrameworkVersion>v4.0</TargetFrameworkVersion>";

        if (content.IndexOf(searchString) != -1)
        {
            content = Regex.Replace(content, searchString, replaceString);
            File.WriteAllText(filePath, content);
            return true;
        }
        else
        {
            return false;
        }
    }
}