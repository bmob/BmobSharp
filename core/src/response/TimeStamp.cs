using cn.bmob.io;

namespace cn.bmob.response
{
    /// <summary>
    /// 获取服务器时间戳
    /// </summary>
    public class TimeStampCallbackData : BmobObject, IBmobWritable
    {
        /// <summary>
        /// UTC时间秒数,时间戳
        /// </summary>
        public BmobInt timestamp { get; set; }
        
        public string datetime { get; set; }

        public override void readFields(BmobInput input)
        {
            this.timestamp = input.getInt("timestamp");
            this.datetime = input.getString("datetime");
        }

        public override void write(BmobOutput output, bool all)
        {
            output.Put("timestamp", this.timestamp);
            output.Put("datetime", this.datetime);
        }

    }

}
