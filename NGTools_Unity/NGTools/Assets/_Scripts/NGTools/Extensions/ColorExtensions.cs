using System;
using System.Globalization;

namespace UnityEngine
{
    public static class ColorExtensions
    {
        public static bool debugLogMessagesOn = true;

        public static string HexString(Color aColor)
        {
            return HexString((Color32)aColor, false);
        }

        public static string HexString(Color aColor,
                bool includeAlpha)
        {
            return HexString((Color32)aColor, includeAlpha);
        }

        public static string HexString(Color32 aColor,
                bool includeAlpha)
        {
            String rs = Convert.ToString(aColor.r, 16).ToUpper();
            String gs = Convert.ToString(aColor.g, 16).ToUpper();
            String bs = Convert.ToString(aColor.b, 16).ToUpper();
            String a_s = Convert.ToString(aColor.a, 16).ToUpper();
            while (rs.Length < 2) rs = "0" + rs;
            while (gs.Length < 2) gs = "0" + gs;
            while (bs.Length < 2) bs = "0" + bs;
            while (a_s.Length < 2) a_s = "0" + a_s;
            if (includeAlpha) return "#" + rs + gs + bs + a_s;
            return "#" + rs + gs + bs;
        }

        public static Color MakeColor(string aStr)
        {
            Color clr = new Color(0, 0, 0);
            if (aStr != null && aStr.Length > 0)
            {
                try
                {
                    int commaCount = 0;
                    foreach (char c in aStr)
                    {
                        if (c == ',') commaCount++;
                    }

                    if (aStr.Substring(0, 1) == "#")
                    {  // #FFFFFF format
                        string str = aStr.Substring(1, aStr.Length - 1);
                        clr.r = int.Parse(str.Substring(0, 2),
                            NumberStyles.AllowHexSpecifier) / 255.0f;
                        clr.g = int.Parse(str.Substring(2, 2),
                            NumberStyles.AllowHexSpecifier) / 255.0f;
                        clr.b = int.Parse(str.Substring(4, 2),
                            NumberStyles.AllowHexSpecifier) / 255.0f;
                        if (str.Length == 8)
                        {
                            clr.a = int.Parse(str.Substring(6, 2),
                               NumberStyles.AllowHexSpecifier) / 255.0f;
                        }
                        else clr.a = 1.0f;
                    }
                    else if (commaCount >= 2 && commaCount <= 3 && aStr.Length >= 2 * commaCount + 1)
                    {  // 0.3, 1.0, 0.2 format
                        int p0 = 0;
                        int p1 = 0;
                        int c = 0;
                        p1 = aStr.IndexOf(",", p0);
                        
                        if (p1 <= 0)
                            throw new FormatException();

                        while (p1 > p0 && c < 4)
                        {
                            float value = float.Parse(aStr.Substring(p0, p1 - p0));
                            value = Mathf.Clamp(value, 0, 1);
                            clr[c++] = value;
                            p0 = p1 + 1;
                            if (p0 < aStr.Length) p1 = aStr.IndexOf(",", p0);
                            if (p1 < 0) p1 = aStr.Length;
                        }
                        if (c < 4) clr.a = 1.0f;
                    }
                    else
                        throw new FormatException();
                }
                catch (Exception e)
                {
                    if (debugLogMessagesOn)
                        Debug.LogError("MakeColor could not convert " + aStr + " to Color. " + e);
                    return Color.magenta;
                }
            }
            else
                return Color.magenta;

            return clr;
        }

