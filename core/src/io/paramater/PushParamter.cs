using System;
using System.Collections.Generic;
using System.Net;

namespace cn.bmob.io
{

    /// <summary>
    /// 接收 Windows Phone 的推送通知: http://msdn.microsoft.com/zh-cn/library/hh202945(v=vs.92).aspx
    /// 
    /// 与 Web 服务进行通信的最佳做法包括：
    ///  * 应用程序应该对其相应的 Web 服务进行身份验证。
    ///  * 应用程序应该在向其相应的 Web 服务发送 URI 之前对其通知通道 URI 进行加密。
    ///  * 如果您的 Web 服务将使用 Windows Phone OS 7.0 中不存在的通知属性(Toast 通知的 Parameter 属性和 Tile 通知的 BackTitle、BackBackgroundImage、BackContent 或 Tile ID 属性)，则您应将操作系统版本信息传递到您的 Web 服务，以便 Web 服务可以正确降级 Windows Phone OS 7.0 客户端的通知。
    ///  * Web 服务应该验证从其相应应用程序接收的通知通道 URI 并采用安全方式进行存储。
    ///  * 当启动应用程序中的会话时，通知通道 URI 应该始终发送到其相应的 Web 服务。
    ///  * Web 服务应该拥有一个可以发送到其相应应用程序的状态代码，该代码将触发应用程序创建新的通知通道 URI。
    /// </summary>
    public sealed class PushParamter : BmobObject
    {
        private BmobQuery target = new BmobQuery();
        public BmobQuery Target { get { return target; } }

        private IDictionary<String, Object> message = new Dictionary<String, Object>();
        public IDictionary<String, Object> Message { get { return this.message; } }

        /// <summary>
        /// "2013-12-04 00:51:13"
        /// </summary>
        public String ExpirationTime;

        /// <summary>
        /// 相对时间（根据push_time做定期推送，从push_time时间开始算起，直到expiration_interval时间后过期）
        /// 
        /// "2012-01-28 00:51:13"
        /// </summary>
        public String PushTime { get; set; }
        /// <summary>
        /// 518400
        /// </summary>
        public BmobInt ExpirationInterval;

#if WINDOWS_PHONE || FRAMEWORK
        /// <summary>
        /// http://msdn.microsoft.com/zh-cn/library/hh202945(v=vs.92).aspx
        /// </summary>
        //private void buildMessage(String type, int batchingInterval, String body)
        //{
        //    IDictionary<String, String> headers = new Dictionary<String, String>();

        //    // X-MessageID:<UUID>
        //    // X-CallbackURI:callbackUri

        //    // 要发送的推送通知的类型。可能的选项为磁贴、Toast 和 Raw。如果此标头不存在，则推送通知将被视为 Raw 通知。
        //    if (type != null)
        //        headers.Add("X-WindowsPhone-Target", type);

        //    // 批处理间隔，指示推送通知将从推送通知服务发送到应用程序的时间。有关此标头的可能值，请参阅 Toast、磁贴和 Raw 通知部分中的表。如果此标头不存在，则推送通知服务会立即发送该消息。
        //    headers.Add("X-NotificationClass", batchingInterval + "");

        //    Message.Add("ContentType", "text/xml");
        //    Message.Add("body", body);
        //    Message.Add("headers", headers);
        //}

        private static String encode(String origin)
        {
            if (origin == null)
            {
                return "";
            }
            return WebUtility.HtmlEncode(origin);
        }

