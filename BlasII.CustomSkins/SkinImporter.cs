using BlasII.ModdingAPI;
using System.IO;

namespace BlasII.CustomSkins;

internal class SkinImporter
{
    // Will return a list of full skin data in the future
    public void LoadAllSkins(string directory)
    {
        Directory.CreateDirectory(directory);

        ModLog.Warn("Found folders");
        foreach (string path in Directory.GetDirectories(directory))
        {
            ModLog.Info(path);
        }
    }
}
