using System.Data;

namespace Utility
{
    public class QueryResult
    {
        /// <summary>
        /// 每页显示数据数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 符合条件的数据总数
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                if (this.ItemCount == 0) return 1;

                return (this.ItemCount - 1) / this.PageSize + 1;
            }
        }

        public string OrderBy { get; set; }

        /// <summary>
        /// 符合条件的数据
        /// </summary>
        public DataTable Data { get; set; }

        public QueryResult()
        {
        }

        public QueryResult(int pageSize, int pageIndex, string orderBy)
        {
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.OrderBy = orderBy;
        }
    }
}
