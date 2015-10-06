using cn.bmob.config;
using cn.bmob.http;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using cn.bmob.exception;

namespace cn.bmob.tools
{
    internal class Utilities
    {
        // 代码格式化 ctrl + k + d
        public static String Version()
        {
#if WIN8_1
            //Assembly.GetName().Version.ToString();
            AssemblyFileVersionAttribute attr = typeof(Utilities).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            return attr.Version;
#else
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
#endif
        }

        public static void CheckNotNull(Object obj, String throwMesg)
        {
            if (obj == null)
            {
                throw new BmobException(throwMesg);
            }
        }
        
        public static String getBaseURL()
        {
            return Configuration.OUTER_NET;
        }

        public static bool Empty(object obj)
        {
            return obj == null || (obj is String && obj.Equals(""));
        }

        /// <summary>
        /// t为null，则返回mDefault的值。
        /// </summary>
        public static T value<T>(T t, T mDefault)
        {
            return t == null ? mDefault : t;
        }

        /*
        public  static  T  Clone<T>(T  obj)
        {
                T  ret  =  default(T);
                if  (obj  !=  null)
                {
                        XmlSerializer  cloner  =  new  XmlSerializer(typeof(T));
                        MemoryStream  stream  =  new  MemoryStream();
                        cloner.Serialize(stream,  obj);
                        stream.Seek(0,  SeekOrigin.Begin);
                        ret  =  (T)cloner.Deserialize(stream);
                }
                return  ret;
        }
         * 没有经过使用测试，可能会用到
        

        static T DeepClone<T>(T element)
        {
            using (MemoryStream ms = new MemoryStream(1000))
            {
                BinaryFormatter bf = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
                bf.Serialize(ms, element);

                ms.Seek(0, SeekOrigin.Begin);

                return (T)bf.Deserialize(ms);
            }
        }
         * 
         * 本地IP地址
using System.Net;

IPHostEntry ipHost = Dns.Resolve(Dns.GetHostName()); ;
IPAddress ipaddress = ipHost.AddressList[0];
string ips = ipaddress.ToString();

MAC地址
 string strMac = "";
 NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    strMac += ni.GetPhysicalAddress().ToString() + "|";//MAC地址
                }
            }
ni.OperationalStatus.ToString();//网络连接状态

------解决方案--------------------

C# code

ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration  where IPEnabled='True' and MACAddress = '" + MACAddress + "'");
                  ManagementObjectCollection queryCollection = query.Get();
                  foreach (ManagementObject mo in queryCollection)
                  {
                      if ((bool)mo["IPEnabled"] == true)
                      {
                          if (mo["IPAddress"] != null)
                              strIP = ((string[])mo["IPAddress"])[0];
                      }
                      else
                      {
                          strIP = "0.0.0.0";
                      }
                  } 
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="packageName"></param>
        /// <param name="aid">设备唯一id</param>
        /// <param name="platform">平台标识[1-ios,0-android]</param>
        /// <returns></returns>
        internal static String EndPointHead(String appkey, String longitude, String latitude, String packageName, String aid, String platform)
        {
            return appkey + "-" + longitude + "-" + latitude + "-" + packageName + "-" + aid + "-" + platform;
        }

    }
}
