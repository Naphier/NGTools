using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NG.HttpClient
{   
    /// <summary>
    /// Base contract for all HttpRequests
    /// </summary>
    public interface IHttpRequest
    {
        event onFinishedDelegate onSuccess;
        event onFinishedDelegate onError;
        event onFinishedDelegate onFinished;
        IEnumerator ExecuteCoroutine();
        void Cancel();
        float GetProgress();
        HttpResult result { get;}
        bool CanBeDestroyed();
    }
    public delegate void onFinishedDelegate(HttpResult result);

    public enum ResultData
    { audioClip, bytes, movie, texture, textureNonReadable, text}
    /// <summary>
    /// Basic HttpRequest abstraction. This should not be used
    /// directly. Concrete HttpRequests should inherit from this
    /// class and must declare how to construct the WWW object.
    /// After instantiating an HttpRequest class you must call the Execute
    /// method to run the request.
    /// 
    /// Options:    
    ///     useHttps - default: false
    ///         Set this to use https protocol.
    ///     timeOut
    ///         Set this to the number of seconds to elapse before forcing
    ///         a timeout.
    ///     retryAttempts - default: 0
    ///         Set this to the number of times to retry the request.
    ///     delayBetweenRetries - default: 0.25f
    ///         Set this to the number of seconds to elapse before a retry is
    ///         attempted.
    ///         
    /// Call Cancel() at any time to cancel and dispose of the request.
    /// 
    /// Call GetProgress() to check on the progress of the download.
    /// 
    /// HandleSuccess and HandleError have been set with defaults to report
    ///     common data about an http request. These methods are virtual so they 
    ///     can be extended or overwritten to serve any needs.
    /// </summary>
    public abstract class HttpRequest : IHttpRequest
    {
        public event onFinishedDelegate onError;
        public event onFinishedDelegate onFinished;
        public event onFinishedDelegate onSuccess;

        public ResultData resultType = ResultData.text;
        public bool useHttps = false;
        public float timeOut = -1f;

        public int retryAttempts = 0;
        public float delayBetweenRetries = 0.25f;

        protected string host;
        protected string filePath;

        private int attempts = 0;

        private bool _cancel = false;
        private bool disposed = false;
        private float startTime;
        private WWWController wwc;
        protected WWW www = null;
        private bool busy = false;
        private bool _canBeDestroyed;
        public bool CanBeDestroyed()
        {
            return _canBeDestroyed;
        }
        protected HttpResult _result;
        public virtual HttpResult result
        {
            get
            {
                if (_result == null)
                    _result = new HttpResult();
                return _result;
            }
        }

        public abstract WWW GetWWW();

        protected HttpRequest(string host , string filePath)
        {
            this.host = host;
            this.filePath = filePath;
        }


        public void Execute()
        {
            if (busy)
                return;

            startTime = Time.time;
            GameObject go = new GameObject("WWWController");
            wwc = go.AddComponent<WWWController>();
            wwc.SetRequest(this);
        }

        public void Cancel()
        {
            _cancel = true;
        }

        public IEnumerator ExecuteCoroutine() 
        {
            busy = true;
            attempts++;
            www = GetWWW();

            bool failed = false;
            while (!www.isDone)
            {
                yield return null;

                result.timeElapsed = Time.time - startTime;

                if (_cancel ||
                    timeOut > 0f && ((result.timeElapsed) > timeOut))
                {
                    if (_cancel)
                    {
                        result.error = StatusMessage.Canceled(host);
                        result.statusCode = StatusCode.CANCELED;
                    }
                    else
                    {
                        result.error = StatusMessage.TimeOut(result.timeElapsed, host);
                        result.statusCode = StatusCode.TIMEOUT;
                    }

                    result.isError = true;
                    result.isSuccess = false;
                    failed = true;
                    disposed = true;
                    www.Dispose();
                    break;
                }
            }

            

            if (!failed)
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    HandleSuccess(result, www);
                }
                else
                {
                    failed = true;        
                }
            }

            if (failed)
                HandleError(result, www);

            busy = false;

            _canBeDestroyed = true;

            if (failed && attempts < retryAttempts)
            {
                _canBeDestroyed = false;
                www = null;
                yield return new WaitForSeconds(delayBetweenRetries);
                startTime = Time.time;
                wwc.Execute();
            }
            else
            {
                if (onFinished != null)
                    onFinished(result);
            }
        }

        public virtual void HandleSuccess(HttpResult result , WWW www)
        {
            result.statusCode = StatusCode.SUCCESS;
            result.isSuccess = true;
            result.isError = false;
            result.response = www.text;
            result.responseHeaders = www.responseHeaders;

            switch (resultType)
            {
                case ResultData.audioClip:
                    result.audioClip = www.GetAudioClip();
                    break;
                case ResultData.bytes:
                    result.bytes = www.bytes;
                    break;
                case ResultData.movie:
                    result.movieTexture = www.GetMovieTexture();
                    break;
                case ResultData.texture:
                    result.texture2d = www.texture;
                    break;
                case ResultData.textureNonReadable:
                    result.texture2d = www.textureNonReadable;
                    break;
            }
            
            if (onSuccess != null)
                onSuccess(result);
        }

        public virtual void HandleError(HttpResult result , WWW www)
        {
            result.statusCode = StatusCode.FAILED;

            if (!disposed)
                result.responseHeaders = www.responseHeaders;

            if (result.responseHeaders.ContainsKey("Status"))
            {
                int code;
                if (int.TryParse(result.responseHeaders["Status"], out code))
                {
                    result.statusCode = code;
                }
            }

            if (result.responseHeaders.ContainsKey("Status-Line"))
            {
                int code;
                if (int.TryParse(result.responseHeaders["Status-Line"], out code))
                {
                    result.statusCode = code;
                }
            }

            result.isSuccess = false;
            result.isError = true;
            if (!disposed)
                result.error = www.error;

            if (!disposed)
                result.response = www.text;


            if (onError != null)
                onError(result);
        }

        public float GetProgress()
        {
            if (!disposed && www != null)
                return www.progress;
            else
                return -1f;
        }
    }


    public enum HttpMethod { GET, POST}
    /// <summary>
    /// Use this convenience class to create requests.
    /// </summary>
    public class Http
    {
        public static HttpRequest Request(string host, string filePath)
        {
            return new HttpGetRequest(host, filePath);   
        }

        public static HttpRequest Request(HttpMethod method, string host, string filePath, string user , string password)
        {
            if (method == HttpMethod.GET)
                return new HttpGetRequestProtected(host, filePath, user, password);
            else
                return new HttpPostRequestProtected(host, filePath, user, password);
        }

        public static HttpRequest Request(string host, string filePath, string user, string password, WWWForm wwwForm)
        {
            return new HttpPostRequestProtected(host, filePath, user, password, wwwForm);
        }

        public static HttpRequest Request(string host, string filePath, WWWForm wwwForm)
        {
            return new HttpPostRequest(host, filePath, wwwForm);
        }

        public static HttpRequest Request(string host, string filePath, WWWForm wwwForm, Dictionary<string,string> headers)
        {
            return new HttpPostRequest(host, filePath, wwwForm, headers);
        }
    }

    

    //TODO
    /*
    DownloadTexture
    DownloadMovie
    DownloadAudio
    DownloadFile
    UploadRawData
    */

}
