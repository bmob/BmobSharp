using System.Collections.Generic;

namespace cn.bmob.io
{
    internal class AddUnique<T> : Operate
    {
        public List<T> objects { get; set; }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put(OP_NAME, "AddUnique");
            output.Put("objects", this.objects);
        }

    }
}
