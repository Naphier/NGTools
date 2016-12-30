using System.Linq;

namespace System.Collections.Generic
{
    public static class CollectionsExtensions
    {
        public static void AddMany<T>(this List<T> list, params T[] elements)
        {
            list.AddRange(elements);
        }

        public const string NULL_LIST = "NULL List";
        public const string EMPTY_LIST = "EMPTY List";
        public static string ToStringDelimited<T>(this List<T> list, string delimiter = "\n")
        {
            if (list == null)
                return NULL_LIST;

            if (list.Count <= 0)
                return EMPTY_LIST;

            string s = "";
            for (int i = 0; i < list.Count; i++)
            {
                s += list[i].ToString();
                if (i < list.Count - 1)
                    s += delimiter;
            }

            return s;
        }


        public const string NULL_DICTIONARY = "NULL Dictionary";
        public const string EMPTY_DICTIONARY = "EMPTY Dictionary";
        public static string ToStringDelimited<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, string delimiter = "\n")
        {
            if (dictionary == null)
                return NULL_DICTIONARY;

            if (dictionary.Count <= 0)
                return EMPTY_DICTIONARY;

            string s = "";
            int index = 0;
            int count = dictionary.Count;

            foreach (var item in dictionary)
            {
                s += string.Format("{0}: {1}{2}",
                    item.Key,
                    (item.Value == null ? "NULL" : item.Value.ToString()),
                    (index < count - 1 ? delimiter : "")
                    );
                index++;
            }

            return s;
        }

        public static T GetRandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.OrderBy(e => Guid.NewGuid()).FirstOrDefault();
        }


        public static IEnumerable<T> GetRandomSelection<T>(this IEnumerable<T> enumerable, int count)
        {
            count = enumerable.Count() < count ? enumerable.Count() : count;
            return enumerable.OrderBy(e => Guid.NewGuid()).Take(count);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }
    }
}
