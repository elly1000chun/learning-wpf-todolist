using System;
using System.IO;

namespace TodoWpf.Tests;

internal static class TestFileHelper
{
    public static string CreateTempFilePath(string fileName)
    {
        var tempFolderPath = Path.Combine(
            Path.GetTempPath(),
            "TodoWpf.Tests",
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(tempFolderPath);

        return Path.Combine(tempFolderPath, fileName);
    }
}
