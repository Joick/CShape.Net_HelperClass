using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// 分页查询用数据模型
    /// </summary>
    public class PageQueryResult
    {
        public PageQueryResult(int pageSize, int pageIndex, string orderBy)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            OrderBy = orderBy;
        }

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

        /// <summary>
        /// 排序规则
        /// </summary>
        public string OrderBy { get; set; }
    }

    /// <summary>
    /// 分页查询用数据模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageQueryResult<T> : PageQueryResult
    {
        public PageQueryResult(int pageSize, int pageIndex, string orderBy)
            : base(pageSize, pageIndex, orderBy)
        {
        }

        /// <summary>
        /// 实际数据
        /// </summary>
        public List<T> ResultData { get; set; }
    }
}
