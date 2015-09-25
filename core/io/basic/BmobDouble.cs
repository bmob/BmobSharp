using System;
using System.Collections.Generic;

using System.Text;

namespace cn.bmob.io
{
    public sealed class BmobDouble : BmobNumber<double>
    {

        public BmobDouble()
            : base()
        { }

        public BmobDouble(double value)
            : base(value)
        {
        }

        #region Implicit Conversions
        public static implicit operator BmobDouble(double data)
        {
            return new BmobDouble(data);
        }

        #endregion

    }
}
