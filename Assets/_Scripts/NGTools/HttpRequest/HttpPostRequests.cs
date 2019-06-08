using UnityEngine;
using System.Collections.Generic;

namespace NG.HttpClient
{
    /// <summary>
    /// Standard POST request.
    /// </summary>
    public class HttpPostRequest : HttpRequest
    {
        private WWWForm wwwForm = null;
        private Dictionary<string, string> headers = null;

        public HttpPostRequest(string host, string filePath, WWWForm wwwForm) : base(host, filePath)
        {
            this.wwwForm = wwwForm;
        }

        public HttpPostRequest(string host, string filePath, WWWForm wwwForm, Dictionary<string,string> headers) : base(host, filePath)
        {
            this.wwwForm = wwwForm;
            this.headers = headers;
        }

        public override WWW GetWWW()
        {
            if (www == null)
            {
                wwwForm.AddField("dummyfield", "dummyvalue");

                if (headers == null)
                    www = new WWW(HttpExtension.EscapeHttpUrl(host, filePath, useHttps), wwwForm);
                else
                    www = new WWW(HttpExtension.EscapeHttpUrl(host, filePath, useHttps), wwwForm.data , headers);
            }
            return www;
        }
    }


    /// <summary>
    /// Use this to make requests (POST or GET) to protected server directories.
    /// </summary>
    public class HttpPostRequestProtected : HttpRequest
    {
        private string user, password;
        private WWWForm wwwForm;
        public HttpPostRequestProtected(string host, string filePath, string user, string password) : base(host, filePath)
        {
            this.user = user;
            this.password = password;

            wwwForm = new WWWForm();
        }

        public HttpPostRequestProtected(string host, string filePath, string user, string password, WWWForm wwwForm) : base(host, filePath)
        {
            this.wwwForm = wwwForm;
            this.user = user;
            this.password = password;
        }

        public override WWW GetWWW()
        {
            if (www == null)
            {
                wwwForm.AddField("dummyfield", "dummyvalue");
                Dictionary<string, string> headers = wwwForm.headers;
                HttpExtension.SetAuthHeader(ref headers, user, password);
                string url = HttpExtension.EscapeHttpUrl(host, filePath, useHttps);
                www = new WWW(url, wwwForm.data, headers);
            }
            return www;
        }
    }

}
