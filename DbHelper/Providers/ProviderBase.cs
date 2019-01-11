using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utility
{
    internal abstract class ProviderBase : ISession
    {
        private DbConnection connection;
        private DbTransaction transaction;

        public abstract string ParameterPrefix { get; }
        public abstract string QuoteHeader { get; }
        public abstract string QuoteFooter { get; }

        protected abstract DbConnection CreateConnection();
        protected abstract DbDataAdapter CreateDataAdapter();

        protected abstract string CreatePageSQL(string sql, QueryResult qr);

        public ProviderBase(string connectionString)
        {
            this.connection = this.CreateConnection();
            this.connection.ConnectionString = connectionString;
            this.connection.Open();
        }

        #region IDispose
        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                this.connection.Close();
            }

            this.disposed = true;
        }
        #endregion

        #region SetParameter

        /// <summary>
        /// SetParameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        private void SetParameter(DbCommand command, string sql, object[] values)
        {
            if (values != null && values.Length > 0)
            {
                string[] names = this.AnalysisParameter(sql);

                if (names.Length != values.Length)
                    throw new ArgumentException("");

                for (int i = 0; i < names.Length; i++)
                {
                    DbParameter parameter = command.CreateParameter();
                    parameter.ParameterName = names[i];
                    parameter.Value = values[i] == null ? DBNull.Value : values[i];
                    command.Parameters.Add(parameter);
                }
            }
        }
        #endregion

        #region AnalysisParameter
        /// <summary>
        /// 解析 SQL 语句中的参数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected virtual string[] AnalysisParameter(string sql)
        {
            string pattern = string.Format(@"[^{0}{1}](?<Parameter>{2}\w+)", this.ParameterPrefix, this.ParameterPrefix, this.ParameterPrefix);

            HashSet<string> names = new HashSet<string>();

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            MatchCollection matches = regex.Matches(string.Concat(sql, " "));

            foreach (Match match in matches)
            {
                names.Add(match.Groups["Parameter"].Value);
            }

            return names.ToArray();
        }
        #endregion

        #region ado.net extension
        public DbTransaction BeginTransaction()
        {
            return this.transaction = this.connection.BeginTransaction();
        }

        public object ExecuteScalar(string sql)
        {
            return this.ExecuteScalar(sql, null);
        }

        public object ExecuteScalar(string sql, params object[] values)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = this.transaction;

                this.SetParameter(command, sql, values);

                return command.ExecuteScalar();
            }
        }

        public DbDataReader ExecuteDataReader(string sql)
        {
            return this.ExecuteDataReader(sql, null);
        }

        public DbDataReader ExecuteDataReader(string sql, params object[] values)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = this.transaction;

                this.SetParameter(command, sql, values);

                DbDataReader reader = command.ExecuteReader();


                return reader;
            }
        }

        public DbDataReader ExecuteDataReaderWithCache(string sql, params object[] values)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = this.transaction;

                this.SetParameter(command, sql, values);

                DbDataReader reader = command.ExecuteReader();

                var dt = new DataTable();
                dt.Load(reader);

                return dt.CreateDataReader();
            }
        }


        public DataTable ExecuteDataTable(string sql)
        {
            return this.ExecuteDataTable(sql, null);
        }

        public DataTable ExecuteDataTable(string sql, params object[] values)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = transaction;

                this.SetParameter(command, sql, values);

                using (DbDataAdapter adapter = this.CreateDataAdapter())
                {
                    DataTable result = new DataTable("Table1");
                    adapter.SelectCommand = command;
                    adapter.Fill(result);
                    return result;
                }
            }
        }

        public DataSet ExecuteDataSet(string sql)
        {
            return ExecuteDataSet(sql, null);
        }

        public DataSet ExecuteDataSet(string sql, params object[] values)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = this.transaction;

                this.SetParameter(command, sql, values);

                using (DbDataAdapter adapter = this.CreateDataAdapter())
                {
                    DataSet result = new DataSet("DataSet1");
                    adapter.SelectCommand = command;
                    adapter.Fill(result);
                    return result;
                }
            }
        }

        public int ExecuteNonQuery(string sql)
        {
            return this.ExecuteNonQuery(sql, null);
        }

        public int ExecuteNonQuery(string sql, params object[] values)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = this.transaction;

                this.SetParameter(command, sql, values);

                return command.ExecuteNonQuery();
            }
        }

        public QueryResult ExecutePageQuery(string sql, int pageSize, int pageIndex, string orderBy)
        {
            return this.ExecutePageQuery(sql, pageSize, pageIndex, orderBy, null);
        }

        public QueryResult ExecutePageQuery(string sql, int pageSize, int pageIndex, string orderBy, params object[] values)
        {
            QueryResult qr = new QueryResult(pageSize, pageIndex, orderBy);

            if (pageIndex < 1)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            if (string.IsNullOrEmpty(orderBy))
            {
                throw new ArgumentNullException("orderBy");
            }

            string sql1 = string.Format("SELECT COUNT(1) FROM ({0}) T1", sql);

            object o = this.ExecuteScalar(sql1, values);

            qr.ItemCount = Convert.ToInt32(o);

            if (qr.ItemCount == 0)
            {
                qr.PageIndex = 1;
            }
            else if (qr.PageIndex > qr.PageCount)
            {
                qr.PageIndex = qr.PageCount;
            }

            string sql2 = this.CreatePageSQL(sql, qr).Replace("\r\n", string.Empty);

            DataTable dt = this.ExecuteDataTable(sql2, values);

            qr.Data = dt;

            return qr;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 将实体对象新增到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eo"></param>
        public virtual int Insert<T>(T eo) where T : EntityObject<T>, new()
        {
            StringBuilder builder = new StringBuilder();
            string fields = string.Empty;
            string paramters = string.Empty;
            List<object> values = new List<object>();

            var mappings = from s in eo.Mappings
                           where !s.IsAutoIncrease && eo.ChangedProperties.Contains(s.ColumnName)
                           select s;

            mappings.Each(mapping =>
           {
               string mappingColumnName = mapping.ColumnName;

               if (fields.IsNullOrEmpty())
               {
                   fields = fields + this.QuoteHeader + mappingColumnName + this.QuoteFooter;
                   paramters = paramters + this.ParameterPrefix + mappingColumnName;
               }
               else
               {
                   fields = fields + ", " + this.QuoteHeader + mappingColumnName + this.QuoteFooter;
                   paramters = paramters + "," + this.ParameterPrefix + mappingColumnName;
               }

               values.Add(mapping.Expression.ToMember().GetValue(eo));
           });

            builder.Append("insert into " + this.QuoteHeader);
            builder.Append(eo.TableName);
            builder.Append(this.QuoteFooter + "( ");
            builder.Append(fields);
            builder.Append(" ) values( ");
            builder.Append(paramters);
            builder.Append(" );");

            return this.ExecuteNonQuery(builder.ToString(), values.ToArray());
        }
        #endregion

        #region Update
        /// <summary>
        /// 将实体对象更新到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eo"></param>
        public virtual int Update<T>(T eo) where T : EntityObject<T>, new()
        {
            StringBuilder builder = new StringBuilder();
            string fields = string.Empty;
            string keys = string.Empty;
            List<object> values = new List<object>();

            var mappings = from s in eo.Mappings
                           where !s.IsAutoIncrease && !s.IsPrimaryKey && eo.ChangedProperties.Contains(s.ColumnName)
                           select s;

            mappings.Each(mapping =>
           {
               string mappingColumnName = mapping.ColumnName;

               if (fields.IsNullOrEmpty())
               {
                   fields = fields + this.QuoteHeader + mappingColumnName + this.QuoteFooter + " = " + this.ParameterPrefix + mappingColumnName;
               }
               else
               {
                   fields = fields + ", " + this.QuoteHeader + mappingColumnName + this.QuoteFooter + " = " + this.ParameterPrefix + mappingColumnName;
               }

               values.Add(mapping.Expression.ToMember().GetValue(eo));
           });

            mappings = from s in eo.Mappings
                       where s.IsPrimaryKey
                       select s;

            mappings.Each(mapping =>
           {
               string mappingColumnName = mapping.ColumnName;

               if (keys.IsNullOrEmpty())
               {
                   keys = keys + this.QuoteHeader + mappingColumnName + this.QuoteFooter + " = " + this.ParameterPrefix + mappingColumnName;
               }
               else
               {
                   keys = keys + " and " + this.QuoteHeader + mappingColumnName + this.QuoteFooter + " = " + this.ParameterPrefix + mappingColumnName;
               }

               values.Add(mapping.Expression.ToMember().GetValue(eo));
           });

            builder.Append("update " + this.QuoteHeader);
            builder.Append(eo.TableName);
            builder.Append(this.QuoteFooter + " set ");
            builder.Append(fields);
            builder.Append(" where ");
            builder.Append(keys);

            return this.ExecuteNonQuery(builder.ToString(), values.ToArray());
        }
        #endregion

        #region Delete

        /// <summary>
        /// 从数据库中删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eo"></param>
        /// <param name="singleRow">是否只删除一行数据</param>
        public virtual int Delete<T>(T eo, bool singleRow = true) where T : EntityObject<T>, new()
        {
            StringBuilder builder = new StringBuilder();
            string keys = string.Empty;
            List<object> values = new List<object>();

            var mappings = from s in eo.Mappings where s.IsPrimaryKey select s;

            mappings.Each(mapping =>
           {
               string mappingColumnName = mapping.ColumnName;

               if (keys.IsNullOrEmpty())
               {
                   keys = keys + mappingColumnName + " = " + this.ParameterPrefix + mappingColumnName;
               }
               else
               {
                   if (singleRow)
                   {
                       keys = keys + " and " + mappingColumnName + " = " + this.ParameterPrefix + mappingColumnName;
                   }
                   else
                   {
                       keys = keys + " or " + mappingColumnName + " = " + this.ParameterPrefix + mappingColumnName;
                   }
               }

               values.Add(mapping.Expression.ToMember().GetValue(eo));
           });

            builder.Append("delete from " + this.QuoteHeader);
            builder.Append(eo.TableName);
            builder.Append(this.QuoteFooter + " where ");
            builder.Append(keys);

            return this.ExecuteNonQuery(builder.ToString(), values.ToArray());
        }
        #endregion

        #region Find
        /// <summary>
        /// 从数据库中获取多个实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eo"></param>
        /// <returns></returns>
        public virtual IList<T> Find<T>(T eo) where T : EntityObject<T>, new()
        {
            StringBuilder builder = new StringBuilder();
            string conditions = string.Empty;
            List<object> values = new List<object>();

            var mappings = from s in eo.Mappings
                           where eo.ChangedProperties.Contains(s.ColumnName)
                           select s;

            mappings.Each(mapping =>
           {
               string mappingColumnName = mapping.ColumnName;

               if (conditions.IsNullOrEmpty())
               {
                   conditions = conditions + this.QuoteHeader + mappingColumnName + this.QuoteFooter + " = " + this.ParameterPrefix + mappingColumnName;
               }
               else
               {
                   conditions = conditions + " and " + this.QuoteHeader + mappingColumnName + this.QuoteFooter + " = " + this.ParameterPrefix + mappingColumnName;
               }

               values.Add(mapping.Expression.ToMember().GetValue(eo));
           });

            builder.Append("select * from " + this.QuoteHeader);
            builder.Append(eo.TableName);
            builder.Append(this.QuoteFooter);
            if (!string.IsNullOrEmpty(conditions))
            {
                builder.Append(" where ");
                builder.Append(conditions);
            }

            DataTable dt = this.ExecuteDataTable(builder.ToString(), values.ToArray());

            return dt.ToEntity<T>();
        }
        #endregion

        public virtual long GetIdentity()
        {
            return this.ExecuteScalar("select last_insert_id();").AsLong();
        }

        public DbParameter CreateParameter()
        {
            using (DbCommand command = this.connection.CreateCommand())
            {
                return command.CreateParameter();
            }
        }

        public void ExecuteStoredProcedure(string storedProcedureName, IList<DbParameter> parameters)
        {
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = storedProcedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = this.transaction;

                if (parameters != null && parameters.Count > 0)
                {
                    foreach (DbParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                command.ExecuteNonQuery();
            }
        }
    }
}
