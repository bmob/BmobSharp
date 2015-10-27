using System;

namespace cn.bmob.io
{
    /// <summary>
    /// BmobRole角色管理类
    /// </summary>
    public class BmobRole : BmobTable
    {
        /// <summary>
        /// 获取表名
        /// </summary>
        public override string table
        {
            get
            {
                return "_Role";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String name { get; set; }
        
        public BmobRole AddUsers(BmobRelation<BmobUser> value)
        {
            AddRelation("users", value);
            return this;
        }
        public BmobRole RemoveUsers(BmobRelation<BmobUser> value)
        {
            RemoveRelation("users", value);
            return this;
        }

        /// <summary>
        /// 一个角色可以包含另一个，可以为 2 个角色建立一个父-子关系。 这个关系的结果就是任何被授予父角色的权限隐含地被授予子角色。
        ///
        /// 这样的关系类型通常在用户管理的内容类的应用上比较常见, 比如在论坛中，有一些少数的用户是 "管理员（Administartors）", 有最高的权限，可以调整系统设置、 创建新的论坛等等。 另一类用户是 "版主（Moderators）"，他们可以对用户发帖的内容进行管理。可见，任何有管理员权限的人都应该有版主的权限。为建立起这种关系, 您应该把 "Administartors" 的角色设置为 "Moderators" 的子角色, 具体来说就是把 "Administrators" 这个角色加入 "Moderators" 对象的 roles 关系之中
        /// </summary>
        public BmobRole AddRoles(BmobRelation<BmobRole> value)
        {
            AddRelation("roles", value);
            return this;
        }
        public BmobRole RemoveRoles(BmobRelation<BmobRole> value)
        {
            RemoveRelation("roles", value);
            return this;
        }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put("name", this.name);
        }

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            this.name = input.getString("name");
        }

    }
}
