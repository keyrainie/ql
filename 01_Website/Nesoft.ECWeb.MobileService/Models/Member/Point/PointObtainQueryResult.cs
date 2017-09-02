using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public class PointObtainQueryResult
    {
        /// <summary>
        /// 积分总数
        /// </summary>
        public int TotalScores { get; set; }

        /// <summary>
        /// 积分获取记录分页查询结果
        /// </summary>
        public List<PointObtainViewModel> ResultList { get; set; }

        public PageInfo PageInfo { get; set; }
    }
}