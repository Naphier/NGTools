using System.Collections.Generic;

namespace UnityEngine
{
    public static class RectExtensions
    {
        public static Rect FullScreen(int x = 0, int y = 0)
        {
            return new Rect(x, y, Screen.width, Screen.height);

        }
    }
}
