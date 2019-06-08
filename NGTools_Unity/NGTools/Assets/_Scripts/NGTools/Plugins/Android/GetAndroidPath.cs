using UnityEngine;
using System.IO;

namespace NG.Android
{
    public enum SpecialFolder
    {
        Download,
        Music,
        Pictures,
        DCIM
    }

    public class GetAndroidPath : MonoBehaviour
    {

        /// <summary>
        /// Returns null if path not found.
        /// </summary>
        /// <returns></returns>
        public static string ExternalStoragePath()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                using (AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject activity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (AndroidJavaClass storage = new AndroidJavaClass("com.naplandgames.Storage"))
                        {
                            string path = storage.CallStatic<string>("GetExternalStoragePath", activity);

                            if (Directory.Exists(path))
                                return path;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns null if External storage path not found otherwise returns the path.
        /// Will create the path if it doesn't exist and 'createIfNotFound' is set to true.
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <param name="createIfNotFound"></param>
        /// <returns></returns>
        public static string SpecialPath(SpecialFolder specialFolder, bool createIfNotFound = false)
        {
            string storagePath = ExternalStoragePath();
            if (storagePath == null)
                return null;

            string path = Path.Combine(storagePath, specialFolder.ToString());

            if (!Directory.Exists(path) && createIfNotFound)
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
