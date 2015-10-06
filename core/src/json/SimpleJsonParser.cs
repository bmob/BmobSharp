using System;

using cn.bmob.io;
using System.Collections.Generic;
using System.IO;
using cn.bmob.tools;
using cn.bmob.exception;
using SimpleJson;

namespace cn.bmob.json
{
    internal class SimpleJsonParser : IJsonParser
    {
        #region 注册自定义的IBmobOject/IBmobValue序列化

        public class BmobWritableConverter : PocoJsonSerializerStrategy, IJsonSerializerStrategy
        {
            private Boolean isPrint = false;

            public BmobWritableConverter() { }
            public BmobWritableConverter(Boolean isPrint) {
                this.isPrint = isPrint;
            }

            // obj to IDictionary<String, Object>
            public override bool TrySerializeNonPrimitiveObject(object input, out object output)
            {
                try
                {
                    Type objectType = input.GetType();

                    if (/*typeof(IBmobWritable).IsAssignableFrom(objectType)*/ input is IBmobWritable )
                    {
                        IBmobWritable obj = (IBmobWritable)input;
                        BmobOutput tOutput = new BmobOutput();
                        obj.write(tOutput, isPrint);

                        output = tOutput.getData();
                        return true;
                    }
                    else if (objectType == typeof(BmobInt) ||
                         objectType == typeof(BmobLong) ||
                         objectType == typeof(BmobDouble) ||
                         objectType == typeof(BmobBoolean) ||
                         objectType == typeof(BmobACL))
                    {

                        object value = 0;
                        if (input is BmobInt)
                            value = (input as BmobInt).Get();
                        else if (input is BmobLong)
                            value = (input as BmobLong).Get();
                        else if (input is BmobDouble)
                            value = (input as BmobDouble).Get();
                        else if (input is BmobBoolean)
                            value = (input as BmobBoolean).Get();
                        else if (input is BmobACL)
                            value = (input as BmobACL).Get();

                        output = value;
                        return true;
                    }

                }
                catch (Exception e)
                {
                    BmobDebug.Log(e);
                }

                return base.TrySerializeNonPrimitiveObject(input, out output);
            }

            // IDictionary<String, Object>/List<X,X>/primitive to obj
            public override object DeserializeObject(object value, Type type)
            {
                throw new NotSupportedException("在BmobInput中处理!!!");
            }

        }

        #endregion

        /// <summary>
        /// 使用反射进行序列化
        /// </summary>
        public String ToRawString(object data)
        {
            return SimpleJson.SimpleJson.SerializeObject(data);
        }

        public String ToDebugJsonString(object data)
        {
            return SimpleJson.SimpleJson.SerializeObject(data, new BmobWritableConverter(true));
        }

        /// <summary>
        /// 使用反射进行反序列化
        /// </summary>
        public T ToObject<T>(String json)
        {
            return SimpleJson.SimpleJson.DeserializeObject<T>(json);
        }

        public String ToJsonString(object data)
        {
            return SimpleJson.SimpleJson.SerializeObject(data, new BmobWritableConverter());
        }

        // 先转成Dictionary（解析为基础类型）然后在进行处理。
        public Object ToObject(String json)
        {
            return SimpleJson.SimpleJson.DeserializeObject(json);
        }

    }
}
