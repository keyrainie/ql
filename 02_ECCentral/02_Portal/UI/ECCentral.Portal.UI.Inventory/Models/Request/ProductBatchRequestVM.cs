using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
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

namespace ECCentral.Portal.UI.Inventory.Models.Request
{
    public class ProductBatchRequestVM : ModelBase
    {
        private string _StockName;
        public string StockName
        {
            get { return _StockName; }
            set { this.SetValue("StockName", ref _StockName, value); }
        }

        private string _ProductSysNo;
        [Validate(ValidateType.Required)]
        public string ProductSysNo
        {
            get { return _ProductSysNo; }
            set { this.SetValue("ProductSysNo", ref _ProductSysNo, value); }
        }

        private string _ProductID;
        [Validate(ValidateType.Required)]
        public string ProductID
        {
            get { return _ProductID; }
            set { this.SetValue("ProductID", ref _ProductID, value); }
        }


        #region  损益单
        private string _AdjustNum;
        [Validate(ValidateType.Required, ErrorMessage = "AdjustNum")]
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.MaxLength,5)]
        public string AdjustNum
        {
            get { return _AdjustNum; }
            set { this.SetValue("AdjustNum", ref _AdjustNum, value); }
        }
        #endregion 

        #region 转换单
        private string _ConvertNum;
        [Validate(ValidateType.Required, ErrorMessage = "ConvertNum")]
        [Validate(ValidateType.Regex, @"^([1-9]?[0-9]|[1-9]?[0-9][0-8])$", ErrorMessage = "请输入0到998之间的正整数")]
        public string ConvertNum
        {
            get { return _ConvertNum; }
            set { this.SetValue("ConvertNum", ref _ConvertNum, value); }
        }

        private decimal? _ConvertCost;
        public decimal? ConvertCost
        {
            get { return _ConvertCost; }
            set { this.SetValue("ConvertCost", ref _ConvertCost, value); }
        }

        private string _ConvertType;
        public string ConvertType
        {
            get { return _ConvertType; }
            set { this.SetValue("ConvertType", ref _ConvertType, value); }
        }
        #endregion

        #region 借货单
        private DateTime? _ReturnDate;
        public DateTime? ReturnDate
        {
            get { return _ReturnDate; }
            set { this.SetValue("ReturnDate", ref _ReturnDate, value); }
        }
        private string _LendNum;
        [Validate(ValidateType.Required, ErrorMessage = "LendNum")]
        [Validate(ValidateType.Regex, @"^([1-9]?[0-9]|[1-9]?[0-9][0-8])$", ErrorMessage = "请输入0到998之间的正整数")]
        public string LendNum
        {
            get { return _LendNum; }
            set { this.SetValue("LendNum", ref _LendNum, value); }
        }

        private string _ReturnNum;
        [Validate(ValidateType.Required, ErrorMessage = "ReturnNum")]
        [Validate(ValidateType.Regex, @"^([1-9]?[0-9]|[1-9]?[0-9][0-8])$", ErrorMessage = "请输入0到998之间的正整数")]
        public string ReturnNum
        {
            get { return _ReturnNum; }
            set { this.SetValue("ReturnNum", ref _ReturnNum, value); }
        }

        #endregion

        #region UIVisible
        public Visibility IsAdjustPage
        {
            get 
            {
                return PType == PageType.Adjust ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _IsConvertPage;
        public Visibility IsConvertPage
        {
            get
            {
                return PType == PageType.Convert ? Visibility.Visible : Visibility.Collapsed;
            }
            set
            {
                this.SetValue("IsConvertPage", ref _IsConvertPage, value);
            }
        }
        public Visibility IsLendPage
        {
            get
            {
                return PType == PageType.Lend ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility IsConvert_HasBatchVisibility
        {
            get
            {
                if (IsConvertPage == Visibility.Visible)
                {
                    if (HasBatch)
                    {
                        return Visibility.Collapsed;
                    }
                    else
                    {
                        return Visibility.Visible;
                    }
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public Visibility IsAdjust_HasBatchVisibility
        {
            get
            {
                if (IsAdjustPage == Visibility.Visible)
                {
                    if (HasBatch)
                    {
                        return Visibility.Collapsed;
                    }
                    else
                    {
                        return Visibility.Visible;
                    }
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        private Visibility isLend_HasBatchVisibility;
        public Visibility IsLend_HasBatchVisibility
        {
            get
            {
                if (IsLendPage == Visibility.Visible)
                {
                    if (HasBatch || !IsNotReturn)
                    {
                        return Visibility.Collapsed;
                    }
                    else
                    {
                        return Visibility.Visible;
                    }
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            set
            {
                SetValue("IsLend_HasBatchVisibility", ref isLend_HasBatchVisibility, value);
            }
        }


        private Visibility _HasBatchVisibility;
        public Visibility HasBatchVisibility
        {
            get
            {
                if (!HasBatch)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            set
            {
                SetValue("HasBatchVisibility", ref _HasBatchVisibility, value);
            }
        }

        private Visibility _ReturnVisibility;
        public Visibility ReturnVisibility
        {
            get
            {
                if (IsLendPage == Visibility.Visible)
                {
                    if (!IsNotReturn && !HasBatch)
                    {
                        return Visibility.Visible;
                    }
                    else 
                    {
                        return Visibility.Collapsed;
                    }
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            set
            {
                SetValue("ReturnVisibility", ref _ReturnVisibility, value);
            }
        }

        private bool _IsEditMode;
        public bool IsEditMode
        {
            get 
            {
                if (IsCreateMode)
                {
                    _IsEditMode = true;
                }
                else
                {
                    _IsEditMode = false;
                }
                return _IsEditMode;
            }
            set 
            {
                SetValue("IsEditMode", ref _IsEditMode, value);
            }
        }

        #endregion

        public PageType PType
        {
            get;
            set;
        }

        public bool IsNotReturn
        {
            get;
            set;
        }

        public int StockSysNo
        {
            get;
            set;
        }

        public bool isCreateMode;
        public bool IsCreateMode
        {
            get { return isCreateMode; }
            set {
                isCreateMode = value;
                RaisePropertyChanged("IsEditMode");
            }
        }
        private bool _HasBatch;
        public bool HasBatch
        {
            get { return _HasBatch; }
            set
            {
                _HasBatch = value;
                RaisePropertyChanged("HasBatchVisibility");
                RaisePropertyChanged("IsConvert_HasBatchVisibility");
                RaisePropertyChanged("IsAdjust_HasBatchVisibility");
                RaisePropertyChanged("IsLend_HasBatchVisibility");
            }
        }
    }

    public enum PageType
    {
        /// <summary>
        /// 供货单
        /// </summary>
        Lend,
        /// <summary>
        /// 损益单
        /// </summary>
        Adjust,
        /// <summary>
        /// 转换单
        /// </summary>
        Convert,
    }
}
 