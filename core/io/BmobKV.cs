using System;
using System.Collections.Generic;

namespace cn.bmob.io
{
    /// <summary>
    /// 主要用于请求参数，推荐在一次性场景下使用！
    /// </summary>
    public class BmobKV : Dictionary<String, Object>, IBmobWritable
    {
        public string _type { get { return "Bmo-KV"; } }

        public BmobKV Put(String key, Object value)
        {
            this.Add(key, value);
            return this;
        }

        public BmobKV PutAll(IDictionary<String, Object> kvs)
        {
            foreach (var entry in kvs)
            {
                this.Put(entry.Key, entry.Value);
            }
            return this;
        }

        public void readFields(BmobInput input)
        {
            foreach (var key in input.keySet())
            {
                this.Add(key, input.getRaw(key));
            }
        }

        public void write(BmobOutput output, Boolean all)
        {
            foreach (var entry in this)
            {
                output.Put(entry.Key, entry.Value);
            }
        }

    }
}
