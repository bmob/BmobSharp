using System.Collections.Generic;
using cn.bmob.io;

namespace cn.bmob.response
{
    /// <summary>
    /// 获取数据列表的回调类
    /// </summary>
    /// <typeparam name="T">用户模型对象</typeparam>
    public class QueryCallbackData<T> : BmobObject, IBmobWritable
    {
        /// <summary>
        /// 返回结果列表
        /// </summary>
        public List<T> results { get; set; }

        /// <summary>
        /// 请求数据总数，返回查询结果总数
        /// </summary>
        public BmobInt count { get; set;  }

        public override void readFields(BmobInput input)
        {
            this.count = input.getInt("count");
            this.results = input.getList<T>("results");
        }

        public override void write(BmobOutput output, bool all)
        {
            output.Put("count", this.count);
            output.Put("results", this.results);
        }

    }

}