        // HSB calculations are not quite right. 
        // Values are typically h:0-360, s:0-100, b:0-100, and a:0-1
        // Extinguished until I actually need to use them.
        /*
        public static Vector3 MakeHSB(Color c)
        {
            c = c.Clamp();

            float minValue = Mathf.Min(c.r, Mathf.Min(c.g, c.b));
            float maxValue = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
            float delta = maxValue - minValue;
            float h = 0.0f;
            float s = 0.0f;
            float b = maxValue;

            // # Calculate the hue (in degrees of a circle, between 0 and 360)
            if (maxValue == c.r)
            {
                if (c.g >= c.b)
                {
                    if (delta == 0.0f) h = 0.0f;
                    else h = 60.0f * (c.g - c.b) / delta;
                }
                else if (c.g < c.b)
                {
                    h = 60.0f * (c.g - c.b) / delta + 360f;
                }
            }
            else if (maxValue == c.g)
            {
                h = 60.0f * (c.b - c.r) / delta + 120f;
            }
            else if (maxValue == c.b)
            {
                h = 60.0f * (c.r - c.g) / delta + 240f;
            }

            // Calculate the saturation (between 0 and 1)
            if (maxValue == 0.0) s = 0.0f;
            else s = 1.0f - (minValue / maxValue);
            return new Vector3(h / 360.0f, s, b);
        }

        public static Vector4 MakeHSBA(Color c)
        {
            c = c.Clamp();

            float minValue = Mathf.Min(c.r, Mathf.Min(c.g, c.b));
            float maxValue = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
            float delta = maxValue - minValue;
            float h = 0.0f;
            float s = 0.0f;
            float b = maxValue;

            // # Calculate the hue (in degrees of a circle, between 0 and 360)
            if (maxValue == c.r)
            {
                if (c.g >= c.b)
                {
                    if (delta == 0.0f) h = 0.0f;
                    else h = 60.0f * (c.g - c.b) / delta;
                }
                else if (c.g < c.b)
                {
                    h = 60.0f * (c.g - c.b) / delta + 360f;
                }
            }
            else if (maxValue == c.g)
            {
                h = 60.0f * (c.b - c.r) / delta + 120f;
            }
            else if (maxValue == c.b)
            {
                h = 60.0f * (c.r - c.g) / delta + 240f;
            }

            // Calculate the saturation (between 0 and 1)
            if (maxValue == 0.0) s = 0.0f;
            else s = 1.0f - (minValue / maxValue);
            return new Vector4(h / 360.0f, s, b, c.a);
        }

        public static Color MakeColor(Vector3 hsb)
        {
            return MakeColor(new Vector4(hsb.x, hsb.y, hsb.z, 1.0f));
        }

        public static Color MakeColor(Vector4 hsba)
        {
            // When saturation = 0, then r, g, b represent grey value (= brightness (z)).
            float r = hsba.z;
            float g = hsba.z;
            float b = hsba.z;
            if (hsba.y > 0.0f)
            {  // saturation > 0
               // Calc sector
                float secPos = (hsba.x * 360.0f) / 60.0f;
                int secNr = Mathf.FloorToInt(secPos);
                float secPortion = secPos - secNr;

                // Calc axes p, q and t
                float p = hsba.z * (1.0f - hsba.y);
                float q = hsba.z * (1.0f - (hsba.y * secPortion));
                float t = hsba.z * (1.0f - (hsba.y * (1.0f - secPortion)));

                // Calc rgb
                if (secNr == 1)
                {
                    r = q;
                    g = hsba.z;
                    b = p;
                }
                else if (secNr == 2)
                {
                    r = p;
                    g = hsba.z;
                    b = t;
                }
                else if (secNr == 3)
                {
                    r = p;
                    g = q;
                    b = hsba.z;
                }
                else if (secNr == 4)
                {
                    r = t;
                    g = p;
                    b = hsba.z;
                }
                else if (secNr == 5)
                {
                    r = hsba.z;
                    g = p;
                    b = q;
                }
                else {
                    r = hsba.z;
                    g = t;
                    b = p;
                }
            }
            return new Color(r, g, b, hsba.w);
        }
        */
        public static Color Clamp(this Color c)
        {
            for (int i = 0; i <= 3; i++)
            {
                if (float.IsNaN(c[i]) ||
                    float.IsNegativeInfinity(c[i]))
                    c[i] = 0;
                else if (float.IsPositiveInfinity(c[i]))
                    c[i] = 1;
                else
                    c[i] = Mathf.Clamp(c[i], 0, 1);
            }

            return c;
        }
    }
}
