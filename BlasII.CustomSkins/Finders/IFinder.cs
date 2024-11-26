using System.Collections.Generic;
using UnityEngine;

namespace BlasII.CustomSkins.Finders;

/// <summary>
/// Handles finding all player sprites from the game
/// </summary>
public interface IFinder
{
    /// <summary>
    /// Finds all player sprites
    /// </summary>
    public Dictionary<string, Sprite> FindAll();
}
