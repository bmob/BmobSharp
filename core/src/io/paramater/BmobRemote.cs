using cn.bmob.api;
using cn.bmob.exception;
using cn.bmob.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cn.bmob.io
{
    /// <summary>
    /// 可以参考BmobCloud
    /// </summary>
    public sealed class BmobRemote : BmobObject
    {

        private class __Bmob : Bmob
        {
            private List<BmobInteractObject> requests = new List<BmobInteractObject>();

            internal override void submit<T>(BmobCommand<T> command, BmobCallback<T> callback)
            {
                requests.Add(command.getReceiver());
            }

            public List<BmobInteractObject> Requests { get { return requests; } }

        }

        private Bmob BmobWrapper = new __Bmob();

        private void NonCallback<T>(T resp, BmobException ex) { }

        #region GenCode --------------------------

        public BmobRemote Batch(BmobBatch requests)
        {
            BmobWrapper.Batch(requests, NonCallback);
            return this;
        }

        public BmobRemote Create(String tablename, IBmobWritable data)
        {
            BmobWrapper.Create(tablename, data, NonCallback);
            return this;
        }

        public BmobRemote Create<T>(T data) where T : cn.bmob.io.BmobTable
        {
            BmobWrapper.Create<T>(data, NonCallback);
            return this;
        }

        public BmobRemote Delete(String tablename, String objectId)
        {
            BmobWrapper.Delete(tablename, objectId, NonCallback);
            return this;
        }

        public BmobRemote Delete<T>(T data) where T : cn.bmob.io.BmobTable
        {
            BmobWrapper.Delete<T>(data, NonCallback);
            return this;
        }

        public BmobRemote DeleteUser(String objectId, String sessionToken)
        {
            BmobWrapper.DeleteUser(objectId, sessionToken, NonCallback);
            return this;
        }

        public BmobRemote DeleteUser<T>(T data) where T : cn.bmob.io.BmobUser
        {
            BmobWrapper.DeleteUser<T>(data, NonCallback);
            return this;
        }

        public BmobRemote EmailVerify(String email)
        {
            BmobWrapper.EmailVerify(email, NonCallback);
            return this;
        }

        public BmobRemote Endpoint<T>(String eMethod, System.Collections.Generic.IDictionary<System.String, System.Object> parameters)
        {
            BmobWrapper.Endpoint<T>(eMethod, parameters, NonCallback);
            return this;
        }

        public BmobRemote Endpoint<T>(String eMethod)
        {
            BmobWrapper.Endpoint<T>(eMethod, NonCallback);
            return this;
        }

        public BmobRemote FileDelete(String url)
        {
            BmobWrapper.FileDelete(url, NonCallback);
            return this;
        }

        public BmobRemote Find<T>(String tablename, BmobQuery query)
        {
            BmobWrapper.Find<T>(tablename, query, NonCallback);
            return this;
        }

        public BmobRemote Get<T>(String tablename, String objectId)
        {
            BmobWrapper.Get<T>(tablename, objectId, NonCallback);
            return this;
        }

        public BmobRemote Get<T>(T data) where T : cn.bmob.io.BmobTable
        {
            BmobWrapper.Get<T>(data, NonCallback);
            return this;
        }

        public BmobRemote Login<T>(String username, String pwd) where T : cn.bmob.io.BmobUser
        {
            BmobWrapper.Login<T>(username, pwd, NonCallback);
            return this;
        }

        public BmobRemote Login(String username, String pwd)
        {
            BmobWrapper.Login(username, pwd, NonCallback);
            return this;
        }
        
        public BmobRemote Push(PushParamter param)
        {
            BmobWrapper.Push(param, NonCallback);
            return this;
        }

        public BmobRemote Reset(String email)
        {
            BmobWrapper.Reset(email, NonCallback);
            return this;
        }
        
        public BmobRemote Signup<T>(T user) where T : cn.bmob.io.BmobUser
        {
            BmobWrapper.Signup<T>(user, NonCallback);
            return this;
        }

        public BmobRemote Signup(BmobUser user)
        {
            BmobWrapper.Signup(user, NonCallback);
            return this;
        }

        public BmobRemote Thumbnail(ThumbnailParameter param)
        {
            BmobWrapper.Thumbnail(param, NonCallback);
            return this;
        }

        public BmobRemote Update(String tablename, String objectId, IBmobWritable data)
        {
            BmobWrapper.Update(tablename, objectId, data, NonCallback);
            return this;
        }

        public BmobRemote Update<T>(T data) where T : cn.bmob.io.BmobTable
        {
            BmobWrapper.Update<T>(data, NonCallback);
            return this;
        }

        public BmobRemote UpdateUser(String objectId, BmobUser data, String sessionToken)
        {
            BmobWrapper.UpdateUser(objectId, data, sessionToken, NonCallback);
            return this;
        }

        public BmobRemote UpdateUser<T>(T data) where T : cn.bmob.io.BmobUser
        {
            BmobWrapper.UpdateUser<T>(data, NonCallback);
            return this;
        }

        #endregion

    }
}
