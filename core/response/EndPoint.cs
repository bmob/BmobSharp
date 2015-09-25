using System;
using System.Collections.Generic;
using System.Text;
using cn.bmob.io;
using System.Collections;
using cn.bmob.exception;
using cn.bmob.json;
using cn.bmob.Extensions;

namespace cn.bmob.response
{

    /// <summary>
    /// 云端代码的执行回调
    /// </summary>
    public class EndPointCallbackData<U> : BmobObject, IBmobWritable
    {
        public BmobException exception { get; set; }
        public U data { get; set; }

        public override void readFields(BmobInput input)
        {

            // EndPoint直接返回数据，不像Table返回值有Container的概念！
            var type = typeof(U);

            // 请求正确，返回数据
            Object jsonData = null;
            // 请求失败（如，云端方法不存在）， 异常状态
            Object statData = null;

            var rawResp = input.getString("result");
            if (typeof(IList).IsAssignableFrom(type))
            {
                this.data = BmobInput.Parse<U>(JsonAdapter.JSON.ToObject(rawResp));
            }
            else
            {
                var rawRespJson = JsonAdapter.JSON.ToObject(rawResp);
                jsonData = rawRespJson;
                statData = rawRespJson;
                
                if (statData is IDictionary || statData is IDictionary<String, Object>)
                {
                    // 要么返回数据，要么就是返回状态值
                    EndPointCallbackStat status = BmobInput.Parse<EndPointCallbackStat>(statData);
                    if (status != null && status.sucess != null)
                    {
                        this.exception = new BmobException("请求失败！错误信息为： " + status.message);
                        this.data = default(U);
                        return;
                    }
                }

                this.data = BmobInput.Parse<U>(rawRespJson);
            }
            
        }

        public override void write(BmobOutput output, bool all)
        {
            output.Put("result", this.data);
        }
    }

    /// <summary>
    /// 云端代码的执行回调
    /// </summary>
    public class EndPointCallbackStat : BmobObject, IBmobWritable
    {

        /// <summary>
        /// 执行结果
        /// </summary>
        public BmobBoolean sucess { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public String message
        {
            get;
            set;
        }

        public override void readFields(BmobInput input)
        {
            if (!input.Contains("sucess"))
            {
                return;
            }

            this.message = input.getString("message");
            this.sucess = input.getBoolean("sucess");

            // // 2014-5-26 13:42:38 返回值
            //{
            //  "code": 101,
            //  "error": "object not found for StudentScore."
            //}
            if (this.message == null)
            {
                this.message = input.getString("error") + "(" + input.getInt("code") + ")";
                this.sucess = false;
            }

        }

        public override void write(BmobOutput output, bool all)
        {
            output.Put("message", this.message);
            output.Put("success", this.sucess);
        }

    }

}
