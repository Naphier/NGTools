using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Generic
{
    [TestFixture]
    [Category("Extensions")]
    public class CollectionsExtensions_Tests
    {
        #region AddMany
        [Test]
        public void AddMany_Normal_Usage()
        {
            List<int> intList = new List<int>();
            intList.AddMany(0, 1, 2, 3);

            for (int i = 0; i < intList.Count; i++)
            {
                Assert.That(intList.Contains(i));
            }
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void AddMany_Null_List_Exception()
        {
            List<int> intList = null;
            intList.AddMany(0, 1, 2, 3);
        }
        #endregion

        #region ExtendedToString<T>(this List<T> list, string delimiter = "\n")
        [Test]
        public void ExtendedToString_List_Test_Normal_Usage_Default_Delimiter()
        {
            List<string> list = new List<string>();
            list.Add("a");
            list.Add("b");
            string expected = "a\nb";
            string actual = list.ExtendedToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExtendedToString_List_Test_Normal_Usage_Custom_Delimiter()
        {
            List<string> list = new List<string>();
            list.Add("a");
            list.Add("b");
            string delimiter = ",";
            string expected = "a" + delimiter + "b";
            string actual = list.ExtendedToString(delimiter);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExtendedToString_List_Test_Null_Delimiter()
        {
            List<string> list = new List<string>();
            list.Add("a");
            list.Add("b");
            string expected = "ab";
            string actual = list.ExtendedToString(null);
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void ExtendedToString_List_Test_Null_List()
        {
            List<string> list = null;
            string expected = CollectionsExtensions.NULL_LIST;
            string actual = list.ExtendedToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExtendedToString_List_Test_Empty_List()
        {
            List<string> list = new List<string>();
            string expected = CollectionsExtensions.EMPTY_LIST;
            string actual = list.ExtendedToString();
            Assert.AreEqual(expected, actual);
        }
        #endregion


        #region ExtendedToString<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, string delimiter = "\n")
        [Test]
        public void ExtendedToString_Dictionary_Test_Normal_Usage_Default_Delimiter()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("a", "1");
            dict.Add("b", "2");
            string expected = "a: 1\nb: 2";
            string actual = dict.ExtendedToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExtendedToString_Dictionary_Test_Normal_Usage_Custom_Delimiter()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("a", "1");
            dict.Add("b", "2");
            string delimiter = ",";
            string expected = "a: 1" + delimiter + "b: 2";
            string actual = dict.ExtendedToString(delimiter);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExtendedToString_Dictionary_Test_Null_Delimiter()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("a", "1");
            dict.Add("b", "2");
            string expected = "a: 1b: 2";
            string actual = dict.ExtendedToString(null);
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void ExtendedToString_Dictionary_Test_Null_Dictionary()
        {
            Dictionary<string, string> dict = null;
            string expected = CollectionsExtensions.NULL_DICTIONARY;
            string actual = dict.ExtendedToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExtendedToString_Dictionary_Test_Empty_Dictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string expected = CollectionsExtensions.EMPTY_DICTIONARY;
            string actual = dict.ExtendedToString();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Random<T>(this IEnumerable<T> enumerable)

        [Test]
        public void Random_From_List_Test_Normal_Usage()
        {
            List<int> intList = new List<int>();
            for (int i = 0; i < 2; i++)
            {
                intList.Add(i);
            }

            int count0 = 0;
            int count1 = 0;
            for (int i = 0; i < 100; i++)
            {
                int random = intList.Random();
                if (random == 0)
                    count0++;
                else if (random == 1)
                    count1++;
                else
                    Assert.Fail(string.Format("Random number '{0}' not in intList", random));
            }

            Assert.That(count0 > 0);
            Assert.That(count1 > 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Random_From_List_Test_Null_List_Exception()
        {
            List<int> intList = null;
            intList.Random();
        }

        #endregion


        #region RandomSelection<T>(this IEnumerable<T> enumerable, int count)
        [Test]
        public void RamdomSelection_Test_Normal_Usage()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 1000; i++)
            {
                list.Add(i);
            }

            int count = 10;

            var randomList = list.RandomSelection(count).ToList();
            Assert.AreEqual(count, randomList.Count);
        }

        #endregion


        #region IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        public void IsNullOrEmpty_List_Test_Null()
        {
            List<int> intList = null;
            bool expected = true;
            bool actual = intList.IsNullOrEmpty();

            Assert.AreEqual(expected, actual);
        }

        public void IsNullOrEmpty_List_Test_Empty()
        {
            List<int> intList = new List<int>();
            bool expected = true;
            bool actual = intList.IsNullOrEmpty();

            Assert.AreEqual(expected, actual);
        }
        #endregion
    }
}
