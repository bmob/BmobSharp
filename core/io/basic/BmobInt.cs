using System;
using System.Collections.Generic;

using System.Text;

namespace cn.bmob.io
{
    public sealed class BmobInt : BmobNumber<int>
    {

        public BmobInt()
            : base()
        { }

        public BmobInt(int value)
            : base(value)
        {
        }

        #region Implicit Conversions
        public static implicit operator BmobInt(int data)
        {
            return new BmobInt(data);
        }

        #endregion
    }
}
