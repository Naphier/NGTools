using System.IO;
using System;
using System.Text;

public class SimpleCSVDebug
{
    string GetPathFile()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string file = "output.csv";
        return path + @"/" + file;
    }

    StringBuilder output = new StringBuilder();
    bool fileInitialized = false;

    // 10kb approximately 5,000 characters.
    // This is to keep the file writes small.
    // However you don't want to make it so small that 
    // you're writing to the file too often.
    int maxBytesBeforeDump = 10000;
    public void AddLineToOutput(string line)
    {
        if (2 * (output.Length + line.Length) >= maxBytesBeforeDump)
        {
            WriteToLog();

            // Clear the stringbuilder.
            output = new StringBuilder();
        }

        output.AppendLine(line);
    }


    void WriteToLog()
    {
        using (StreamWriter writer = new StreamWriter(GetPathFile(), fileInitialized))
        {
            fileInitialized = true;
            writer.Write(output);
            writer.Close();
            UnityEngine.Debug.Log("Log written to " + GetPathFile());
        }
    }

    public string DelimitedFormat(char delimiter, string formatCode, params object[] parameters)
    {
        string format = "";
        formatCode = (string.IsNullOrEmpty(formatCode) ? "" : ":" + formatCode);
        for (int i = 0; i < parameters.Length; i++)
        {

            format += "{" + i.ToString() + formatCode + "}";
            if (parameters.Length > 1 && i < parameters.Length - 1)
            {
                format += delimiter;
            }
        }

        return string.Format(format, parameters);
    }

    bool finalWrite = false;
    public void FinalWrite()
    {
        // Allow FinalWrite() to be called only once.
        if (finalWrite) return;

        if (output.Length > 0)
            WriteToLog();
        else
            UnityEngine.Debug.Log("output is empty. Nothing to write.");

        finalWrite = true;
    }


    public int OuputSizeInBytes()
    {
        return output.Length * 2;
    }
}
