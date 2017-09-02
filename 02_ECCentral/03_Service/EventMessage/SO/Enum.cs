using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.SO
{
    /// <summary>
    /// 订单拆分类型
    /// </summary>
    public enum SOSplitType
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 系统强制拆单
        /// </summary>
        Force = 1,
        /// <summary>
        /// 客户选择拆单
        /// </summary>
        Customer = 2,
        /// <summary>
        /// 被拆分子订单
        /// </summary>
        SubSO = 3
    }
    /// <summary>
    /// 订单欺诈类型
    /// </summary>
    public enum SOFPType
    {
        /// <summary>
        /// 已验证
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 可疑
        /// </summary>
        Suspect = 1,
        /// <summary>
        /// 串货
        /// </summary>
        ChuanHuo = 2,
        /// <summary>
        /// 炒货
        /// </summary>
        ChaoHuo = 3
    }

    public enum SOType
    {
        /// <summary>
        /// 0:表示普通订单
        /// </summary>
        General = 0,
        /// <summary>
        /// 7:团购订单
        /// </summary>
        GroupBuy = 7,
        /// <summary>
        /// 咨询行程申请单订单
        /// </summary>
        TravelRequest = 11,
        /// <summary>
        /// 电子套餐卡订单
        /// </summary>
        EPackageCard = 12,
        /// <summary>
        /// 实物套餐卡订单
        /// </summary>
        PPackageCard = 13,
        /// <summary>
        /// 激活套餐卡生成的订单
        /// </summary>
        Package = 14,
        /// <summary>
        /// 邮游卡订单
        /// </summary>
        YYCard = 15
    }
}
