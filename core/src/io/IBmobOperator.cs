using System;
namespace cn.bmob.io
{

    /// <summary>
    /// 操作类。可以作用于任一对象的任何字段，这里提取出来作为公共的基类。
    /// </summary>
    public interface IBmobOperator
    {
        IBmobOperator Increment(string column);
        IBmobOperator Increment(string column, int amount);

        IBmobOperator Add<T>(string column, System.Collections.Generic.List<T> values);
        IBmobOperator AddUnique<T>(string column, System.Collections.Generic.List<T> values);
        IBmobOperator Remove<T>(string column, System.Collections.Generic.List<T> values);

        IBmobOperator AddRelation<T>(string column, BmobRelation<T> values) where T : BmobTable;
        IBmobOperator RemoveRelation<T>(string column, BmobRelation<T> values) where T : BmobTable;
    }
}
