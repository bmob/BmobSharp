using cn.bmob.io;

namespace cn.bmob.response
{
    /// <summary>
    /// 上传文件的返回数据回调类
    /// </summary>
    public class UploadCallbackData : BmobFile
    {
    }

    /// <summary>
    /// 上传文件的返回数据回调类
    /// </summary>
    public class ThumbnailCallbackData : BmobFile
    {
        /// <summary>
        /// 内容的base64
        /// </summary>
        public string file { get; set; }

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            this.file = input.getString("file");
        }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put("file", this.file);
        }
    }

}
