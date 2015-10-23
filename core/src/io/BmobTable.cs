using System;
using System.Collections.Generic;
using System.Collections;

namespace cn.bmob.io
{
    /// <summary>
    /// 数据表操作类
    /// </summary>
    public class BmobTable : BmobObject, IBmobOperator
    {

        public sealed override string _type { get { return this.table; } }

        /// <summary>
        /// 获取表名， 默认为对象的名称
        /// </summary>
        public virtual String table { get { return this.GetType().Name; } }

        /// <summary>
        /// 数据的唯一标识。放开set功能！
        /// 
        /// TODO 如果设置了objectId则为更新！！！
        /// </summary>
        public String objectId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public String createdAt { get; internal set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public String updatedAt { get; internal set; }

        /// <summary>
        /// ACL信息，每条数据默认都有ACL信息
        /// </summary>
        public BmobACL ACL { get; set; }

        private IDictionary operates = new Dictionary<String, Object>();

        internal IBmobOperator handle(String column, Operate op)
        {
            BmobOutput.Save(operates, column, op);
            return this;
        }

        public IBmobOperator Increment(String column, int amount)
        {
            var value = new Increment();
            value.amount = amount;
            return handle(column, value);
        }

        /// <summary>
        /// 自增操作
        /// </summary>
        /// <param name="column">需要自增的字段</param>
        /// <returns></returns>
        public IBmobOperator Increment(String column)
        {
            return Increment(column, 1);
        }

        /// <summary>
        /// 删除一个对象中一个字段
        /// </summary>
        /// <param name="column">需要删除的字段名</param>
        /// <returns></returns>
        public IBmobOperator Delete(String column)
        {
            var value = new Delete();
            return handle(column, value);
        }

        /// <summary>
        /// 更新数组类型字段的数据。
        /// 
        /// 举个例子，技能skills是一个类似于集合的数组类型，那么我们可以使用该方法在原有skills值基础上添加一些对象（只有在skills原来的对象中不包含这些值的情况下才会被加入）
        /// </summary>
        public IBmobOperator AddUnique<T>(String column, List<T> values)
        {
            var value = new AddUnique<T>();
            value.objects = values;
            return handle(column, value);
        }

        /// <summary>
        /// 添加数组数据。
        /// 
        /// 添加一行记录时创建一个普通的类似于列表的数组类型字段。
        /// </summary>
        public IBmobOperator Add<T>(String column, List<T> values)
        {
            var value = new Add<T>();
            value.objects = values;
            return handle(column, value);
        }

        /// <summary>
        /// 删除数组数据。
        /// 
        /// 把values这些对象从column字段值中移除
        /// </summary>
        public IBmobOperator Remove<T>(String column, List<T> values)
        {
            var value = new Remove<T>();
            value.objects = values;
            return handle(column, value);
        }

        /// <summary>
        /// 添加关联信息
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="column">字段</param>
        /// <param name="values">关联信息</param>
        /// <returns></returns>
        public IBmobOperator AddRelation<T>(String column, BmobRelation<T> values) where T : BmobTable
        {
            var value = new AddRelation<T>();
            value.objects = values;
            return handle(column, value);
        }

        /// <summary>
        /// 删除关联信息
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="column">字段</param>
        /// <param name="values">关联信息</param>
        /// <returns></returns>
        public IBmobOperator RemoveRelation<T>(String column, BmobRelation<T> values) where T : BmobTable
        {
            var value = new RemoveRelation<T>();
            value.objects = values;
            return handle(column, value);
        }

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            this.objectId = input.getString("objectId");
            this.createdAt = input.getString("createdAt");
            this.updatedAt = input.getString("updatedAt");
            this.ACL = input.Get<BmobACL>("ACL");
        }

        public override void write(BmobOutput output, Boolean all)
        {
            base.write(output, all);
            output.Put("ACL", this.ACL);

            foreach (String key in operates.Keys)
            {
                output.Put(key, (Operate)operates[key]);
            }

            if (all)
            {
                output.Put("objectId", this.objectId);
                output.Put("createdAt", this.createdAt);
                output.Put("updatedAt", this.updatedAt);
            }
        }

    }
}
