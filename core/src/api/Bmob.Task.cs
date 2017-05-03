#if UNITY_5 || UNITY_4 || UNITY_4_6 
#define Unity
#endif

using cn.bmob.io;
using cn.bmob.response;
using System;

using cn.bmob.http;
using System.Collections.Generic;

#if !Unity

using System.Threading.Tasks;
using System.Threading;

namespace cn.bmob.api
{
    /// <summary>
    /// 在windows下，可以直接调用然后调用Task.Result等待结果返回！但是在windowsphone中，需要进行额外的async/await处理，如：
    /// 
    /// private async void createTask_Click(object sender, RoutedEventArgs e)
    /// {
    ///    BmobApi table = new BmobApi();
    ///    table.name = "hello wp";
    ///
    ///    var result = await Bmob.CreateTaskAsync(TABLE_NAME, table);
    ///
    ///    string status = "OK";
    ///    if (result == null)
    ///    {
    ///        status = "ERROR";
    ///    }
    ///    updateStatus(createTask, status);
    /// }
    /// 
    /// 或者使用（这货不又是回调嘛，还不让直接用自带的！当然复杂的任务调用还是有它的优势的！）：
    /// 
    /// Bmob.CreateTaskAsync(TABLE_NAME, table).ContinueWith(f =>
    ///     {
    ///         string status = "OK";
    ///         if (f.Result == null)
    ///         {
    ///             status = "ERROR";
    ///         }
    ///
    ///         Dispatcher.BeginInvoke(() =>
    ///         {
    ///             updateStatus(createTask, status);
    ///         });
    ///     });
    /// 
    /// </summary>
    public partial class Bmob
    {
        #region GenTaskAsyncCode

        // / @see TEST#GenCode


        public Task<List<Dictionary<String, Object>>> BatchTaskAsync(BmobBatch requests)
        {
            return ExecuteTaskAsync<List<Dictionary<String, Object>>>(callback => { Batch(requests, callback); }, CancellationToken.None);
        }

        public Task<CreateCallbackData> CreateTaskAsync(String tablename, IBmobWritable data)
        {
            return ExecuteTaskAsync<CreateCallbackData>(callback => { Create(tablename, data, callback); }, CancellationToken.None);
        }

        public Task<CreateCallbackData> CreateTaskAsync<T>(T data) where T : cn.bmob.io.BmobTable
        {
            return ExecuteTaskAsync<CreateCallbackData>(callback => { Create(data, callback); }, CancellationToken.None);
        }

        public Task<DeleteCallbackData> DeleteTaskAsync(String tablename, String objectId)
        {
            return ExecuteTaskAsync<DeleteCallbackData>(callback => { Delete(tablename, objectId, callback); }, CancellationToken.None);
        }

        public Task<DeleteCallbackData> DeleteTaskAsync<T>(T data) where T : cn.bmob.io.BmobTable
        {
            return ExecuteTaskAsync<DeleteCallbackData>(callback => { Delete(data, callback); }, CancellationToken.None);
        }

        public Task<DeleteCallbackData> DeleteUserTaskAsync(String objectId, String sessionToken)
        {
            return ExecuteTaskAsync<DeleteCallbackData>(callback => { DeleteUser(objectId, sessionToken, callback); }, CancellationToken.None);
        }

        public Task<DeleteCallbackData> DeleteUserTaskAsync<T>(T data) where T : cn.bmob.io.BmobUser
        {
            return ExecuteTaskAsync<DeleteCallbackData>(callback => { DeleteUser(data, callback); }, CancellationToken.None);
        }

        public Task<EmptyCallbackData> EmailVerifyTaskAsync(String email)
        {
            return ExecuteTaskAsync<EmptyCallbackData>(callback => { EmailVerify(email, callback); }, CancellationToken.None);
        }

        public Task<EndPointCallbackData<T>> EndpointTaskAsync<T>(String eMethod, System.Collections.Generic.IDictionary<System.String, System.Object> parameters)
        {
            return ExecuteTaskAsync<EndPointCallbackData<T>>(callback => { Endpoint(eMethod, parameters, callback); }, CancellationToken.None);
        }

        public Task<EndPointCallbackData<T>> EndpointTaskAsync<T>(String eMethod)
        {
            return ExecuteTaskAsync<EndPointCallbackData<T>>(callback => { Endpoint(eMethod, callback); }, CancellationToken.None);
        }

        public Task<EmptyCallbackData> FileDeleteTaskAsync(String url)
        {
            return ExecuteTaskAsync<EmptyCallbackData>(callback => { FileDelete(url, callback); }, CancellationToken.None);
        }

        public Task<EmptyCallbackData> FileDeleteTaskAsync(BmobFile file)
        {
            return ExecuteTaskAsync<EmptyCallbackData>(callback => { FileDelete(file, callback); }, CancellationToken.None);
        }

        public Task<UploadCallbackData> FileUploadTaskAsync(BmobLocalFile file)
        {
            return ExecuteTaskAsync<UploadCallbackData>(callback => { FileUpload(file, callback); }, CancellationToken.None);
        }

        public Task<UploadCallbackData> FileUploadTaskAsync(String localPath)
        {
            return ExecuteTaskAsync<UploadCallbackData>(callback => { FileUpload(localPath, callback); }, CancellationToken.None);
        }

        public Task<QueryCallbackData<T>> FindTaskAsync<T>(String tablename, BmobQuery query)
        {
            return ExecuteTaskAsync<QueryCallbackData<T>>(callback => { Find(tablename, query, callback); }, CancellationToken.None);
        }

