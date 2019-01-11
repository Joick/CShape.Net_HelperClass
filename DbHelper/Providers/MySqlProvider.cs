using MySql.Data.MySqlClient;
using System;
using System.Data.Common;

namespace Utility
{
    internal class MySqlProvider : ProviderBase
    {
        public MySqlProvider(string connectionString)
            : base(connectionString)
        {
        }

        protected override DbConnection CreateConnection()
        {
            return new MySqlConnection();
        }

        protected override DbDataAdapter CreateDataAdapter()
        {
            return new MySqlDataAdapter();
        }

        protected override string CreatePageSQL(string sql, QueryResult qr)
        {
            return string.Format("select * from ({0}) T order by {1} limit {2}, {3}", sql, qr.OrderBy, (qr.PageIndex - 1) * qr.PageSize, qr.PageSize);
        }

        public override string ParameterPrefix
        {
            get { return "@"; }
        }

        public override string QuoteHeader
        {
            get { return "`"; }
        }

        public override string QuoteFooter
        {
            get { return "`"; }
        }

    }
}
