using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using System.Net;
using System.Text;
using System.Threading;
using cn.bmob.api;
using cn.bmob.Extensions;

namespace cn.bmob.http
{

    public class Http
    {

        public String Method { get; set; }

        public IDictionary<String, String> Headers { get; set; }

        public String RequestContentType { get; set; }

        public byte[] RequestBodyBytes { get; set; }

        public String RequestBodyString { get; set; }

        public Uri Url { get; set; }

        private TimeOutState _timeoutState;

        /// <summary>
        /// Timeout in milliseconds to be used for the request
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// The number of milliseconds before the writing or reading times out.
        /// </summary>
        public int ReadWriteTimeout { get; set; }

        /// <summary>
        /// Whether to invoke async callbacks using the SynchronizationContext.Current captured when invoked
        /// </summary>
        public bool UseSynchronizationContext { get; set; }

        public RequestAsyncHandle ExecuteAsync(Action<HttpResponse, RequestAsyncHandle> callback)
        {
            var asyncHandle = new RequestAsyncHandle();

            Action<HttpResponse> response_cb = r => ProcessResponse(r, asyncHandle, callback);

#if !PocketPC
            if (UseSynchronizationContext && SynchronizationContext.Current != null)
            {
                var ctx = SynchronizationContext.Current;
                var cb = response_cb;

                response_cb = resp => ctx.Post(s => cb(resp), null);
            }
#endif

            asyncHandle.WebRequest = AsAsync(Method, response_cb);
            return asyncHandle;
        }

        private void ProcessResponse(HttpResponse httpResponse, RequestAsyncHandle asyncHandle, Action<HttpResponse, RequestAsyncHandle> callback)
        {
            callback(httpResponse, asyncHandle);
        }

        public HttpWebRequest AsAsync(string method, Action<HttpResponse> callback)
        {
            HttpWebRequest webRequest = null;
            try
            {
                webRequest = ConfigureAsyncWebRequest(method, Url);
                PreparePostBody(webRequest);

                WriteRequestBodyAsync(webRequest, callback);
            }
            catch (Exception ex)
            {
                ExecuteCallback(CreateErrorResponse(ex), callback);
            }

            return webRequest;
        }

        private void PreparePostBody(HttpWebRequest webRequest)
        {
            webRequest.ContentType = RequestContentType;
        }

        private HttpResponse CreateErrorResponse(Exception ex)
        {
            var response = new HttpResponse();
            var webException = ex as WebException;
            if (webException != null && webException.Status == WebExceptionStatus.RequestCanceled)
            {
                response.ResponseStatus = _timeoutState.TimedOut ? ResponseStatus.TimedOut : ResponseStatus.Aborted;
                return response;
            }

            response.ErrorMessage = ex.Message;
            response.ErrorException = ex;
            response.ResponseStatus = ResponseStatus.Error;
            return response;
        }

        private HttpWebRequest ConfigureAsyncWebRequest(string method, Uri url)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            AppendHeaders(webRequest);

            webRequest.Method = method;

            ////req.ServicePoint.Expect100Continue = false;
            ////req.Timeout = Configuration.REQUEST_TIMEOUT;
            // req.KeepAlive = true;

            // make sure Content-Length header is always sent since default is -1
#if !WINDOWS_PHONE && !PocketPC && !WIN8_1
            // WP7 doesn't as of Beta doesn't support a way to set this value either directly
            // or indirectly
            webRequest.ContentLength = 0;
            //对发送的数据不使用缓存
            webRequest.AllowWriteStreamBuffering = false;
#endif

            return webRequest;
        }

        private void AppendHeaders(HttpWebRequest webRequest)
        {
            foreach (var header in Headers)
            {
                webRequest.Headers[header.Key] = header.Value;
            }
        }

        private void WriteRequestBodyAsync(HttpWebRequest webRequest, Action<HttpResponse> callback)
        {
            IAsyncResult asyncResult;
            _timeoutState = new TimeOutState { Request = webRequest };

            if (Method == "GET")
            {
                // GET请求直接获取response
                asyncResult = webRequest.BeginGetResponse(r => ResponseCallback(r, callback), webRequest);
            }
            else
            {

#if !WINDOWS_PHONE && !PocketPC && !WIN8_1
                webRequest.ContentLength = CalculateContentLength();
#endif

                asyncResult = webRequest.BeginGetRequestStream(result => RequestStreamCallback(result, callback), webRequest);
            }

            SetTimeout(asyncResult, _timeoutState);
        }

