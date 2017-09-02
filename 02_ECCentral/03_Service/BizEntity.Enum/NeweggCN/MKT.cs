using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ECCentral.BizEntity.MKT
{
    #region 泰隆优选大使公告
    /// <summary>
    /// 泰隆优选大使公告状态显示。
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.NeweggCN.Resources.ResMKTEnum_NeweggCN")]
    public enum AmbassadorNewsStatus
    {
        /// <summary>
        /// 不显示
        /// </summary>
        UnDisplay,

        /// <summary>
        /// 显示
        /// </summary>
        Display
    }
    #endregion

    #region 泰隆优选大使

    [Description("ECCentral.BizEntity.Enum.NeweggCN.Resources.ResMKTEnum_NeweggCN")]
    public enum AmbassadorStatus
    {
        /// <summary>
        /// 已激活,2
        /// </summary>
        Active,

        /// <summary>
        /// 未激活,1
        /// </summary>
        UnActive,

        /// <summary>
        /// 已取消,3
        /// </summary>
        Canceled
    }

    /// <summary>
    /// 用户类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.NeweggCN.Resources.ResMKTEnum_NeweggCN")]
    public enum UserType
    {
        /// <summary>
        /// 泰隆优选大使，0
        /// </summary>
        NeweggAmbassador,

        /// <summary>
        /// 泰隆优选员工，4
        /// </summary>
        NeweggEmployee
    }

    /// <summary>
    /// 用户类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.NeweggCN.Resources.ResMKTEnum_NeweggCN")]
    public enum PointStatus
    {
        /// <summary>
        /// 未发放
        /// </summary>
        NotSended,


        /// <summary>
        /// 已发放，1
        /// </summary>
        HasSended

    }

    /// <summary>
    /// 用户类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.NeweggCN.Resources.ResMKTEnum_NeweggCN")]
    public enum PayStatus
    {
        /// <summary>
        /// 已作废
        /// </summary>
        Invalid = -1,

        /// <summary>
        /// 待确认
        /// </summary>
        ForConfirm = 0,


        /// <summary>
        /// 已确认，1
        /// </summary>
        Confirmed = 1

    }


    #endregion

    #region 单品/套餐限购

    /// <summary>
    /// 限购规则类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.NeweggCN.Resources.ResMKTEnum_NeweggCN")]
    public enum LimitType
    {
        /// <summary>
        /// 单品
        /// </summary>
        SingleProduct = 0,

        /// <summary>
        /// 套餐
        /// </summary>
        Combo = 1
    }

    /// <summary>
    /// 限购规则状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.NeweggCN.Resources.ResMKTEnum_NeweggCN")]
    public enum LimitStatus
    {
        /// <summary>
        /// 无效
        /// </summary>
        Invalid=0,

        /// <summary>
        /// 有效
        /// </summary>
        Valid = 1
    }

    #endregion

    #region 销售立减规则

    /// <summary>
    /// 销售立减规则类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.NeweggCN.Resources.ResMKTEnum_NeweggCN")]
    public enum SaleDiscountRuleType
    {
        /// <summary>
        /// 限定金额
        /// </summary>
        AmountRule = 0,

        /// <summary>
        /// 限定数量
        /// </summary>
        QtyRule = 1
    }

    /// <summary>
    /// 销售立减规则状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.NeweggCN.Resources.ResMKTEnum_NeweggCN")]
    public enum SaleDiscountRuleStatus
    {
        /// <summary>
        /// 无效
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// 有效
        /// </summary>
        Valid = 1
    }

    #endregion
}
