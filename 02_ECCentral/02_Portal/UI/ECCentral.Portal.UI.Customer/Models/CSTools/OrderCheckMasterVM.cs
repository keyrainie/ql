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
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.UI.Customer.Models.CSTools
{
    public class OrderCheckMasterVM : ModelBase
    {
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }
        }
        private string checkType;
        public string CheckType
        {
            get { return checkType; }
            set
            {
                base.SetValue("CheckType", ref checkType, value);
            }
        }
        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                base.SetValue("Description", ref description, value);
            }
        }

        public OrderCheckStatus? Status
        {
            get { return IsCheck ? OrderCheckStatus.Valid : OrderCheckStatus.Invalid; ; }
            set
            {
                IsCheck = (OrderCheckStatus.Valid == value ? true : false);
            }
        }
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


        private int? creatUserSysNo;
        public int? CreatUserSysNo
        {
            get { return creatUserSysNo; }
            set
            {
                base.SetValue("CreatUserSysNo", ref creatUserSysNo, value);
            }
        }

        private DateTime? creatDate;
        public DateTime? CreatDate
        {
            get { return creatDate; }
            set
            {
                base.SetValue("CreatDate", ref creatDate, value);
            }
        }

        private int? lastEditUserSysNo;
        public int? LastEditUserSysNo
        {
            get { return lastEditUserSysNo; }
            set
            {
                base.SetValue("LastEditUserSysNo", ref lastEditUserSysNo, value);
            }
        }

        private DateTime? lastEditDate;
        public DateTime? LastEditDate
        {
            get { return lastEditDate; }
            set
            {
                base.SetValue("LastEditDate", ref lastEditDate, value);
            }
        }
        /// <summary>
        /// 渠道编号
        /// </summary>
        private int? channelSysNo;
        public int? ChannelSysNo
        {
            get { return channelSysNo; }
            set
            {
                base.SetValue("ChannelSysNo", ref channelSysNo, value);
            }
        }

        public string LastEditUserName
        {
            get;
            set;
        }
        public Visibility HyperlinkButtonVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(Url))
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }

        public Visibility TextBlockVisibility
        {
            get
            {
                if (!string.IsNullOrEmpty(Url))
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }

        public string Url
        {
            get
            {
                switch (CheckType.ToUpper().Trim())
                {
                    case "KW":
                        return ConstValue.Customer_OrderCheckKeywordSetUrl;

                    case "AMT":
                        return ConstValue.Customer_OrderCheckAmountSetUrl;

                    case "AT":
                        return ConstValue.Customer_OrderCheckAutoCheckTimeSetUrl;

                    case "ST":
                        return ConstValue.Customer_OrderCheckShippingTypeSetUrl;

                    case "PT":
                        return ConstValue.Customer_OrderCheckPayTypeSetUrl;

                    case "FP":
                        return ConstValue.Customer_OrderCheckFPSetUrl;

                    case "CT":
                        return ConstValue.Customer_OrderCheckCustomerTypeSetUrl;

                    case "DT":
                        return ConstValue.Customer_OrderCheckDistributionServiceTypeSetUrl;

                    case "PC3":
                        return ConstValue.Customer_OrderCheckProductAnd3CSetUrl;

                    case "PC":
                        return ConstValue.Customer_OrderCheckPCSetUrl;

                    default:
                        return string.Empty;

                }
            }
        }
    }
}
