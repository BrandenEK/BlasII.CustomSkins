using BlasII.CustomSkins.Extensions;
using BlasII.CustomSkins.Models;
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
        yield return null;
        Result = Resources.FindObjectsOfTypeAll<Sprite>()
            .Where(x => !string.IsNullOrEmpty(x.name))
            .DistinctBy(x => x.name)
            .OrderBy(x => x.name)
            .ToDictionary(x => x.name, x => x)
            .AsCollection();
    }
}
