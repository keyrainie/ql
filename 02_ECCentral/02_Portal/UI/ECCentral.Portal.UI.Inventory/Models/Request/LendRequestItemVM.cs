using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models.Inventory;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class LendRequestItemVM : ModelBase
    {
        public LendRequestItemVM()
        {
            this.IsChecked = false;
            this.RequestStatus = LendRequestStatus.Origin;
            this.BatchDetailsInfoList = new List<ProductBatchInfoVM>();
        }

        #region 业务属性

        private int? sysNo;
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }
        }

        private int? productSysNo;
        public int? ProductSysNo
        {
            get
            {
                return productSysNo;
            }
            set
            {
                base.SetValue("ProductSysNo", ref productSysNo, value);
            }
        }

        private string productID;
        public string ProductID
        {
            get
            {
                return productID;
            }
            set
            {
                base.SetValue("ProductID", ref productID, value);
            }
        }

        private string productName;
        public string ProductName
        {
            get
            {
                return productName;
            }
            set
            {
                base.SetValue("ProductName", ref productName, value);
            }
        }


        private string webChannelName;
        public string WebChannelName
        {
            get
            {
                return webChannelName;
            }
            set
            {
                base.SetValue("WebChannelName", ref webChannelName, value);
            }
        }


        private string stockName;
        public string StockName
        {
            get
            {
                return stockName;
            }
            set
            {
                base.SetValue("StockName", ref stockName, value);
            }
        }

        private int? pmUserSysNo;
        public int? PMUserSysNo
        {
            get
            {
                return pmUserSysNo;
            }
            set
            {
                base.SetValue("PMUserSysNo", ref pmUserSysNo, value);
            }
        }

        private string pmUserName;
        public string PMUserName
        {
            get
            {
                return pmUserName;
            }
            set
            {
                base.SetValue("PMUserName", ref pmUserName, value);
            }
        }

        private int? lendQuantity;
        public int? LendQuantity
        {
            get
            {
                return lendQuantity;
            }
            set
            {
                base.SetValue("LendQuantity", ref lendQuantity, value);
            }
        }

        private int? toReturnQuantity;
        public int? ToReturnQuantity
        {
            get
            {
                return toReturnQuantity;
            }
            set
            {
                base.SetValue("ToReturnQuantity", ref toReturnQuantity, value);
            }
        }

        private int? returnQuantity;
        public int? ReturnQuantity
        {
            get
            {
                return returnQuantity;
            }
            set
            {
                base.SetValue("ReturnQuantity", ref returnQuantity, value);
            }
        }

        private DateTime? returnDateETA;
        public DateTime? ReturnDateETA
        {
            get
            {
                return returnDateETA;
            }
            set
            {
                base.SetValue("ReturnDateETA", ref returnDateETA, value);
            }
        }

        public List<ProductBatchInfoVM> BatchDetailsInfoList { get; set; }

        public int IsHasBatch { get; set; }
        #endregion 业务属性

        #region UI扩展属性

        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                base.SetValue("IsChecked", ref isChecked, value);
            }
        }

        private LendRequestStatus? requestStatus;
        public LendRequestStatus? RequestStatus
        {
            get
            {
                return requestStatus;
            }
            set
            {
                base.SetValue("RequestStatus", ref requestStatus, value);
            }
        }


        public Visibility ActionEditVisibility 
        {
            get
            {
                return this.IsEditMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility ActionReturnVisibility
        {
            get
            {
                return this.IsToReturnMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public bool IsEditMode
        {
            get
            {
                return this.RequestStatus == LendRequestStatus.Origin;
            }
        }



        public bool IsToReturnMode
        {
            get
            {
                if ((this.RequestStatus == LendRequestStatus.OutStock
                    || this.RequestStatus == LendRequestStatus.ReturnPartly))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool IsBatchReturn
        {
            get 
            {
                if ((this.RequestStatus == LendRequestStatus.OutStock
                    || this.RequestStatus == LendRequestStatus.ReturnPartly)&&!HasBatchInfo)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsFullReture
        {
            get
            {
                return !(this.ReturnQuantity == this.LendQuantity);
            }
        }

        public bool IsViewReturnMode
        {
            get
            {
                return this.RequestStatus == LendRequestStatus.ReturnPartly
                    || this.RequestStatus == LendRequestStatus.ReturnAll;
            }
        }

        public Visibility IsBatch
        {
            get 
            {
                return IsHasBatch > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public bool IsBatchMode
        {
            get
            {
                return (IsHasBatch == 0 && IsEditMode) ? true : false;
            }
        }

        public bool HasBatchInfo
        {
            get
            {
                return this.BatchDetailsInfoList != null && this.BatchDetailsInfoList.Count > 0;
            }
        }
        #endregion UI扩展属性
    }
}
