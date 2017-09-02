using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 活动客户范围条件
    /// </summary>
    public class PSCustomerCondition
    {   
        /// <summary>
        /// 指定用户组设置
        /// </summary>
        public RelCustomerRank RelCustomerRanks { get; set; }

        /// <summary>
        /// 指定用户ID设置
        /// </summary>
        public RelCustomer RelCustomers { get; set; }

        /// <summary>
        /// 指定地区设置
        /// </summary>
        public RelArea RelAreas { get; set; }

        /// <summary>
        /// 是否需验证手机号
        /// </summary>
        public bool? NeedMobileVerification { get; set; }

        /// <summary>
        /// 是否需验证email.
        /// </summary>
        public bool? NeedEmailVerification { get; set; }

        /// <summary>
        /// 泰隆优选大使不可使用
        /// </summary>
        public bool? InvalidForAmbassador { get; set; }

    }

    /// <summary>
    /// 指定用户组设置
    /// </summary>
    public class RelCustomerRank  
    {
        /// <summary>
        /// 是否是包含关系
        /// </summary>
        public bool? IsIncludeRelation { get; set; }
        /// <summary>
        /// 顾客等级关系列表
        /// </summary>
        public List<SimpleObject> CustomerRankList { get; set; }
    }

    /// <summary>
    /// 指定用户ID设置
    /// </summary>
    public class RelCustomer  
    {
        /// <summary>
        /// 是否包含关系
        /// </summary>
        public bool? IsIncludeRelation { get; set; }
        /// <summary>
        /// 用户列表
        /// </summary>
        public List<CustomerAndSend> CustomerIDList { get; set; }
    }

    public class CustomerAndSend
    {      
        /// <summary>
        /// 发放状态：Y-已发放，N-未发放
        /// </summary>
        public string SendStatus { get; set; }
        /// <summary>
        /// 顾客系统编号
        /// </summary>
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 顾客编号
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 顾客姓名
        /// </summary>
        public string CustomerName { get; set; }

    }


    /// <summary>
    /// 指定地区设置
    /// </summary>
    public class RelArea  
    {
        /// <summary>
        /// 是否包含关系
        /// </summary>
        public bool? IsIncludeRelation { get; set; }

        /// <summary>
        /// 区域列表
        /// </summary>
        public List<SimpleObject> AreaList { get; set; }
    }

}
