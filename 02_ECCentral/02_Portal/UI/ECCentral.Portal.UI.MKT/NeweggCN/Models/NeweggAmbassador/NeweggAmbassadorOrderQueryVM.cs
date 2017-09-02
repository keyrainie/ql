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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.SO;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.MKT.Models
{
    public class NeweggAmbassadorOrderQueryVM : ModelBase
    {
        public NeweggAmbassadorOrderQueryVM()
        {
            this._UserTypePairList = EnumConverter.GetKeyValuePairs<UserType>();
            this._SOStatusPairList = EnumConverter.GetKeyValuePairs<SOStatus>();
            this._SOStatusPairList.Insert(0, new KeyValuePair<SOStatus?, string>(null, ResCommonEnum.Enum_All));

            this._PointStatusPairList = EnumConverter.GetKeyValuePairs<PointStatus>();
            this._PointStatusPairList.Insert(0, new KeyValuePair<PointStatus?, string>(null, ResCommonEnum.Enum_All));
            
        }

        private List<KeyValuePair<UserType?, string>> _UserTypePairList;

        /// <summary>
        /// 所有用户类型。
        /// </summary>
        public List<KeyValuePair<UserType?, string>> UserTypePairList
        {
            get
            {
                return _UserTypePairList;
            }
        }

        private List<KeyValuePair<SOStatus?, string>> _SOStatusPairList;

        /// <summary>
        /// 所有订单状态。
        /// </summary>
        public List<KeyValuePair<SOStatus?, string>> SOStatusPairList
        {
            get
            {
                return _SOStatusPairList;
            }
        }

        private List<KeyValuePair<PointStatus?, string>> _PointStatusPairList;

        /// <summary>
        /// 所有积分发放状态。
        /// </summary>
        public List<KeyValuePair<PointStatus?, string>> PointStatusPairList
        {
            get
            {
                return _PointStatusPairList;
            }
        }

        private int? _BigAreaSysNo;

        /// <summary>
        /// 大区
        /// </summary>
        public int? BigAreaSysNo
        {
            get { return _BigAreaSysNo; }
            set { base.SetValue("BigAreaSysNo", ref _BigAreaSysNo, value); }
        }

        private UserType _SelectedUserType;

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType SelectedUserType
        {
            get { return _SelectedUserType; }
            set { base.SetValue("SelectedUserType", ref _SelectedUserType, value); }
        }

        private string _CustomerID;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string CustomerID
        {
            get { return _CustomerID; }
            set { base.SetValue("CustomerID", ref _CustomerID, value); }
        }

        private string _OrderTimeFrom;

        /// <summary>
        /// 下单时间从。
        /// </summary>
        public string OrderTimeFrom
        {
            get { return _OrderTimeFrom; }
            set { base.SetValue("OrderTimeFrom", ref _OrderTimeFrom, value); }
        }

        private string _OrderTimeTo;

        /// <summary>
        /// 下单时间到。
        /// </summary>
        public string OrderTimeTo
        {
            get { return _OrderTimeTo; }
            set { base.SetValue("OrderTimeTo", ref _OrderTimeTo, value); }
        }

        private string _PointTimeFrom;

        /// <summary>
        /// 积分发放时间从。
        /// </summary>
        public string PointTimeFrom
        {
            get { return _PointTimeFrom; }
            set { base.SetValue("PointTimeFrom", ref _PointTimeFrom, value); }
        }

        private string _PointTimeTo;

        /// <summary>
        /// 积分发放时间到。
        /// </summary>
        public string PointTimeTo
        {
            get { return _PointTimeTo; }
            set { base.SetValue("PointTimeTo", ref _PointTimeTo, value); }
        }

        private SOStatus? _SelectedSOStatus;

        /// <summary>
        /// 订单状态。
        /// </summary>
        public SOStatus? SelectedSOStatus
        {
            get { return _SelectedSOStatus; }
            set { base.SetValue("SelectedSOStatus", ref _SelectedSOStatus, value); }
        }

        private string _OrderSysNo;

        /// <summary>
        /// 订单编号
        /// </summary>
         [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,8}$", ErrorMessage = "请输入正整数！")]
        public string OrderSysNo
        {
            get { return _OrderSysNo; }
            set { base.SetValue("OrderSysNo", ref _OrderSysNo, value); }
        }

        private string _CreateSOCustomerID;

        /// <summary>
        /// 下单ID
        /// </summary>
        public string CreateSOCustomerID
        {
            get { return _CreateSOCustomerID; }
            set { base.SetValue("CreateSOCustomerID", ref _CreateSOCustomerID, value); }
        }

        private PointStatus? _SelectedPointStatus;

        /// <summary>
        /// 积分发放状态。
        /// </summary>
        public PointStatus? SelectedPointStatus
        {
            get { return _SelectedPointStatus; }
            set { base.SetValue("SelectedPointStatus", ref _SelectedPointStatus, value); }
        }

    }
}
