using System.Collections.Generic;

namespace cn.bmob.io
{
    public class BmobRelation<T> : List<BmobPointer<T>> where T : BmobTable
    {
    }
}
