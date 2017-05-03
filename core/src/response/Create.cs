using System;
using System.Collections.Generic;

using System.Text;
using cn.bmob.io;

namespace cn.bmob.response
{
    /// <summary>
    /// 添加数据的回调类，创建成功，返回objectId和createAt信息
    /// </summary>
    public class CreateCallbackData : BmobObject, IBmobWritable
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public string createdAt { get; set; }

        /// <summary>
        /// objectId
        /// </summary>
        public string objectId { get; set; }

        public override void readFields(BmobInput input)
        {
            this.createdAt = input.getString("createdAt");
            this.objectId = input.getString("objectId");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="all"></param>
        public override void write(BmobOutput output, Boolean all)
        {
            output.Put("createdAt", this.createdAt);
            output.Put("objectId", this.objectId);
        }
    }

}
