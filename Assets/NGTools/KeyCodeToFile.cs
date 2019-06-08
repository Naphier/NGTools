using System;
using UnityEngine;
using System.IO;

namespace NG
{
    public class KeyCodeToFile
    {
        public static void Write(string filePath)
        {
            using (StreamWriter w = new StreamWriter(filePath))
            {
                for (int i = 0; i <= 329; i++)
                {
                    string name = ((KeyCode)i).ToString();
                    int z = 0;
                    if (!int.TryParse(name, out z) && name.ToLower() != "none")
                    {
                        w.WriteLine((KeyCode)i);
                    }
                }
            }
        }

    }
}
