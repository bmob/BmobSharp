using System;
using cn.bmob.tools;
using System.Collections;
using System.Collections.Generic;
using cn.bmob.Extensions;

namespace cn.bmob.io
{
    public class BmobOutput
    {

        private IDictionary real = new Dictionary<String, Object>();

        /// <summary>
        /// internal
        /// </summary>
        public IDictionary getData()
        {
            return real;
        }

        public void PutIfNotNull(string key, object value)
        {
            if (value == null)
            {
                return;
            }

            Put(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">cannot be null !!!</param>
        public void Put(string key, object value)
        {
            var type = value.GetType();
            if (type == typeof(bool))
            {
                Put(key, (bool)value);
            }
            else if (type == typeof(String))
            {
                Put(key, (String)value);
            }
            else if (type == typeof(int))
            {
                Put(key, (int)value);
            }
            else if (type == typeof(long))
            {
                Put(key, (long)value);
            }
            else if (type == typeof(double))
            {
                Put(key, (double)value);
            }
            else if (typeof(BmobLong).IsAssignableFrom(type))
            {
                Put(key, (BmobLong)value);
            }
            else if (typeof(BmobInt).IsAssignableFrom(type))
            {
                Put(key, (BmobInt)value);
            }
            else if (typeof(BmobDouble).IsAssignableFrom(type))
            {
                Put(key, (BmobDouble)value);
            }
            else if (typeof(BmobBoolean).IsAssignableFrom(type))
            {
                Put(key, (BmobBoolean)value);
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                Put(key, (IDictionary)value);
            }
            else if (typeof(IDictionary<String, Object>).IsAssignableFrom(type))
            {
                Put(key, (IDictionary<String, Object>)value);
            }
            else if (typeof(IBmobWritable).IsAssignableFrom(type))
            {
                Put(key, (IBmobWritable)value);
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                Put(key, (IList)value);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 添加键值对。加入已经存在的键时，会覆盖原有的值！不同与Hashtable#Add方法抛出异常的方式。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put(string key, IBmobWritable value)
        {
            Save(real, key, value);
        }

        /// <summary>
        /// 添加键值对。加入已经存在的键时，会覆盖原有的值！不同与Hashtable#Add方法抛出异常的方式。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put<T>(string key, IBmobValue<T> value)
        {
            Save(real, key, value);
        }

        /// <summary>
        /// 添加键值对。加入已经存在的键时，会覆盖原有的值！不同与Hashtable#Add方法抛出异常的方式。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="list"></param>
        public void Put<T>(string key, List<T> list)
        {
            Save(real, key, list);
        }

        /// <summary>
        /// 实现 {column: {key: value}} 的效果
        /// </summary>
        internal static void Composite(IDictionary data, String column, String key, Object value)
        {
            var part = data[column];
            if (part == null || !(part is IDictionary))
            {
                // ！！SimpleJson处理Dictionary<string, Object>才正常
                part = new Dictionary<String, Object>();
            }
            ((IDictionary)part).Add(key, value);

            BmobOutput.Save(data, column, part);
        }

        /// <summary>
        /// 如果data中已经存在key，则覆盖为value
        /// </summary>
        internal static void Save(IDictionary data, String key, Object value)
        {
            if (value == null)
                return;

            try
            {
                data.Add(key, value);
            }
            catch (ArgumentException e)
            {
                BmobDebug.Log("ERROR: " + e.Message);

                // 处理重复修改同一列的值
                if (data.Contains(key))
                {
                    data.Remove(key);
                    data.Add(key, value);
                }
            }
        }

        internal static void Save<V>(IDictionary<String, V> data, String key, V value)
        {
            if (value == null)
                return;

            try
            {
                data.Add(key, value);
            }
            catch (ArgumentException e)
            {
                BmobDebug.Log("ERROR: " + e.Message);

                // 处理重复修改同一列的值
                if (data.ContainsKey(key))
                {
                    data.Remove(key);
                    data.Add(key, value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put(string key, bool value)
        {
            Put(key, new BmobBoolean(value));
        }

        public void Put(string key, int value)
        {
            Put(key, new BmobInt(value));
        }

        public void Put(string key, long value)
        {
            Put(key, new BmobLong(value));
        }

        public void Put(string key, double value)
        {
            Put(key, new BmobDouble(value));
        }

        public void Put(string key, string value)
        {
            Save(real, key, value);
        }

        /// <summary>
        /// value的键值对都是基础类型，或为hashtable类型！否则请实现IBmobWritable来处理该数据。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put(string key, IDictionary value)
        {
            Save(real, key, value);
        }

        public void Put(string key, IDictionary<String, Object> value)
        {
            Save(real, key, value);
        }

        public void Put(string key, IList value)
        {
            Save(real, key, value);
        }

    }
}
