using System.Collections;
using cn.bmob.io;
using System.Collections.Generic;
using System;

namespace cn.bmob.json
{
    /// <summary>
    /// 对象与JSON互相转换的接口
    /// 
    /// <para>用于可以自定义JSON转换的实现：
    /// <list type="number">
    /// <item><term>实现<c>IJsonParser</c>；</term></item>
    /// <item><term>调用<c>JSONAdapter.register(parser)</c>进行注册。</term></item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IJsonParser
    {
        string ToRawString(object data);

        /// <summary>
        /// 用于打印调试用！结合writable接口的write第二个参数一起使用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string ToDebugJsonString(object data);

        T ToObject<T>(string json);

        /// <summary>
        /// 把对象转换为JSON字符串. 
        /// ATTENTION: 只能处理基本的类型，Writable/IDictionary/IList/string/Numeric/Boolean/null
        /// </summary>
        /// <param name="data">需序列化的对象.</param>
        /// <returns>JSON字符串.</returns>
        /*internal*/
        string ToJsonString(object data);

        /// <summary>
        /// IDictionary<String, Object> | IList | primitive
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        /*internal*/
        Object ToObject(string json);

    }

    /// <summary>
    /// 对象和JSON转换的适配器
    /// </summary>
    public class JsonAdapter
    {
        static JsonAdapter()
        {
            // Default JSON Parser
            //JSON = new NewtonsoftJsonParser();
            JSON = new SimpleJsonParser();
        }

        /// <summary>
        /// 注册的JsonParser对象
        /// </summary>
        public static IJsonParser JSON
        {
            get;
            internal set;
        }

        /// <summary>
        /// 注册自定义的JsonParser
        /// 实现IJsonParser，然后调用该方法。即可以实现自己的JSON序列化的功能。
        /// </summary>
        /// <param name="parser">parser</param>
        internal static void register(IJsonParser parser)
        {
            JsonAdapter.JSON = parser;
        }

    }
}
