using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Utility
{
    public interface ISession : IDisposable
    {
        /// <summary>
        /// 开始一个事务
        /// </summary>
        /// <returns></returns>
        DbTransaction BeginTransaction();

        /// <summary>
        /// 执行 SQL 语句，返回一个单一对象
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <returns>单一对象</returns>
        object ExecuteScalar(string sql);

        /// <summary>
        /// 执行带参数的 SQL 语句，返回一个单一对象
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="values">参数值</param>
        /// <returns>单一对象</returns>
        object ExecuteScalar(string sql, params object[] values);

        /// <summary>
        /// 执行 SQL 语句，返回一个 DataReader 对象
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <returns>DataReader 对象</returns>
        DbDataReader ExecuteDataReader(string sql);

        /// <summary>
        /// 执行带参数 SQL 语句，返回一个 DataReader 对象
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="values">参数值</param>
        /// <returns>DataReader 对象</returns>
        DbDataReader ExecuteDataReader(string sql, params object[] values);

        /// <summary>
        /// 执行带参数 SQL 语句，返回一个 DataReader 对象
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="values">参数值</param>
        /// <returns>DataReader 对象</returns>
        DbDataReader ExecuteDataReaderWithCache(string sql, params object[] values);


        /// <summary>
        /// 执行 SQL 语句，返回一个 DataTable 对象
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <returns>DataTable 对象</returns>
        DataTable ExecuteDataTable(string sql);

        /// <summary>
        /// 执行带参数 SQL 语句，返回一个 DataTable 对象
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="values">参数值</param>
        /// <returns>DataTable 对象</returns>
        DataTable ExecuteDataTable(string sql, params object[] values);

        /// <summary>
        /// 执行 SQL 语句，返回一个 DataSet 对象
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <returns>DataSet 对象</returns>
        DataSet ExecuteDataSet(string sql);

        /// <summary>
        /// 执行带参数 SQL 语句，返回一个 DataTable 对象
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="values">参数值</param>
        /// <returns>DataSet 对象</returns>
        DataSet ExecuteDataSet(string sql, params object[] values);

        /// <summary>
        /// 执行非查询 SQL 语句，返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <returns>受影响的行数</returns>
        int ExecuteNonQuery(string sql);

        /// <summary>
        /// 执行带参数的非查询 SQL 语句，返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="values">参数值</param>
        /// <returns>受影响的行数</returns>
        int ExecuteNonQuery(string sql, params object[] values);

        /// <summary>
        /// 执行分页查询语句，并返回 DataTable 对象
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="sort">排序字段</param>
        /// <returns>DataTable 对象</returns>
        QueryResult ExecutePageQuery(string sql, int pageSize, int pageIndex, string sort);

        /// <summary>
        /// 执行带参数的分页查询语句，并返回 DataTable 对象
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sort">排序字段</param>
        /// <param name="values">参数值</param>
        /// <returns>DataTable 对象</returns>
        QueryResult ExecutePageQuery(string sql, int index, int pageSize, string pageIndex, params object[] values);

        /// <summary>
        /// 向数据库插入一个实体对象
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="eo">实体对象</param>
        int Insert<T>(T eo) where T : EntityObject<T>, new();

        /// <summary>
        /// 将实体对象的更改更新到数据库中
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="eo">实体对象</param>
        int Update<T>(T eo) where T : EntityObject<T>, new();

        /// <summary>
        /// 将数据库中对象对应的记录删除
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="eo">实体对象</param>
        /// <param name="singleRow">是否只删除一行数据</param>
        int Delete<T>(T eo,bool singleRow = true) where T : EntityObject<T>, new();

        IList<T> Find<T>(T eo) where T : EntityObject<T>, new();

        long GetIdentity();

        DbParameter CreateParameter();

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="inParameters">传入参数</param>
        /// <param name="outParameters">传出参数</param>
        void ExecuteStoredProcedure(string storedProcedureName, IList<DbParameter> parameters);
    }
}
