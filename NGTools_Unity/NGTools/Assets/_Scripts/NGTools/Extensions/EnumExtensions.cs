namespace System
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts string to enum of type T. T must be an enum
        /// otherwise an exception is thrown.
        /// </summary>
        public static T ToEnum<T>(this string value) where T : new()
        {
            if (!typeof(T).IsEnum)
                throw new NotSupportedException("T must be an Enum");

            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
