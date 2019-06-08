using NUnit.Framework;

namespace System
{
    [TestFixture]
    [Category("Extensions")]
    public class EnumExtensions_Tests
    {
        public enum TestEnum { none = 0, a, b, c, d}

        [TestCase("a", ExpectedResult = TestEnum.a)]
        [TestCase("b", ExpectedResult = TestEnum.b)]
        [TestCase("c", ExpectedResult = TestEnum.c)]
        [TestCase("d", ExpectedResult = TestEnum.d)]
        [TestCase("none", ExpectedResult = TestEnum.none)]
        [TestCase("not in enum elements", ExpectedResult = TestEnum.none)]
        [TestCase("A", ExpectedResult = TestEnum.a)] //case insensitivity
        public TestEnum ToEnum_Test_Normal_Usage(string toConvert)
        {
            return toConvert.ToEnum<TestEnum>();
        }

        [Test]
        public void ToEnum_Test_Exception()
        {
            string silly = "sillyString";
            Assert.Throws(typeof(NotSupportedException), () => { silly.ToEnum<DateTime>(); });
        }
    }
}
