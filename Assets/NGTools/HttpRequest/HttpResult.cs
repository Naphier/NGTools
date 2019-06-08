using System.Collections.Generic;
using UnityEngine;

namespace NG.HttpClient
{
    public static class StatusCode
    {
        public const int SUCCESS = 200;
        public const int TIMEOUT = 1;
        public const int CANCELED = 2;
        public const int FAILED = -1;
    }

    public static class StatusMessage
    {
        public static string TimeOut(float elapsed, string url)
        {
            return "Request timeout (" + (elapsed).ToString() + " s) on URL: " + url;
        }

        public static string Canceled(string url)
        {
            return "Request canceled on URL: " + url;
        }
    }

    public class HttpResult
    {
        public bool isError;
        public bool isSuccess;
        public int statusCode;
        public string error = "";
        public string response = "";
        public Dictionary<string, string> responseHeaders = new Dictionary<string, string>();
        public float timeElapsed = 0f;
        public Texture2D texture2d = null;
        public MovieTexture movieTexture = null;
        public AudioClip audioClip = null;
        public byte[] bytes = null;
        
        public override string ToString()
        {
            string output = string.Format(
                "isError: {0}\n" +
                "isSuccess: {1}\n" +
                "statusCode: {2}\n" +
                "error: {3}\n" +
                "response: {4}\n" +
                "response headers: {5}\n" +
                "time elapsed: {6}",
                isError, isSuccess, statusCode, error, StringExtension.Truncate(response, 256), ResponseHeaders(), timeElapsed);
            return output;
        }

        public string ResponseHeaders()
        {
            string output = "";
            foreach (KeyValuePair<string, string> item in responseHeaders)
            {
                output += item.Key + ": " + item.Value + "\n";
            }
            return output;
        }
    }
}
