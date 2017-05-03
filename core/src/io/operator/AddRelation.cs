
namespace cn.bmob.io
{
    /// <summary>
    /// Relation 类型被用在多对多的类型上, 移动端的库将使用 BmobRelation 作为值, 它有一个 className 字段表示目标对象的类名
    /// 
    /// 当使用查询时， Relation 对象的行为很像是 Pointer 的数组, 任何操作针对于 Pointer 的数组的 (除了 include) 都可以对 Relation 起作用.
    /// </summary>
    internal class AddRelation<T> : Operate where T : BmobTable
    {
        public BmobRelation<T> objects { get; set; }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put(OP_NAME, "AddRelation");
            output.Put("objects", this.objects);
        }
    }
}
