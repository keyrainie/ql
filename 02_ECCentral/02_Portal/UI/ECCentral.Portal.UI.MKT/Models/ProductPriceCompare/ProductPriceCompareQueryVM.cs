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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ProductPriceCompareQueryVM : ModelBase
    {
        private string _channelID;
        /// <summary>
        /// 渠道编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }

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
        private DateTime? _createTimeFrom;
        /// <summary>
        /// 创建时间开始
        /// </summary>
        public DateTime? CreateTimeFrom
        {
            get { return _createTimeFrom; }
            set
            {
                base.SetValue("CreateTimeFrom", ref _createTimeFrom, value);
            }
        }
        private DateTime? _createTimeTo;
        /// <summary>
        /// 创建时间结束
        /// </summary>
        public DateTime? CreateTimeTo
        {
            get { return _createTimeTo; }
            set
            {
                base.SetValue("CreateTimeTo", ref _createTimeTo, value);
            }
        }
        private int? _productSysNo;
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo
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
        private string _status;
        /// <summary>
        /// 状态
        /// </summary>
        public string Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }
        private int? _c3SysNo;
        /// <summary>
        /// 三级分类系统编号
        /// </summary>
        public int? C3SysNo
        {
            get { return _c3SysNo; }
            set
            {
                base.SetValue("C3SysNo", ref _c3SysNo, value);
            }
        }
        private int? _c2SysNo;
        /// <summary>
        /// 二级分类系统编号
        /// </summary>
        public int? C2SysNo
        {
            get { return _c2SysNo; }
            set
            {
                base.SetValue("C2SysNo", ref _c2SysNo, value);
            }
        }
        private int? _c1SysNo;
        /// <summary>
        /// 一级分类系统编号
        /// </summary>
        public int? C1SysNo
        {
            get { return _c1SysNo; }
            set
            {
                base.SetValue("C1SysNo", ref _c1SysNo, value);
            }
        }
        private int? _pMSysNo;
        /// <summary>
        /// PM系统编号
        /// </summary>
        public int? PMSysNo
        {
            get { return _pMSysNo; }
            set
            {
                base.SetValue("PMSysNo", ref _pMSysNo, value);
            }
        }

    }
}
