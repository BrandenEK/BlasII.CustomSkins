using BlasII.CustomSkins.Extensions;
using BlasII.ModdingAPI;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BlasII.CustomSkins.Finders;

/// <summary>
/// Finds all player sprites using <see cref="Resources"/>
/// </summary>
public class ResourcesFinder : IFinder
{
    /// <inheritdoc/>
    public SpriteCollection Result { get; private set; }

    /// <inheritdoc/>
    public IEnumerator FindAll()
    {
        ModLog.Info("Starting find");
        yield return new WaitForSeconds(1);

        Result = Resources.FindObjectsOfTypeAll<Sprite>()
            .Where(x => x.name.StartsWith("TPO"))
            .DistinctBy(x => x.name)
            .OrderBy(x => x.name)
            .ToDictionary(x => x.name, x => x)
            .AsCollection();
    }
}
