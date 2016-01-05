using System;
using System.Collections;
using System.Collections.Generic;
using cn.bmob.json;
using cn.bmob.tools;
using cn.bmob.Extensions;

namespace cn.bmob.io
{
    public class BmobInput
    {
        private IDictionary<String, Object> real;

        internal BmobInput() { }

        internal BmobInput(IDictionary<String, Object> real)
        {
            this.real = real;
        }

        public Boolean Contains(string name)
        {
            return real.ContainsKey(name);
        }

        public ICollection<String> keySet()
        {
            return real.Keys;
        }

        /// <summary>
        /// 简单类型/简单类型的键值对
        /// </summary>
        internal object getRaw(string name)
        {
            if (Contains(name))
            {
                return real[name];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 这里获取到的list是没有被序列化的，list中的元素还是hashtable/valuetype对象，没有对应到用户自定义的类型的！
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private IList getListRaw(string name)
        {
            return getRaw(name) as IList;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="W">获取数据对应的模型类型</typeparam>
        /// <param name="name">字段名</param>
        public W Get<W>(string name)
        {
            return Parse<W>(getRaw(name));
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="U">列表中对象的具体类型</typeparam>
        /// <param name="name">字段名</param>
        public List<U> getList<U>(String name)
        {
            IList raw = getListRaw(name);
            if (raw == null)
            {
                BmobDebug.D("获取" + name + "的列表数据为空！");
                return null;
            }

            Type type = typeof(U);
            List<U> result = new List<U>(raw.Count);

            foreach (var element in raw)
            {
                result.Add(Parse<U>(element));
            }

            return result;
        }

        // / 包装方法

        public BmobDate getDate(string name)
        {
            return Get<BmobDate>(name);
        }

        public BmobGeoPoint getGeoPoint(string name)
        {
            return Get<BmobGeoPoint>(name);
        }

        public BmobFile getFile(string name)
        {
            return Get<BmobFile>(name);
        }

        public BmobBoolean getBoolean(string name)
        {
            return Parse<BmobBoolean>(getRaw(name));
        }

        public BmobInt getInt(string name)
        {
            return Parse<BmobInt>(getRaw(name));
        }

        public BmobLong getLong(string name)
        {
            return Parse<BmobLong>(getRaw(name));
        }

        public BmobDouble getDouble(String name)
        {
            return Parse<BmobDouble>(getRaw(name));
        }

        public string getString(String name)
        {
            return Parse<string>(getRaw(name));
        }

        // / 

        // / 私有核心方法

        /// <summary>
        /// 对象转换的公共类
        /// </summary>
        // <typeparam name="W">支持List<>/Dictionary<X,X>/int/long/double/:BmobWritable/:BmobValue的类型</typeparam>
        // <param name="value">SDK会把JSON解析为List<X,X>和Dictionary<X,X>。 值对象，可以传入null</param>
        public static W Parse<W>(object value)
        {
            if (value == null)
            {
                return default(W);
            }

            W w = (W)convert(typeof(W), value);

            if (w == null)
            {
                BmobDebug.E("最外层对象**推荐**使用IBmobWritable对象，序列化定制化程度更高！");

                // 搞不定，使用反射方式重新解析
                string json = JsonAdapter.JSON.ToDebugJsonString(value);
                BmobDebug.E("反序列化不正确，使用默认的JSON反序列化方式再次处理！ " + json);
                w = JsonAdapter.JSON.ToObject<W>(json);
            }

            return w;
        }

        // covert dispatch
        private static object convert(Type type, Object data)
        {

            if (data.GetType() == type || type == typeof(Object))
            {
                return data;
            }

            // 基本类型
            if (type == typeof(int))
            {
                return toInt(data);
            }
            else if (type == typeof(long))
            {
                return toLong(data);
            }
            else if (type == typeof(double))
            {
                return toDouble(data);
            }
            else if (type == typeof(bool))
            {
                return toBoolean(data);
            }
            else if (type == typeof(string))
            {
                return toString(data);
            }

            // Bmob包装类型
            if (type == typeof(BmobInt))
            {
                return new BmobInt(toInt(data));
            }
            else if (type == typeof(BmobLong))
            {
                return new BmobLong(toLong(data));
            }
            else if (type == typeof(BmobDouble))
            {
                return new BmobDouble(toDouble(data));
            }
            else if (type == typeof(BmobBoolean))
            {
                return new BmobBoolean(toBoolean(data));
            }

            // 复杂类型
            if (typeof(IBmobWritable).IsAssignableFrom(type))
            {
                IDictionary<String, Object> raw = (IDictionary<String, Object>)data;

                IBmobWritable result = (IBmobWritable)Activator.CreateInstance(type);
                result.readFields(new BmobInput(raw));

                return result;
            }
            else if (typeof(IBmobValue).IsAssignableFrom(type))
            {
                IDictionary<String, Object> raw = (IDictionary<String, Object>)data;

                IBmobValue result = (IBmobValue)Activator.CreateInstance(type);
                result.Set(raw);

                return result;
            }

            if (typeof(IList).IsAssignableFrom(type))
            {
                IList raw = (IList)data;

                IList result = (IList)Activator.CreateInstance(type);
                foreach (var element in raw)
                {
                    var isGeneric = type.IsGenericType();
                    var dataType = isGeneric ? type.GetGenericArguments()[0] : null;

                    isGeneric = isGeneric && dataType != typeof(Object);
                    result.Add(isGeneric ? convert(dataType, element) : element);
                }

                return result;
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                IDictionary<String, Object> raw = (IDictionary<String, Object>)data;

                IDictionary result = (IDictionary)Activator.CreateInstance(type);

                var isGeneric = type.IsGenericType();
                var vt = isGeneric ? type.GetGenericArguments()[1] : null;
                foreach (var entry in raw)
                {
                    isGeneric = isGeneric && type != typeof(Object);
                    result.Add(entry.Key, isGeneric ? convert(vt, entry.Value) : entry.Value);
                }

                return result;
            }

            throw new ArgumentException("UnExcept type: " + type + ". Use Generic Type List<T>/Dictionary<K,V> instead!");
        }

        private static int toInt(Object data)
        {
            if (data is int) return (int)data;
            else if (data is long) return (int)((long)data);
            else if (data is double) return (int)((double)data);
            else
            {
                BmobDebug.Log("[ERROR] 获取数值类型失败，原值为：" + JsonAdapter.JSON.ToDebugJsonString(data));
            }

            return 0;
        }
        private static long toLong(Object data)
        {
            if (data is int) return (int)data;
            else if (data is long) return (long)data;
            else if (data is double) return (long)((double)data);
            else
            {
                BmobDebug.Log("[ERROR] 获取数值类型失败，原值为：" + JsonAdapter.JSON.ToDebugJsonString(data));
            }

            return 0;
        }
        private static double toDouble(Object data)
        {
            if (data is int) return (int)data;
            else if (data is long) return (long)data;
            else if (data is double) return (double)data;
            else
            {
                BmobDebug.Log("[ERROR] 获取数值类型失败，原值为：" + JsonAdapter.JSON.ToDebugJsonString(data));
            }

            return 0;
        }
        private static bool toBoolean(Object data)
        {
            return (bool)data;
        }
        private static String toString(Object data)
        {
            return (String)data;
        }

    }
}
