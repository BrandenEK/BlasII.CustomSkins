using UnityEngine;

namespace BlasII.CustomSkins;

/// <summary>
/// Represents a 2D point
/// </summary>
public struct Vector
{
    /// <summary>
    /// The x coordinate
    /// </summary>
    public float X { get; set; }
    /// <summary>
    /// The y coordinate
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Creates a vector with default values
    /// </summary>
    public Vector()
    {
        X = 0;
        Y = 0;
    }

    /// <summary>
    /// Creates a vector with the specified values
    /// </summary>
    public Vector(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Converts a Vector to a Vector2
    /// </summary>
    public static implicit operator Vector2(Vector v) => new Vector2(v.X, v.Y);
    /// <summary>
    /// Converts a Vector2 to a Vector
    /// </summary>
    public static implicit operator Vector(Vector2 v) => new Vector(v.x, v.y);
}