        private long CalculateContentLength()
        {
            if (RequestBodyBytes != null)
                return RequestBodyBytes.Length;

            if (RequestBodyString != null)
                return Encoding.UTF8.GetByteCount(RequestBodyString);

            return 0;
        }

        private void RequestStreamCallback(IAsyncResult result, Action<HttpResponse> callback)
        {
            var webRequest = (HttpWebRequest)result.AsyncState;

            if (_timeoutState.TimedOut)
            {
                var response = new HttpResponse { ResponseStatus = ResponseStatus.TimedOut };
                ExecuteCallback(response, callback);
                return;
            }

            // write body to request stream
            try
            {
                WriteRequestBody(webRequest, result);
            }
            catch (Exception ex)
            {
                ExecuteCallback(CreateErrorResponse(ex), callback);
                return;
            }

            IAsyncResult asyncResult = webRequest.BeginGetResponse(r => ResponseCallback(r, callback), webRequest);
            SetTimeout(asyncResult, _timeoutState);
        }

        private void WriteRequestBody(HttpWebRequest webRequest, IAsyncResult result)
        {
            if (RequestBodyBytes != null || RequestBodyString != null)
            {
                using (var requestStream = webRequest.EndGetRequestStream(result))
                {
                    if (RequestBodyBytes != null)
                    {
                        requestStream.Write(RequestBodyBytes, 0, RequestBodyBytes.Length);
                    }
                    else
                    {
                        WriteStringTo(requestStream, RequestBodyString);
                    }
                }
            }
        }


        private static void WriteStringTo(Stream stream, string toWrite)
        {
            var bytes = Encoding.UTF8.GetBytes(toWrite);
            stream.Write(bytes, 0, bytes.Length);
        }

        private void SetTimeout(IAsyncResult asyncResult, TimeOutState timeOutState)
        {
#if FRAMEWORK && !PocketPC && !WIN8_1
            if (Timeout != 0)
            {
                ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), timeOutState, Timeout, true);
            }
