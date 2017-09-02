using System;
using System.Linq;
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
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Components.Models;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Inventory.Resources;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class StockInfoVM : ModelBase
    {
        public StockInfoVM()
        {
            warehouseInfo = new WarehouseInfoVM();
            WebChannel = new WebChannelVM();
        }

        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }
        //private string webChannelID;
        //[Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required, ErrorMessageResourceName = "Msg_WebChannel_IsRequired", ErrorMessageResourceType = typeof(ResStockQuery))]
        //public string WebChannelID
        //{
        //    get
        //    {
        //        return WebChannel == null ? null : WebChannel.ChannelID;
        //    }
        //    set
        //    {
        //        SetValue("WebChannelID", ref webChannelID, value);
        //        WebChannel = WebChannel ?? new WebChannelVM();
        //        WebChannel.ChannelID = webChannelID;
        //    }
        //}

        private WebChannelVM webChannel;
        public WebChannelVM WebChannel
        {
            get { return webChannel; }
            set { base.SetValue("WebChannel", ref webChannel, value); }
        }

        /// <summary>
        /// 渠道仓库编号
        /// </summary>
        private string stockID;
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required, ErrorMessageResourceName = "Msg_StockID_IsRequired", ErrorMessageResourceType = typeof(ResStockQuery))]
        [Validate(ValidateType.Regex, @"^[\w-#]+$", ErrorMessageResourceName = "Msg_StockID_Format", ErrorMessageResourceType = typeof(ResStockQuery))]
        public string StockID
        {
            get { return stockID; }
            set { base.SetValue("StockID", ref stockID, value); }
        }

        /// <summary>
        /// 渠道仓库名称
        /// </summary>
        private string stockName;

        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required, ErrorMessageResourceName = "Msg_StockName_IsRequired", ErrorMessageResourceType = typeof(ResStockQuery))]
        public string StockName
        {
            get { return stockName; }
            set { base.SetValue("StockName", ref stockName, value); }
        }

        /// <summary>
        /// 渠道仓库状态
        /// </summary>
        private ValidStatus stockStatus;
        public ValidStatus StockStatus
        {
            get { return stockStatus; }
            set { base.SetValue("StockStatus", ref stockStatus, value); }
        }
        //private int? warehouseSysNo;
        //[Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required, ErrorMessageResourceName = "Msg_Stock_WH_IsRequired", ErrorMessageResourceType = typeof(ResStockQuery))]
        //public int? WarehouseSysNo
        //{
        //    get { return WarehouseInfo == null ? null : WarehouseInfo.SysNo; }
        //    set
        //    {
        //        base.SetValue("WarehouseSysNo", ref warehouseSysNo, value);
        //        WarehouseInfo = WarehouseInfo ?? new WarehouseInfoVM();
        //        WarehouseInfo.SysNo = warehouseSysNo;
        //    }
        //}

        /// <summary>
        /// 渠道仓库所属仓库
        /// </summary>
        private WarehouseInfoVM warehouseInfo;
        public WarehouseInfoVM WarehouseInfo
        {
            get { return warehouseInfo; }
            set { base.SetValue("WarehouseInfo", ref warehouseInfo, value); }
        }

        private List<KeyValuePair<ValidStatus?, string>> stockStatusList;
        public List<KeyValuePair<ValidStatus?, string>> StockStatusList
        {
            get
            {
                stockStatusList = stockStatusList ?? EnumConverter.GetKeyValuePairs<ValidStatus>();
                return stockStatusList;
            }
        }

        private List<WarehouseInfoVM> warehouseList;
        public List<WarehouseInfoVM> WarehouseList
        {
            get
            {
                return warehouseList;
            }
            set
            {
                base.SetValue("WarehouseList", ref warehouseList, value);
            }
        }

        public List<UIWebChannel> WebChannelList
        {
            get
            {
                List<UIWebChannel> list = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
                //list.Insert(0, new UIWebChannel
                //{
                //    ChannelID = null,
                //    ChannelName = ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_Select,
                //});
                return list;
            }
        }
    }
}
