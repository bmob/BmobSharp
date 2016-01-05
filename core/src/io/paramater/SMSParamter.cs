using System;
using System.Collections.Generic;
using System.Net;

namespace cn.bmob.io
{

    /// <summary>
    /// 短信
    /// </summary>
    public sealed class SMSParamter : BmobObject
    {
        public string mobilePhoneNumber { get; set; }
        public string template { get; set; }

        public string code { get; set; }

        public string smsId { get; set; }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put("mobilePhoneNumber", mobilePhoneNumber);
            output.Put("template", template);
        }
    }
}
