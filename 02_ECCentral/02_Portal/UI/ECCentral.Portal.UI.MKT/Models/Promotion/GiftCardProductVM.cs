using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class GiftCardProductVM : ModelBase
    {
        public GiftCardProductVM()
        {
            this.RelationProducts = new ObservableCollection<GiftVoucherProductRelationVM>();
        }

        private int sysNo;
        public int SysNo
        {
            get { return sysNo; }
            set
            {
                sysNo = value;
                RaisePropertyChanged("AuditBtnVisibility");
            }
        }

        private string price;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(-)?\d+(\.\d\d)?$", ErrorMessage = "只能输入两位有效正数")]
        public string Price
        {
            get { return price; }
            set { SetValue("Price", ref price, value); }
        }

        private string productSysNo;
        [Validate(ValidateType.Required)]
        public string ProductSysNo
        {
            get { return productSysNo; }
            set { SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private string productID;
        [Validate(ValidateType.Required)]
        public string ProductID
        {
            get { return productID; }
            set { SetValue("ProductID", ref productID, value); }
        }

        

        public ObservableCollection<GiftVoucherProductRelationVM> RelationProducts { get; set; }

        #region UI
        public Visibility AuditBtnVisibility
        {
            get
            {
                return SysNo > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion
    }

    public class GiftVoucherProductRelationVM : ModelBase
    {
        private bool saleInWeb;
        public bool SaleInWeb
        {
            get { return saleInWeb; }
            set { SetValue("SaleInWeb", ref saleInWeb, value); }
        }

        public GiftVoucherType GiftVoucherType
        {
            get
            {
                return SaleInWeb ? GiftVoucherType.SaleVoucherProduct : GiftVoucherType.NoSaleVoucherProduct;
            }
        }

        public int? SysNo { get; set; }

        public bool? IsUIClickDel { get; set; }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetValue("IsChecked", ref isChecked, value); }
        }

        private int productSysNo;
        public int ProductSysNo
        {
            get { return productSysNo; }
            set { SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private string _productID;
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID
        {
            get { return _productID; }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
            }
        }

        private ProductInventoryType? inventoryType;
        /// <summary>
        /// 库存模式
        /// </summary>
        public ProductInventoryType? InventoryType
        {
            get { return inventoryType; }
            set { base.SetValue("InventoryType", ref inventoryType, value); }
        }

        private string _IsConsign;
        public string IsConsign
        {
            get { return _IsConsign; }
            set { SetValue("IsConsign", ref _IsConsign, value); }
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

        private int? _availableQty;
        /// <summary>
        /// 可用库存
        /// </summary>
        public int? AvailableQty
        {
            get { return _availableQty; }
            set
            {
                base.SetValue("AvailableQty", ref _availableQty, value);
            }
        }

        private ProductStatus? _productStatus;
        /// <summary>
        /// 商品状态
        /// </summary>
        public ProductStatus? ProductStatus
        {
            get { return _productStatus; }
            set
            {
                base.SetValue("ProductStatus", ref _productStatus, value);
            }
        }

        private ProductType? _productType;
        /// <summary>
        /// 商品类型
        /// </summary>
        public ProductType? ProductType
        {
            get { return _productType; }
            set
            {
                base.SetValue("ProductType", ref _productType, value);
            }
        }

        private GVRReqType? _RequestType;
        /// <summary>
        /// 请求类型
        /// </summary>
        public GVRReqType? Type
        {
            get { return _RequestType; }
            set { 
                SetValue("Type", ref _RequestType, value);
                RaisePropertyChanged("IsEnable");
            }
        }

        private GVRReqAuditStatus? _AuditStatus;
        /// <summary>
        /// 请求状态
        /// </summary>
        public GVRReqAuditStatus? AuditStatus
        {
            get { return _AuditStatus; }
            set { SetValue("AuditStatus", ref _AuditStatus, value); }
        }

        private GiftVoucherRelateProductStatus relStatus;
        public GiftVoucherRelateProductStatus RelationStatus
        {
            get { return relStatus; }
            set { SetValue("RelationStatus", ref relStatus, value); }
        }

        public ObservableCollection<GiftVoucherProductRelationReqVM> RequestLogs { get; set; }

        #region UI
        public bool IsEnable
        {
            get
            {
                if (IsUIClickDel.HasValue&&IsUIClickDel.Value)
                {
                    return false;
                }
                else
                {
                    return (this.AuditStatus == GVRReqAuditStatus.AuditWaitting) ? false : true;
                }
            }
        }
        #endregion
    }

    public class GiftVoucherProductRelationReqVM : ModelBase
    {
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetValue("IsChecked", ref isChecked, value); }
        }

        private int sysNo;
        public int SysNo
        {
            get { return sysNo; }
            set { SetValue("SysNo", ref sysNo, value); }
        }

        private int relationSysNo;
        public int RelationSysNo
        {
            get { return relationSysNo; }
            set { SetValue("RelationSysNo", ref relationSysNo, value); }
        }

        private GVRReqType? _RequestType;
        /// <summary>
        /// 请求类型
        /// </summary>
        public GVRReqType? Type
        {
            get { return _RequestType; }
            set
            {
                SetValue("Type", ref _RequestType, value);
                RaisePropertyChanged("IsEnable");
            }
        }

        private GVRReqAuditStatus? _AuditStatus;
        /// <summary>
        /// 请求状态
        /// </summary>
        public GVRReqAuditStatus? AuditStatus
        {
            get { return _AuditStatus; }
            set { SetValue("AuditStatus", ref _AuditStatus, value); }
        }

        private string createUser;
        public string CreaterName
        {
            get { return createUser; }
            set { SetValue("CreaterName", ref createUser, value); }
        }

        private string auditUser;
        public string AuditerName
        {
            get { return auditUser; }
            set { SetValue("AuditerName", ref auditUser, value); }
        }

        private DateTime createDate;
        public DateTime CreateDate
        {
            get { return createDate; }
            set { SetValue("CreateDate", ref createDate, value); }
        }

        private DateTime? auditDate;
        public DateTime? AuditDate
        {
            get { return auditDate; }
            set { SetValue("AuditDate", ref auditDate, value); }
        }

        private string _productID;
        /// <summary>
        /// 商品ID
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

    }

    public class GiftCardProductFilterVM : ModelBase
    {
        private string price;
        [Validate(ValidateType.Regex, @"^(-)?\d+(\.\d\d)?$",@"只能输入两位有效正数")]
        public string Price
        {
            get { return price; }
            set { SetValue("Price", ref price, value); }
        }

        public int? SysNo { get; set; }

        public int? RelationSysNo { get; set; }
    }

    
}