        public Task<T> GetTaskAsync<T>(String tablename, String objectId)
        {
            return ExecuteTaskAsync<T>(callback => { Get(tablename, objectId, callback); }, CancellationToken.None);
        }

        public Task<T> GetTaskAsync<T>(T data) where T : cn.bmob.io.BmobTable
        {
            return ExecuteTaskAsync<T>(callback => { Get(data, callback); }, CancellationToken.None);
        }

        public Task<T> LoginTaskAsync<T>(String username, String pwd) where T : cn.bmob.io.BmobUser
        {
            return ExecuteTaskAsync<T>(callback => { Login(username, pwd, callback); }, CancellationToken.None);
        }

        public Task<BmobUser> LoginTaskAsync(String username, String pwd)
        {
            return ExecuteTaskAsync<BmobUser>(callback => { Login(username, pwd, callback); }, CancellationToken.None);
        }


        public Task<EmptyCallbackData> PushTaskAsync(PushParamter param)
        {
            return ExecuteTaskAsync<EmptyCallbackData>(callback => { Push(param, callback); }, CancellationToken.None);
        }

        public Task<EmptyCallbackData> ResetTaskAsync(String email)
        {
            return ExecuteTaskAsync<EmptyCallbackData>(callback => { Reset(email, callback); }, CancellationToken.None);
        }


        public Task<T> SignupTaskAsync<T>(T user) where T : cn.bmob.io.BmobUser
        {
            return ExecuteTaskAsync<T>(callback => { Signup(user, callback); }, CancellationToken.None);
        }

        public Task<BmobUser> SignupTaskAsync(BmobUser user)
        {
            return ExecuteTaskAsync<BmobUser>(callback => { Signup(user, callback); }, CancellationToken.None);
        }

        public Task<ThumbnailCallbackData> ThumbnailTaskAsync(ThumbnailParameter param)
        {
            return ExecuteTaskAsync<ThumbnailCallbackData>(callback => { Thumbnail(param, callback); }, CancellationToken.None);
        }

        public Task<TimeStampCallbackData> TimestampTaskAsync()
        {
            return ExecuteTaskAsync<TimeStampCallbackData>(callback => { Timestamp(callback); }, CancellationToken.None);
        }

        public Task<QueryCallbackData<T>> SqlTaskAsync<T>(string bql, List<Object> values = default(List<Object>))
        {
            return ExecuteTaskAsync<QueryCallbackData<T>>(callback => { Sql(bql, values, callback); }, CancellationToken.None);
        }

        public Task<UpdateCallbackData> UpdateTaskAsync(String tablename, String objectId, IBmobWritable data)
        {
            return ExecuteTaskAsync<UpdateCallbackData>(callback => { Update(tablename, objectId, data, callback); }, CancellationToken.None);
        }

        public Task<UpdateCallbackData> UpdateTaskAsync<T>(T data) where T : cn.bmob.io.BmobTable
        {
            return ExecuteTaskAsync<UpdateCallbackData>(callback => { Update(data, callback); }, CancellationToken.None);
        }

        public Task<UpdateCallbackData> UpdateUserTaskAsync(String objectId, BmobUser data, String sessionToken)
        {
            return ExecuteTaskAsync<UpdateCallbackData>(callback => { UpdateUser(objectId, data, sessionToken, callback); }, CancellationToken.None);
        }

        public Task<UpdateCallbackData> UpdateUserTaskAsync<T>(T data) where T : cn.bmob.io.BmobUser
        {
            return ExecuteTaskAsync<UpdateCallbackData>(callback => { UpdateUser(data, callback); }, CancellationToken.None);
        }

        public Task<RequestSmsCodeCallbackData> RequestSmsCodeTaskAsync(string mobilePhoneNumber)
        {
            return ExecuteTaskAsync<RequestSmsCodeCallbackData>(callback => { RequestSmsCode(mobilePhoneNumber, callback); }, CancellationToken.None);
        }
        public Task<RequestSmsCodeCallbackData> RequestSmsCodeTaskAsync(string mobilePhoneNumber, string template)
        {
            return ExecuteTaskAsync<RequestSmsCodeCallbackData>(callback => { RequestSmsCode(mobilePhoneNumber, template, callback); }, CancellationToken.None);
        }
        public Task<VerifySmsCodeCallbackData> VerifySmsCodeTaskAsync(string mobilePhoneNumber, string code)
        {
            return ExecuteTaskAsync<VerifySmsCodeCallbackData>(callback => { VerifySmsCode(mobilePhoneNumber, code, callback); }, CancellationToken.None);
        }
        public Task<QuerySmsCallbackData> QuerySmsTaskAsync(String smsId)
        {
            return ExecuteTaskAsync<QuerySmsCallbackData>(callback => { QuerySms(smsId, callback); }, CancellationToken.None);
        }

        #endregion

        private Task<TResult> ExecuteTaskAsync<TResult>(Action<BmobCallback<TResult>> request, CancellationToken token)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var taskCompletionSource = new TaskCompletionSource<TResult>();

            try
            {
                request((response, exception) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        taskCompletionSource.TrySetCanceled();
                    }
                    else if (exception != null)
                    {
                        taskCompletionSource.TrySetException(exception);
                    }
                    else
                    {
                        taskCompletionSource.TrySetResult(response);
                    }
                });

                token.Register(() =>
                {
                    taskCompletionSource.TrySetCanceled();
                });
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }

            return taskCompletionSource.Task;
        }


    }

}

#endif