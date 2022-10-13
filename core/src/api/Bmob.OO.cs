using cn.bmob.exception;
using cn.bmob.http;
using cn.bmob.io;
using cn.bmob.response;
using System;
using System.Collections.Generic;

namespace cn.bmob.api
{
    public partial class Bmob
    {


        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="callback"></param>
        public void FileUpload(String localPath, BmobCallback<UploadCallbackData> callback)
        {
#if !WIN8_1
            this.FileUpload(new BmobLocalFile(localPath), callback);
#else
            callback(null, new BmobException("SDK WIN8.1暂不支持该方法！！！"));
#endif
        }

        public void RequestSmsCode(string mobilePhoneNumber, BmobCallback<RequestSmsCodeCallbackData> callback)
        {
            this.RequestSmsCode(mobilePhoneNumber, null, callback);
        }

        public void Endpoint<T>(String eMethod, BmobCallback<EndPointCallbackData<T>> callback)
        {
            this.Endpoint<T>(eMethod, new Dictionary<String, Object>(), callback);
        }

        public void Create<T>(T data, BmobCallback<CreateCallbackData> callback) where T : BmobTable
        {
            this.Create(data.table, data, callback);
        }

        public void Get<T>(T data, BmobCallback<T> callback) where T : BmobTable
        {
            this.Get<T>(data.table, data.objectId, callback);
        }

        public void Update<T>(T data, BmobCallback<UpdateCallbackData> callback) where T : BmobTable
        {
            this.Update(data.table, data.objectId, data, callback);
        }

        public void Delete<T>(T data, BmobCallback<DeleteCallbackData> callback) where T : BmobTable
        {
            this.Delete(data.table, data.objectId, callback);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="callback">返回内置的BmobUser对象。如果User表中添加了字段，请使用Login<T>泛型调用方式。</param>
        public void Login(String username, String pwd, BmobCallback<BmobUser> callback)
        {
            this.Login<BmobUser>(username, pwd, callback);
        }

        /// <summary>
        /// 使用默认的BmobUser进行注册。即不添加任何额外的字段情况下使用。
        /// </summary>
        public void Signup(BmobUser user, BmobCallback<BmobUser> callback)
        {
            this.Signup<BmobUser>(user, callback);
        }

        public void UpdateUser<T>(T data, BmobCallback<UpdateCallbackData> callback) where T : BmobUser
        {
            this.UpdateUser(data.objectId, data, data.sessionToken, callback);
        }

        public void DeleteUser<T>(T data, BmobCallback<DeleteCallbackData> callback) where T : BmobUser
        {
            this.DeleteUser(data.objectId, data.sessionToken, callback);
        }

        public void FileDelete(BmobFile file, BmobCallback<EmptyCallbackData> callback)
        {
            this.FileDelete(file.url, callback);
        }
    }
}
