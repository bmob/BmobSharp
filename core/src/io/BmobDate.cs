using System;

namespace cn.bmob.io
{
    /// <summary>
    /// BmobDate时间类
    /// </summary>
    public sealed class BmobDate : BmobObject
    {
        /// <summary>
        /// 返回类型标识
        /// </summary>
        public override String _type { get { return "Date"; } }

        /// <summary>
        /// 时间的IOS格式（yyyy-MM-dd HH:mm:ss）
        /// </summary>
        public String iso { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BmobDate()
        {
        }

        public override void readFields(BmobInput input)
        {
            this.iso = input.getString("iso");
        }

        public override void write(BmobOutput output, Boolean all)
        {
            output.Put(TYPE_NAME, this._type);
            output.Put("iso", this.iso);
        }

        #region Implicit Conversions
        public static implicit operator BmobDate(DateTime data)
        {
            var date = new BmobDate();
            date.iso = data.ToString("yyyy-MM-dd HH:mm:ss");
            return date;
        }

        #endregion

    }
}
