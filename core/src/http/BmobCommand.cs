using System;
using System.Collections.Generic;
using cn.bmob.api;
using cn.bmob.response;
using System.Collections;
using System.Text;
using System.IO;
using cn.bmob.tools;
using cn.bmob.io;
using cn.bmob.json;
using cn.bmob.Extensions;
using cn.bmob.config;
using cn.bmob.exception;

namespace cn.bmob.http
{
    /// <summary>
    /// 请求回调函数
    /// 
    /// <para>Bmob服务器请求到数据后调用该委托实现。
    /// </para>
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="response">请求返回数据</param>
    /// <param name="exception">调用过程中发生的异常，包括调用执行过程中的校验和服务端返回状态非200情况。</param>
    public delegate void BmobCallback<T>(T response, BmobException exception);

    internal class BmobCommand<T>
    {
        private BmobInteractObject receiver;

        public BmobCommand(BmobInteractObject mReceiver)
        {
            this.receiver = mReceiver;
        }

        internal BmobInteractObject getReceiver()
        {
            return receiver;
        }

        protected virtual IDictionary<String, String> getHeaders()
        {
            if (receiver.AppKey == null || receiver.RestKey == null)
            {
                throw new BmobException("applicationid and restkey must be set!!! please invoke Bmob#initialize first. ");
            }
            IDictionary<String, String> headers = new Dictionary<String, String>();
            headers.Add("X-Bmob-Application-Id", receiver.AppKey);
            headers.Add("X-Bmob-REST-API-Key", receiver.RestKey);

            if (receiver.MasterKey != null)
            {
                headers.Add("X-Bmob-Master-Key", receiver.MasterKey);
            }

            // 如果是用户表修改的请求需要检查是否设置了sessiontoken
            if (receiver.SessionToken != null)
                headers.Add("X-Bmob-Session-Token", receiver.SessionToken);

            // XXX 默认Unity WWW已经添加了压缩标志; WindowsPhone8默认不支持GzipStream，暂时不添加压缩！
            // headers.Add("Accept-Encoding", "gzip,deflate");
            return headers;
        }

        /// <typeparam name="R">操作返回值类型</typeparam>
        /// <param name="request">参数对应为： url, contenttype, requestData, headers, callback(请求服务器数据返回值回调函数)</param>
        /// <param name="fCallback">返回结果回调函数</param>
        /// <returns>操作返回值</returns>
        public virtual R execute<R>(Func<String, String, String, Byte[], IDictionary<String, String>, Action<String, Status, BmobException>, R> request, BmobCallback<T> fCallback)
        {
            var url = receiver.Url;

            var contentType = receiver.ContentType;
            var postData = getPostData();
            var headers = getHeaders();

            return Execute(request, url, contentType, postData, headers, fCallback);
        }


        protected R Execute<R>(Func<String, String, String, Byte[], IDictionary<String, String>, Action<String, Status, BmobException>, R> request,
                    String url, String contentType, Byte[] postData, IDictionary<String, String> headers, BmobCallback<T> fCallback)
        {


            BmobDebug.T("\r\n\t请求的URL : " + url
                        + "\r\n\t交互对象(以请求的数据为准): " + JsonAdapter.JSON.ToRawString(receiver)
                        + "\r\n\t请求的数据: " + JsonAdapter.JSON.ToJsonString(receiver.Data));

            return request.Invoke(url, receiver.Method, contentType, postData, headers, (resp, status, ex) =>
            {
                if (BmobDebug.Debug)
                {
                    BmobDebug.D("返回数据内容为: " + resp);
                }
                else
                {
                    var rp = resp.Length > 400 ? resp.Substring(0, 200) + " ... ... ... ... ... ... " + resp.Substring(resp.Length - 200) : resp;
                    BmobDebug.I("返回数据内容为: " + rp);
                }

                onPostExecute(resp, status, ex, fCallback);
            });

        }

        protected virtual byte[] getPostData()
        {
            if (receiver.Data == null || receiver.Method.Equals("GET"))
            {
                return null;
            }

            var data = JsonAdapter.JSON.ToJsonString(receiver.Data);
            return data.GetBytes();
        }

        protected void onPostExecute(String result, Status status, BmobException exception, BmobCallback<T> fCallback)
        {
            T data;
            BmobException ex;
            if (exception != null)
            {
                data = default(T);
                if (result == null)
                    ex = exception;
                else
                    ex = new BmobException(exception.Message + ", and response content is " + result, exception.InnerException);
            }
            else
            {
                BmobResponseParser<T> parser = getResponseParser(status);
                parser.parse(result);

                data = parser.data;
                ex = parser.exception;
            }

            if (ex != null)
            {
                BmobDebug.T("[ BmobCommand ] after parse response, error: '" + ex.Message + "'");
            }

            fCallback(data, ex);
        }

        protected virtual BmobResponseParser<T> getResponseParser(Status status)
        {
            return new BmobResponseParser<T>(status);
        }

    }

    internal class BmobFileCommand<T> : BmobCommand<T>
    {
        public BmobFileCommand(BmobInteractObject mReceiver) : base(mReceiver)
        {
        }

        protected override byte[] getPostData()
        {
            var data = getReceiver().Data as BmobLocalFile;
            return data.Content();
        }
    }

}
