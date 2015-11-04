using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace NG.AssertDebug
{
    public class DebugToFile
    {
        /// <summary>
        /// Add strings to this to dump all into a file at a later time
        /// </summary>
        public static List<string> dOut = new List<string>();

        /// <summary>
        /// Add strings to this to dump all into a file at a later time
        /// Only for use by NG.Assert and NG.Debug
        /// </summary>
        public static List<string> dOutDebugAssert = new List<string>();

        /// <summary>
        /// Writes all dOut strings to file
        /// </summary>
        public static void WriteToLog(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (string s in dOut)
                {
                    sw.WriteLine(s);
                }
                sw.Close();
            }

            dOut = new List<string>();
        }


        /// <summary>
        /// For use only in NG.Assert and NG.Debug
        /// </summary>
        public static void WriteToLogAssertDebug(string fileName)
        {
            bool writeHeader = false;
            if (!File.Exists(fileName))
                writeHeader = true;

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                if (writeHeader)
                    sw.WriteLine(AssertDebugConfig.instance.debugFileHeader);

                foreach (string s in dOutDebugAssert)
                {
                    sw.WriteLine(s);
                }
                sw.Close();
            }

            dOutDebugAssert = new List<string>();
        }

        /// <summary>
        /// Writes to output.csv - must add strings to dOut list
        /// </summary>
        public static void WriteToLog()
        {
            WriteToLog(Application.dataPath + "/output.csv");
        }

        /// <summary>
        /// Writes a single line to log file - for use only with NG.Assert and NG.Debug
        /// </summary>
        public static void WriteLineToLogAssertDebug(string fileName, string line)
        {
            bool writeHeader = false;
            if (!File.Exists(fileName))
                writeHeader = true;

            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                if (writeHeader)
                    sw.WriteLine(AssertDebugConfig.instance.debugFileHeader);

                sw.WriteLine(line);
                sw.Close();
            }
        }

        /// <summary>
        /// Writes a single line to log file
        /// </summary>
        public static void WriteLineToLog(string fileName, string line)
        {
            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine(line);
                sw.Close();
            }
        }

        public static string CleanLine(string line)
        {
            line = line.Replace("\n", " ");
            line = line.Replace("\r", " ");
            line = line.Replace("\t", " ");
            return line;
        }

        public static void HandleLogCallback(string logString, string stackTrace, LogType type)
        {
#pragma warning disable 0162
#pragma warning disable 0429
            if (AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.none)
                return;

            string line = AssertDateStamp.LogEntryTime() + "\t" +
                            type.ToString() + "\t" +
                            DebugToFile.CleanLine(logString) + "\t" +
                            DebugToFile.CleanLine(stackTrace);

            if (type == LogType.Assert)
            {
                if (AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.all ||
                    AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.assertOnly)
                {
                    DebugToFile.WriteLineToLogAssertDebug(AssertDebugConfig.instance.GetDebugFileName(), line);
                }
                else if (AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.assertFailureOnly ||
                            AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.allFailureOnly)
                {
                    DebugToFile.dOutDebugAssert.Add(line);
                }
            }
            else
            {
                if (AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.all ||
                    AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.assertOnly)
                {
                    DebugToFile.WriteLineToLogAssertDebug(AssertDebugConfig.instance.GetDebugFileName(), line);
                }
                else if (AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.debugErrorOnly ||
                            AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.allFailureOnly)
                {
                    DebugToFile.dOutDebugAssert.Add(line);
                }
            }
#pragma warning restore
        }
    }
}
