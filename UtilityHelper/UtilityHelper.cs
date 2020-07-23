using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace Utility
{
    /// <summary>
    /// 基本属性转换帮助类
    /// </summary>
    public static class UtilityExtension
    {
        #region IsNullOrEmpty

        /// <summary>
        /// 判断对象是否为空对象、空字符串、DBNull
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object o)
        {
            if (o == null)
            {
                return true;
            }

            if (o is string)
            {
                return string.IsNullOrEmpty(o.ToString());
            }

            if (o is DBNull)
            {
                return o == DBNull.Value;
            }

            return false;
        }

        #endregion

        #region SplitPascalCase

        /// <summary>
        /// 将符合Pascal命名标准的字符串分割为多个单词
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SplitPascalCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (input.ToUpper() == "ID")
                return input;

            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        #endregion

        #region Each

        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> each)
        {
            foreach (var item in enumerable)
            {
                each(item);
            }
        }

        #endregion

        public static long AsLong(this object o)
        {
            return Convert.ToInt64(o);
        }

        public static int AsInt(this object o)
        {
            return Convert.ToInt32(o);
        }

        public static bool AsBoolean(this object o)
        {
            return Convert.ToBoolean(o);
        }

        public static decimal AsDecimal(this object o)
        {
            return Convert.ToDecimal(o);
        }

        public static long AsLongWithDefault(this object o)
        {
            long data = 0L;

            if (!o.IsNullOrEmpty() && !DBNull.Value.Equals(o))
            {
                long.TryParse(o.ToString(), out data);
            }

            return data;
        }

        public static int AsIntWithDefault(this object o)
        {
            int data = 0;

            if (!o.IsNullOrEmpty() && !DBNull.Value.Equals(o))
            {
                int.TryParse(o.ToString(), out data);
            }

            return data;
        }

        public static bool AsBooleanWithDefault(this object o)
        {
            bool data = false;

            if (!o.IsNullOrEmpty() && !DBNull.Value.Equals(o))
            {
                bool.TryParse(o.ToString(), out data);
            }

            return data;
        }

        public static decimal AsDecimalWithDefault(this object o)
        {
            decimal data = 0.0m;

            if (!o.IsNullOrEmpty() && !DBNull.Value.Equals(o))
            {
                decimal.TryParse(o.ToString(), out data);
            }

            return data;
        }

        public static decimal AsDecimalWithDefault(this object o, int l)
        {
            decimal data = 0.0m;

            if (!o.IsNullOrEmpty() && !DBNull.Value.Equals(o))
            {
                decimal.TryParse(o.ToString(), out data);
            }

            return Math.Round(data, l);
        }

        public static string AsStringWithDefault(this object o)
        {
            if (o.IsNullOrEmpty() || DBNull.Value.Equals(o))
            {
                return string.Empty;
            }

            return o.ToString();
        }

        public static string AsStringWithDefault(this DateTime o, string f)
        {
            if (o.IsNullOrEmpty() || o == DateTime.MinValue)
            {
                return string.Empty;
            }

            return o.ToString(f);
        }

        public static DateTime AsDateTimeWithDefault(this object o)
        {
            DateTime data = DateTime.MinValue;

            if (!o.IsNullOrEmpty() && !DBNull.Value.Equals(o))
            {
                DateTime.TryParse(o.ToString(), out data);
            }

            return data;
        }

        public static DateTime? AsDateTimeWithNull(this object o)
        {
            DateTime data = DateTime.MinValue;

            if (!o.IsNullOrEmpty() && !DBNull.Value.Equals(o))
            {
                DateTime.TryParse(o.ToString(), out data);
            }
            if (data == DateTime.MinValue)
            {
                return null;
            }
            return data;
        }

        public static object GetSafeValue(this DataRow o, string columnName)
        {
            if (o.Table.Columns.Contains(columnName))
            {
                return o[columnName];
            }

            return null;
        }

        /// <summary>
        /// 保留小数(不四舍六入五取偶)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static decimal AsRetainDecimal(this decimal num, int count = 2)
        {
            decimal temp = 0m;

            string converted = num.AsRetainDecimalStr(count);

            if (decimal.TryParse(converted, out temp))
            {
                return temp;
            }
            else
            {
                return 0m;
            }
        }

        /// <summary>
        /// 保留小数(不四舍六入五取偶)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string AsRetainDecimalStr(this decimal num, int count = 2)
        {
            string _length = "1";
            for (int i = 0; i < count; i++)
            {
                _length += "0";
            }

            int multipleNum = int.Parse(_length);

            if (multipleNum > 1)
            {
                int hundredTimes = (int)(num * multipleNum);

                int integerNum = (int)num;
                int decimalNum = hundredTimes % multipleNum;
                string result;

                if (decimalNum >= 10)
                {
                    result = string.Format("{0}.{1}", integerNum, decimalNum);
                }
                else
                {
                    result = string.Format("{0}.0{1}", integerNum, decimalNum);
                }

                return result;
            }
            else
            {
                return ((int)num).ToString();
            }
        }

        /// <summary>
        /// 获取int64类型随机数
        /// </summary>
        /// <param name="min">取数下限</param>
        /// <param name="max">取数上限</param>
        /// <returns></returns>
        public static long GetRandomInt64(long min, long max)
        {
            byte[] buffer = new byte[8];

            var random = new Random();

            random.NextBytes(buffer);

            long longRand = BitConverter.ToInt64(buffer, 0);

            return (Math.Ads(longRand % (max - min)) + min);
        }

        /// <summary>
        /// 深拷贝(使用二进制序列化和反序列化)
        /// model必须使用Serializable特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Clone<T>(this T obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// 深拷贝(使用json序列化和反序列化)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Clone1<T>(this T obj)
        {
            var str = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
