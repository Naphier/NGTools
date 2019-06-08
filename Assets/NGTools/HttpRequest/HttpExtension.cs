using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NG.HttpClient
{
    public static class HttpExtension
    {
        public static string EscapeHttpUrl(string host, string path, bool https)
        {
            return GetHttpProtocol(https) + host + GetPath(path);
        }

        public static string EscapeHttpUrl(string host, string path, bool https, string user, string password)
        {
            return GetHttpProtocol(https) + user + ":" + password + "@" + host + GetPath(path);
        }

        private static string GetHttpProtocol(bool https)
        {
            if (https)
                return "https://";
            else
                return "http://";
        }

        private static string GetPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string[] split = path.Split('/');

                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = WWW.EscapeURL(split[i]);
                }

                path = "/" + string.Join("/", split);
            }
            return path;
        }

        private const string AUTH = "Authorization";
        public static void SetAuthHeader(ref Dictionary<string,string> headers, string user, string password)
        {
            string auth = "Basic " + System.Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes(user + ":" + password));
            if (headers.ContainsKey(AUTH))
                headers[AUTH] = auth;
            else
                headers.Add(AUTH, auth);
        }

        public static string GetRequestHeader(WWWForm wwwForm)
        {
            string header = "";
            foreach (KeyValuePair<string,string> item in wwwForm.headers)
            {
                header += item.Key + ": " + item.Value + "\n";
            }

            return header;
        }
    }
}
