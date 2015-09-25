using System;
using System.Collections.Generic;
using System.Threading;
using cn.bmob.io;
using cn.bmob.response;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;
using System.Diagnostics;
using cn.bmob.http;
using cn.bmob.Extensions;
using cn.bmob.config;
using cn.bmob.exception;
#if WINDOWS_PHONE
using Microsoft.Phone.Notification;
#endif
using cn.bmob.tools;

namespace cn.bmob.api
{

#if WINDOWS_PHONE
    public class BmobWindowsPhone : BmobWindows
    {
        // The name of our push channel.
        private static String pushChannel = "BmobPushChannel";
        public static String PushChannel { get { return pushChannel; } set { pushChannel = value; } }

        public static int RequestTimeout
        {
            get
            {
                return Configuration.REQUEST_TIMEOUT;
            }
            set
            {
                Configuration.REQUEST_TIMEOUT = value;
            }
        }

        public BmobWindowsPhone()
        {
            Configuration.PLATFORM = SDKTarget.WindowsPhone;
        }

        public void StartPush(params IPushListener[] listeners)
        {
            new BmobPush().register(listeners).rebind();
        }

        public void StartPush()
        {
            new BmobPush().rebind();
        }

        public void StopPush()
        {
            new BmobPush().release();
        }

    }


    public interface IPushListener
    {
        /// <summary>
        /// Event handler for when the push channel Uri is updated.
        /// </summary>
        void ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e);
        void ErrorOccurred(object sender, NotificationChannelErrorEventArgs e);

        //注册 ConnectionStatusChanged 事件。通过监视 Microsoft 推送通知服务连接的状态，通知功能可能会适当降低。
        //ConnectionStatusChanged 

        // Register for this notification only if you need to receive the notifications while your application is running.
        /// <summary>
        /// Event handler for when a toast notification arrives while your application is running.  
        /// The toast will not display if your application is running so you must add this
        /// event handler if you want to do something with the toast notification.
        /// </summary>
        void ShellToastNotificationReceived(object sender, NotificationEventArgs e);

