using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class CountdownQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID { get; set; }

        public int? SysNumber
        {
            get;
            set;
        }

        public string PromotionTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 是否促销计划
        /// </summary>
        public int? IsPromotionSchedule
        {
            get;
            set;
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间区间从
        /// </summary>
        public DateTime? CreateFromTime
        {
            get;
            set;
        }
        /// <summary>
        /// 创建时间区间到
        /// </summary>
        public DateTime? CreateToTime
        {
            get;
            set;
        }

        /// <summary>
        /// 促销开始时间区间从
        /// </summary>
        public DateTime? CountdownFromStartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 促销开始时间区间到
        /// </summary>
        public DateTime? CountdownToStartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 促销结束时间区间从
        /// </summary>
        public DateTime? CountdownFromEndTime
        {
            get;
            set;
        }

        /// <summary>
        /// 促销结束时间区间到
        /// </summary>
        public DateTime? CountdownToEndTime
        {
            get;
            set;
        }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID
        {
            get;
            set;
        }


        /// <summary>
        /// 单据状态
        /// </summary>
        public CountdownStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 首页限时抢购
        /// </summary>
        public int? IsHomePageShow
        {
            get;
            set;
        }

        /// <summary>
        /// 专区显示
        /// </summary>
        public int? IsCountDownAreaShow
        {
            get;
            set;
        }

        public int? IsSecondKill { get; set; }

        /// <summary>
        /// 是否组团
        /// </summary>
        public string IsGroupOn
        {
            get;
            set;
        }

        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int? MerchantSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 分类页面显示
        /// </summary>
        public string IsC1Show { get; set; }
        public string IsC2Show { get; set; }

        //权限
        public int PMRole { get; set; }
        //当前登录名称
        public string PMUserName { get; set; }
    }
}
