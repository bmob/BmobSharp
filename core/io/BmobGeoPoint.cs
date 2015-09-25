using System;

namespace cn.bmob.io
{
    /// <summary>
    /// 地理位置处理类
    /// 
    /// 纬度的范围应该是在-90.0到90.0之间。经度的范围应该是在-180.0到180.0之间。
    /// </summary>
    public sealed class BmobGeoPoint : BmobObject
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BmobGeoPoint() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        public BmobGeoPoint(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// 获取类型信息
        /// </summary>
        public override string _type { get { return "GeoPoint"; } }

        private BmobDouble latitude;
        /// <summary>
        /// 纬度 [-90, 90]
        /// </summary>
        public BmobDouble Latitude
        {
            get { return this.latitude; }
            set
            {
                this.latitude = value;
                if (this.latitude.Get() < -90 || this.latitude.Get() > 90)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        private BmobDouble longitude;
        /// <summary>
        /// 经度 [-180, 180]
        /// </summary>
        public BmobDouble Longitude
        {
            get { return this.longitude; }
            set
            {
                this.longitude = value;
                if (this.longitude.Get() < -180 || this.longitude.Get() > 180)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public override void readFields(BmobInput input)
        {
            this.latitude = input.getDouble("latitude");
            this.longitude = input.getDouble("longitude");
        }

        public override void write(BmobOutput output, Boolean all)
        {
            output.Put(TYPE_NAME, this._type);
            output.Put("latitude", this.latitude);
            output.Put("longitude", this.longitude);
        }

    }
}
