
namespace BlasII.CustomSkins.Finders;

/// <summary>
/// Handles finding all player sprites from the game
/// </summary>
public interface IFinder
{
    /// <summary>
    /// Finds all player sprites
    /// </summary>
    public SpriteCollection FindAll();
}
