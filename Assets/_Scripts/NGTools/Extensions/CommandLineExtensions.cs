namespace System
{
    public static class CommandLineExtensions
    {
        private static string[] FLAG_PREFIXES
        { get { return new string[] { @"/", @"-" }; } }

        private static string[] _commandLineArguments = null;
        public static string[] CommandLineArguments
        {
            get
            {
                if (_commandLineArguments == null)
                {
                    _commandLineArguments = Environment.GetCommandLineArgs();
                }

                return _commandLineArguments;
            }

        }

        /// <summary>
        /// Gets the command line argument for a given flag.
        /// For example if your command line is 
        ///     foo.exe /U bar
        /// Calling this with "U" as the argumentFlag
        /// will return "bar"
        /// </summary>
        /// <param name="argumentFlag">
        /// The argument flag to check. Set FLAG_PREFIXES to contain
        /// the characters you want to use as prefixes to the flags.
        /// </param>
        /// <returns>
        /// The string on the command line following the argumentFlag
        /// </returns>
        public static string GetCommandLineArgParameter(string argumentFlag)
        {
            string[] flagVariants = new string[FLAG_PREFIXES.Length];
            for (int i = 0; i < flagVariants.Length; i++)
            {
                flagVariants[i] = FLAG_PREFIXES[i] + argumentFlag.ToLower();
            }

            for (int i = 0; i < CommandLineArguments.Length; i++)
            {
                if (flagVariants.Contains(CommandLineArguments[i]) &&
                    i + 1 < CommandLineArguments.Length)
                {
                    return CommandLineArguments[i + 1];
                }

            }

            return "";
        }

        /// <summary>
        /// Given a command line argument flag, this will check to see if the
        /// parameter for that flag is an existing directory, if not it will
        /// check to see if it is a directory in the current command line's path.
        /// </summary>
        /// <param name="directoryArgumentFlag">
        /// Argument flag (i.e. 'd') that indicates a path should follow (i.e. '/d c:\users').
        /// </param>
        /// <returns>
        /// Correct directory if possible.
        /// </returns>
        public static string InferDirectory(string directoryArgumentFlag)
        {
            string directory = GetCommandLineArgParameter(directoryArgumentFlag);
            if (!string.IsNullOrEmpty(directory))
            {
                if (IO.Directory.Exists(directory))
                {
                    return IO.Path.GetFullPath(directory);
                }
            }

            return "";
        }
    }
}
