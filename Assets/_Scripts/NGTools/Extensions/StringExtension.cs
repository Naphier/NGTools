// Standard number formatting codes.
// https://msdn.microsoft.com/en-us/library/dwhawy9k(v=vs.110).aspx

using System;
using System.Collections.Generic;
using System.Linq;

public static class StringExtension 
{
    public static string DelimitedFormat(string tag, char delimiter, params object[] parameters)
    {
        string format = "";

        List<string> nonNullParams = new List<string>();
        if (parameters != null)
        {
            foreach (var item in parameters)
            {
                if (item != null)
                {
                    string param = item.ToString();
                    if (!string.IsNullOrEmpty(param))
                        nonNullParams.Add(param);
                }
            }
        }
        for (int i = 0; i < nonNullParams.Count; i++)
        {
            format += "{" + i.ToString() + "}";
            if (nonNullParams.Count > 1 && i < nonNullParams.Count - 1)
            {
                format += delimiter;
            }
        }

        if (!string.IsNullOrEmpty(tag) && nonNullParams.Count > 0)
            tag += " ";

        return string.Format(tag + format, nonNullParams.ToArray());
    }

    public static string DelimitedFormat(char delimiter, params object[] parameters)
    {
        return DelimitedFormat("", delimiter, parameters);
    }

    public static string CSVFormat(string tag, params object[] parameters)
    {
        return DelimitedFormat(tag, ',', parameters);
    }
    

    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;

        if (maxLength < 0)
            return string.Empty;

        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

    public static bool Contains(this string[] array, string valueToCheck, bool ignoreCase = true)
    {
        System.StringComparison comparison = 
            (ignoreCase ?
            System.StringComparison.OrdinalIgnoreCase :
            System.StringComparison.Ordinal);


        for (int i = 0; i < array.Length; i++)
        {
            if (string.Equals(array[i], valueToCheck, comparison))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Random alphanumeric string generator
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string RandomString(int length)
    {
        Random random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