#endif
        }

        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (!timedOut)
                return;

            var timeoutState = state as TimeOutState;

            if (timeoutState == null)
            {
                return;
            }

            lock (timeoutState)
            {
                timeoutState.TimedOut = true;
            }

            if (timeoutState.Request != null)
            {
                timeoutState.Request.Abort();
            }
        }

        private static void GetRawResponseAsync(IAsyncResult result, Action<HttpWebResponse> callback)
        {
            var response = new HttpResponse();
            response.ResponseStatus = ResponseStatus.None;

            HttpWebResponse raw = null;

            try
            {
                var webRequest = (HttpWebRequest)result.AsyncState;
                raw = webRequest.EndGetResponse(result) as HttpWebResponse;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.RequestCanceled)
                {
                    throw ex;
                }

                // Check to see if this is an HTTP error or a transport error.
                // In cases where an HTTP error occurs ( status code >= 400 )
                // return the underlying HTTP response, otherwise assume a
                // transport exception (ex: connection timeout) and
                // rethrow the exception

                if (ex.Response is HttpWebResponse)
                {
                    raw = ex.Response as HttpWebResponse;
                }
                else
                {
                    throw ex;
                }
            }

            callback(raw);
            raw.Close();
        }

        private void ResponseCallback(IAsyncResult result, Action<HttpResponse> callback)
        {
            var response = new HttpResponse { ResponseStatus = ResponseStatus.None };

            try
            {
                if (_timeoutState.TimedOut)
                {
                    response.ResponseStatus = ResponseStatus.TimedOut;
                    ExecuteCallback(response, callback);
                    return;
                }

                GetRawResponseAsync(result, webResponse =>
                {
                    ExtractResponseData(response, webResponse);
                    ExecuteCallback(response, callback);
                });
            }
            catch (Exception ex)
            {
                ExecuteCallback(CreateErrorResponse(ex), callback);
            }
        }

        private static void ExecuteCallback(HttpResponse response, Action<HttpResponse> callback)
        {
            //using (Stream stream = response.GetResponseStream())
            //using (StreamReader reader = new StreamReader(stream))
            //{
            //    string contents = reader.ReadToEnd();

            //    callback(contents, null);
            //    /*//通过呼叫UI Thread来改变页面的显示 WINDOWPOHONE8
            //    Dispatcher.BeginInvoke(() =>
            //    {
            //        httpWebRequestTextBlock.Text = contents.ToString().Substring(begin + 7,
            //            end - begin - 7); textBox2.Text = note;
            //    });*/

            //}

            callback(response);
        }


        private void ExtractResponseData(HttpResponse response, HttpWebResponse webResponse)
        {
            using (webResponse)
            {
#if FRAMEWORK && !WIN8_1
                response.ContentEncoding = webResponse.ContentEncoding;
                response.Server = webResponse.Server;
#endif
                response.ContentType = webResponse.ContentType;
                response.ContentLength = webResponse.ContentLength;
                Stream webResponseStream = webResponse.GetResponseStream();

                ProcessResponseStream(webResponseStream, response);

                response.StatusCode = webResponse.StatusCode;
                response.StatusDescription = webResponse.StatusDescription;
                response.ResponseUri = webResponse.ResponseUri;
                response.ResponseStatus = ResponseStatus.Completed;

                webResponse.Close();
            }
        }

        private void ProcessResponseStream(Stream webResponseStream, HttpResponse response)
        {
            response.RawBytes = webResponseStream.ReadAsBytes();
        }

    }


    /// <summary>
    /// Representation of an HTTP parameter (QueryString or Form value)
    /// </summary>
    public class HttpParameter
    {
        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Value of the parameter
        /// </summary>
        public string Value { get; set; }
    }

    public enum ResponseStatus
    {
        None,
        Completed,
        Error,
        TimedOut,
        Aborted
    }

    /// <summary>
    /// HTTP response data
    /// </summary>
    public class HttpResponse
    {
        private string _content;

        /// <summary>
        /// Default constructor
        /// </summary>
        public HttpResponse()
        {
        }

        /// <summary>
        /// MIME content type of response
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// Length in bytes of the response content
        /// </summary>
        public long ContentLength { get; set; }
        /// <summary>
        /// Encoding of the response content
        /// </summary>
        public string ContentEncoding { get; set; }
        /// <summary>
        /// Lazy-loaded string representation of response content
        /// </summary>
        public string Content
        {
            get
            {
                if (_content == null)
                {
                    _content = RawBytes.AsString();
                }
                return _content;
            }
        }
        /// <summary>
        /// HTTP response status code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Description of HTTP status returned
        /// </summary>
        public string StatusDescription { get; set; }
        /// <summary>
        /// Response content
        /// </summary>
        public byte[] RawBytes { get; set; }
        /// <summary>
        /// The URL that actually responded to the content (different from request if redirected)
        /// </summary>
        public Uri ResponseUri { get; set; }
        /// <summary>
        /// HttpWebResponse.Server
        /// </summary>
        public string Server { get; set; }

        private ResponseStatus _responseStatus = ResponseStatus.None;
        /// <summary>
        /// Status of the request. Will return Error for transport errors.
        /// HTTP errors will still return ResponseStatus.Completed, check StatusCode instead
        /// </summary>
        public ResponseStatus ResponseStatus
        {
            get
            {
                return _responseStatus;
            }
            set
            {
                _responseStatus = value;
            }
        }

        /// <summary>
        /// Transport or other non-HTTP error generated while attempting request
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Exception thrown when error is encountered.
        /// </summary>
        public Exception ErrorException { get; set; }
    }

    public class RequestAsyncHandle
    {
        public HttpWebRequest WebRequest;

        public RequestAsyncHandle()
        {
        }

        public RequestAsyncHandle(HttpWebRequest webRequest)
        {
            WebRequest = webRequest;
        }

        public void Abort()
        {
            if (WebRequest != null)
                WebRequest.Abort();
        }
    }

    public class TimeOutState
    {
        public bool TimedOut { get; set; }
        public HttpWebRequest Request { get; set; }
    }

}
