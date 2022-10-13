using System;
using System.Collections.Generic;
using System.Collections;

namespace cn.bmob.io
{
    /// <summary>
    /// 数据表操作类
    /// </summary>
    public class BmobTable2 : BmobTable
    {
        private String _table;
        public BmobTable2(String tablename)
            : base()
        {
            this._table = tablename;
        }

        public override String table { get { return this._table; } }

        private Dictionary<String, Object> kvs = new Dictionary<string, object>();

        public BmobTable2 Set(Dictionary<string, object> value)
        {
            this.kvs = value;
            return this;
        }

        public BmobTable2 Put(String key, Object value)
        {
            kvs.Add(key, value);
            return this;
        }

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            foreach (var key in input.keySet())
            {
                kvs.Add(key, input.getRaw(key));
            }
        }

        public override void write(BmobOutput output, Boolean all)
        {
            base.write(output, all);

            foreach (var entry in kvs)
            {
                var key = entry.Key;
                if (key == "objectId" || key == "createdAt" || key == "updatedAt")
                {
                    continue;
                }
                output.Put(key, entry.Value);
            }
        }

    }
}
