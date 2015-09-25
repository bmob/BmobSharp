using System;
using System.Collections.Generic;
using System.Text;

namespace cn.bmob.io
{
    internal class Remove<T> : Operate
    {

        public List<T> objects { get; set; }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put(OP_NAME, "Remove");
            output.Put("objects", this.objects);
        }

    }
}
