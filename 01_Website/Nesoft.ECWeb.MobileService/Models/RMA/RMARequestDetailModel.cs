using Nesoft.ECWeb.MobileService.Models.Common;
using Nesoft.ECWeb.MobileService.Models.MemberService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.RMA
{
    public class RMARequestDetailModel
    {
        /// <summary>
        /// 地区省市区
        /// </summary>
        public AreaViewModel AreaView { get; set; }

        /// <summary>
        /// 商家信息
        /// </summary>
        public StoreBasicInfoModel StoreBasicInfo { get; set; }

        /// <summary>
        /// 售后基本信息
        /// </summary>
        public RMARequestInfoModel RMARequestInfo { get; set; }
    }
}