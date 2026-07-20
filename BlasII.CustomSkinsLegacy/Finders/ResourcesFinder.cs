using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlasII.CustomSkins.Finders;

/// <summary>
/// Finds all player sprites using <see cref="Resources"/>
/// </summary>
public class ResourcesFinder : IFinder
{
    /// <inheritdoc/>
    public IEnumerable<Sprite> Result { get; private set; }

    /// <inheritdoc/>
    public IEnumerator FindAll()
    {
        yield return null;
        Result = Resources.FindObjectsOfTypeAll<Sprite>()
            .Where(x => !string.IsNullOrEmpty(x.name))
            .OrderBy(x => x.name);
    }
}
