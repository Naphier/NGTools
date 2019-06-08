using NUnit.Framework;

namespace UnityEngine
{
    [TestFixture]
    [Category("Extensions")]
    public class ColorExtensions_Tests
    {
        #region Test Case Sources
#pragma warning disable 0414
        static object[] HexStringCasesNoAlpha =
        {
            new object[] {1,0,0,1,false,"#FF0000"},
            new object[] {1,1,1,1,false,"#FFFFFF"},
            new object[] {0,0,0,1,false,"#000000"},
            new object[] {100,100,100,1,false,"#FFFFFF"},
            new object[] {-100,-100,-100,1,false,"#000000"},
        };

        static object[] HexStringCasesAlphas =
        {
            new object[] {1,0,0,1,true,"#FF0000FF"},
            new object[] {1,1,1,1,true,"#FFFFFFFF"},
            new object[] {0,0,0,0,true,"#00000000"},
            new object[] {100,100,100,100,true,"#FFFFFFFF"},
            new object[] {-100,-100,-100,-100,true, "#00000000" }
        };


        static object[] MakeColorHexStringCases =
        {
            new object[] {"#FF0000FF", Color.red},
            new object[] {"#FFFFFFFF", Color.white},
            new object[] {"#00000000", new Color(0,0,0,0)},
            new object[] {"#00000080", new Color(0,0,0,128f/255f)}
        };

        static object[] RGBStringCases =
        {
            new object[] {"1,0,0", Color.red},
            new object[] {"1,1,1", Color.white},
            new object[] {"0,0,0", Color.black},
            new object[] {"100,100,100", Color.white},
            new object[] {"-100,-100,-100", Color.black},
        };

        static object[] RGBAStringCases =
        {
            new object[] {"1,0,0,1", Color.red},
            new object[] {"1,1,1,1", Color.white},
            new object[] {"0,0,0,1", Color.black},
            new object[] {"0,0,0,0.5", new Color(0,0,0,0.5f)},
            new object[] {"100,100,100,100", Color.white},
            new object[] {"-100,-100,-100,-100", new Color(0,0,0,0)},
            new object[] {"0.5,0.5,0.5,0.5", new Color(0.5f,0.5f,0.5f,0.5f)}
        };

        static object[] BadRGBStrings =
        {
            new object[] { "a,b,c"},
            new object[] { "a,b,c,d"},
            new object[] { ",,,"},
            new object[] { ",,,,"},
            new object[] { ","},
            new object[] { ",,,abcd"},
            new object[] { "abcd,,,"},
        };

        static object[] ClampCases =
        {
            new object[] { Color.white, Color.white},
            new object[] { new Color(0,0,0,0), new Color(0,0,0,0)},
            new object[] {
                new Color(float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue),
                Color.white},
            new object[] { new Color(float.MinValue, float.MinValue, float.MinValue, float.MinValue),
                new Color(0,0,0,0)},
            new object[] {
                new Color(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity),
                Color.white},
            new object[] {
                new Color(float.NaN, float.NaN, float.NegativeInfinity, float.NegativeInfinity),
                new Color(0,0,0,0)},
        };

        static object[] MakeHSBColorCases =
        {
            new object[] { Color.black, Vector3.zero},
            new object[] { Color.white, new Vector3(0, 0, 1)}, 
            new object[] { Color.red, new Vector3(0, 1, 1)},
            new object[] { new Color(0, 1, 1), new Vector3(0, 1, 1)},
            new object[] {
                new Color(float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue),
                 new Vector3(0, 0, 1)},
            new object[] { new Color(float.MinValue, float.MinValue, float.MinValue, float.MinValue),
                Vector3.zero},

        };
#pragma warning restore
        #endregion

        #region HexString methods
        [Test, TestCaseSource("HexStringCasesNoAlpha"), TestCaseSource("HexStringCasesAlphas")]
        public void HexString_Test_Color32_IncludeAlpha(float r, float g, float b, float a, bool includeAlpha, string expected)
        {
            Color32 color = new Color(r, g, b, a);
            string actual = ColorExtensions.HexString(color, includeAlpha);
            Assert.AreEqual(expected, actual);
        }


        [Test, TestCaseSource("HexStringCasesNoAlpha"), TestCaseSource("HexStringCasesAlphas")]
        public void HexString_Test_Color_IncludeAlpha(float r, float g, float b, float a, bool includeAlpha, string expected)
        {
            Color color = new Color(r, g, b, a);
            string actual = ColorExtensions.HexString(color, includeAlpha);
            Assert.AreEqual(expected, actual);
        }

        [Test, TestCaseSource("HexStringCasesNoAlpha")]
        public void HexString_Test_Color_NoAlpha(float r, float g, float b, float a, bool includeAlpha, string expected)
        {
            Color color = new Color(r, g, b, a);
            string actual = ColorExtensions.HexString(color);
            Assert.AreEqual(expected, actual);
        }

        [Test, TestCaseSource("MakeColorHexStringCases")]
        public void MakeColor_Test_HexString_Normal_Usage(string hexString, Color expected)
        {
            Color actual = ColorExtensions.MakeColor(hexString);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region MakeColor(string aStr)
        [Test]
        public void MakeColor_Test_Null_Param()
        {
            Color expected = Color.magenta;
            Color actual = ColorExtensions.MakeColor(null);
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void MakeColor_Test_Empty_Param()
        {
            Color expected = Color.magenta;
            Color actual = ColorExtensions.MakeColor("");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeColor_Test_Fail_Hex_Parse()
        {
            string badHex = "#XXXXXX";
            Color expected = Color.magenta;
            // Supress debug log error message during unit tests
            ColorExtensions.debugLogMessagesOn = false;
            Color actual = ColorExtensions.MakeColor(badHex);
            Assert.AreEqual(expected, actual);
        }

        [Test, TestCaseSource("RGBStringCases")]
        public void MakeColor_Test_RBGString_Normal_Usage(string rgb, Color expected)
        {
            Color actual = ColorExtensions.MakeColor(rgb);
            Assert.AreEqual(expected, actual);
        }

        [Test, TestCaseSource("RGBAStringCases")]
        public void MakeColor_Test_RBGAString_Normal_Usage(string rgb, Color expected)
        {
            Color actual = ColorExtensions.MakeColor(rgb);
            Assert.AreEqual(expected, actual);
        }

        [Test, TestCaseSource("BadRGBStrings")]
        public void MakeColor_Test_Fail_RGB_Parse(string badRgbString)
        {
            Color expected = Color.magenta;
            ColorExtensions.debugLogMessagesOn = false;
            Color actual = ColorExtensions.MakeColor(badRgbString);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region color.Clamp()
        [Test, TestCaseSource("ClampCases")]
        public void Clamp_Test(Color color, Color expected)
        {
            Color actual = color.Clamp();
            Assert.AreEqual(expected, actual);
        }
        #endregion


        #region MakeHSB(Color c)
        // Values do not conform to HSB standards
        /*
        [Test, TestCaseSource("MakeHSBColorCases")]
        public void MakeHSB(Color c, Vector3 expected)
        {
            Vector3 actual = ColorExtensions.MakeHSB(c);
            if (actual != expected)
                Assert.Fail("expected: {0}\nactual:{1}", expected, actual);
        }
        */

        #endregion
    }
}
