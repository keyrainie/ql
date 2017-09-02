using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Common
{
    [Serializable]
    public class ViewQueryResult<T>
    {
        public ViewQueryResult()
        {
            PagingInfo = new ViewPagingInfo();
        }

        public ViewQueryResult(List<T> resultList, ViewPagingInfo pagingInfo)
        {
            this.ResultList = resultList;
            this.PagingInfo = pagingInfo;
        }

        public List<T> ResultList { get; set; }

        public ViewPagingInfo PagingInfo { get; set; }
    }


    [Serializable]
    public class ViewPagingInfo
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页显示的记录数量
        /// </summary>
        public int PageSize { get; set; }




        /// <summary>
        /// 总记录数量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                if (TotalCount > 0 && PageSize > 0)
                {
                    return TotalCount % PageSize == 0 ? TotalCount / PageSize : TotalCount / PageSize + 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 分页链接切割的段落数量
        /// </summary>
        public int PageSectionCount
        {
            get
            {
                //至少有一个分页链接段
                int result = 1;
                //pageIndex从0开始
                int currentIndex = PageIndex + 1;
                if (currentIndex > DisplayPageCount)
                {
                    int num = (currentIndex % DisplayPageCount);
                    if (num > 0)
                    {
                        result = ((currentIndex / DisplayPageCount) * DisplayPageCount) + 1;
                    }
                    else
                    {
                        result = (((currentIndex / DisplayPageCount) - 1) * DisplayPageCount) + 1;
                    }
                }
                return result;
            }

        }
        /// <summary>
        /// 分页链接显示的个数,设为10个
        /// </summary>
        public int DisplayPageCount
        {
            get { return 5; }
        }
    }
}