        /// <summary>
        /// Toast 通知
        /// 
        /// 可以显示的文本数量取决于在 Toast 消息中使用的字符以及“标题”（粗体）和“内容”（非粗体）的长度。
        /// * 如果只设置了一个“标题”，则可以显示大约 40 个字符，之后的字符将被截断。
        /// * 如果只设置了“内容”，则可以显示大约 47 个字符。
        /// * 如果一个 Toast 在“标题”和“内容”之间平均拆分，则可以显示大约 41 个字符。无法放在 Toast 上的任何文本都将被截断。
        /// </summary>
        /// <param name="text1">标题</param>
        /// <param name="text2">内容</param>
        /// <param name="param">参数。如果用户点按 Toast，则将参数值传递给您的应用程序，而不进行显示。该参数可以指示应用程序应该启动到的页面。该参数还包含传递到应用程序的名称-值对。在 XML 架构中，该字符串定义为 Param 属性。
        /// 
        /// 用于设置 Parameter 属性的 Toast 通知只能发送到运行 Windows Phone OS 7.1 或更高版本的设备。将具有 Parameter 属性的通知发送到 Windows Phone OS 7.0 设备将导致 PushErrorTypePayloadFormatInvalid 错误，并且通道会关闭。
        /// 
        /// * /page1.xaml – 定义应用程序启动时导航到的应用程序中的页面。该页面必须以“/”开头。
        /// * /page1.xaml?value1=1234 &amp;value2=9876 – 定义应用程序启动时导航到的页面，以及信息的名称/值对。该页面必须以“/”开头。
        /// * ?value1=1234 &amp;value2=9876 – 包含传递给应用程序默认开始页面的信息名称/值对。该页面必须以“?”开头。
        /// </param>
        /// <param name="batchingInterval">2 立即发送; 12 在 450 秒内发送; 22 在 900 秒内发送。</param>
        public PushParamter toast(String text1, String text2/*, String param, int batchingInterval*/)
        {
            Message.Add("alert", encode(text1));
            Message.Add("wpAlert", encode(text2));
            Message.Add("wp", 2);

            return this;
        }
        /*
        public PushParamter tokenClear()
        {
            return token(null, null, null, null, null, null, true, 1);
        }
        */
        public PushParamter token(String title, int count, String backTitle, String backContent)
        {
            return token(title, null, count, null, backTitle, backContent, false, 1);
        }

        /// <summary>
        /// 磁贴通知
        /// 
        /// 用于设置 BackTitle、BackBackgroundImage 或 BackContent 属性的磁贴通知或通过在负载中设置磁贴 ID 来指定次要磁贴的通知只能发送到运行 Windows Phone OS 7.1 或更高版本的设备。将具有这些值的通知发送到 Windows Phone OS 7.0 设备将导致 PushErrorTypePayloadFormatInvalid 错误，并且通道会关闭。
        /// 
        /// 添加图片：
        /// 在“解决方案资源管理器”中选择 Red.jpg。在“属性”窗口中，将“生成操作”设置为“内容”以将该图形包含在 .xap 文件中。对其他 .jpg 文件重复这些步骤。
        /// 
        /// 图像上一些用于 BackgroundImage 和 BackBackgroundImage 属性的附加说明：
        /// * 磁贴图像可以是 .jpg 或 .png 文件。
        /// * 由于网络可变性和性能原因，请考虑对磁贴图像使用本地资源。
        /// * 对图像使用具有透明部分的 .png 将允许用户透过它显示主题颜色。
        /// * 磁贴为 173 x 173 像素。如果您提供的图像具有不同的尺寸，则会将其拉伸以适合 173 x 173 像素的大小。
        /// * 可以使用本地或远程资源作为图像。如果使用本地资源，则该资源必须作为 XAP 包的一部分安装。
        /// * 如果用户关闭磁贴通知，则磁贴应该包含常规信息。不应该包含任何过时的数据，如过期的天气预报或流量报告。
        /// * 不支持 https 作为远程图像。
        /// * 远程图像限制为 80 KB 或更少。如果大于 80 KB，则将不会下载。
        /// * 远程图像必须在 30 秒或更少的时间内下载，否则将不会下载。
        /// * 如果 BackgroundImage 或 BackBackgroundImage 图像由于任何原因无法加载，则不会更改在更新中设置的其他任何属性。
        /// </summary>
        /// <param name="Id">Navigation Uri of the Tile to update. 
        /// 
        /// Id 指定要更新的磁贴（如果应用程序有次要磁贴）。若要更新应用程序磁贴，可以忽略此 Id;
        /// 否则此 ID 应该包含次要磁贴的确切导航 URI. 如： /SecondaryTile.xaml?DefaultTitle=FromTile</param>
        /// <param name="clear">若要清除磁贴属性的值，请将 Action 特性设置为该属性的 Clear。最总发送给MPNS时会作为节点属性发送请求。
        /// 
        /// 请注意，不能清除 BackgroundImage 属性，因为您应该始终在磁贴的正面设置背景图像。</param>
        /// <param name="title">标题。指示应用程序标题的字符串。标题必须适合单行文本并且不应该比实际磁贴宽。标题中大约可以包含 15 个字符，多余部分将被截断。</param>
        /// <param name="backgroundImage">BackgroundImage。显示在磁贴正面的图像。建议您在磁贴正面始终拥有背景图像。</param>
        /// <param name="count">计数（也称为徽章）。从 1 到 99 的整数值。如果未设置“计数”的值或者设置为 0，则不会在磁贴上显示圆形图像和值。</param>
        /// <param name="backTitle">BackTitle。显示在磁贴背面底部的字符串。BackTitle 必须适合单行文本并且不应该比实际磁贴宽。标题中大约可以包含 15 个字符，多余部分将被截断。</param>
        /// <param name="backContent">BackContent。显示在磁贴背面中心的字符串。磁贴中大约可以包含 40 个字符，多余部分将被截断。</param>
        /// <param name="backBackgroundImage">BackBackgroundImage。显示在磁贴背面的图像。</param>
        /// <param name="batchingInterval">1 立即发送; 11 在 450 秒内发送; 21 在 900 秒内发送。</param>
        public PushParamter token(/*String Id, */String title, String backgroundImage, int? count, String backBackgroundImage, String backTitle, String backContent, bool clear, int batchingInterval)
        {
            Message.Add("alert", encode(title));
            Message.Add("backgroundImage", encode(backgroundImage));
            Message.Add("count", count);
            Message.Add("backBackgroundImage", encode(backBackgroundImage));
            Message.Add("backTitle", encode(backTitle));
            Message.Add("backContent", encode(backContent));
            Message.Add("wp", 1);

            return this;
        }

