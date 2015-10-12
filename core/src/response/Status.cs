using System;
using System.Collections.Generic;
using System.Text;
using cn.bmob.io;

namespace cn.bmob.response
{
    /// <summary>
    /// 请求返回值的状态节点
    /// </summary>
    public class Status : BmobObject
    {

        public Status() { }

        public Status(int code, String message)
        {
            this.code = code;
            this.message = message;
        }

        /// <summary>
        /// 判断返回状态是否为200
        /// </summary>
        /// <returns>true为200</returns>
        public bool ok()
        {
            return this.code.Get() < 300;
        }

        /// <summary>
        /// 返回值的状态编码
        /// </summary>
        public BmobInt code { get; internal set; }

        /// <summary>
        /// 返回值的状态信息
        /// </summary>
        public string message { get; internal set; }

        /// <summary>
        /// 对象对应的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.message + "(" + this.code + ")";
        }

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            this.code = input.getInt("code");
            this.message = input.getString("message");
            if (this.message == null)
            {
                this.message = input.getString("error");
            }
        }

        public override void write(BmobOutput output, Boolean all)
        {
            base.write(output, all);

            output.Put("code", this.code);
            output.Put("message", this.message);
        }
    }

}
