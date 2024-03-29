﻿using cn.bmob.tools;
using System;
using System.Reflection;

namespace cn.bmob.config
{

    internal class Configuration
    {

        /// <summary>
        /// 此SDK版本号
        /// </summary>
        public static String BUILD_VERSION = Utilities.Version();

        /// <summary>
        /// 平台识别字段
        /// 
        /// 默认为WindowsDisktop。
        /// </summary>
        internal static SDKTarget PLATFORM = SDKTarget.WindowsDesktop;

        /// <summary>
        /// 平台序列号
        /// </summary>
        internal const String PLATFORM_KEY = "4";

        internal static string CLIENT_REQ_UID = Guid.NewGuid().ToString().Replace("-", "");

        internal static string DATA_URL = "https://api.codenow.cn/1";

        internal static string FILE_URL ="https://api.codenow.cn/2";

        /// <summary>
        /// 请求Bmob后端接口版本号
        /// </summary>
        /*internal const String VERSION = "1";

        /// <summary>
        /// 请求Bmob后端'文件'接口版本号
        /// </summary>
        internal const String VERSION_NEW = "2";

        //public String FILE_NET = "https://api.codenow.cn/" + VERSION_NEW;
        
        internal static String OUTER_NET = "https://api.codenow.cn/" + VERSION;

        /// <summary>
        /// 请求Bmob后端'文件'接口url前缀
        /// </summary>
        internal static String NEW_OUTER_NET = "https://api.codenow.cn/" + VERSION_NEW;

        public static String Url
        {
            get
            {
                return this.NEW_OUTER_NET;
            }
            set
            {
                this.NEW_OUTER_NET=value;
            }
        }*/

        public const String CHARSET = "UTF-8";
        internal const String JSON_CONTENT_TYPE = "application/json; charset=" + CHARSET;

        //设置获得响应的超时时间（10秒）
        internal static int REQUEST_TIMEOUT = 10 * 1000;

    }

    public enum SDKTarget
    {
        WindowsDesktop,
        WindowsPhone, //WP8
        Windows81, // Win8 or WP8.1
        Unity
    }
}
