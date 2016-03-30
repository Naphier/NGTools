namespace UnityEngine
{
    public class TextureExtensions
    {
        public static Texture2D MakePixel(Color color)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }
    }
}
