using System;
using System.Collections.Generic;
using System.Text;
using cn.bmob.io;

namespace cn.bmob.response
{
    /// <summary>
    /// 更新数据的回调类
    /// </summary>
    public class UpdateCallbackData : BmobObject, IBmobWritable
    {
        /// <summary>
        /// 数据的更新时间
        /// </summary>
        public String updatedAt { get; set; }

        public override void readFields(BmobInput input)
        {
            this.updatedAt = input.getString("updatedAt");
        }

        public override void write(BmobOutput output, bool all)
        {
            output.Put("updateAt", this.updatedAt);
        }
    }

}
