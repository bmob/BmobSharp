using System;

namespace cn.bmob.io
{
    /// <summary>
    /// mode:模式 0: 指定宽， 高自适应，等比例缩放
    //    模式 1: 指定高， 宽自适应，等比例缩放
    //     模式 2: 指定最长边，短边自适应，等比例缩放
    //    模式 3: 指定最短边，长边自适应，等比例缩放
    //    模式 4: 指定最大宽高， 等比例缩放
    //     模式 5: 固定宽高， 居中裁剪    
    //image:原图片url
    //width:宽度，模式 0, 4, 5必填
    //height：高度，模式 1, 4, 5必填
    //longEdge：长边，模式 2必填
    //shortEdge：短边，模式 3必填
    //quality：质量，选填, 范围 1-100
    //outType：输出类型，0:默认，输出url；1:输出base64编码的字符串流
    /// </summary>
    public sealed class ThumbnailParameter : BmobObject, IBmobWritable
    {
        public ThumbnailParameter(double width, double height, String image)
        {
            Mode = 4; Width = width; Height = height; Quality = 100; OutType = 1; Image = image;
        }

        public ThumbnailParameter(double width, String image)
        {
            Mode = 0; Width = width; Quality = 100; OutType = 1; Image = image;
        }

        public ThumbnailParameter()
        {
            Mode = 0; Quality = 100; OutType = 1;
        }

        public BmobInt Mode { get; set; }
        public string Image { get; set; }
        public BmobDouble Width { get; set; }
        public BmobDouble Height { get; set; }
        public BmobDouble LongEdge { get; set; }
        public BmobDouble ShortEdge { get; set; }
        public BmobInt Quality { get; set; }
        public BmobInt OutType { get; set; }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            if (Mode != null) output.Put("mode", Mode);
            if (Width != null) output.Put("width", Width);
            if (Height != null) output.Put("height", Height);
            if (LongEdge != null) output.Put("longEdge", LongEdge);
            if (ShortEdge != null) output.Put("shortEdge", ShortEdge);
            if (Quality != null) output.Put("quality", Quality);
            if (OutType != null) output.Put("outType", OutType);

            if (Image != null) output.Put("image", Image);
        }

    }
}
