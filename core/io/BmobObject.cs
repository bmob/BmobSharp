using System;
using cn.bmob.json;
using System.Collections;

namespace cn.bmob.io
{
    /// <summary>
    /// 所有键值对对象的基类。
    /// 
    /// 重写<c>ToString</c>方法，实现打印输出当前对象的JSON字符串。
    /// </summary>
    public class BmobObject : IBmobWritable
    {
        public const String TYPE_NAME = "__type";

        public virtual String _type { get { return "Object"; } }

        public virtual void write(BmobOutput output, Boolean all)
        {
        }

        public virtual void readFields(BmobInput input)
        {
        }

        /// <summary>
        /// 输出当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            return GetType().FullName + " " + ToString(this);
        }

        /// <summary>
        /// 输出对象的JSON字符串
        /// </summary>
        /// <param name="ele">需要转换为JSON字符串的对象</param>
        /// <returns>字符串</returns>
        public static string ToString(IBmobWritable ele)
        {
            return JsonAdapter.JSON.ToDebugJsonString(ele);
        }

    }
}
