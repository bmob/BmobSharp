using System;
using System.Collections.Generic;
using System.Text;
using cn.bmob.io;

namespace cn.bmob.response
{
    /// <summary>
    /// 删除数据的回调
    /// </summary>
    public class DeleteCallbackData : BmobObject, IBmobWritable
    {
        /// <summary>
        /// 返回信息
        /// </summary>
        public string msg { get; set; }

        public override void readFields(BmobInput input)
        {
            this.msg = input.getString("msg");
        }

        public override void write(BmobOutput output, Boolean all)
        {
            output.Put("msg", this.msg);
        }

    }
}
