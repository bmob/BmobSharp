using System;
using System.Collections;
using System.Collections.Generic;

namespace cn.bmob.tools
{
    /// <summary>
    /// Bmob提供的数组处理类
    /// ps：之所以提供这个类，是因为反序列化的数组只支持List
    /// </summary>
    public class BmobArrays
    {
        /// <summary>
        /// 将数组转成强类型列表
        /// </summary>
        /// <typeparam name="U">泛型</typeparam>
        /// <param name="eles">数组</param>
        /// <returns>列表</returns>
        public static List<U> wrap<U>(params U[] eles)
        {
            List<U> result = new List<U>();
            foreach (U t in eles)
            {
                result.Add(t);
            }
            return result;
        }

        /// <summary>
        /// 将列表转换为强类型列表
        /// </summary>
        /// <typeparam name="U">泛型</typeparam>
        /// <param name="list">列表</param>
        /// <returns>列表</returns>
        public static List<U> wrap<U>(IList<U> list)
        {
            List<U> result = new List<U>();
            foreach (U t in list)
            {
                result.Add(t);
            }
            return result;
        }
    }
}
