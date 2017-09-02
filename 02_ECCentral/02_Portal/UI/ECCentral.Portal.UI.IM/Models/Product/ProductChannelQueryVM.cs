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
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductChannelQueryVM : ModelBase
    {

        public List<ChannelInfo> ChannelList { get; set; }


        public List<KeyValuePair<string, string>> ChannelProductStatusList { get; set; }

        public string Status { get; set; }



        public ProductChannelQueryVM()
        {
            List<KeyValuePair<string, string>> statusList = new List<KeyValuePair<string, string>>();

            statusList.Add(new KeyValuePair<string, string>("", ResCategoryKPIMaintain.SelectTextAll));
            statusList.Add(new KeyValuePair<string, string>("A", ResCategoryKPIMaintain.SelectTextValid));
            statusList.Add(new KeyValuePair<string, string>("D", ResCategoryKPIMaintain.SelectTextInvalid));
            this.ChannelProductStatusList = statusList;
        }

        /// <summary>
        /// 渠道编号
        /// </summary>
        public int ChannelSysNo { get; set; }

        /// <summary>
        /// 一级类
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 二级类
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 三级类
        /// </summary>
        public int? C3SysNo { get; set; }


        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        private string productSysNo;
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,8}$", ErrorMessageResourceType=typeof(ResBrandQuery),ErrorMessageResourceName="Error_ValidateIntHint")]
        public string ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        /// <summary>
        /// 渠道商品编号对于数据库SynProductID
        /// </summary>
        public string ChannelProductID { get; set; }

        /// <summary>
        /// 淘宝SKU码
        /// </summary>
        public string TaobaoSKU { get; set; }


    }

    public class ProductChannelPeriodPriceQueryVM : ModelBase
    {     
        /// <summary>
        /// 一级类
        /// </summary>
        public int? ChannelProductSysNo { get; set; }

    }
}
