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
using System.Collections.Generic;
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerPointsAddVM : ModelBase
    {

        /// <summary>
        /// 系统编号
        /// </summary>
        private int systemNumber;

        /// <summary>
        /// 顾客编号
        /// </summary>
        private int? customerSysNo;

        /// <summary>
        /// 顾客ID
        /// </summary>
        private string customerID;

        /// <summary>
        /// 顾客姓名
        /// </summary>
        private string customerName;

        /// <summary>
        /// 相关单据编号
        /// </summary>
        private string soSysNo;

        /// <summary>
        /// 系统账号
        /// </summary>
        private string newEggAccount;

        /// <summary>
        /// 增加积分数
        /// </summary>
        private string point;

        /// <summary>
        /// 原因
        /// </summary>
        private string memo;

        /// <summary>
        /// 说明
        /// </summary>
        private string note;

        private int createUserSysNo;

        private DateTime? createTime;

        private int confirmUserSysNo;

        private DateTime? confirmTime;

        private string confirmNote;

        private string showAuditStatus;

        private CustomerPointsAddRequestStatus status;

        private string ownByDepartment;

        private int pointLogType;

        private string pointLogTypeName;

        private int? validScore;

        private DateTime? pointExpiringDate;

        private int? accountValidScore;

        private string createUserName;

        private string lastEditUserName;

        private string createUserAndTime;

        private string confirmUserAndTime;

        private string statusName;

        private int? optType;

        private string vipCard;
        private string pM_GroupSysNos;
        private string pMGroupNames;

        private List<CustomerPointsAddItemVM> pointsItemList;

        private string inUser;
        private DateTime? inDate;
        private string editUser;
        private DateTime? editDate;


        /// <summary>
        /// 系统编号
        /// </summary>
        public int SystemNumber
        {
            get
            {
                return systemNumber;
            }
            set
            {
                base.SetValue("SystemNumber", ref systemNumber, value);
            }
        }

        /// <summary>
        /// 顾客编号
        /// </summary>
        [Validate(ValidateType.Interger)]
        public int? CustomerSysNo
        {
            get
            {
                return customerSysNo;
            }
            set
            {
                base.SetValue("CustomerSysNo", ref customerSysNo, value);
            }
        }

        /// <summary>
        /// 顾客ID
        /// </summary>
        [Validate(ValidateType.Required)]
        public string CustomerID
        {
            get
            {
                return customerID;
            }
            set
            {
                base.SetValue("CustomerID", ref customerID, value);
            }
        }

        /// <summary>
        /// 顾客姓名
        /// </summary>
        public string CustomerName
        {
            get
            {
                return customerName;
            }
            set
            {
                base.SetValue("CustomerName", ref customerName, value);
            }
        }

        /// <summary>
        /// 相关单据编号
        /// </summary>
        [Validate(ValidateType.Interger)]
        public string SOSysNo
        {
            get
            {
                return soSysNo;
            }
            set
            {
                base.SetValue("SOSysNo", ref soSysNo, value);
            }
        }

        /// <summary>
        /// 系统账号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string NewEggAccount
        {
            get
            {
                return newEggAccount;
            }
            set
            {
                base.SetValue("NewEggAccount", ref newEggAccount, value);
            }
        }

        /// <summary>
        /// 增加积分数
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(([1-9]\d{0,8})|(\-[1-9]\d{0,8}))$", ErrorMessage = "请输入有效的积分数")]
        public string Point
        {
            get
            {
                return point;
            }
            set
            {
                base.SetValue("Point", ref point, value);
            }
        }

        /// <summary>
        /// 原因
        /// </summary>
        public string Memo
        {
            get
            {
                return memo;
            }
            set
            {
                base.SetValue("Memo", ref memo, value);
            }
        }

        /// <summary>
        /// 说明
        /// </summary>
        [Validate(ValidateType.Required)]
        public string Note
        {
            get
            {
                return note;
            }
            set
            {
                base.SetValue("Note", ref note, value);
            }
        }

        public int CreateUserSysNo
        {
            get
            {
                return createUserSysNo;
            }
            set
            {
                base.SetValue("CreateUserSysNo", ref createUserSysNo, value);
            }
        }

        public DateTime? CreateTime
        {
            get
            {
                return createTime;
            }
            set
            {
                base.SetValue("CreateTime", ref createTime, value);
            }
        }

        public int ConfirmUserSysNo
        {
            get
            {
                return confirmUserSysNo;
            }
            set
            {
                base.SetValue("ConfirmUserSysNo", ref confirmUserSysNo, value);
            }
        }

        public DateTime? ConfirmTime
        {
            get
            {
                return confirmTime;
            }
            set
            {
                base.SetValue("ConfirmTime", ref confirmTime, value);
            }
        }

        public string ConfirmNote
        {
            get
            {
                return confirmNote;
            }
            set
            {
                base.SetValue("ConfirmNote", ref confirmNote, value);
            }
        }

        public string ShowAuditStatus
        {
            get
            {
                return showAuditStatus;
            }
            set
            {
                base.SetValue("ShowAuditStatus", ref showAuditStatus, value);
            }
        }

        public CustomerPointsAddRequestStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }
        [Validate(ValidateType.Required)]
        public string OwnByDepartment
        {
            get
            {
                return ownByDepartment;
            }
            set
            {
                base.SetValue("OwnByDepartment", ref ownByDepartment, value);
            }
        }

        public int PointLogType
        {
            get
            {
                return pointLogType;
            }
            set
            {
                base.SetValue("PointLogType", ref pointLogType, value);
            }
        }

        public string PointLogTypeName
        {
            get
            {
                return pointLogTypeName;
            }
            set
            {
                base.SetValue("PointLogTypeName", ref pointLogTypeName, value);
            }
        }

        public int? ValidScore
        {
            get
            {
                return validScore;
            }
            set
            {
                base.SetValue("ValidScore", ref validScore, value);
            }
        }


        public DateTime? PointExpiringDate
        {
            get
            {
                return pointExpiringDate;
            }
            set
            {
                base.SetValue("PointExpiringDate", ref pointExpiringDate, value);
            }
        }


        public int? AccountValidScore
        {
            get
            {
                return accountValidScore;
            }
            set
            {
                base.SetValue("AccountValidScore", ref accountValidScore, value);
            }
        }

        public string CreateUserName
        {
            get
            {
                return createUserName;
            }
            set
            {
                base.SetValue("CreateUserName", ref createUserName, value);
            }
        }

        public string LastEditUserName
        {
            get
            {
                return lastEditUserName;
            }
            set
            {
                base.SetValue("LastEditUserName", ref lastEditUserName, value);
            }
        }

        public string CreateUserAndTime
        {
            get
            {
                return createUserAndTime;
            }
            set
            {
                base.SetValue("CreateUserAndTime", ref createUserAndTime, value);
            }
        }

        public string ConfirmUserAndTime
        {
            get
            {
                return confirmUserAndTime;
            }
            set
            {
                base.SetValue("ConfirmUserAndTime", ref confirmUserAndTime, value);
            }
        }

        public string StatusName
        {
            get
            {
                return statusName;
            }
            set
            {
                base.SetValue("StatusName", ref statusName, value);
            }
        }

        public int? OptType
        {
            get
            {
                return optType;
            }
            set
            {
                base.SetValue("OptType", ref optType, value);
            }
        }

        public string VipCard
        {
            get
            {
                return vipCard;
            }
            set
            {
                base.SetValue("VipCard", ref vipCard, value);
            }
        }

        public string PM_GroupSysNos
        {
            get
            {
                return pM_GroupSysNos;
            }
            set
            {
                base.SetValue("PM_GroupSysNos", ref pM_GroupSysNos, value);
            }
        }

        public string PMGroupNames
        {
            get
            {
                return pMGroupNames;
            }
            set
            {
                base.SetValue("PMGroupNames", ref pMGroupNames, value);
            }
        }

        public List<CustomerPointsAddItemVM> PointsItemList
        {
            get
            {
                return pointsItemList;
            }
            set
            {
                base.SetValue("PointsItemList", ref pointsItemList, value);
            }
        }


        public string InUser
        {
            get
            {
                return inUser;
            }
            set
            {
                base.SetValue("InUser", ref inUser, value);
            }
        }
        public DateTime? InDate
        {
            get
            {
                return inDate;
            }
            set
            {
                base.SetValue("InDate", ref inDate, value);
            }
        }
        public string EditUser
        {
            get
            {
                return editUser;
            }
            set
            {
                base.SetValue("EditUser", ref editUser, value);
            }
        }
        public DateTime? EditDate
        {
            get
            {
                return editDate;
            }
            set
            {
                base.SetValue("EditDate", ref editDate, value);
            }
        }

        private Visibility _OwnByDepartmentVisibility;

        public Visibility OwnByDepartmentVisibility
        {
            get { return _OwnByDepartmentVisibility; }
            set
            {
                base.SetValue("OwnByDepartmentVisibility", ref _OwnByDepartmentVisibility, value);
            }
        }

        private Visibility _RequestItemVisibility;

        public Visibility RequestItemVisibility
        {
            get { return _RequestItemVisibility; }
            set
            {
                base.SetValue("RequestItemVisibility", ref _RequestItemVisibility, value);
            }
        }

        public CustomerPointsAddVM()
        {
            this.PointsItemList = new List<CustomerPointsAddItemVM>();
            OwnByDepartmentVisibility = Visibility.Collapsed;
            RequestItemVisibility = Visibility.Collapsed;
            ownByDepartment = null;
        }
    }

    public class CustomerPointsAddItemVM : ModelBase
    {

        private int? sysNo;
        private bool isCheckedItem;
        private int pointsAddRequestSysNo;
        private int? soSysNo;
        private int productSysNo;
        private string productID;
        private string briefName;
        private int quantity;
        private decimal? currentPrice;
        private string point;
        private string status;
        private string inUser;
        private DateTime? inDate;
        private string editUser;
        private DateTime? editDate;
        private string companyCode;


        public int? SysNo
        {
            get { return sysNo; }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }
        }
        public bool IsCheckedItem
        {
            get { return isCheckedItem; }
            set
            {
                base.SetValue("IsCheckedItem", ref isCheckedItem, value);
            }
        }
        public int PointsAddRequestSysNo
        {
            get { return pointsAddRequestSysNo; }
            set
            {
                base.SetValue("PointsAddRequestSysNo", ref pointsAddRequestSysNo, value);
            }
        }
        public int? SOSysNo
        {
            get { return soSysNo; }
            set
            {
                base.SetValue("SOSysNo", ref soSysNo, value);
            }
        }
        public int ProductSysNo
        {
            get { return productSysNo; }
            set
            {
                base.SetValue("ProductSysNo", ref productSysNo, value);
            }
        }
        public string ProductID
        {
            get { return productID; }
            set
            {
                base.SetValue("ProductID", ref productID, value);
            }
        }
        public string BriefName
        {
            get { return briefName; }
            set
            {
                base.SetValue("BriefName", ref briefName, value);
            }
        }
        public int Quantity
        {
            get { return quantity; }
            set
            {
                base.SetValue("Quantity", ref quantity, value);
            }
        }
        public decimal? CurrentPrice
        {
            get { return currentPrice; }
            set
            {
                base.SetValue("CurrentPrice", ref currentPrice, value);
            }
        }
        public string Point
        {
            get { return point; }
            set
            {
                base.SetValue("Point", ref point, value);
                if (string.IsNullOrEmpty(value))
                {
                    IsCheckedItem = false;
                }
                else
                {
                    IsCheckedItem = true;
                }
            }
        }
        public string Status
        {
            get { return status; }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }
        public string InUser
        {
            get { return inUser; }
            set
            {
                base.SetValue("InUser", ref inUser, value);
            }
        }
        public DateTime? InDate
        {
            get { return inDate; }
            set
            {
                base.SetValue("InDate", ref inDate, value);
            }
        }
        public string EditUser
        {
            get { return editUser; }
            set
            {
                base.SetValue("EditUser", ref editUser, value);
            }
        }
        public DateTime? EditDate
        {
            get { return editDate; }
            set
            {
                base.SetValue("EditDate", ref editDate, value);
            }
        }
        public string CompanyCode
        {
            get { return companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref companyCode, value);
            }
        }
    }
}
