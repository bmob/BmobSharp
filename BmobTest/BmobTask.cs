using cn.bmob.api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cn.bmob;
using System.Collections;
using cn.bmob.response;
using cn.bmob.io;
using cn.bmob.tools;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using cn.bmob.Extensions;
using cn.bmob.exception;
using System.Threading.Tasks;
using cn.bmob.json;

namespace cn.bmob.api.unit
{

    /// <summary>
    ///这是 BmobTest 的测试类，旨在
    ///包含所有 BmobTest 单元测试
    ///</summary>
    [TestClass()]
    public class BmobTaskTest : BmobTestBase
    {
        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        public const String TABLENAME = "T_BMOB_API_WP";


        [TestMethod()]
        public void BatchCreate500Test()
        {
            var b = new BmobBatch();

            for (var i = 1; i < 500; i++)
            {
                Create(
                    data =>
                    {
                        b.Create(data);
                    }
                );

                if (i % 50 == 0)
                {
                    var future = Bmob.BatchTaskAsync(b);
                    FinishedCallback(future.Result, null);
                    b = new BmobBatch();
                }

            }
        }

        [TestMethod()]
        public void CreateTest()
        {
            Create(
                data =>
                {
                    var future = Bmob.CreateTaskAsync(data);
                    FinishedCallback(future.Result, null);
                }
            );
        }

        private void Create(Action<BmobTable> action)
        {
            var data = new GameObject(TABLENAME);
            data.arrint = BmobArrays.wrap<int>(1, 2, 3);
            data.arrstring = BmobArrays.wrap<string>("1", "2", "3");

            data.jo2 = 123;

            // 用于下面的区间查询
            Random rnd = new Random();
            data.jo = rnd.Next(-100, 100);
            data.s = "String+String";

            action(data);

        }

        [TestMethod()]
        public void UpdateTest()
        {
            var updateData = new GameObject();
            updateData.jo = 12341234;

            var future = Bmob.UpdateTaskAsync(TABLENAME, LatestObjectId, updateData);
            FinishedCallback(future.Result, null);
        }

        [TestMethod()]
        public void GetTest()
        {
            var future = Bmob.GetTaskAsync<GameObject>(TABLENAME, LatestObjectId);
            FinishedCallback(future.Result, null);
        }

        [TestMethod()]
        public void FindTest()
        {
            var query = new BmobQuery();
            query.Limit(1);
            var future = Bmob.FindTaskAsync<GameObject>(TABLENAME, query);
            FinishedCallback(future.Result, null);
        }


        [TestMethod()]
        public void DeleteTest()
        {
            var future = Bmob.DeleteTaskAsync(TABLENAME, LatestObjectId);
            FinishedCallback(future.Result, null);
        }

        [TestMethod()]
        public void BQLTest()
        {
            var future = Bmob.SqlTaskAsync<GameObject>("select * from " + TABLENAME);
            FinishedCallback(future.Result, null);
        }

        [TestMethod()]
        public void BQLCountTest()
        {
            var future = Bmob.SqlTaskAsync<GameObject>("select count(*) from " + TABLENAME);
            FinishedCallback(future.Result, null);
        }

        [TestMethod()]
        public void FindByWhereContainedInTest()
        {
            var query = new BmobQuery();
            query.WhereContainedIn("jo", 10, 30);
            var future = Bmob.FindTaskAsync<GameObject>(TABLENAME, query);
            FinishedCallback(future.Result, null);
        }

        [TestMethod()]
        public void FindByWhereCountByCreatedAtTest()
        {
            BmobDate start = new DateTime(2014, 10, 1);
            BmobDate end = new DateTime(2016, 1, 1);

            var startQuery = new BmobQuery();
            startQuery.WhereGreaterThanOrEqualTo("createdAt", start);

            var endQuery = new BmobQuery();
            endQuery.WhereLessThan("createdAt", end);

            var query = startQuery.And(endQuery);
            // 不返回具体内容
            query.Limit(0);
            query.Count();

            var future = Bmob.FindTaskAsync<Object>(TABLENAME, query);
            FinishedCallback(future.Result, null);
        }

        [TestMethod()]
        public void PushTest()
        {
            // TODO
        }

