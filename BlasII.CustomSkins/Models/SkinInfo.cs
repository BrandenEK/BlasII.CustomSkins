
namespace BlasII.CustomSkins.Models;

internal class SkinInfo(string id, string name, string author, string version)
{
    public string Id { get; } = id;

    public string Name { get; } = name;

    public string Author { get; } = author;

    public string Version { get; } = version;
}
