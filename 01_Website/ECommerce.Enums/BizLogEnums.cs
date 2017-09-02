using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ECommerce.Enums
{
    public enum EBizLogType
    {
        /// <summary>
        /// 商品-类别管理
        /// </summary>
        [Description("商品-类别管理")]
        ProductCategory  = 101,
        /// <summary>
        /// 商品-商品管理
        /// </summary>
        [Description("商品-商品管理")]
        ProductMgt = 102,

        /// <summary>
        /// 销售-订单管理
        /// </summary>
        [Description("销售-订单管理")]
        OrderMgt = 201,
        /// <summary>
        /// 销售-退单管理
        /// </summary>
        [Description("销售-退单管理")]
        OrderRMA = 202,
        /// <summary>
        /// 销售-退款管理
        /// </summary>
        [Description("销售-退款管理")]
        OrderRefund = 203,
        /// <summary>
        /// 销售-询价管理
        /// </summary>
        [Description("销售-询价管理")]
        OrderEnquiry = 204,
        /// <summary>
        /// 销售-供求管理
        /// </summary>
        [Description("销售-供求管理")]
        OrderSupplyDemand = 205,

        /// <summary>
        /// 商家-企业信息
        /// </summary>
        [Description("商家-企业信息")]
        MerchantMgt = 301,
        /// <summary>
        /// 商家-账号维护
        /// </summary>
        [Description("商家-账号维护")]
        MerchantAcct = 302,

        /// <summary>
        /// 内容-栏目管理
        /// </summary>
        [Description("内容-栏目管理")]
        WebContentCategory = 401,
        /// <summary>
        /// 内容-内容管理
        /// </summary>
        [Description("内容-内容管理")]
        WebContentTopic = 402,
        /// <summary>
        /// 广告-Banner推荐
        /// </summary>
        [Description("广告-Banner推荐")]
        WebContentBanner = 411,
        /// <summary>
        /// 反馈-建议反馈
        /// </summary>
        [Description("反馈-建议反馈")]
        WebAdvise = 421,

        /// <summary>
        /// 顾客-顾客管理
        /// </summary>
        [Description("顾客-顾客管理")]
        CustomerMgt = 501,
        /// <summary>
        /// 顾客-信息管理
        /// </summary>
        [Description("顾客-信息管理")]
        CustomerMessge = 502,

        /// <summary>
        /// 系统-用户管理
        /// </summary>
        [Description("系统-用户管理")]
        SystemUser = 901,
        /// <summary>
        /// 系统-角色管理
        /// </summary>
        [Description("系统-角色管理")]
        SystemRole = 902,
        /// <summary>
        /// 系统-角色权限管理
        /// </summary>
        [Description("系统-角色权限管理")]
        SystemRoleAuth = 903,
    }
}
