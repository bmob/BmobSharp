
using System;
namespace cn.bmob.io
{
    /// <summary>
    /// 原子计数器
    /// </summary>
    internal class Increment : Operate
    {

        protected string op { get { return "Increment"; } }

        public int amount { get; set; }

        public override void write(BmobOutput output, Boolean all)
        {
            output.Put("__op", this.op);
            output.Put("amount", this.amount);
        }
    }
}