        /// <summary>
        /// Raw 通知
        /// 
        /// 可以使用 Raw 通知向您的应用程序发送信息。如果您的应用程序当前未运行，则 Raw 通知会在 Microsoft 推送通知服务上丢弃并且不会发送到设备。
        /// </summary>
        /// <param name="batchingInterval">3 立即发送; 13 在 450 秒内发送; 23 在 900 秒内发送。</param>
        public PushParamter raw(String message/*, int batchingInterval*/)
        {
            // string tileMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            //"<root>" +
            //    "<Value1><UserValue1><Value1>" +
            //    "<Value2><UserValue2><Value2>" +
            //"</root>"
            //            也可以只传递字节流。下面的代码显示了一个示例。
            //new byte[] {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08};

            Message.Add("alert", message);
            Message.Add("wpAlert", "");
            Message.Add("wp", 3);

            return this;
        }
#endif

        public PushParamter whereChannels(IList<string> channels)
        {
            Target.WhereEqualTo("channels", channels);
            return this;
        }

        public PushParamter whereDeviceType(String deviceType)
        {
            Target.WhereEqualTo("deviceType", deviceType);
            return this;
        }

        public PushParamter whereInstallationId(String installationId)
        {
            Target.WhereEqualTo("installationId", installationId);
            return this;
        }

        public PushParamter whereDeviceToken(String deviceToken)
        {
            Target.WhereEqualTo("deviceToken", deviceToken);
            return this;
        }

        public PushParamter whereNotificationUri(String notificationUri)
        {
            Target.WhereEqualTo("notificationUri", notificationUri);
            return this;
        }

        public PushParamter where(BmobQuery anotherQuery)
        {
            this.target = Target.And(anotherQuery);
            return this;
        }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put("where", Target.where);
            output.Put("data", Message);

            output.Put("expiration_time", ExpirationTime);

            output.Put("push_time", PushTime);
            output.Put("expiration_interval", ExpirationInterval);
        }
    }
}
