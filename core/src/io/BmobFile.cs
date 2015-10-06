
using cn.bmob.config;

namespace cn.bmob.io
{
    /// <summary>
    /// 文件处理类
    /// </summary>
    public class BmobFile : BmobObject
    {

        public override string _type
        {
            get
            {
                return "File";
            }
        }

        /// <summary>
        /// 文件名
        /// </summary>
        public string filename { get; set; }

        /// <summary>
        /// 文件的组名
        /// </summary>
        public string group { get; set; }

        /// <summary>
        /// 相对于Bmob文件服务器的位置，结果需要附加上http://file.codenow.cn
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 获取文件地址
        /// </summary>
        /// <returns>文件地址</returns>
        public string getPath()
        {
            return Configuration.FILE_NET + this.url;
        }

        //public Image Image
        //{
        //    get
        //    {
        //        return 
        //    }
        //}

        public override void readFields(BmobInput input)
        {
            base.readFields(input);
            this.filename = input.getString("filename");
            this.group = input.getString("group");
            this.url = input.getString("url");
        }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);
            output.Put(TYPE_NAME, this._type);
            output.Put("filename", this.filename);
            output.Put("group", this.group);
            output.Put("url", this.url);
        }
 
    }
}
