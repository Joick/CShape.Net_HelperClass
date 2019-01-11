using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Utility
{
    /// <summary>
    /// 数据库帮助类
    /// </summary>
    public static class DbHelper
    {
        /// <summary>
        /// 生成数据库连接加密字符串
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static string GenerateEncryptSql(string connectionString)
        {
            return SessionFactory.EncryptConnectionString(connectionString);
        }

        public static string DecryptSql(string encryptString)
        {
            return SessionFactory.DecryptConnectionString(encryptString);
        }

        public static object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null);
        }

        public static object ExecuteScalar(string sql, params object[] values)
        {
            using (ISession session = SessionFactory.GetSession())
            {
                return session.ExecuteScalar(sql, values);
            }
        }

        public static DbDataReader ExecuteDataReader(string sql)
        {
            return ExecuteDataReader(sql, null);
        }

        public static DbDataReader ExecuteDataReader(string sql, params object[] values)
        {
            using (ISession session = SessionFactory.GetSession())
            {
                return session.ExecuteDataReader(sql, values);
            }
        }

        public static DbDataReader ExecuteDataReaderWithCache(string sql, params object[] values)
        {
            using (ISession session = SessionFactory.GetSession())
            {
                return session.ExecuteDataReaderWithCache(sql, values);
            }
        }

        public static DataTable ExecuteDataTable(string sql)
        {
            return ExecuteDataTable(sql, null);
        }

        public static DataTable ExecuteDataTable(string sql, params object[] values)
        {
            using (ISession session = SessionFactory.GetSession())
            {
                return session.ExecuteDataTable(sql, values);
            }
        }

        public static DataSet ExecuteDataSet(string sql)
        {
            return ExecuteDataSet(sql, null);
        }

        public static DataSet ExecuteDataSet(string sql, params object[] values)
        {
            using (ISession session = SessionFactory.GetSession())
            {
                return session.ExecuteDataSet(sql, values);
            }
        }

        public static int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, null);
        }

        public static int ExecuteNonQuery(string sql, params object[] values)
        {
            using (ISession session = SessionFactory.GetSession())
            {
                return session.ExecuteNonQuery(sql, values);
            }
        }

        public static QueryResult ExecutePageQuery(string sql, int pageSize, int pageIndex, string orderBy)
        {
            return ExecutePageQuery(sql, pageSize, pageIndex, orderBy, null);
        }

        public static QueryResult ExecutePageQuery(string sql, int pageSize, int pageIndex, string orderBy, params object[] values)
        {
            using (ISession session = SessionFactory.GetSession())
            {
                return session.ExecutePageQuery(sql, pageSize, pageIndex, orderBy, values);
            }
        }

        public static long Insert<T>(T eo, bool isReturnId = false) where T : EntityObject<T>, new()
        {
            using (ISession session = SessionFactory.GetSession())
            {
                long id = session.Insert<T>(eo);

                if (isReturnId)
                {
                    id = session.GetIdentity();
                }

                return id;
            }
        }

        public static int Update<T>(T eo) where T : EntityObject<T>, new()
        {
            using (ISession session = SessionFactory.GetSession())
            {
                return session.Update<T>(eo);
            }
        }

        public static int Delete<T>(T eo) where T : EntityObject<T>, new()
        {
            using (ISession session = SessionFactory.GetSession())
            {
                return session.Delete<T>(eo);
            }
        }

        public static IList<T> Find<T>(T eo) where T : EntityObject<T>, new()
        {
            using (ISession session = SessionFactory.GetSession())
            {
                return session.Find<T>(eo);
            }
        }

        public static void ExecuteStoredProcedure(string storedProcedureName, IList<DbParameter> parameters)
        {
            using (ISession session = SessionFactory.GetSession())
            {
                session.ExecuteStoredProcedure(storedProcedureName, parameters);
            }
        }
    }
}
