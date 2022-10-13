using System;

namespace cn.bmob.io
{
    /// <summary>
    /// Bmob自定义的序列化反序列化的对象，Hashtable仅支持简单类型和Hashtable。对象就是简单类型的组合！
    /// </summary>
    public interface IBmobWritable
    {
        /// <summary>
        /// 获取自定义对象的类型
        /// </summary>
        string _type { get; }

        /// <summary>
        ///  把服务端返回的数据转化为本地对象值
        /// </summary>
        /// <param name="input"></param>
        void readFields(BmobInput input);

        /// <summary>
        /// 把本地对象写入到output，后续序列化提交到服务器
        /// </summary>
        /// <param name="output"></param>
        /// <param name="all">用于区分请求/打印输出序列化</param>
        void write(BmobOutput output, Boolean all);
    }

}
