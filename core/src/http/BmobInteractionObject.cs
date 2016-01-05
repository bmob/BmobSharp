using System;
using System.Collections;
using cn.bmob.io;
using System.Collections.Generic;
using cn.bmob.config;
using cn.bmob.tools;
using System.Text;
using cn.bmob.Extensions;
using cn.bmob.json;

namespace cn.bmob.http
{

    /// <summary>
    /// request请求的包装类
    /// </summary>
    internal class BmobInteractObject
    {
        protected Func<BmobInteractObject, String> calcUrlAction;

        public BmobInteractObject(String method) : this(method, null)
        {
        }

        public BmobInteractObject(String method, Func<BmobInteractObject, String> calcUrlAction)
        {
            this.Method = method;
            this.calcUrlAction = calcUrlAction;
        }

        /// <summary>
        /// 请求的类型POST/PUT/DELETE/GET
        /// </summary>
        public String Method { get; internal set; }

        public String AppKey { get; set; }

        public String RestKey { get; set; }

        public String MasterKey { get; set; }

        private String fContentType = Configuration.JSON_CONTENT_TYPE;
        public String ContentType
        {
            get { return this.fContentType; }
            set { this.fContentType = value; }
        }


        /// <summary>
        /// 请求数据，相当于curl的-d参数的值
        /// </summary>
        public Object Data { get; set; }

        /// <summary>
        /// 更新|删除，当c="_User"时，必须先登录获取sessionToken的值。然后作为X-Bmob-Session-Token的Http头部操作ACL数据
        /// </summary>
        public String SessionToken { get; set; }

        /// <summary>
        /// 云端代码的名称
        /// </summary>
        public String EndPointName { get; set; }

        public String ObjectId { get; set; }
        public String Table { get; set; }

        public virtual String Url { get { return Utilities.getBaseURL() + calcUrlAction(this); } }

        // TODO 1.6 URL整体优化
        private static String getURLTablePart(String tablename)
        {
            if (tablename.StartsWith("_"))
            {
                switch (tablename)
                {
                    case "_Installation":
                        return "/installations";
                    case "_User":
                        return "/users";
                    case "_Role":
                        return "/roles";
                    default:
                        throw new ArgumentException("unknow special table: " + tablename);
                }
            }
            else
            {
                return "/classes/" + tablename;
            }
        }

        public static BmobInteractObject Create { get { return new BmobInteractObject("POST", bio => getURLTablePart(bio.Table)); } }
        public static BmobInteractObject Get { get { return new BmobInteractObject("GET", bio => getURLTablePart(bio.Table) + "/" + bio.ObjectId); } }

        public static BmobInteractObject Find { get { return new GetInteractObject(bio => getURLTablePart(bio.Table)); } }

        public static BmobInteractObject Update { get { return new BmobInteractObject("PUT", bio => getURLTablePart(bio.Table) + "/" + bio.ObjectId); } }
        public static BmobInteractObject Delete { get { return new BmobInteractObject("DELETE", bio => getURLTablePart(bio.Table) + "/" + bio.ObjectId); } }

        public static BmobInteractObject Signup { get { return new BmobInteractObject("POST", bio => getURLTablePart(bio.Table)); } }

        public static BmobInteractObject Login { get { return new GetInteractObject(bio => "/login"); } }

        public static BmobInteractObject PwdReset { get { return new BmobInteractObject("POST", bio => "/requestPasswordReset"); } }
        public static BmobInteractObject EmailVerify { get { return new BmobInteractObject("POST", bio => "/requestEmailVerify"); } }

        public static BmobInteractObject Files
        {
            get
            {
                return new BmobInteractObject(
                    "POST",
                    bio =>
                    {
                        var data = bio.Data as BmobLocalFile;
                        return "/files/" + data.Filename();
                    }
                );
            }
        }
        public static BmobInteractObject FileDelete
        {
            get
            {
                return new BmobInteractObject(
                    "DELETE ",
                    bio =>
                    {
                        var data = bio.Data as BmobKV;
                        return "/files/" + data["filename"];
                    }
               );
            }
        }

        public static BmobInteractObject Thumbnail { get { return new BmobInteractObject("POST", bio => "/images/thumbnail"); } }

        public static BmobInteractObject Batch { get { return new BmobInteractObject("POST", bio => "/batch"); } }
        public static BmobInteractObject Endpoint { get { return new BmobInteractObject("POST", bio => "/functions/" + bio.EndPointName); } }
        public static BmobInteractObject Push { get { return new BmobInteractObject("POST", bio => "/push"); } }

        public static BmobInteractObject Timestamp { get { return new GetInteractObject(bio => "/timestamp"); } }
        public static BmobInteractObject BQL { get { return new GetInteractObject(bio => "/cloudQuery"); } }

        public static BmobInteractObject RequestSMS { get { return new BmobInteractObject("POST", bio => "/requestSmsCode"); } }
        public static BmobInteractObject VerifySMS
        {
            get
            {
                return new BmobInteractObject("POST", bio =>
                {
                    var data = bio.Data as SMSParamter;
                    return "/verifySmsCode/" + data.code;
                });
            }
        }
        public static BmobInteractObject QuerySMS
        {
            get
            {
                return new GetInteractObject(bio =>
                {
                    var data = bio.Data as SMSParamter;
                    return "/querySms/" + data.smsId;
                });
            }
        }


        #region GET

        public class GetInteractObject : BmobInteractObject
        {
            public GetInteractObject(Func<BmobInteractObject, String> calcUrlAction) : base("GET", calcUrlAction)
            {
            }

            public override String Url
            {
                get
                {
                    var querystring = new StringBuilder();
                    var data = this.Data as IBmobWritable;
                    if (data != null)
                    {
                        var bo = new BmobOutput();
                        data.write(bo, false);
                        var kv = bo.getData();
                        foreach (var key in kv.Keys)
                        {
                            var name = key as String;
                            if (name == null)
                            {
                                continue;
                            }

                            Object valueObject = kv[key];
                            String value = valueObject is String ? (String)valueObject : JsonAdapter.JSON.ToJsonString(valueObject);
                            if (querystring.Length > 1)
                                querystring.Append("&");
                            querystring.AppendFormat("{0}={1}", name.UrlEncode(), value.UrlEncode());
                        }

                    }

                    var baseUrl = Utilities.getBaseURL() + calcUrlAction(this);
                    return querystring.Length > 1 ? baseUrl + "?" + querystring.ToString() : baseUrl;

                }
            }

        }

        #endregion

    }

}
