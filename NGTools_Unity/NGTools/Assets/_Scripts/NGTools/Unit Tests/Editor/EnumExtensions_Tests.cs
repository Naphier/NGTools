using NUnit.Framework;

namespace System
{
    [TestFixture]
    [Category("Extensions")]
    public class EnumExtensions_Tests
    {
        public enum TestEnum { none = 0, a, b, c, d}

        [TestCase("a", Result = TestEnum.a)]
        [TestCase("b", Result = TestEnum.b)]
        [TestCase("c", Result = TestEnum.c)]
        [TestCase("d", Result = TestEnum.d)]
        [TestCase("none", Result = TestEnum.none)]
        [TestCase("not in enum elements", Result = TestEnum.none)]
        [TestCase("A", Result = TestEnum.a)] //case insensitivity
        public TestEnum ToEnum_Test_Normal_Usage(string toConvert)
        {
            return toConvert.ToEnum<TestEnum>();
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void ToEnum_Test_Exception()
        {
            string silly = "sillyString";
            silly.ToEnum<DateTime>();
        }
         
        
    }
}
