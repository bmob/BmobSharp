using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace cn.bmob.Extensions
{
    /// <summary>
    /// copy restsharp
    /// </summary>
    public static class MiscExtensions
    {
#if WIN8_1
        public static void Close(this HttpWebResponse m)
        {
            m.Dispose();
        }

        public static Type[] GetGenericArguments(this Type t)
        {
            return System.Reflection.IntrospectionExtensions.GetTypeInfo(t).GenericTypeArguments;
        }

        public static bool IsAssignableFrom(this Type a, Type b)
        {
            var at = System.Reflection.IntrospectionExtensions.GetTypeInfo(a);
            var bt = System.Reflection.IntrospectionExtensions.GetTypeInfo(b);
            return at.IsAssignableFrom(bt);
        }
#endif

        public static bool IsGenericType(this Type t)
        {
#if WIN8_1
            return System.Reflection.IntrospectionExtensions.GetTypeInfo(t).IsGenericType;
#else
            return t.IsGenericType;
#endif
        }

        /// <summary>
        /// Read a stream into a byte array
        /// </summary>
        /// <param name="input">Stream to read</param>
        /// <returns>byte[]</returns>
        public static byte[] ReadAsBytes(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static byte[] GetBytes(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        public static String ToBase64(this string s)
        {
            return Convert.ToBase64String(s.GetBytes());
        }

        public static String ToBase64(this byte[] s)
        {
            return Convert.ToBase64String(s);
        }

        public static String FromBase64(this string s)
        {
            return Convert.FromBase64String(s).AsString();
        }

        private static char[] HEX_ALPHA = new char[]{ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        public static String ToHexString(this byte[] src)
        {
            if (src == null || src.Length <= 0)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < src.Length; i++)
            {
                sb.Append(HEX_ALPHA[src[i] >> 4 & 0xF]);
                sb.Append(HEX_ALPHA[src[i] & 0xF]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts a byte array to a string, using its byte order mark to convert it to the right encoding.
        /// http://www.shrinkrays.net/code-snippets/csharp/an-extension-method-for-converting-a-byte-array-to-a-string.aspx
        /// </summary>
        /// <param name="buffer">An array of bytes to convert</param>
        /// <returns>The byte as a string.</returns>
        public static string AsString(this byte[] buffer)
        {
            if (buffer == null) return "";

            // UTF8 as default
            Encoding encoding = Encoding.UTF8;

#if FRAMEWORK
            return encoding.GetString(buffer, 0, buffer.Length);
#else
            if (buffer == null || buffer.Length == 0)
                return "";

            /*
                EF BB BF		UTF-8 
                FF FE UTF-16	little endian 
                FE FF UTF-16	big endian 
                FF FE 00 00		UTF-32, little endian 
                00 00 FE FF		UTF-32, big-endian 
                */

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
            {
                encoding = Encoding.UTF8;
            }
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
            {
                encoding = Encoding.Unicode;
            }
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
            {
                encoding = Encoding.BigEndianUnicode; // utf-16be
            }

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
#endif
        }
        
        /// <summary>
        /// 将列表连接为字符串，连接符为,号
        /// </summary>
        /// <param name="list">列表</param>
        /// <returns>连接之后的字符串</returns>
        public static String join(this IList list)
        {
            return list.join(',');
        }

        /// <summary>
        /// 将列表连接为字符串
        /// </summary>
        /// <param name="list">列表</param>
        /// <param name="separator">连接符</param>
        /// <returns>连接之后的字符串</returns>
        public static String join(this IList list, char separator)
        {
            StringBuilder result = new StringBuilder();

            bool first = true;
            foreach (object ele in list)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    result.Append(separator);
                }

                result.Append(ele.ToString());
            }
            return result.ToString();
        }

        public static string UrlEncode(this string input)
        {
            const int maxLength = 32766;
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.Length <= maxLength)
                return Uri.EscapeDataString(input);

            StringBuilder sb = new StringBuilder(input.Length * 2);
            int index = 0;
            while (index < input.Length)
            {
                int length = Math.Min(input.Length - index, maxLength);
                string subString = input.Substring(index, length);
                sb.Append(Uri.EscapeDataString(subString));
                index += subString.Length;
            }

            return sb.ToString();
        }

    }
}
