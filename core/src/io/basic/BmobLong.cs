using System;
using System.Collections.Generic;

using System.Text;

namespace cn.bmob.io
{
    public sealed class BmobLong : BmobNumber<long>
    {

        public BmobLong()
            : base()
        { }

        public BmobLong(long value)
            : base(value)
        {
        }

        #region Implicit Conversions
        public static implicit operator BmobLong(long data)
        {
            return new BmobLong(data);
        }

        #endregion

    }
}
