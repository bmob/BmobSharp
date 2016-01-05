using cn.bmob.http;
using cn.bmob.io;
using cn.bmob.response;
using System;
using System.Collections.Generic;

namespace cn.bmob.api
{
    /// <summary>
    /// SDK对外API抽象接口类
    /// </summary>
    public interface IBmobAPI
    {

        void initialize(string appKey, String restKey);

        // /

        void Create(string tablename, IBmobWritable data, BmobCallback<CreateCallbackData> callback);
        void Update(string tablename, string objectId, IBmobWritable data, BmobCallback<UpdateCallbackData> callback);
        void Delete(string tablename, string objectId, BmobCallback<DeleteCallbackData> callback);
        void Get<T>(string tablename, string objectId, BmobCallback<T> callback);
        void Find<T>(string tablename, BmobQuery query, BmobCallback<QueryCallbackData<T>> callback);

        // /

        void Signup<T>(T user, BmobCallback<T> callback) where T : BmobUser;
        void Login<T>(string username, string pwd, BmobCallback<T> callback) where T : BmobUser;

        void UpdateUser(string objectId, BmobUser data, string sessionToken, BmobCallback<UpdateCallbackData> callback);
        void DeleteUser(string objectId, string sessionToken, BmobCallback<DeleteCallbackData> callback);

        void Reset(string email, BmobCallback<EmptyCallbackData> callback);
        void EmailVerify(string email, BmobCallback<EmptyCallbackData> callback);
        
        // /

        void Batch(BmobBatch requests, BmobCallback<List<Dictionary<String, Object>>> callback);
        void Endpoint<T>(string eMethod, IDictionary<String, Object> parameters, BmobCallback<EndPointCallbackData<T>> callback);

        void FileUpload(BmobLocalFile file, BmobCallback<UploadCallbackData> callback);
        void FileDelete(String url, BmobCallback<EmptyCallbackData> callback);
        
        void Thumbnail(ThumbnailParameter param, BmobCallback<ThumbnailCallbackData> callback);
        void Push(PushParamter param, BmobCallback<EmptyCallbackData> callback);

        void Timestamp(BmobCallback<TimeStampCallbackData> callback);
        void Sql<T>(string bql, List<Object> values, BmobCallback<QueryCallbackData<T>> callback);

        void RequestSmsCode(string mobilePhoneNumber, string template, BmobCallback<RequestSmsCodeCallbackData> callback);
        void VerifySmsCode(string mobilePhoneNumber, string smsId, BmobCallback<VerifySmsCodeCallbackData> callback);
        void QuerySms(String smsId, BmobCallback<QuerySmsCallbackData> callback);
    }
}
