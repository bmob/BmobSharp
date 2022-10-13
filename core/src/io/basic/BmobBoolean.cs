using System;
using System.Collections.Generic;

using System.Text;

namespace cn.bmob.io
{
    public sealed class BmobBoolean : IBmobValue<Boolean>
    {
        private Boolean value;

        public BmobBoolean() { }

        public BmobBoolean(bool value)
        {
            this.value = value;
        }

        public Boolean Get()
        {
            return this.value;
        }

        public void Set(object o)
        {
            if (o is Boolean)
            {
                this.value = (Boolean)o;
            }
        }

        public override string ToString()
        {
            return Get().ToString();
        }

        #region Implicit Conversions
        public static implicit operator BmobBoolean(Boolean data)
        {
            return new BmobBoolean(data);
        }

        #endregion

    }
}
