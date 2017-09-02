using System;
using System.Collections.Generic;
using ECCentral.BizEntity.MKT.Promotion;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models.Promotion
{
    public class ProductPayTypeVM : ModelBase
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public string _productID;

        public string ProductID
        {
            get { return _productID; }
            set { SetValue("ProductID", ref _productID, value); }
        }

        /// <summary>
        /// 商品ID列表
        /// </summary>
        public string _productIds;
        [Validate(ValidateType.Required)]
        public string ProductIds
        {
            get { return _productIds; }
            set { SetValue("ProductIds", ref _productIds, value); }
        }

        /// <summary>
        /// 支付方式ID
        /// </summary>
        public string _payTypeSysNo;

        public string PayTypeSysNo
        {
            get { return _payTypeSysNo; }
            set { SetValue("PayTypeSysNo", ref _payTypeSysNo, value); }
        }

        /// <summary>
        /// 支付方式ID
        /// </summary>
        public string _payTypeName;

        public string PayTypeName
        {
            get { return _payTypeName; }
            set { SetValue("PayTypeName", ref _payTypeName, value); }
        }

        /// <summary>
        /// 支付方式列表
        /// </summary>
        public string _payTypeIds;
        [Validate(ValidateType.Required)]
        public string PayTypeIds
        {
            get { return _payTypeIds; }
            set { SetValue("PayTypeIds", ref _payTypeIds, value); }
        }

        /// <summary>
        /// 开始生效时间
        /// </summary>
        public DateTime? _beginDate;
        [Validate(ValidateType.Required)]
        public DateTime? BeginDate
        {
            get { return _beginDate; }
            set { SetValue("BeginDate", ref _beginDate, value); }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? _endDate;
        [Validate(ValidateType.Required)]
        public DateTime? EndDate
        {
            get { return _endDate; }
            set { SetValue("EndDate", ref _endDate, value); }
        }

        /// <summary>
        /// 最后编辑人
        /// </summary>
        public string _editUser;

        public string EditUser
        {
            get { return _editUser; }
            set { SetValue("EditUser", ref _editUser, value); }
        }

        public List<PayTypeInfo> PayTypeList
        {
            get;
            set;
        }
    }
}
