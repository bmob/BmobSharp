
namespace cn.bmob.io
{
    internal class RemoveRelation<T> : Operate where T : BmobTable
    {
        public BmobRelation<T> objects { get; set; }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put(OP_NAME, "RemoveRelation");
            output.Put("objects", this.objects);
        }
    }
}
