using System;
using System.Collections.Generic;

using System.Text;

namespace cn.bmob.io
{
    /// <summary>
    /// Pointer 类型是当前对象要指向另一个对象时使用，它包含了 className 和 objectId 作为一个指针正确指向的必填值。
    /// T类型必须继承自BmobTable！
    /// 
    /// 如果你使用
    /// 
    /// 如，指向用户对象的 Pointer 的 className 为_User, 前面加一个下划线表示开发者不能定义的类名, 而且所指的类是系统内置的。
    /// </summary>
    public class BmobPointer<T> : BmobObject where T : BmobTable
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public BmobPointer() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="t">指针数据</param>
        public BmobPointer(T t)
        {
            this.reference = t;
        }

        /// <summary>
        /// 指针类型
        /// </summary>
        public override string _type { get { return "Pointer"; } }

        /// <summary>
        /// 关联表的名称
        /// </summary>
        public string className
        {
            get { return reference == null ? "" : reference.table; }
        }

        // TODO 获取其他字段时远程请求
        public T reference { get; set; }

        /// <summary>
        /// 序列化时优先获取refObjectId的值，refObjectId值为null时才取reference的objectId值。
        /// </summary>
        private string _refObjectId;

        /// <summary>
        /// use T.objectId instead.
        /// </summary>
        [Obsolete]
        public string refObjectId
        {
            get
            {
                if (_refObjectId == null && reference != null)
                {
                    return reference.objectId;
                }

                return _refObjectId;
            }
            set { this._refObjectId = value; }
        }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put(TYPE_NAME, this._type);
            output.Put("className", this.className);
            output.Put("objectId", refObjectId);
        }

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            var className = input.getString("className");
            var objectId = input.getString("objectId");
            var type = input.getString(TYPE_NAME);

            // 返回了对象的详细信息时，type字段值为空。
            if (type == this._type)
            {
                this.refObjectId = objectId;
            }
            else
            {
                this.reference = Activator.CreateInstance<T>();
                this.reference.readFields(input);
            }

        }

        #region Implicit Conversions
        public static implicit operator BmobPointer<T>(T data)
        {
            return new BmobPointer<T>(data);
        }

        #endregion
    }
}
