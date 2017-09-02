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
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Inventory.Models;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class StockQueryVM : ModelBase
    {
        public PagingInfo PagingInfo { get; set; }

        public StockQueryVM()
        {
            PagingInfo = new PagingInfo();
        }

        private string stockSysNo;
        [Validate(ValidateType.Regex, @"^[,\. ]*\d+[\d,\. ]*$", ErrorMessageResourceName = "Msg_StockSysNo_Format", ErrorMessageResourceType = typeof(ResStockQuery))]
        public string StockSysNo
        {
            get
            {
                return stockSysNo;
            }
            set
            {
                base.SetValue("StockSysNo", ref stockSysNo, value);
            }
        }
        /// <summary>
        /// 渠道仓库编号
        /// </summary>
        private string stockID;

        [Validate(ValidateType.Regex, @"^[, ]*\w+[\w-#, ]*$", ErrorMessageResourceName = "Msg_StockID_Format", ErrorMessageResourceType = typeof(ResStockQuery))]
        public string StockID
        {
            get { return stockID; }
            set { base.SetValue("StockID", ref stockID, value); }
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

        private int? warehouseSysNo;
        public int? WarehouseSysNo
        {
            get
            {
                return warehouseSysNo;
            }
            set
            {
                base.SetValue("WarehouseSysNo", ref warehouseSysNo, value);
            }
        }


        private string webChannelID;
        public string WebChannelID
        {
            get
            {
                return webChannelID;
            }
            set
            {
                base.SetValue("WebChannelID", ref webChannelID, value);
                if (!string.IsNullOrEmpty(value))
                {
                    //需要修改UIWebChannel
                    this.WebChannelSysNo = int.Parse(value);
                }
            }
        }

        private int? webChannelSysNo;
        public int? WebChannelSysNo
        {
            get
            {
                return webChannelSysNo;
            }
            set
            {
                base.SetValue("WebChannelSysNo", ref webChannelSysNo, value);
            }
        }

        private ValidStatus? stockStatus;
        public ValidStatus? StockStatus
        {
            get
            {
                return stockStatus;
            }
            set
            {
                base.SetValue("StockStatus", ref stockStatus, value);
            }
        }

        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }

        private List<KeyValuePair<ValidStatus?, string>> stockStatusList;
        public List<KeyValuePair<ValidStatus?, string>> StockStatusList
        {

            get
            {
                stockStatusList = stockStatusList ?? EnumConverter.GetKeyValuePairs<ValidStatus>(EnumConverter.EnumAppendItemType.All);
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
                list.Insert(0, new UIWebChannel
                {                    
                    ChannelID = null,
                    ChannelName = ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_All,
                });
                return list;
            }
        }
    }
    public class StockQueryView : ModelBase
    {
        public StockQueryVM QueryInfo
        {
            get;
            set;
        }
        private List<dynamic> result;
        public List<dynamic> Result
        {
            get { return result; }
            set
            {
                SetValue("Result", ref result, value);
            }
        }
        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                SetValue<int>("TotalCount", ref totalCount, value);
            }
        }
        public StockQueryView()
        {
            QueryInfo = new StockQueryVM();
        }
    }
}
