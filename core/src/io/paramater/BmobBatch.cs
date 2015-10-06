using cn.bmob.api;
using cn.bmob.exception;
using cn.bmob.http;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

namespace cn.bmob.io
{
    /// <summary>
    /// 为了减少因为网络通讯次数太多而带来的时间浪费, 你可以使用下面的批量(batch)操作，在一个请求中对多个普通对象(不支持系统内置的用户对象)进行添加(create)、更新(update)、删除(delete) 操作
    /// 
    /// 上限为50个, 这些操作会以发送过去的顺序来执行
    /// </summary>
    public sealed class BmobBatch : BmobObject
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

        private __Bmob BmobWrapper = new __Bmob();

        private void NonCallback<T>(T resp, BmobException ex) { }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            var requests = new List<IDictionary>();
            foreach (BmobInteractObject req in BmobWrapper.Requests)
            {
                String method = req.Method;
                if (method == null)
                {
                    continue;
                }

                // XXX 1.6 根据restful接口优化
                String path = "";
                if (method.Equals("POST"))
                {
                    path = "/1/classes/" + req.Table;
                }
                else if (method.Equals("DELETE") || method.Equals("PUT"))
                {
                    path = "/1/classes/" + req.Table + "/" + req.ObjectId;
                }

                IDictionary one = new Dictionary<String, Object>();

                BmobOutput.Save(one, "method", method);
                if (BmobUser.CurrentUser != null)
                    BmobOutput.Save(one, "token", BmobUser.CurrentUser.sessionToken);
                BmobOutput.Save(one, "path", path);
                BmobOutput.Save(one, "body", req.Data);
                requests.Add(one);
            }

            output.Put("requests", requests);
        }

        #region GenCode

        public BmobBatch Create(String tablename, IBmobWritable data)
        {
            BmobWrapper.Create(tablename, data, NonCallback);
            return this;
        }

        public BmobBatch Create<T>(T data) where T : cn.bmob.io.BmobTable
        {
            BmobWrapper.Create<T>(data, NonCallback);
            return this;
        }

        public BmobBatch Delete(String tablename, String objectId)
        {
            BmobWrapper.Delete(tablename, objectId, NonCallback);
            return this;
        }

        public BmobBatch Delete<T>(T data) where T : cn.bmob.io.BmobTable
        {
            BmobWrapper.Delete<T>(data, NonCallback);
            return this;
        }

        public BmobBatch Update(String tablename, String objectId, IBmobWritable data)
        {
            BmobWrapper.Update(tablename, objectId, data, NonCallback);
            return this;
        }

        public BmobBatch Update<T>(T data) where T : cn.bmob.io.BmobTable
        {
            BmobWrapper.Update<T>(data, NonCallback);
            return this;
        }

        #endregion

    }
}
