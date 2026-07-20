using MelonLoader;

namespace BlasII.CustomSkins;

internal class Main : MelonMod
{
    public static CustomSkins CustomSkins { get; private set; }

    public override void OnLateInitializeMelon()
    {
        CustomSkins = new CustomSkins();
    }
}