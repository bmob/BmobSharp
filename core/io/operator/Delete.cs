
namespace cn.bmob.io
{
    internal class Delete : Operate
    {

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);
            output.Put(OP_NAME, "Delete");
        }

    }
}
