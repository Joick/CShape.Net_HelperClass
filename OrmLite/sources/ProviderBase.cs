using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Jiajue.BeiJi.Utility
{
    /// <summary>
    /// provider base server
    /// </summary>
    public static class ProviderBase
    {
        public const string ParameterPrefix = "@";

        /// <summary>
        /// generate page query sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="qr"></param>
        /// <returns></returns>
        public static string CreatePageSql(string sql, PageQueryResult qr)
        {
            return string.Format("select * from ({0}) T order by {1} limit {2}, {3}", sql, qr.OrderBy, (qr.PageIndex - 1) * qr.PageSize, qr.PageSize);
        }


        /// <summary>
        /// formatter param names from sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string[] AnalysisParameter(string sql)
        {
            string pattern = string.Format(@"[^{0}{1}](?<Parameter>{2}\w+)", ParameterPrefix, ParameterPrefix, ParameterPrefix);

            HashSet<string> names = new HashSet<string>();

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            MatchCollection matches = regex.Matches(string.Concat(sql, " "));

            foreach (Match match in matches)
            {
                names.Add(match.Groups["Parameter"].Value);
            }

            return names.ToArray();
        }

        /// <summary>
        /// generate sql params by select sql and request params
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GenerateParams(string sql, params object[] values)
        {
            Dictionary<string, object> paramList = new Dictionary<string, object>();

            string[] names = AnalysisParameter(sql);

            if (names.Length != values.Length)
                throw new ArgumentException("");

            for (int i = 0, length = names.Length; i < length; i++)
            {
                paramList.Add(names[i], values[i] == null ? DBNull.Value : values[i]);
            }

            return paramList;
        }
    }
}
