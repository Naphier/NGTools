namespace System.Text
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendFormatLine
            (this StringBuilder stringBuilder, string format, params object[] args)
        {
            return stringBuilder.AppendLine(string.Format(format, args));
        }
    }
}
