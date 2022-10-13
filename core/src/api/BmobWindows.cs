using System;
using System.Collections.Generic;
using System.Threading;
using cn.bmob.io;
using cn.bmob.response;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;
using System.Diagnostics;
using cn.bmob.http;
using cn.bmob.Extensions;
using cn.bmob.config;
using cn.bmob.exception;
using System.Text.RegularExpressions;

namespace cn.bmob.api
{

#if FRAMEWORK || WINDOWS_PHONE
    public class BmobWindows : Bmob
    {

        public BmobWindows()
        {
            Configuration.PLATFORM = SDKTarget.WindowsDesktop;
        }

        internal override void submit<T>(BmobCommand<T> command, BmobCallback<T> callback)
        {
            command.execute<int>(Request, callback);
        }

        /* internal */
        public int Request(String url, String method, String contentType, byte[] postData, IDictionary<String, String> headers, Action<String, Status, BmobException> callback)
        {
            return requestInternal(url, method, contentType, postData, headers, callback);
        }

        private int requestInternal(String url, String method, string ContentType, byte[] postData, IDictionary<string, string> headers, Action<String, Status, BmobException> callback)
        {
            var http = new Http();
            http.Method = method;
            http.RequestContentType = ContentType;
            http.Headers = headers;
            http.RequestBodyBytes = postData;
            http.Url = new Uri(url);

            // http模块有包括了网络异常的处理
            http.ExecuteAsync((raw, _) =>
            {
                var status = new Status((int)raw.StatusCode, raw.StatusDescription);
                callback(raw.Content, status, raw.ErrorException != null ? new BmobException(raw.ErrorException) : null);
            });

            return 0;
        }
    }
#endif

}
