using System;
using System.Collections.Generic;
using System.Text;
using cn.bmob.io;

namespace cn.bmob.response
{
    /// <summary>
    /// SMS数据的回调类
    /// </summary>
    public class RequestSmsCodeCallbackData : BmobObject, IBmobWritable
    {
        public BmobInt smsId { get; set; }

        public override void readFields(BmobInput input)
        {
            this.smsId = input.getInt("smsId");
        }

        public override void write(BmobOutput output, bool all)
        {
            output.Put("smsId", this.smsId);
        }
    }

    public class VerifySmsCodeCallbackData : BmobObject, IBmobWritable
    {
        public string msg { get; set; }

        public override void readFields(BmobInput input)
        {
            this.msg = input.getString("msg");
        }

        public override void write(BmobOutput output, bool all)
        {
            output.Put("msg", this.msg);
        }
    }

    public class QuerySmsCallbackData : BmobObject, IBmobWritable
    {
        public string sms_state { get; set; }
        public BmobBoolean verify_state { get; set; }

        public override void readFields(BmobInput input)
        {
            this.sms_state = input.getString("sms_state");
            this.verify_state = input.getBoolean("verify_state");
        }

        public override void write(BmobOutput output, bool all)
        {
            output.Put("sms_state", this.sms_state);
            output.Put("verify_state", this.verify_state);
        }
    }

}
