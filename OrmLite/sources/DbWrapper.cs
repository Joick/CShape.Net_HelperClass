using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Jiajue.BeiJi.Utility
{
    /// <summary>
    /// 数据库封装类
    /// </summary>
    public static class DbWrapper
    {
        private static readonly BaseDataAccess baseAccess = new BaseDataAccess();

        /// <summary>
        /// DB连接
        /// </summary>
        public static OrmLiteConnectionFactory DbConn
        {
            get
            {
                return baseAccess.DbFactory;
            }
        }

        /// <summary>
        /// 新增,返回id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="po"></param>
        /// <returns></returns>
        public static long Insert<T>(T po)
        {
            using (var db = DbConn.Open())
            {
                return db.Insert(po, true);
            }
        }

        /// <summary>
        /// 新增(批量)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listPo"></param>
        public static void InsertAll<T>(IEnumerable<T> listPo)
        {
            using (var db = DbConn.Open())
            {
                db.InsertAll(listPo);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="po"></param>
        /// <returns></returns>
        public static int Update<T>(T po)
        {
            using (var db = DbConn.Open())
            {
                return db.Update(po);
            }
        }

        /// <summary>
        /// 更新(批量)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listPo"></param>
        public static int UpdateAll<T>(IEnumerable<T> listPo)
        {
            using (var db = DbConn.Open())
            {
                return db.UpdateAll(listPo);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="po"></param>
        /// <returns>number of rows deleted</returns>
        public static int Delete<T>(T po)
        {
            using (var db = DbConn.Open())
            {
                return db.Delete(po);
            }
        }

        /// <summary>
        /// 删除(根据主键id) E.g: DeleteById<User>(1)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">primary key</param>
        /// <returns>number of rows deleted</returns>
        public static int Delete<T>(long id)
        {
            using (var db = DbConn.Open())
            {
                return db.DeleteById<T>(id);
            }
        }

        /// <summary>
        /// 删除(批量)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static int DeleteAll<T>(IEnumerable<T> rows)
        {
            using (var db = DbConn.Open())
            {
                return db.DeleteAll(rows);
            }
        }

        /// <summary>
        /// 根据sql和参数查询,返回受影响条数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbParams"></param>
        /// <returns>受影响条数</returns>
        public static int ExecuteSql(string sql, object dbParams)
        {
            using (var db = DbConn.Open())
            {
                return db.ExecuteSql(sql, dbParams);
            }
        }

        /// <summary>
        /// 根据sql查询,返回受影响条数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>受影响条数</returns>
        public static int ExecuteSql(string sql)
        {
            using (var db = DbConn.Open())
            {
                return db.ExecuteSql(sql);
            }
        }

        /// <summary>
        /// 根据sql和参数查询,返回受影响条数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbParams"></param>
        /// <returns>受影响条数</returns>
        public static int ExecuteSql(string sql, params object[] values)
        {
            using (var db = DbConn.Open())
            {
                var dbParams = ProviderBase.GenerateParams(sql, values);

                return db.ExecuteSql(sql, dbParams);
            }
        }

        /// <summary>
        /// 查询(根据主键id) E.g Find<User>(1)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T Find<T>(long id)
        {
            using (var db = DbConn.Open())
            {
                return db.SingleById<T>(id);
            }
        }

        /// <summary>
        /// 查询(使用linq条件)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<T> Find<T>(Expression<Func<T, bool>> predicate)
        {
            using (var db = DbConn.Open())
            {
                return db.Select(predicate);
            }
        }

        /// <summary>
        /// 根据LINQ查询返回第一条(或默认)数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FindFirst<T>(Expression<Func<T, bool>> predicate)
        {
            return Find(predicate).FirstOrDefault();
        }

        /// <summary>
        /// 根据LINQ查询返回最新的一条(或默认)数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FindLastest<T>(Expression<Func<T, bool>> predicate)
        {
            return Find(predicate).LastOrDefault();
        }

        /// <summary>
        /// 根据sql查询返回第一条(或默认)数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T FindFirst<T>(string sql)
        {
            return Select<T>(sql).FirstOrDefault();
        }

        /// <summary>
        /// 根据sql查询返回最新(或默认)数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T FindLastest<T>(string sql)
        {
            return Select<T>(sql).LastOrDefault();
        }

        /// <summary>
        /// 根据sql语句和参数值查询 E.g:
        ///     db.Select<user>("Age > @age", new { age = 40})
        ///     db.Select<user>("select * from user where Age > @age", new { age = 40})
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="anonType"></param>
        /// <returns></returns>
        public static List<T> Select<T>(string sql, object anonType)
        {
            using (var db = DbConn.Open())
            {
                return db.Select<T>(sql, anonType);
            }
        }

        /// <summary>
        /// 根据sql语句和参数值查询 E.g:
        ///     db.Select<user>("Age > @age", 1,2,3)
        ///     db.Select<user>("select * from user WHERE Age > @age", 1,2,3)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<T> Select<T>(string sql, params object[] values)
        {
            using (var db = DbConn.Open())
            {
                var paramList = ProviderBase.GenerateParams(sql, values);

                return db.Select<T>(sql, paramList);
            }
        }

        /// <summary>
        /// 根据sql查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> Select<T>(string sql)
        {
            using (var db = DbConn.Open())
            {
                return db.Select<T>(sql);
            }
        }

        /// <summary>
        /// 查询符合条件的数量(linq)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static long GetCount<T>(Expression<Func<T, bool>> predicate)
        {
            using (var db = DbConn.Open())
            {
                return db.Count(predicate);
            }
        }

        /// <summary>
        /// 查询count(1)结果 同:select count(1) from user.....
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="anonType"></param>
        /// <returns></returns>
        public static int Scalar(string sql, object anonType = null)
        {
            using (var db = DbConn.Open())
            {
                return db.SqlScalar<int>(sql, anonType);
            }
        }

        /// <summary>
        /// 根据sql和参数值查询count(1)值. 同: select count(1) from user where .....
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int Scalar(string sql, params object[] values)
        {
            using (var db = DbConn.Open())
            {
                var param = ProviderBase.GenerateParams(sql, values);

                return db.SqlScalar<int>(sql, param);
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="orderBy"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static PageQueryResult<T> QueryPageData<T>(string sql, int pageSize, int pageIndex, string orderBy, params object[] values)
        {
            PageQueryResult<T> qr = new PageQueryResult<T>(pageSize, pageIndex, orderBy);

            string _sqlCount = string.Format("select count(1) from ({0})", sql);

            qr.ItemCount = Scalar(_sqlCount, values);

            if (qr.ItemCount == 0)
            {
                qr.PageIndex = 1;
            }
            else if (qr.PageIndex > qr.PageCount)
            {
                qr.PageIndex = qr.PageCount;
            }

            string _dataSql = ProviderBase.CreatePageSql(sql, qr);

            List<T> data = Select<T>(_dataSql, values);

            qr.ResultData = data;

            return qr;
        }

        /// <summary>
        /// 事务处理逻辑
        /// </summary>
        /// <param name="func">事务</param>
        public static bool TransactionLogic(Func<IDbConnection, bool> func)
        {
            using (var db = DbConn.Open())
            {
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        var result = func(db);

                        if (result)
                            tran.Commit();
                        else
                            return false;

                        return true;
                    }
                    catch
                    {
                        tran.Rollback();

                        return false;
                    }
                }
            }
        }
    }
}
