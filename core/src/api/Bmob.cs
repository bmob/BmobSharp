using System;
using cn.bmob.response;
using System.Collections.Generic;
using cn.bmob.io;
using cn.bmob.tools;
using cn.bmob.config;
using cn.bmob.http;
using cn.bmob.exception;
using cn.bmob.Extensions;

namespace cn.bmob.api
{
    /// <summary>
    /// 基本功能实现
    /// </summary>
    public abstract partial class Bmob : IBmobAPI
    {
        // 全局唯一，一个appkey对一个SDK
        private static String applicationId;
        private static String restfulKey;

        internal virtual String appKey
        {
            get { return applicationId; }
            set { applicationId = value; }
        }

        internal virtual String restKey
        {
            get { return restfulKey; }
            set { restfulKey = value; }
        }

        internal abstract void submit<T>(BmobCommand<T> command, BmobCallback<T> callback);

        private void fillInteractObject(BmobInteractObject interact)
        {
            interact.AppKey = this.appKey;
            interact.RestKey = this.restKey;
            if (interact.SessionToken == null && BmobUser.CurrentUser != null)
            {
                interact.SessionToken = BmobUser.CurrentUser.sessionToken;
            }
        }

        internal void submitUploadFile<T>(BmobInteractObject interact, BmobCallback<T> callback)
        {
            fillInteractObject(interact);
            submit(new BmobFileCommand<T>(interact), callback);
        }

