namespace System.IO
{
    public class FileHandling
    {
        /// <summary>
        /// Saves a temp file to the same directory. Returns that file name.
        /// If the temp file already exists it will be overwritten.
        /// Throws exception if parameter is null. 
        /// </summary>
        public static string SaveTempFile(string originalFilePath)
        {
            if (string.IsNullOrEmpty(originalFilePath))
                throw new ArgumentNullException("originalFilePath");

            string newFilePath = null;
            if (File.Exists(originalFilePath))
            {
                newFilePath = Path.GetDirectoryName(originalFilePath) + "/" + Path.GetFileNameWithoutExtension(originalFilePath) + ".temp";
                File.Copy(originalFilePath, newFilePath, true);
            }

            return newFilePath;
        }
    }
}
