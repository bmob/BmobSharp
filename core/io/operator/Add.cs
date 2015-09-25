using System.Collections.Generic;

namespace cn.bmob.io
{
    internal class Add<T> : Operate
    {

        public List<T> objects { get; set; }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put(OP_NAME, "Add");
            output.Put("objects", this.objects);
        }

    }
}
