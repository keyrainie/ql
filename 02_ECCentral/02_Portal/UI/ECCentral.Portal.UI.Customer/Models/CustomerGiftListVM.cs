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
using System.ComponentModel.DataAnnotations;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerGiftListVM : ModelBase
    {
        private static Type _customerGiftStatusType = typeof(CustomerGiftStatus);
        #region 界面展示专用属性
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                base.SetValue("IsChecked", ref _isChecked, value);
            }
        }

        /// <summary>
        /// 状态表示，将代码转换成描述
        /// </summary>
        public string StatusDesc
        {
            get
            {
                return EnumConverter.GetDescription(this.Status,_customerGiftStatusType);
            }
        }

        #endregion

        private int _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo
        {
            get { return _sysNo; }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }

        private int _customerSysNo;
        /// <summary>
        /// 顾客系统编号
        /// </summary>
        public int CustomerSysNo
        {
            get { return _customerSysNo; }
            set
            {
                base.SetValue("CustomerSysNo", ref _customerSysNo, value);
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
        private int _productSysNo;
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo
        {
            get { return _productSysNo; }
            set
            {
                base.SetValue("ProductSysNo", ref _productSysNo, value);
            }
        }
        private string _productID;
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID
        {
            get { return _productID; }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
            }
        }
        private string _productName;
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName
        {
            get { return _productName; }
            set
            {
                base.SetValue("ProductName", ref _productName, value);
            }
        }
        private int _quantity;
        /// <summary>
        /// 奖品数量
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                base.SetValue("Quantity", ref _quantity, value);
            }
        }
        private int _status;
        /// <summary>
        /// 奖品信息状态
        /// </summary>
        public int Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);

                //更新UI上的状态描述
                RaisePropertyChanged("StatusDesc");
            }
        }
        private int? _soSysNo;
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int? SoSysNo
        {
            get 
            {
                if (_soSysNo == 0)
                    return null;
                return _soSysNo;
            }
            set
            {
                base.SetValue("SoSysNo", ref _soSysNo, value);
            }
        }
        private DateTime _createDate;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get { return _createDate; }
            set
            {
                base.SetValue("CreateDate", ref _createDate, value);
            }
        }
        private string _createUserName;
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName
        {
            get { return _createUserName; }
            set
            {
                base.SetValue("CreateUserName", ref _createUserName, value);
            }
        }     
    }
}
