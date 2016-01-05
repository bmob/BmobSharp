using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;
using cn.bmob.json;

namespace cn.bmob.io
{
    /// <summary>
    /// ACL基础类 
    /// 
    /// ！！SimpleJson处理Dictionary<string, Object>才正常
    /// </summary>
    public class BmobACL : IBmobValue<Dictionary<string, Object>>
    {
        private Dictionary<string, Object> acls = new Dictionary<String, Object>(); //new Dictionary<String, Dictionary<string, bool>>();

        /// <summary>
        /// 获取ACL权限信息 { key : {write : true}, key2 : {read: true} }
        /// </summary>
        /// <returns>返回ACL列表， Object的内容为Dictionary<string, bool>可进行强转</returns>
        public Dictionary<string, Object> Get()
        {
            return acls;
        }

        public void Set(Object acls)
        {
            if (acls is Dictionary<string, Object>)
            {
                this.acls = (Dictionary<string, Object>)acls;
            }
            else if (acls is IDictionary<string, Object>)
            {
                IDictionary<String, Object> kvs = (IDictionary<string, Object>)acls;
                foreach (var entry in kvs)
                {
                    this.acls.Add(entry.Key, entry.Value);
                }
            }

        }

        /// <summary>
        /// key是objectId（用户表某个用户对应的objectId）或者是 *(表示公共的访问权限)，ACL 的值是 "读和写的权限", 这个JSON对象的key总是权限名, 而这些key的值总是 true
        /// </summary>
        public BmobACL ReadAccess(String objectId)
        {
            BmobOutput.Composite(acls, objectId, "read", true);
            return this;
        }

        public BmobACL WriteAccess(String objectId)
        {
            BmobOutput.Composite(acls, objectId, "write", true);
            return this;
        }

        public BmobACL RoleReadAccess(String rolename)
        {
            BmobOutput.Composite(acls, "role:" + rolename, "read", true);
            return this;
        }

        public BmobACL RoleWriteAccess(String rolename)
        {
            BmobOutput.Composite(acls, "role:" + rolename, "write", true);
            return this;
        }

    }
}
