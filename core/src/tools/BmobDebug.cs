using cn.bmob.config;
using System;
using System.Collections.Generic;
using System.Text;


/// <summary>
/// Bmob工具集
/// </summary>
namespace cn.bmob.tools
{
    /// <summary>
    /// 日志公共类
    /// </summary>
    public class BmobDebug
    {
        private static Action<Object> logger =
#if WIN8_1
              msg => System.Diagnostics.Debug.WriteLine(msg);
#else
              Console.WriteLine;  
#endif

        // 对于大数据量的操作，控制日志输出级别非常重要！
        public static Level level = Level.WARN;

        public enum Level
        {
            TRACE,
            DEBUG,
            INFO,
            WARN,
            ERROR
        }

        /// <summary>
        /// 注册打印日志的实现方法
        /// </summary>
        /// <param name="l">打印日志的输出方法</param>
        public static void Register(Action<Object> l, Level level)
        {
            BmobDebug.level = level;
            logger = l;

            BmobDebug.I("bmob version " + Configuration.BUILD_VERSION + "/" + Configuration.PLATFORM);
        }

        public static void Register(Action<Object> l)
        {
            Register(l, Level.DEBUG);
        }

        public static Boolean Debug
        {
            get { return Level.DEBUG >= level; }
        }

        public static void Log(object msg)
        {
            if (Level.INFO >= level) logger(msg);
        }

        // TODO 优化下，使用Action的方式，可以优化一些字符串拼接的消耗
        public static void T(object msg)
        {
            if (Level.TRACE >= level) logger("[TRACE] [Bmob] " + msg);
        }

        public static void I(object msg)
        {
            if (Level.INFO >= level) logger("[INFO ] [Bmob] " + msg);
        }

        public static void D(object msg)
        {
            if (Level.DEBUG >= level) logger("[DEBUG] [Bmob] " + msg);
        }

        public static void E(object msg)
        {
            if (Level.WARN >= level) logger("[ERROR] [Bmob] " + msg);
        }
    }
}
