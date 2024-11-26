﻿using System.Collections;

namespace BlasII.CustomSkins.Finders;

/// <summary>
/// Handles finding all player sprites from the game
/// </summary>
public interface IFinder
{
    /// <summary>
    /// The sprites that were found
    /// </summary>
    public SpriteCollection Result { get; }

    /// <summary>
    /// Finds all player sprites
    /// </summary>
    public IEnumerator FindAll();
}