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
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.Models.CSTools
{
    public class OrderCheckItemVM : ModelBase
    {
        public OrderCheckItemVM()
        {
            this.CompanyCode = CPApplication.Current.CompanyCode;
        }
        public string CompanyCode { get; set; }
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }
        }

        private string referenceType;
        public string ReferenceType
        {
            get { return referenceType; }
            set
            {
                base.SetValue("ReferenceType", ref referenceType, value);
            }
        }

        private string referenceContent;
        public string ReferenceContent
        {
            get { return referenceContent; }
            set
            {
                base.SetValue("ReferenceContent", ref referenceContent, value);
            }
        }
        
        private string sysNos;
        public string SysNos
        {
            get { return sysNos; }
            set
            {
                base.SetValue("SysNos", ref sysNos, value);
            }
        }
        private string description;
        public string Description
        {
            get { return description; }
            set { base.SetValue("Description", ref description, value); }
        }


        private OrderCheckStatus? status;
        public OrderCheckStatus? Status
        {
            get { return IsCheck ? OrderCheckStatus.Valid : OrderCheckStatus.Invalid; ; }
            set
            {
                IsCheck = (OrderCheckStatus.Valid == value ? true : false);
            }
        }

        //public OrderCheckStatus? Status
        //{
        //    get { return IsCheck ? OrderCheckStatus.Valid : OrderCheckStatus.Invalid; ; }
        //    set
        //    {
        //        IsCheck = value == OrderCheckStatus.Valid ? true : false;
        //    }
        //}
        public bool isCheck;
        public bool IsCheck
        {
            get
            {
                return isCheck;
            }
            set
            {
                base.SetValue("IsCheck", ref isCheck, value);
            }
        }


        private int? createUserSysNo;
        public int? CreateUserSysNo
        {
            get { return createUserSysNo; }
            set { base.SetValue("CreateUserSysNo", ref createUserSysNo, value); }
        }

        private DateTime? createDate;
        public DateTime? CreateDate
        {
            get { return createDate; }
            set { base.SetValue("CreateDate", ref createDate, value); }
        }

        private DateTime? lastEditDate;
        public DateTime? LastEditDate
        {
            get { return lastEditDate; }
            set { base.SetValue("LastEditDate", ref lastEditDate, value); }
        }

        private int? lastEditUserSysNo;
        public int? LastEditUserSysNo
        {
            get { return lastEditUserSysNo; }
            set { base.SetValue("LastEditUserSysNo", ref lastEditUserSysNo, value); }
        }
        private int? channelSysNo;
        public int? ChannelSysNo
        {
            get { return channelSysNo; }
            set
            {
                base.SetValue("ChannelSysNo", ref channelSysNo, value);
            }
        }

        #region 关键字类型
        public bool IsCA { get; set; }
        public bool IsCN { get; set; }
        public bool IsCP { get; set; }
        #endregion

        #region 配送时间点
        public DateTime? ServiceTime_First { get; set; }
        public DateTime? ServiceTime_Second { get; set; }
        #endregion

        #region 产品及产品类别
        private bool _IsProductType = true;

        public bool IsProductType     //商品类型
        {
            get { return _IsProductType; }
            set
            {
                base.SetValue("IsProductType", ref _IsProductType, value);
            }
        }

        public bool IsProductSysNo { get; set; } //商品编号
        #endregion

        private string referenceTypeName;
        public string ReferenceTypeName
        {
            get { return referenceTypeName; }
            set
            {
                base.SetValue("ReferenceTypeName", ref referenceTypeName, value);
            }
        }

        #region
        public string LastEditUserName { get; set; }
        public string Category3Name { get; set; }

        private int? _Category3No;

        public int? Category3No
        {
            get { return _Category3No; }
            set { base.SetValue("Category3No", ref _Category3No, value); }
        }
        public string ShipTypeName { get; set; }
        private string _ProductID;

        public string ProductID
        {
            get { return _ProductID; }
            set { base.SetValue("ProductID", ref _ProductID, value); }
        }
        #endregion

        #region forUI
        public OrderCheckStatus? Operator
        {
            get
            {
                if (Status.Value == OrderCheckStatus.Invalid)
                    return OrderCheckStatus.Valid;
                else
                    return OrderCheckStatus.Invalid;
            }
        }
        #endregion
    }
}
