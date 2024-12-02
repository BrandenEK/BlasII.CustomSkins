using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlasII.CustomSkins.Finders;

/// <summary>
/// Handles finding all player sprites from the game
/// </summary>
public interface IFinder
{
    /// <summary>
    /// The sprites that were found
    /// </summary>
    public IEnumerable<Sprite> Result { get; }

    /// <summary>
    /// Finds all player sprites
    /// </summary>
    public IEnumerator FindAll();
}
