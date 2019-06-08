using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NG.HttpClient
{
    /// <summary>
    /// Standard GET request.
    /// </summary>
    public class HttpGetRequest : HttpRequest
    {
        public HttpGetRequest(string host, string filePath) : base(host, filePath) { }

        public override WWW GetWWW()
        {
            if (www == null)
            {
                www = new WWW(HttpExtension.EscapeHttpUrl(host, filePath, useHttps));
            }
            return www;
        }
    }


    /// <summary>
    /// GET request for a file in a protected directory.
    /// Not a strongly secure method as the URL is passed with the credentials as a plain string.
    /// Use HttpPostRequestProtected for a more secure method.
    /// </summary>
    public class HttpGetRequestProtected : HttpRequest
    {
        private string user, password;
        public HttpGetRequestProtected(string host, string filePath, string user, string password) : base(host, filePath)
        {
            this.user = user;
            this.password = password;
        }

        public override WWW GetWWW()
        {
            if (www == null)
            {
                www = new WWW(HttpExtension.EscapeHttpUrl(host, filePath, useHttps, user, password));
            }
            return www;
        }
    }

}
