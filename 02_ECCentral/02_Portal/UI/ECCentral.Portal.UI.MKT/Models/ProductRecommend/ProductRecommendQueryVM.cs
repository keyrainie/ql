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
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ProductRecommendQueryVM : ModelBase
    {
        private string _companyCode;
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get { return _companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
            }
        }
        private string _channelID;
        /// <summary>
        /// 渠道编号
        /// </summary>
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }
        private int? _pageType;
        /// <summary>
        /// 页面类型ID
        /// </summary>
        public int? PageType
        {
            get { return _pageType; }
            set
            {
                base.SetValue("PageType", ref _pageType, value);
            }
        }
        private int? _pageID;
        /// <summary>
        /// 页面ID
        /// </summary>
        public int? PageID
        {
            get { return _pageID; }
            set
            {
                base.SetValue("PageID", ref _pageID, value);
            }
        }
        private int? _positionID;
        /// <summary>
        /// 位置编号
        /// </summary>
        public int? PositionID
        {
            get { return _positionID; }
            set
            {
                base.SetValue("PositionID", ref _positionID, value);
            }
        }
        private ADStatus? _status;
        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus? Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }
        private DateTime? _beginDateFrom;
        /// <summary>
        /// 生效开始时间from
        /// </summary>
        public DateTime? BeginDateFrom
        {
            get { return _beginDateFrom; }
            set
            {
                base.SetValue("BeginDateFrom", ref _beginDateFrom, value);
            }
        }
        private DateTime? _beginDateTo;
        /// <summary>
        /// 生效开始时间to
        /// </summary>
        public DateTime? BeginDateTo
        {
            get { return _beginDateTo; }
            set
            {
                base.SetValue("BeginDateTo", ref _beginDateTo, value);
            }
        }
        private DateTime? _endDateFrom;
        /// <summary>
        /// 失效结束日期from
        /// </summary>
        public DateTime? EndDateFrom
        {
            get { return _endDateFrom; }
            set
            {
                base.SetValue("EndDateFrom", ref _endDateFrom, value);
            }
        }
        private DateTime? _endDateTo;
        /// <summary>
        /// 失效结束日期to
        /// </summary>
        public DateTime? EndDateTo
        {
            get { return _endDateTo; }
            set
            {
                base.SetValue("EndDateTo", ref _endDateTo, value);
            }
        }
        private string _productSysNo;
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public string ProductSysNo
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

        private ProductStatus? _productStatus;
        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus? ProductStatus
        {
            get { return _productStatus; }
            set
            {
                base.SetValue("ProductStatus", ref _productStatus, value);
            }
        }
    }
}
