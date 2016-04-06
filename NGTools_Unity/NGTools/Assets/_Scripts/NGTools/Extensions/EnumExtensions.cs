namespace System
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts string to enum of type T. T must be an enum
        /// otherwise a 'NotSupportedException' exception is thrown.
        /// </summary>
        public static T ToEnum<T>(this string value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new NotSupportedException("Must be an Enum! type: " + typeof(T).Name);

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
