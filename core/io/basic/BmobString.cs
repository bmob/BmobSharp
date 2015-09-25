using System;
using System.Collections.Generic;

using System.Text;

namespace cn.bmob.io
{
    public sealed class BmobString : IBmobValue<Boolean>
    {
        private String value;

        public BmobString() { }

        public BmobString(String value)
        {
            this.value = value;
        }

        public String Get()
        {
            return this.value;
        }

        public override string ToString()
        {
            return Get().ToString();
        }

        public static bool operator >(String input, BmobString arg)
        {
            return String.Compare(input, arg.Get()) > 0;
        }

        public static bool operator >(BmobString input, String arg)
        {
            return String.Compare(input.Get(), arg) > 0;
        }

        public static bool operator >(BmobString input, BmobString arg)
        {
            return String.Compare(input.Get(), arg.Get()) > 0;
        }

        #region Implicit Conversions
        public static implicit operator BmobString(String data)
        {
            return new BmobString(data);
        }

        #endregion

    }
}
