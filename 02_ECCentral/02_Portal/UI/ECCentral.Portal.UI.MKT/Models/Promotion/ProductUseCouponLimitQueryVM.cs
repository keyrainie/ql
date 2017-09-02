using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ProductUseCouponLimitQueryVM : ModelBase
    {
        public ProductUseCouponLimitQueryVM()
        {
            this.StatusList = EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.All);
            this.CouponLimitTypeList = EnumConverter.GetKeyValuePairs<CouponLimitType>(EnumConverter.EnumAppendItemType.All);
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        private string productSysNo;
        public string ProductSysNo 
        {
            get { return productSysNo; }
            set { SetValue("ProductSysNo", ref productSysNo, value); }
        }
        /// <summary>
        /// 商品ID
        /// </summary>
        private string productId;
        public string ProductId
        {
            get { return productId; }
            set { SetValue("ProductId", ref productId, value); }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus? Status { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public CouponLimitType? CouponLimitType { get; set; }

        public List<KeyValuePair<ADStatus?, string>> StatusList { get; set; }
        public List<KeyValuePair<CouponLimitType?, string>> CouponLimitTypeList { get; set; }

    }
}
