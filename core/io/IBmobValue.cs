
namespace cn.bmob.io
{
    /// <summary>
    /// 实现该接口的类直接进行赋值
    /// 
    /// Visible For API! 暂时仅支持API自带的值类型. 由于泛型T，导致在JSON解析注册时很麻烦，不推荐用户实现值类型！
    /// </summary>
    public interface IBmobValue<T> : IBmobValue
    {
        T Get();
    }

    public interface IBmobValue
    {
        void Set(object data);
    }
}
