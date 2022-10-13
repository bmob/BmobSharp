using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using cn.bmob.api;
using cn.bmob.exception;

namespace cn.bmob.io
{
    /// <summary>
    /// 文件处理类
    /// 
    /// byte[]优先级最高，流，最后才是判断filename获取本地文件的内容
    /// </summary>
    public sealed class BmobLocalFile
    {
        private String filename;
        private byte[] datas;
        private Stream inputStream;

        public BmobLocalFile(byte[] datas)
        {
            this.datas = datas;
        }

        public BmobLocalFile(Stream inputStream)
        {
            this.inputStream = inputStream;
        }

        /// <summary>
        /// 上传附件内容构造函数
        /// </summary>
        /// <param name="datas">文件内容</param>
        /// <param name="name">文件标识，[文件名称.后缀]的形式组成，如: bmob.png。上传成功后回调的filename属性的值</param>
        public BmobLocalFile(byte[] datas, string name)
        {
            this.datas = datas;
            this.filename = name;
        }

        /// <summary>
        /// 上传附件内容构造函数
        /// </summary>
        /// <param name="inputStream">附件流</param>
        /// <param name="name">文件标识，[文件名称.后缀]的形式组成，如: bmob.png。上传成功后回调的filename属性的值</param>
        public BmobLocalFile(Stream inputStream, string name)
        {
            this.inputStream = inputStream;
            this.filename = name;
        }

#if !WIN8_1
        
        /// <summary>
        /// 构造函数（设置文件路径）
        /// </summary>
        /// <param name="path">文件路径</param>
        public BmobLocalFile(String path)
        {
            this.filename = path;
        }

        /// <summary>
        /// 构造函数（设置文件路径）
        /// </summary>
        /// <param name="file">文件对象</param>
        public BmobLocalFile(FileInfo file)
        {
            this.filename = file.FullName;
        }
#endif

        ///// <summary>
        ///// 获得文本文件的内容信息
        ///// </summary>
        ///// <returns>文件内容</returns>
        //public string Get()
        //{
        //    // // byte[] ContentsBody = System.IO.File.ReadAllBytes(filepath);
        //    return File.ReadAllText(filename);
        //}

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <returns>文件流</returns>
        public byte[] Content()
        {
            var data = contentData();
            if (data.Length > 10 * 1024 * 1024)
            {
                throw new BmobException("file maxsize is 10M, but upload file size is " + data.Length);
            }

            return data;
        }

        private byte[] contentData()
        {
            // 要上传的文件
            // byte[] ContentsBody = System.IO.File.ReadAllBytes(filepath);
            //byte[] ContentsBody;
            //using (var filestream = File.OpenRead(filepath))
            //{
            //    int fileLen = (int)filestream.Length;
            //    ContentsBody = new byte[fileLen];
            //    filestream.Read(ContentsBody, 0, fileLen);
            //}
            if (datas != null)
            {
                return datas;
            }
            else
            {
#if !WIN8_1
                Boolean close = false;
                try
                {
                    if (inputStream == null)
                    {
                        close = true;
                        inputStream = File.OpenRead(filename);
                    }

                    int fileLen = (int)inputStream.Length;
                    byte[] ContentsBody = new byte[fileLen];
                    inputStream.Read(ContentsBody, 0, fileLen);
                    return ContentsBody;
                }
                finally
                {
                    if (close && inputStream != null)
                    {
                        inputStream.Close();
                    }
                }
#else 
                throw new FileNotFoundException("SDK WIN8.1暂不支持传入文件名的方式上传数据！！！");
#endif
            }

        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <returns>文件名</returns>
        public String Filename()
        {
            return this.filename == null 
                ? "[Binary].bmo" 
                : this.filename.Replace("_", ""); // 服务端下划线BUG[2014-08-19]
        }

        ///// <summary>
        ///// 获取文件名（BASE64编码处理）
        ///// </summary>
        ///// <returns></returns>
        //public String getFilenameBase64()
        //{
        //    return Convert.ToBase64String(Encoding.UTF8.GetBytes(filename));
        //}

    }
}
