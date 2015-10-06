using System;

namespace cn.bmob.io
{
    /// <summary>
    /// 用户操作类
    /// </summary>
    public class BmobUser : BmobTable
    {
        /// <summary>
        /// 对应的操作数据表，注意：操作对应的表是_User，这是系统内置的表
        /// </summary>
        public const String TABLE = "_User";

        /// <summary>
        /// 获取当前用户
        /// </summary>
        public static BmobUser CurrentUser { get; internal set; }

        /// <summary>
        /// 退出登录
        /// </summary>
        public static void LogOut()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        public override sealed string table { get { return TABLE; } }

        /// <summary>
        /// 用户名
        /// </summary>
        public String username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public String password { get; set; }

        /// <summary>
        /// 手机号，接收修改密码短信验证码
        /// </summary>
        public String phone { get; set; }

        /// <summary>
        /// 邮箱验证信息
        /// </summary>
        public BmobBoolean emailVerified { get; set; }

        /// <summary>
        /// 邮箱，用于校验和重置密码！
        /// </summary>
        public String email { get; set; }

        /// <summary>
        /// 登录之后的会话信息
        /// </summary>
        public String sessionToken { get; set; }

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            this.username = input.getString("username");
            this.password = input.getString("password");
            this.email = input.getString("email");
            this.sessionToken = input.getString("sessionToken");

            this.emailVerified = input.getBoolean("emailVerified");
        }

        public override void write(BmobOutput output, Boolean all)
        {
            base.write(output, all);

            if (all)
            {
                output.Put("sessionToken", this.sessionToken);
            }

            output.Put("username", this.username);
            output.Put("password", this.password);
            output.Put("email", this.email);
        }

    }
}
