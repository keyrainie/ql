//************************************************************************
// 用户名				泰隆优选
// 系统名				商家商品管理
// 子系统名		        商家商品管理QueryModels
// 作成者				Kevin
// 改版日				2012.6.8
// 改版内容				新建
//************************************************************************
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using System.Text.RegularExpressions;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductChannelMemberVM : ModelBase
    {
        /// <summary>
        /// 渠道会员价格表编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 渠道会员编号
        /// </summary>
        public int ChannelSysNO { get; set; }

        /// <summary>
        /// 渠道会员名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 泰隆优选价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 折扣价
        /// </summary>
        private string memberPrice;
        [Validate(ValidateType.Regex, @"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntDoubleMessage")]
        public string MemberPrice
        {
            get { return memberPrice; }
            set { base.SetValue("MemberPrice", ref  memberPrice, value); }
        }

        /// <summary>
        /// 折扣比例
        /// </summary>
        private string memberPricePercent;
        [Validate(ValidateType.Regex, @"^100$|^0$|^[1-9]\d{0,1}$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputInt")]
        public string MemberPricePercent
        {
            get { return memberPricePercent; }
            set
            {
                base.SetValue("MemberPricePercent", ref  memberPricePercent , value);
            }
        }
        
        /// <summary>
        /// 折扣1
        /// </summary>
        public string Discount1 
        { 
            get 
            {
                return
                    !String.IsNullOrEmpty(MemberPrice)
                    ? (CurrentPrice - decimal.Parse(MemberPrice)).ToString("0.00") : String.Empty;
            } 
        }

        /// <summary>
        /// 折扣2
        /// </summary>
        public string Discount2 
        {
            get
            {
                return
                    !String.IsNullOrEmpty(MemberPricePercent)
                    ? (CurrentPrice * (1 - decimal.Parse(MemberPricePercent) / 100)).ToString("0.00")
                    : String.Empty;
            }
        }
    }

    public class ProductChannelMemberEditVM : ModelBase
    {
        /// <summary>
        /// 渠道会员价格表编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 渠道会员名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 泰隆优选价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 折扣价
        /// </summary>
        private string memberPrice;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntDoubleMessage")]
        public string MemberPrice
        {
            get { return memberPrice; }
            set { base.SetValue("MemberPrice", ref  memberPrice, value); }
        }

        /// <summary>
        /// 折扣比例
        /// </summary>
        private string memberPricePercent;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^100$|^0$|^[1-9]\d{0,1}$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputInt")]
        public string MemberPricePercent
        {
            get { return memberPricePercent; }
            set { base.SetValue("MemberPricePercent", ref  memberPricePercent, value); }
        }
    }
}
