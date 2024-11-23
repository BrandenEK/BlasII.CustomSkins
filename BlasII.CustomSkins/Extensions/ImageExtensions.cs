using UnityEngine;

namespace BlasII.CustomSkins.Extensions;

internal static class ImageExtensions
{
    public static Texture2D GetSlicedTexture(this Sprite sprite)
    {
        Vector2Int position = new((int)sprite.rect.x, (int)sprite.rect.y);
        Vector2Int size = new((int)sprite.rect.width, (int)sprite.rect.height);

        Color[] pixels = sprite.texture.Duplicate().GetPixels(position.x, position.y, size.x, size.y);
        var output = new Texture2D(size.x, size.y);
        output.SetPixels(pixels);
        output.Apply();

        Object.Destroy(output);
        return output;
    }

    public static Texture2D Duplicate(this Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);

        Object.Destroy(readableText);
        return readableText;
    }
}
