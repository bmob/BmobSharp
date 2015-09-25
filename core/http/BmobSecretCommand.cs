
using cn.bmob.config;
using cn.bmob.tools;
using System;
using System.IO;
using System.Security.Cryptography;
using cn.bmob.Extensions;
using System.Collections.Generic;
using cn.bmob.exception;
using cn.bmob.json;
using System.Text;

namespace cn.bmob.http
{

    internal class BmobSecretCommand<T> : BmobCommand<T>
    {
        private static String Secret { get; set; }

        public BmobSecretCommand(BmobInteractObject mReceiver)
            : base(mReceiver)
        {
        }

        internal override IDictionary<String, String> getHeaders()
        {
            var headers = new Dictionary<String, String>();
            return headers;
        }

        public override R execute<R>(Func<String, String, String, Byte[], IDictionary<String, String>, Action<String, BmobException>, R> request, BmobCallback<T> fCallback)
        {
            var action = getReceiver().Action;
            var url = Utilities.getRequestURL(action);

            var contentType = Configuration.V8_CONTENT_TYPE;
            var agent = "Bmob/" + DateTime.Now.Second + Configuration.PLATFORM.ToString() + Configuration.BUILD_VERSION;

            var headers = getHeaders();

            var key = agent.Substring(agent.Length - 16);

            var postData = AESEncrypt(getPostData(), key.GetBytes(), key.GetBytes()).ToBase64().GetBytes();

            return Execute(request, url, contentType, agent, postData, headers, fCallback);
        }

        protected override R Execute<R>(Func<String, String, String, Byte[], IDictionary<String, String>, Action<String, BmobException>, R> request,
                    String url, String contentType, String userAgent, byte[] postData, IDictionary<String, String> headers, BmobCallback<T> fCallback)
        {
            BmobDebug.T(" ->\r\n\tAppKey : " + getReceiver().AppKey
                        + "\r\n\t请求的URL : " + url
                        + "\r\n\t请求的数据: " + JsonAdapter.JSON.ToJson(getReceiver()));

            return request.Invoke(url, contentType, userAgent, postData, headers, (resp, ex) =>
            {
                if (BmobDebug.Debug)
                {
                    BmobDebug.D("返回数据内容为: " + resp);
                }
                else
                {
                    var rp = resp.Length > 400 ? resp.Substring(0, 200) + " ... ... ... ... ... ... " + resp.Substring(resp.Length - 200) : resp;
                    BmobDebug.I("返回数据内容为: " + rp);
                }

                onPostExecute(resp, ex, fCallback);
            });

        }

        public static byte[] AESEncrypt(byte[] input, byte[] key, byte[] initializationVector)
        {
            //分组加密算法  
            SymmetricAlgorithm des = Rijndael.Create();
            //RijndaelManaged des = new RijndaelManaged();
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;

            //设置密钥及密钥向量  
            des.Key = key;
            des.IV = key;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(input, 0, input.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();//得到加密后的字节数组  
                }
            }
        }

        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="cipherText">密文字节数组</param>  
        /// <param name="key">密钥</param>  
        /// <returns>返回解密后的字符串</returns>  
        public static byte[] AESDecrypt(byte[] cipherText, byte[] key, byte[] initializationVector)
        {
            //SymmetricAlgorithm des = Rijndael.Create();
            RijndaelManaged des = new RijndaelManaged();
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;

            des.Key = key;
            des.IV = key;

            using (MemoryStream ms = new MemoryStream(cipherText))
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] decryptBytes = new byte[cipherText.Length];
                    cs.Read(decryptBytes, 0, decryptBytes.Length);
                    return decryptBytes;
                }
            }
        }



        /// <summary>
        /// 使用AES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密字符串</param>
        /// <param name="encryptKey">加密密匙</param>
        /// <param name="salt">盐</param>
        /// <returns>加密结果，加密失败则返回源串</returns>
        public static string EncryptAES(string encryptString, string encryptKey, string salt)
        {
            AesManaged aes = null;
            MemoryStream ms = null;
            CryptoStream cs = null;

            try
            {
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(encryptKey, Encoding.UTF8.GetBytes(salt));

                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                ms = new MemoryStream();
                cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);

                byte[] data = Encoding.UTF8.GetBytes(encryptString);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return encryptString;
            }
            finally
            {
                if (cs != null)
                    cs.Close();

                if (ms != null)
                    ms.Close();

                if (aes != null)
                    aes.Clear();
            }
        }

        /// <summary>
        /// 使用AES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密字符串</param>
        /// <param name="decryptKey">解密密匙</param>
        /// <param name="salt">盐</param>
        /// <returns>解密结果，解谜失败则返回源串</returns>
        public static string DecryptAES(string decryptString, string decryptKey, string salt)
        {
            AesManaged aes = null;
            MemoryStream ms = null;
            CryptoStream cs = null;

            try
            {
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(decryptKey, Encoding.UTF8.GetBytes(salt));

                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                ms = new MemoryStream();
                cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);

                byte[] data = Convert.FromBase64String(decryptString);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();

                return Encoding.UTF8.GetString(ms.ToArray(), 0, ms.ToArray().Length);
            }
            catch
            {
                return decryptString;
            }
            finally
            {
                if (cs != null)
                    cs.Close();

                if (ms != null)
                    ms.Close();

                if (aes != null)
                    aes.Clear();
            }
        }

    }

}
