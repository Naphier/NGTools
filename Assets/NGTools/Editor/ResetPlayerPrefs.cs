using UnityEngine;
using UnityEditor;

public class ResetPlayerPrefs : Editor
{
    [MenuItem("Edit/Reset PlayerPrefs")]
    static void DoIt()
    {
        PlayerPrefs.DeleteAll();
    }
}