        [TestMethod()]
        public void RoleTest()
        {
            //这里创建四个用户对象指针，分别为老板、人事小张、出纳小谢和自己
            // just for test
            BmobPointer<BmobUser> boss = new BmobUser() { objectId = "1" };
            BmobPointer<BmobUser> hr_zhang = new BmobUser() { objectId = "2" };
            BmobPointer<BmobUser> hr_luo = new BmobUser() { objectId = "3" };
            BmobPointer<BmobUser> cashier_xie = new BmobUser() { objectId = "4" };
            BmobPointer<BmobUser> me = new BmobUser() { objectId = "5" };
            
            {
                //创建HR和Cashier两个用户角色（这里为了举例BmobRole的使用，将这段代码写在这里，正常情况下放在员工管理界面会更合适）
                BmobRole hr = new BmobRole();
                hr.name = "HR";
                var users = new BmobRelation<BmobUser>();
                users.Add(hr_zhang);
                users.Add(hr_luo);

                //将hr_zhang和hr_luo归属到hr角色中
                hr.AddUsers(users);

                //保存到云端角色表中（web端可以查看Role表）
                var future = Bmob.CreateTaskAsync(hr);
                FinishedCallback(future.Result, null);
            }
            
            {
                BmobRole cashier = new BmobRole();
                cashier.name = "Cashier";
                var users = new BmobRelation<BmobUser>();
                users.Add(cashier_xie);

                //将cashier_xie归属到cashier角色中
                cashier.AddUsers(users);

                //保存到云端角色表中（web端可以查看Role表）
                var future = Bmob.CreateTaskAsync(cashier);
                FinishedCallback(future.Result, null);
            }
        }

        [TestMethod()]
        public void EndPointTest()
        {
            //var future = Bmob.EndpointTaskAsync<QueryCallbackData<Object>>("second", null);
            //FinishedCallback(future.Result, null);

            var future = Bmob.EndpointTaskAsync<Dictionary<string, string>>("first", new BmobKV().Put("a", 11324));
            FinishedCallback(future.Result, null);

            /*
function onRequest(request, response, modules) {
    var res =  {"value": "just string..."} ;
    response.end(JSON.stringify(res));
}        
Bmob.Endpoint<Dictionary<string, string>>("testString", new Dictionary<String, Object>(), (resp, ex) =>
            {
                BmobDebug.Log(resp);
            });                                           
*/

        }

        [TestMethod()]
        public void LoginTest()
        {
            Object result = Bmob.LoginTaskAsync("winse", "winse").Result;
            Console.WriteLine(BmobUser.CurrentUser);
        }

        [TestMethod()]
        public void BatchTest()
        {
            var data = new GameObject(TABLENAME);
            data.arrint = BmobArrays.wrap<int>(1, 2, 3);
            data.arrstring = BmobArrays.wrap<string>("1", "2", "3");

            data.jo2 = 123;

            // 用于下面的区间查询
            Random rnd = new Random();
            data.jo = rnd.Next(-50, 170);
            data.s = "String";

            var reqs = new BmobBatch().Create(data);
            var future = Bmob.BatchTaskAsync(reqs);

            //{"data":[{"success":{"createdAt":"2014-08-23 08:00:38","objectId":"6fcb5d0eab"}},{"success":{"createdAt":"2014-08-23 08:00:38","objectId":"2d626312e3"}}],"result":{"code":200,"message":"ok"}}
            //{"data":[{"error":{"code":105,"error":"It is a reserved field: objectId."}},{"error":{"code":105,"error":"It is a reserved field: objectId."}}],"result":{"code":200,"message":"ok"}}
            //{"data":[{"error":{"code":105,"error":"It is a reserved field: objectId."}}],"result":{"code":200,"message":"ok"}}


            // TODO 处理返回值
            FinishedCallback(future.Result, null);
        }


        [TestMethod()]
        public void ImageThumbnailTest()
        {
            var Image = "http://file.bmob.cn/M00/06/70/wKhkA1PoFVCADG7fAAAUWOcV3Ew398.png";
            var param = new ThumbnailParameter(200, Image);

            var future = Bmob.ThumbnailTaskAsync(param);
            FinishedCallback(future.Result, null);
        }


        [TestMethod()]
        public void FileUploadTest()
        {
            Byte[] data = null;
            using (var stream = File.OpenRead("R:/1.png"))
            {
                data = stream.ReadAsBytes();
            }

            var future = Bmob.FileUploadTaskAsync(new BmobLocalFile(data, "21.png"));
            FinishedCallback(future.Result, null);
        }

        [TestMethod()]
        public void FileUploadHugeTest()
        {
            try
            {
                Byte[] data = null;
                using (var stream = File.OpenRead("E:/local/opt/eclipse-jee-luna-R-win32-x86_64.zip"))
                {
                    data = stream.ReadAsBytes();
                }

                var future = Bmob.FileUploadTaskAsync(new BmobLocalFile(data, "超大文件.zip"));
                FinishedCallback(future.Result, null);
            }
            catch
            {
                return;
            }

            Assert.Fail("should throw exception. but it sees not!");
        }

        [TestMethod()]
        public void TimeStampTest()
        {
            Object result = Bmob.TimestampTaskAsync().Result;
            Console.WriteLine(result);
        }

        [TestMethod()]
        public void SignupTest()
        {
            BmobUser user = new BmobUser();
            user.username = "signtwice";
            user.password = "justpasswd";

            // 1
            try
            {
                var future = Bmob.SignupTaskAsync(user);
                FinishedCallback(future.Result, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            // 2
            try
            {
                var future = Bmob.SignupTaskAsync(user);
                FinishedCallback(future.Result, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
