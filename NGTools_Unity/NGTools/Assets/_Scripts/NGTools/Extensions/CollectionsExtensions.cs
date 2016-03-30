namespace System.Collections.Generic
{
    public static class CollectionsExtensions
    {
        public static void AddMany<T>(this List<T> list, params T[] elements)
        {
            list.AddRange(elements);
        }

        public static string ExtendedToString<T>(this List<T> list)
        {
            if (list == null)
                return "NULL List";

            if (list.Count <= 0)
                return "EMPTY List";

            string s = "";
            foreach (var item in list)
            {
                s += item.ToString() + "\n";
            }

            return s;
        }

        public static string ExtendedToString<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
                return "NULL dictionary";

            if (dictionary.Count <= 0)
                return "EMPTY dictionary";

            string s = "";
            foreach (var item in dictionary)
            {
                s += string.Format("{0}: {1}\n",
                    item.Key,
                    (item.Value == null ? "NULL" : item.Value.ToString())
                    );
            }

            return s;
        }
    }
}
