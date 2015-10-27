using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using cn.bmob.io;
using cn.bmob.json;
using cn.bmob.tools;
using cn.bmob.exception;

using cn.bmob.api;

namespace cn.bmob.api.unit
{

    public abstract class BmobTestBase : BmobObject
    {

        protected static BmobWindows Bmob = new BmobWindows();

        private static String LatestObjectIdFile = System.IO.Path.GetTempPath() + "/BmobTest_LatestObjectId";
        public static String LatestObjectId
        {
            get
            {
                using (var sr = new StreamReader(LatestObjectIdFile))
                {
                    return sr.ReadToEnd();
                }
            }
            set
            {
                using (var sw = new StreamWriter(LatestObjectIdFile))
                {
                    sw.Write(value);
                }

            }
        }
        private static String LatestSessionTokenFile = System.IO.Path.GetTempPath() + "/BmobTest_LatestSessionToken";
        public static String LatestSessionToken
        {
            get
            {
                using (var sr = new StreamReader(LatestSessionTokenFile))
                {
                    return sr.ReadToEnd();
                }
            }
            set
            {
                using (var sw = new StreamWriter(LatestSessionTokenFile))
                {
                    sw.Write(value);
                }

            }
        }

        static BmobTestBase()
        {
            Bmob.initialize("4414150cb439afdf684d37dc184e0f9f", "e1deb317442129c125b228ddf78e5f22");
            BmobDebug.Register(msg => { Debug.WriteLine(msg); });
            BmobDebug.level = BmobDebug.Level.TRACE;
        }

        public virtual void FinishedCallback<T>(T resp, BmobException ex)
        {
            if (resp != null)
            {
                var pObjectId = resp.GetType().GetProperty("objectId");
                if (pObjectId != null)
                {
                    var value = (String)pObjectId.GetValue(resp, null);
                    if (!Utilities.Empty(value))
                        LatestObjectId = value;
                }

                var pSessionToken = resp.GetType().GetProperty("sessionToken");
                if (pSessionToken != null)
                {
                    var value = (String)pSessionToken.GetValue(resp, null);
                    if (!Utilities.Empty(value))
                        LatestSessionToken = value;
                }
            }

            Console.WriteLine();
            Console.WriteLine("\n返回结果打印输出(用户可以获取的数据)： " + JsonAdapter.JSON.ToDebugJsonString(resp) );
            Console.WriteLine("\n返回结果： " + JsonAdapter.JSON.ToDebugJsonString(resp));
            Console.WriteLine("\n返回结果异常信息输出： " + ex);

            if (ex != null)
            {
                Assert.Fail(ex.Message);
            }

        }

        public String toJson(IBmobWritable data)
        {
            return ToString(data);
        }

    }

    public abstract class WaitRequestFinishTest : BmobTestBase
    {

        // http://club.sm160.com/showtopic-886147.aspx
        public static ManualResetEvent WaitUnitFinish = new ManualResetEvent(false);

        public static void releaseLock()
        {
            WaitUnitFinish.Set();
        }

        //~WaitRequestFinishTest()
        //{
        //    WaitUnitFinish.Close();
        //}

        public override void FinishedCallback<T>(T resp, BmobException ex)
        {
            try
            {
                base.FinishedCallback(resp, ex);
            }
            finally
            {
                releaseLock();
            }
        }

        #region 附加测试特性
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        public virtual void setup()
        {
            WaitUnitFinish.Reset();
        }
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        public virtual void teardown()
        {
            WaitUnitFinish.WaitOne();
        }
        //
        #endregion
    }
}
