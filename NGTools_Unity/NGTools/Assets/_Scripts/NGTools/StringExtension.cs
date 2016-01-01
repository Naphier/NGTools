// Standard number formatting codes.
// https://msdn.microsoft.com/en-us/library/dwhawy9k(v=vs.110).aspx

using System;
using System.Linq.Expressions;

public static class StringExtension 
{
    public static string DelimitedFormat(char delimiter, params object[] parameters)
    {
        return DelimitedFormat("", delimiter, parameters);
    }

    public static string CSVFormat(string tag, params object[] parameters)
    {
        return DelimitedFormat(tag, ',', parameters);
    }


    public static string DelimitedFormat(string tag, char delimiter, params object[] parameters)
    {
        string format = "";
        for (int i = 0; i < parameters.Length; i++)
        {
            
            format += "{" + i.ToString() + "}";
            if (parameters.Length > 1 && i < parameters.Length - 1)
            {
                format += delimiter;
            }
        }

        if (!string.IsNullOrEmpty(tag))
            tag += " ";

        return string.Format(tag + format, parameters);
    }


    // Works only at top level
    public static string GetName<T>(Expression<Func<T>> expr)
    {
        var body = ((MemberExpression)expr.Body);
        return body.Member.Name;
    }
}
