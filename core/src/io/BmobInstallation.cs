using System;
using System.Collections.Generic;

namespace cn.bmob.io
{
    /// <summary>
    /// 订阅配置表
    /// 
    /// 继承该类，定制更通用的推送。
    /// </summary>
    public partial class BmobInstallation : BmobTable
    {
        public const String DeviceType = "windows phone";
        public const String TABLE = "_Installation";

        public override string table
        {
            get
            {
                return TABLE;
            }
        }

        public BmobInt badge { get; set; } // iOS应用中右上角的图标标识，这会在服务端修改，用于未来推送消息的自增统计
        public List<String> channels { get; set; } // 当前这个设备订阅的渠道名称数组
        public String timeZone { get; set; }  // 设备所在位置的时区， 如Asia/Shanghai，这个会在每个Installation对象更新时同步（只读）

        /// <summary>
        /// 设备的类型, 值为："ios" 或 "android",或"windows phone" (只读)
        /// </summary>
        public String deviceType { get; set; }

        /// <summary>
        /// Bmob使用的设备唯一号，唯一标示（与用于一对一即可！）. Android设备是必须的，iOS可选 (只读)
        /// 
        /// 如果仅仅为了进行数据推送，可以使用UUID生成，然后把该值写入到用户表中！**该字段是前后端联系的枢纽**！！
        /// </summary>
        public String installationId { get; set; }
        public String deviceToken { get; set; }  // iOS设备由Apple APNS生成的唯一性token标识 (只读)
        public String notificationUri { get; internal set; } // Microsoft WindowsPhone的设备通知标识
        public String deviceId { get; internal set; } // 设备标识

        /// <summary>
        /// 订阅
        /// </summary>
        public void subscribe(List<String> channels)
        {
            this.AddUnique("channels", channels);
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        public void unsubscribe(List<String> channels)
        {
            this.Remove("channels", channels);
        }

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            this.badge = input.getInt("badge");
            this.channels = input.getList<String>("channels");
            this.timeZone = input.getString("timeZone");
            this.deviceType = input.getString("deviceType");

            this.installationId = input.getString("installationId");
            this.deviceToken = input.getString("deviceToken");
            this.notificationUri = input.getString("notificationUri");
            this.deviceId = input.getString("deviceId");
        }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put("badge", this.badge);
            output.Put("channels", this.channels);
            output.Put("timeZone", this.timeZone);
            output.Put("deviceType", this.deviceType);

            output.Put("installationId", this.installationId);
            output.Put("deviceToken", this.deviceToken);
            output.Put("notificationUri", this.notificationUri);
            output.Put("deviceId", this.deviceId);

        }
    }
}
