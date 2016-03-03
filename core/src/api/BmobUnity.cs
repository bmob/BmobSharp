#if UNITY_5 || UNITY_4 || UNITY_4_6 
#define Unity
#endif

using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;
using System.Linq;
using cn.bmob.io;
using cn.bmob.http;
using cn.bmob.config;
using cn.bmob.exception;
using cn.bmob.response;
using System.Text.RegularExpressions;

#if Unity

using UnityEngine;
using System.Collections.ObjectModel;
using cn.bmob.tools;

namespace System
{
    //public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);

    public delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    public class AggregateException : Exception
    {
        //
        // Properties
        //
        public ReadOnlyCollection<Exception> InnerExceptions
        {
            get;
            private set;
        }

        //
        // Constructors
        //
        public AggregateException(IEnumerable<Exception> innerExceptions)
        {
            this.InnerExceptions = new ReadOnlyCollection<Exception>(innerExceptions.ToList<Exception>());
        }

        //
        // Methods
        //
        public AggregateException Flatten()
        {
            List<Exception> list = new List<Exception>();
            foreach (Exception current in this.InnerExceptions)
            {
                AggregateException ex = current as AggregateException;
                if (ex != null)
                {
                    list.AddRange(ex.Flatten().InnerExceptions);
                }
                else
                {
                    list.Add(current);
                }
            }
            return new AggregateException(list);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(base.ToString());
            foreach (Exception current in this.InnerExceptions)
            {
                stringBuilder.AppendLine("\n-----------------");
                stringBuilder.AppendLine(current.ToString());
            }
            return stringBuilder.ToString();
        }
    }

    namespace Threading
    {
        namespace Tasks
        {
        }
    }

}


namespace cn.bmob.api
{

    public partial class Bmob : MonoBehaviour
    {
    }

    /// <summary>
    /// Bmob SDK入口类，开发者直接调用该类即可获取Bmob提供的各种服务。
    /// </summary>
    public class BmobUnity : Bmob
    {

        private MonoBehaviour go;

        /// <summary>
        /// Unity Behavior 
        /// </summary>
        public void Update() { }

        public BmobUnity()
        {
            Configuration.PLATFORM = SDKTarget.Unity;
            go = this;

        }

        /// <summary>
        /// 仅用于在界面设置
        /// </summary>
        public String ApplicationId;
        public String RestKey;

        internal override string appKey
        {
            get
            {
                return ApplicationId;
            }
            set
            {
                ApplicationId = value;
                base.appKey = value;
            }
        }
        internal override String restKey
        {
            get { return RestKey; }
            set
            {
                RestKey = value;
                base.restKey = value;
            }
        }


        internal override void submit<T>(BmobCommand<T> command, BmobCallback<T> callback)
        {
            this.go.StartCoroutine(execute<T>(command, callback));
        }

        /// <summary>
        /// 调用
        /// </summary>
        private IEnumerator execute<T>(BmobCommand<T> command, BmobCallback<T> callback)
        {
            return command.execute<IEnumerator>(Request, callback);
        }

        protected virtual IEnumerator Request(String url, String method, String contentType, byte[] postData, IDictionary<String, String> headers, Action<String, Status, BmobException> callback)
        {
            BmobOutput.Save(headers, "Content-Type", contentType);

            // http://answers.unity3d.com/questions/785798/put-with-wwwform.html
            // www不支持PUT和DELETE操作，需要服务端支持！！服务端已添加filter 2015年9月25日09:57:52
            BmobOutput.Save(headers, "X-HTTP-Method-Override", method);

            return RequestInternal(url, method, postData, headers, callback);
        }

        private IEnumerator RequestInternal(String url, String method, byte[] postData, IDictionary<String, String> headers, Action<String, Status, BmobException> callback)
        {
            var table = new Dictionary<String, String>();
            foreach (var header in headers)
            {
                table.Add(header.Key, header.Value);
            }
            WWW www = new WWW(url, method.Equals("GET") ? null : postData, table);

            yield return www;

            var error = www.error;
            var text = www.text;

            BmobDebug.T("[ BmobUnity ] after fetch www message, Response: '" + text + "', Error: ' " + error + "'");


            var status = new Status(200, error);
            if (www.responseHeaders.ContainsKey("STATUS"))
            {
                var respStatus = www.responseHeaders["STATUS"];
                var statusCode = Regex.Replace(respStatus, @"[^ ]* (\d*) .*", "$1");
                status.code = Convert.ToInt32(statusCode);
            }
            if (error != null && error != "")
            {
                // 返回了错误的内容，不表示返回的内容就为空！！
                callback(text, status, new BmobException(error));
            }
            else
            {
                callback(text, status, null);
            }
        }

    }

}

#endif