        internal void submit<T>(BmobInteractObject interact, BmobCallback<T> callback)
        {
            fillInteractObject(interact);
            submit(new BmobCommand<T>(interact), callback);
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="appKey">应用程序密钥AppKey</param>
        public void initialize(String appKey, String restKey)
        {
            this.appKey = appKey;
            this.restKey = restKey;
        }

        /// <summary>
        /// 创建
        /// 
        /// 往Bmob云数据库中添加一条数据记录
        /// </summary>
        /// <param name="data">需要添加的数据</param>
        /// <param name="callback">添加之后的结果回调</param>
        public void Create(String tablename, IBmobWritable data, BmobCallback<CreateCallbackData> callback)
        {
            var bia = BmobInteractObject.Create;
            bia.Data = data;
            bia.Table = tablename;

            submit(bia, callback);
        }

        /// <summary>
        /// 更新Bmob云数据库中的某一条记录信息
        /// </summary>
        /// <param name="tablename">数据表名称</param>
        /// <param name="objectId">这个记录对应的objectId</param>
        /// <param name="data">需要更新的记录信息</param>
        /// <param name="callback">更新之后的回调</param>
        public void Update(String tablename, String objectId, IBmobWritable data, BmobCallback<UpdateCallbackData> callback)
        {
            var bia = BmobInteractObject.Update;
            bia.Data = data;
            bia.Table = tablename;
            bia.ObjectId = objectId;

            submit(bia, callback);
        }


        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="objectId">这条用户信息对应的objectId</param>
        /// <param name="data">需要更新的内容</param>
        /// <param name="sessionToken">这个用户登录之后获取的sessionToken</param>
        /// <param name="callback">更新之后的回调</param>
        public void UpdateUser(String objectId, BmobUser data, String sessionToken, BmobCallback<UpdateCallbackData> callback)
        {
            if (Utilities.Empty(sessionToken))
            {
                callback(null, new BmobException("删除用户表数据时，必须先登录获取sessionToken的值！"));
                return;
            }

            var bia = BmobInteractObject.Update;
            bia.Data = data;
            bia.Table = "_User";
            bia.ObjectId = objectId;
            bia.SessionToken = sessionToken;

            submit(bia, callback);
        }

        /// <summary>
        /// 删除
        /// 
        /// 从Bmob云数据库中删除一条数据记录
        /// </summary>
        /// <param name="tablename">数据表名称</param>
        /// <param name="objectId">这条数据记录的objectId</param>
        /// <param name="callback">删除数据结果的回调</param>
        public void Delete(String tablename, String objectId, BmobCallback<DeleteCallbackData> callback)
        {
            var bia = BmobInteractObject.Delete;
            bia.Table = tablename;
            bia.ObjectId = objectId;

            submit(bia, callback);
        }


        /// <summary>
        /// 删除用户
        /// 
        /// 从Bmob云数据库中删除一个用户
        /// </summary>
        /// <param name="objectId">这个用户记录的objectId</param>
        /// <param name="sessionToken">这个用户的sessionToken信息</param>
        /// <param name="callback">删除用户结果的回调</param>
        public void DeleteUser(String objectId, String sessionToken, BmobCallback<DeleteCallbackData> callback)
        {
            if (Utilities.Empty(sessionToken))
            {
                callback(null, new BmobException("删除用户表数据时，必须先登录获取sessionToken的值！"));
                return;
            }

            var bia = BmobInteractObject.Delete;
            bia.Table = "_User";
            bia.ObjectId = objectId;
            bia.SessionToken = sessionToken;

            submit(bia, callback);
        }

        /// <summary>
        /// 查找
        /// 
        /// 从Bmob云数据库中获取数据记录列表
        /// </summary>
        /// <typeparam name="T">推荐使用继承自BmobTable类型</typeparam>
        /// <param name="tablename">数据表名称</param>
        /// <param name="query">查询条件</param>
        /// <param name="callback">查询结果回调</param>
        public void Find<T>(String tablename, BmobQuery query, BmobCallback<QueryCallbackData<T>> callback)
        {
            var bia = BmobInteractObject.Find;
            bia.Table = tablename;
            bia.Data = query;

            submit(bia, callback);
        }

        /// <summary>
        /// 根据objectId获取一条记录
        /// 
        /// 从Bmob云数据库中获取某一条数据记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename">数据表名称</param>
        /// <param name="objectId">这条数据记录的objectId</param>
        /// <param name="callback">获取数据的结果回调</param>
        public void Get<T>(String tablename, string objectId, BmobCallback<T> callback)
        {
            var bia = BmobInteractObject.Get;
            bia.Table = tablename;
            bia.ObjectId = objectId;

            submit(bia, callback);
        }


        /// <summary>
        /// 用户登录
        /// 
        /// 返回用户User表改用户的所有字段
        /// </summary>
        /// <typeparam name="T">BmobUser用户类泛型</typeparam>
        /// <param name="username">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="callback">用户登录的结果回调</param>
        public void Login<T>(string username, string pwd, BmobCallback<T> callback) where T : BmobUser
        {
            var bia = BmobInteractObject.Login;
            bia.Table = "_User";

            var user = new BmobUser();
            user.username = username;
            user.password = pwd;
            bia.Data = user;

            submit<T>(bia, (cu, ex) =>
            {
                // 记录当前已登录的用户！
                BmobUser.CurrentUser = cu;
                callback(cu, ex);
            });
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <typeparam name="T">BmobUser用户类泛型</typeparam>
        /// <param name="user">用户注册信息</param>
        /// <param name="callback">注册结果回调。 仅返回用户User表的objectId,createdAt,sessionToken字段！同时会把当前登录的用户信息保存到BmobUser.CurrentUser中！</param>
        public void Signup<T>(T user, BmobCallback<T> callback) where T : BmobUser
        {
            if (Utilities.Empty(user) || Utilities.Empty(user.username) || Utilities.Empty(user.password))
            {
                callback(null, new BmobException("用户注册必须包含用户名和密码！"));
                return;
            }

            var bia = BmobInteractObject.Signup;
            bia.Table = "_User";
            bia.Data = user;

            submit(bia, callback);
        }

        /// <summary>
        /// 重置密码
        /// 
        /// 查找用户表中对应email，并发邮件到该邮箱修改密码。（往注册用户的邮箱中发送一封密码重置的邮件）
        /// </summary>
        /// <param name="email">用户的Email</param>
        /// <param name="callback">重置结果的回调</param>
        public void Reset(String email, BmobCallback<EmptyCallbackData> callback)
        {
            var bia = BmobInteractObject.PwdReset;
            bia.Table = "_User";

            var user = new BmobUser();
            user.email = email;
            bia.Data = user;

            submit(bia, callback);
        }

        /// <summary>
        /// 邮箱验证
        /// </summary>
        /// <param name="email">需要验证的邮箱号</param>
        /// <param name="callback">验证结果的回调</param>
        public void EmailVerify(String email, BmobCallback<EmptyCallbackData> callback)
        {
            var bia = BmobInteractObject.EmailVerify;
            bia.Table = "_User";

            var user = new BmobUser();
            user.email = email;
            bia.Data = user;

            submit(bia, callback);
        }


        /// <summary>
        /// 云端代码
        /// 
        /// 执行Bmob的云端代码. 
        /// 
        /// 注意：SDK暂时不支持直接从云端返回primitive类型的数据。也就是返回的值要么是键值对，要么是数组
        /// </summary>
        /// <typeparam name="T">对象。 FIXME 当前处理List的返回有问题！需要进一步的完善。</typeparam>
        /// <param name="eMethod">云端代码方法名</param>
        /// <param name="callback">云端代码结果的回调</param>
        public void Endpoint<T>(String eMethod, IDictionary<String, Object> parameters, BmobCallback<EndPointCallbackData<T>> callback)
        {
            var bia = BmobInteractObject.Endpoint;
            bia.EndPointName = eMethod;

            var endpoint = new BmobKV();
            if (parameters != null) endpoint.PutAll(parameters);
            bia.Data = endpoint;

            submit(bia, callback);
        }

        /// <summary>
        /// 文件上传
        /// 
        /// 将文件上传到Bmob文件服务中
        /// </summary>
        /// <param name="file">本地文件对象。使用文件名/文件内容/二进制流来构造。</param>
        /// <param name="callback">上传文件的结果回调，返回BmobFile对象</param>
        public void FileUpload(BmobLocalFile file, BmobCallback<UploadCallbackData> callback)
        {
            var bia = BmobInteractObject.Files;
            bia.Data = file;
            bia.Table = "BMOB";

            submitUploadFile(bia, callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">如：M02/C2/95/oYYBAFYDuR6AD3AbAAAUWOcV3Ew650.png</param>
        /// <param name="callback"></param>
        public void FileDelete(String url, BmobCallback<EmptyCallbackData> callback)
        {
            var bia = BmobInteractObject.FileDelete;
            bia.Data = new BmobKV().Put("filename", url);

            submit(bia, callback);
        }

        public void Batch(BmobBatch requests, BmobCallback<List<Dictionary<String, Object>>> callback)
        {
            var bia = BmobInteractObject.Batch;
            bia.Data = requests;

            submit(bia, callback);
        }

        public void Thumbnail(ThumbnailParameter param, BmobCallback<ThumbnailCallbackData> callback)
        {
            var bia = BmobInteractObject.Thumbnail;
            bia.Data = param;
            bia.Table = "BMOB";

            submit(bia, callback);
        }

        public void Push(PushParamter param, BmobCallback<EmptyCallbackData> callback)
        {
            var bia = BmobInteractObject.Push;
            bia.Data = param;

            submit(bia, callback);
        }

        /// <summary>
        /// 获取服务器的时间
        ///
        /// 获取Bmob云服务器的时间戳
        /// </summary>
        /// <param name="callback">结果回调</param>
        public void Timestamp(BmobCallback<TimeStampCallbackData> callback)
        {
            var bia = BmobInteractObject.Timestamp;
            submit(bia, callback);
        }

        /// <summary>
        /// 官方文档： http://docs.bmob.cn/bql/index.html?menukey=otherdoc&key=bql
        /// </summary>
        /// <param name="bql">e.g. : select * from Player where name=? limit ?,? order by name' </param>
        /// <param name="values">必须是JsonAdapter.JSON能正常序列化的对象。 e.g. : ["dennis", 0, 100]</param>
        public void Sql<T>(string bql, List<Object> values, BmobCallback<QueryCallbackData<T>> callback)
        {
            var bia = BmobInteractObject.BQL;
            var kv = new BmobKV().Put("bql", bql);
            if (values != null)
            {
                kv.Put("values", values);
            }
            bia.Data = kv;

            submit(bia, callback);
        }

        public void RequestSmsCode(string mobilePhoneNumber, string template, BmobCallback<RequestSmsCodeCallbackData> callback)
        {
            var bia = BmobInteractObject.RequestSMS;
            var kv = new SMSParamter();
            kv.mobilePhoneNumber = mobilePhoneNumber;
            kv.template = template;

            bia.Data = kv;

            submit(bia, callback);
        }
        public void VerifySmsCode(string mobilePhoneNumber, string code, BmobCallback<VerifySmsCodeCallbackData> callback)
        {
            var bia = BmobInteractObject.VerifySMS;
            var kv = new SMSParamter();
            kv.mobilePhoneNumber = mobilePhoneNumber;
            kv.code = code;

            bia.Data = kv;

            submit(bia, callback);
        }
        public void QuerySms(String smsId, BmobCallback<QuerySmsCallbackData> callback)
        {
            var bia = BmobInteractObject.QuerySMS;
            var kv = new SMSParamter();
            kv.smsId = smsId;

            bia.Data = kv;

            submit(bia, callback);
        }
    }

}
