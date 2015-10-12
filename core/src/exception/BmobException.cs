using cn.bmob.response;
using System;

namespace cn.bmob.exception
{
    /// <summary>
    /// Bmob自定义异常处理类
    /// </summary>
    public class BmobException : Exception
    {
        public Status status { get; private set; }

        public BmobException(Status status) : base(status.ToString())
        {
            this.status = status;
        }

        public BmobException(string msg) : base(msg) { }

        public BmobException(Exception real) : base(real.Message, real) { }

        public BmobException(string msg, Exception real) : base(msg, real) { }
    }
}
