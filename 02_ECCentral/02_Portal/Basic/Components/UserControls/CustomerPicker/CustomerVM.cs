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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.Basic.Components.UserControls.CustomerPicker
{
    public delegate void CustomerIsCheckedChange(bool ischecked, CustomerVM vm);
    public class CustomerVM : ModelBase
    {
        #region 界面展示专用属性
        public CustomerIsCheckedChange onIsCheckedChange;
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                base.SetValue("IsChecked", ref _isChecked, value);
                if (onIsCheckedChange != null)
                    onIsCheckedChange(value, this);
            }
        }


        /// <summary>
        /// 顾客电话，界面展示用
        /// </summary>
        public string PhoneUI
        {
            get
            {
                string seperator = "";
                if (!string.IsNullOrEmpty(this.Phone) && !string.IsNullOrEmpty(this.CellPhone))
                {
                    //如果电话和手机同时存在，用逗号分隔
                    seperator = ",";
                }
                return this.Phone + seperator + this.CellPhone;
            }
        }
        #endregion
        private int? _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get { return _sysNo; }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }

        private string _customerID;
        /// <summary>
        /// 顾客帐号
        /// </summary>
        public string CustomerID
        {
            get { return _customerID; }
            set
            {
                base.SetValue("CustomerID", ref _customerID, value);
            }
        }

        private string _customerName;
        /// <summary>
        /// 顾客名称
        /// </summary>
        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                base.SetValue("CustomerName", ref _customerName, value);
            }
        }

        private string _email;
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get { return _email; }
            set
            {
                base.SetValue("Email", ref _email, value);
            }
        }

        private string _phone;
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone
        {
            get { return _phone; }
            set
            {
                base.SetValue("Phone", ref _phone, value);
            }
        }

        private string _cellPhone;
        /// <summary>
        /// 手机
        /// </summary>
        public string CellPhone
        {
            get { return _cellPhone; }
            set
            {
                base.SetValue("CellPhone", ref _cellPhone, value);
            }
        }

        private DateTime? _registerTime;
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime? RegisterTime
        {
            get { return _registerTime; }
            set
            {
                base.SetValue("RegisterTime", ref _registerTime, value);
            }
        }

        private CustomerStatus? _status;
        /// <summary>
        /// 顾客状态
        /// </summary>
        public CustomerStatus? Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }
        private bool? _isEmailConfirmed;
        /// <summary>
        /// 邮箱已确认(是/否)
        /// </summary>
        public bool? IsEmailConfirmed
        {
            get { return _isEmailConfirmed; }
            set
            {
                base.SetValue("IsEmailConfirmed", ref _isEmailConfirmed, value);
            }
        }
        private CustomerRank? _rank;
        /// <summary>
        /// 客户等别
        /// </summary>
        public CustomerRank? Rank
        {
            get { return _rank; }
            set
            {
                base.SetValue("Rank", ref _rank, value);
            }
        }
        private VIPRank? _vIPRank;
        /// <summary>
        /// 客户VIP等级
        /// </summary>
        public VIPRank? VIPRank
        {
            get { return _vIPRank; }
            set
            {
                base.SetValue("VIPRank", ref _vIPRank, value);
            }
        }
        private bool _ispublicUser;
        /// <summary>
        /// 内部用户(是/否)
        /// </summary>
        public bool IspublicUser
        {
            get { return _ispublicUser; }
            set
            {
                base.SetValue("IspublicUser", ref _ispublicUser, value);
            }
        }
        private CustomerType? _customersType;
        /// <summary>
        /// 客户类型
        /// </summary>
        public CustomerType? CustomersType
        {
            get { return _customersType; }
            set
            {
                base.SetValue("CustomersType", ref _customersType, value);
            }
        }

        private string _FromLinkSource;

        public string FromLinkSource
        {
            get { return _FromLinkSource; }
            set
            {
                base.SetValue("FromLinkSource", ref _FromLinkSource, value);
            }
        }

        private string _Address;

        public string DwellAddress
        {
            get { return _Address; }
            set
            {
                base.SetValue("DwellAddress", ref _Address, value);
            }
        }

    }
}
