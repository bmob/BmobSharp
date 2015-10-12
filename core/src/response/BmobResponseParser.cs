using System;
using System.Collections.Generic;
using cn.bmob.json;
using cn.bmob.io;
using System.Collections;
using cn.bmob.exception;
using cn.bmob.Extensions;

namespace cn.bmob.response
{
    internal class BmobResponseParser<T>
    {
        private Status result;

        public BmobResponseParser(Status status)
        {
            this.result = status;
        }

        protected static BmobException newPaserException(String json)
        {
            return new BmobException("请求失败！错误信息为： " + json);
        }

        public virtual void parse(String json)
        {
            if (!result.ok())
            {
                // RestAPI 如果不是200，说明返回内容有“错误”，此时解析内容
                var raw = (IDictionary<String, Object>)JsonAdapter.JSON.ToObject(json);
                var status = BmobInput.Parse<Status>(raw);

                this.exception = new BmobException(status.code == null ? result : status);
                this.data = default(T);
            }
            else
            {
                var type = typeof(T);
                if (type.IsArray || typeof(IList).IsAssignableFrom(type))
                {
                    // batch or ...
                    var raw = (IList)JsonAdapter.JSON.ToObject(json);
                    this.data = BmobInput.Parse<T>(raw);
                }
                else
                {
                    // 解析[CRUD]的返回值对象
                    var raw = (IDictionary<String, Object>)JsonAdapter.JSON.ToObject(json);

                    this.data = BmobInput.Parse<T>(raw);
                }
            }

        }

        /// <summary>
        /// 对应返回Json字符串的data节点数据对象
        /// </summary>
        public T data { get; set; }

        public BmobException exception { get; set; }
    }


    internal class UploadReponseParser<T> : BmobResponseParser<T>
    {

        public UploadReponseParser(Status status) : base(status) { }

        private static object get(IDictionary<String, Object> parent, String name)
        {
            return parent.ContainsKey(name) ? parent[name] : null;
        }


        public override void parse(String json)
        {
            // 文件上传返回值
            var raw = (IDictionary<String, Object>)JsonAdapter.JSON.ToObject(json);

            var firstR = get(raw, "r");
            if (firstR == null)
            {
                throw newPaserException(json);
            }

            bool isok = firstR is IDictionary<String, Object>;

            object secondR = null;
            if (isok)
                secondR = get(firstR as IDictionary<String, Object>, "r");

            if (secondR == null || !(secondR is IDictionary || secondR is IDictionary<String, Object>))
            {
                this.exception = new BmobException("文件上传失败！");
                this.data = default(T);
            }
            else
            {
                this.data = BmobInput.Parse<T>(secondR as IDictionary<String, Object>);
            }

        }

    }



}
