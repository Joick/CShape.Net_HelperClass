using System;
using System.Linq.Expressions;

namespace Utility
{
    [Serializable]
    public class ColumnMapping<T> where T : EntityObject<T>, new()
    {
        [NonSerialized]
        private Expression<Func<T, object>> expression;
        internal Expression<Func<T, object>> Expression
        {
            get { return this.expression; }
            set { this.expression = value; }
        }

        internal string PropertyName { get; set; }

        internal string ColumnName { get; set; }

        internal bool IsPrimaryKey { get; set; }

        internal bool IsAutoIncrease { get; set; }

        /// <summary>
        /// 设置属性映射的字段名
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public ColumnMapping<T> Column(string columnName)
        {
            this.ColumnName = columnName;
            return this;
        }

        /// <summary>
        /// 指定字段是否为主键字段
        /// </summary>
        /// <returns></returns>
        public ColumnMapping<T> PrimaryKey()
        {
            this.IsPrimaryKey = true;
            return this;
        }

        /// <summary>
        /// 指定字段是否为自动增长字段
        /// </summary>
        /// <returns></returns>
        public ColumnMapping<T> AutoIncrease()
        {
            this.IsAutoIncrease = true;
            return this;
        }
    }
}