        void HttpNotificationReceived(object sender, HttpNotificationEventArgs e);
    }


    /// <summary>
    /// 接收 Windows Phone 的推送通知
    /// 
    /// * 接收 Windows Phone 的推送通知： http://msdn.microsoft.com/zh-cn/library/hh202940(v=vs.92).aspx
    /// * Windows Phone 的推送通知概述： http://msdn.microsoft.com/zh-cn/library/ff402558(v=vs.92).aspx
    /// </summary>
    public class BmobPush : IPushListener
    {
        public static String DeviceType { get; internal set; }
        public static String DeviceId { get; internal set; }
        public static String NotificationUri { get; internal set; }

        static BmobPush()
        {
            DeviceType = BmobInstallation.DeviceType;

            try
            {
                var DeviceUniqueId = Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId") as byte[];
                DeviceId = DeviceUniqueId.ToHexString();
            }
            catch (Exception e)
            {
                BmobDebug.E("获取设备唯一ID失败!!! 请确认添加ID_CAP_IDENTITY_DEVICE功能? ");
                throw e;
            }
        }

        public static BmobPush CURRENT;

        private BmobInstallation installaction = new BmobInstallation();

        public BmobPush()
        {
            installaction.deviceType = "windows phone";
            installaction.deviceId = DeviceId;

            installaction.notificationUri = NotificationUri;
            CURRENT = this;
        }

        public void release()
        {
            HttpNotificationChannel channel = HttpNotificationChannel.Find(BmobWindowsPhone.PushChannel);
            if (channel != null)
            {
                channel.Close();
                channel.Dispose();
            }
        }

        /// <summary>
        /// authorized: 
        ///  ID_CAP_PUSH_NOTIFICATION 
        ///  ID_CAP_IDENTITY_DEVICE
        /// </summary>
        public void rebind()
        {
            HttpNotificationChannel channel = HttpNotificationChannel.Find(BmobWindowsPhone.PushChannel);

            //如果用户通过更改应用程序中的设置关闭了通知，如应用程序策略的第 2.9 节中所述，则您应该确保使用 Close()()()() 方法来关闭推送通道。

            if (channel == null)
            {
                // 感谢赵越大哥无私的贡献！
                channel = new HttpNotificationChannel(BmobWindowsPhone.PushChannel, "urn:wp-ac-hash-2:bchdqmkdpwamzk1umxagzovixy2mwp8-b9vfeea9l2c");
                registerPushChannelEvent(channel);

                channel.Open();

                /// 如果您想接收 Toast 通知，则应该调用 BindToShellToast()方法将通道绑定到 Toast 通知。
                channel.BindToShellToast();
                // 如果您想接收磁贴通知，则将通道绑定到磁贴通知，方法是：调用 BindToShellTile()方法以访问设备上的本地资源或调用
                // BindToShellTile(Collection<(Of <<'(Uri>)>>)) 方法以访问远程资源。若要访问远程资源，您必须提供从中访问远程图像的所允许域的集合。集合中的每个 URI 都限制为 256 个字符。
                channel.BindToShellTile();
            }
            else
            {
                registerPushChannelEvent(channel);
                NotificationUri = channel.ChannelUri.ToString();
                BmobDebug.Log("NotificationUri: " + NotificationUri);

                fetchAndUpdateNotifactionUri();
            }
        }

        #region PUSH_RSYNC

        // TODO 写入本地
        private static String MY_INST_OBJECTID = null;

        private void fetchNotifactionUri(Action<String> action)
        {
            // 为null一律再查一次，因为不知道null是没初始化还是没有记录！
            if (MY_INST_OBJECTID != null)
            {
                action(MY_INST_OBJECTID);
            }
            else
            {
                new BmobWindowsPhone().Find<BmobInstallation>(
                    BmobInstallation.TABLE,
                    new BmobQuery().WhereEqualTo("deviceType", DeviceType).WhereEqualTo("deviceId", DeviceId).Limit(1),
                    (resp, ex) =>
                    {
                        if (ex == null)
                        {
                            if (resp.results.Count > 0)
                            {
                                MY_INST_OBJECTID = resp.results[0].objectId;
                            }
                            action(MY_INST_OBJECTID);
                        }
                    });
            }
        }

        private void fetchAndUpdateNotifactionUri()
        {
            fetchNotifactionUri((objectId) =>
            {
                if (objectId == null)
                {
                    new BmobWindowsPhone().Create(installaction, (resp, ex) =>
                    {
                        if (ex == null && resp != null)
                        {
                            MY_INST_OBJECTID = resp.objectId;

                            BmobDebug.D("Installaction设置成功， 对象ID为： " + MY_INST_OBJECTID + "; MPNS: " + NotificationUri);
                        }
                    });
                }
                else
                {
                    new BmobWindowsPhone().Update(BmobInstallation.TABLE, MY_INST_OBJECTID, new BmobKV().Put("notificationUri", NotificationUri), (resp, ex) =>
                    {
                        if (ex == null)
                        {
                            BmobDebug.D("Installaction更新成功， 对象ID为： " + MY_INST_OBJECTID + "; MPNS: " + NotificationUri);
                            BmobDebug.D("Update NotificationUri success.");
                        }
                        else
                        {
                            BmobDebug.E(ex);
                        }
                    });
                }
            });
        }

        #endregion

        #region Listener监听器

        private HashSet<IPushListener> _Listeners = new HashSet<IPushListener>();

        public BmobPush register(IPushListener listener)
        {
            _Listeners.Add(listener);
            return this;
        }

        public BmobPush register(params IPushListener[] listeners)
        {
            foreach (var l in listeners)
            {
                register(l);
            }
            return this;
        }

        private void registerPushChannelEvent(HttpNotificationChannel pushChannel)
        {
            var listeners = new IPushListener[_Listeners.Count + 1];
            listeners[0] = this;

            _Listeners.CopyTo(listeners, 1, _Listeners.Count);
            foreach (var listener in listeners)
            {
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(listener.ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(listener.ErrorOccurred);
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(listener.ShellToastNotificationReceived);
                pushChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(listener.HttpNotificationReceived);
            }
        }

        #endregion

        #region PushListener接口实现

        /// <summary>
        /// Event handler for when the push channel Uri is updated.
        /// </summary>
        public void ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            NotificationUri = e.ChannelUri.ToString();
            installaction.notificationUri = NotificationUri;

            // Display the new URI for testing purposes.   Normally, the URI would be passed back to your web service at this point.
            BmobDebug.Log(String.Format("Channel Uri is {0}", NotificationUri));

            fetchAndUpdateNotifactionUri();
        }

        public void ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            BmobDebug.E(String.Format("A push notification {0} error occurred.  {1} ({2}) {3}", e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData));
        }

        public void ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            string relativeUri = string.Empty;

            message.AppendFormat("Received Toast {0}:\n", DateTime.Now.ToShortTimeString());

            // Parse out the information that was part of the message.
            foreach (string key in e.Collection.Keys)
            {
                message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);

                if (string.Compare(
                    key,
                    "wp:Param",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.CompareOptions.IgnoreCase) == 0)
                {
                    relativeUri = e.Collection[key];
                }
            }

            BmobDebug.Log(message.ToString());

        }

        public void HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            string message;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(e.Notification.Body))
            {
                message = reader.ReadToEnd();
            }
            BmobDebug.Log(String.Format("Received Notification {0}:\n{1}", DateTime.Now.ToShortTimeString(), message));
        }

        #endregion

    }

#endif

}
