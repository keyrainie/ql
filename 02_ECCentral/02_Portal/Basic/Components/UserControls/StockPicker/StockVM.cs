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
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.Basic.Components.UserControls.StockPicker
{
    public class StockVM : ModelBase
    {
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

        private string _stockID;
        /// <summary>
        /// 商品ID
        /// </summary>
        public string StockID
        {
            get { return _stockID; }
            set
            {
                base.SetValue("StockID", ref _stockID, value);
            }
        }

        private string _stockName;
        /// <summary>
        /// 商品名称
        /// </summary>
        public string StockName
        {
            get { return _stockName; }
            set
            {
                base.SetValue("StockName", ref _stockName, value);
            }
        }

        private string _webChannelID;
        /// <summary>
        /// 销售渠道ID
        /// </summary>
        public string WebChannelID
        {
            get { return _webChannelID; }
            set
            {
                base.SetValue("WebChannelID", ref _webChannelID, value);
            }
        }

        private int? _webChannelSysNo;
        /// <summary>
        /// 销售渠道ID
        /// </summary>
        public int? WebChannelSysNo
        {
            get { return _webChannelSysNo; }
            set
            {
                base.SetValue("WebChannelSysNo", ref _webChannelSysNo, value);
            }
        }

        private string _warehouseSysNo;
        /// <summary>
        /// 所属仓库编号
        /// </summary>
        public string WarehouseSysNo
        {
            get { return _warehouseSysNo; }
            set
            {
                base.SetValue("WarehouseSysNo", ref _warehouseSysNo, value);
            }
        }

        private string _countryCode;
        /// <summary>
        /// 区域代码
        /// </summary>
        public string CountryCode
        {
            get { return _countryCode; }
            set { _countryCode = value; }
        }
    }
}
