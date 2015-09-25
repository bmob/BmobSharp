using System;
using System.Collections.Generic;

using System.Text;

namespace cn.bmob.io
{
    public abstract class BmobNumber<T> : IBmobValue<T>
    {
        private T value;

        public BmobNumber() { }

        public BmobNumber(T value)
        {
            this.value = value;
        }

        public T Get()
        {
            return this.value;
        }

        public void Set(Object o)
        {
            if (o is T)
            {
                this.value = (T)o;
            }
        }

        public override string ToString()
        {
            return Get().ToString();
        }

    }
}
