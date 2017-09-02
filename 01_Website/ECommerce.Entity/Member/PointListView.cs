using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    public class PointListView
    {
        /// <summary>
        /// 积分明细列表
        /// </summary>
        public QueryResult<CustomerPointInfo> PointList { get; set; }
        /// <summary>
        /// 积分获得列表
        /// </summary>
        public QueryResult<PointObtainInfo> PointObtainList { get; set; }
        /// <summary>
        /// 积分消费列表
        /// </summary>
        public QueryResult<PointConsumeInfo> PointConsumeList { get; set; }

        /// <summary>
        /// 积分总和
        /// </summary>
        public int PointTotal { get; set; }
    }
}
