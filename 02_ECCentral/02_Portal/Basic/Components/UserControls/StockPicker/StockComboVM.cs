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
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.Models;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.Basic.Components.UserControls.StockPicker
{
    public class StockComboVM : ModelBase
    {

        public StockComboVM()
        {
            //Set Default WebChannel, Need to Change.
            this.WebChannelID = "1";
        }

        private int? stockSysNo;
        public int? StockSysNo
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

        private List<WebChannelVM> webChannelList;
        public List<WebChannelVM> WebChannelList
        {
            get
            {
                return webChannelList;
            }
            set
            {
                base.SetValue("WebChannelList", ref webChannelList, value);
            }

        }

        private List<StockVM> stockList;
        public List<StockVM> StockList
        {
            get
            {
                return stockList;
            }
            set
            {
                base.SetValue("StockList", ref stockList, value);
            }

        }
    }

    public class MultiStockComboVM : ModelBase
    {
        public MultiStockComboVM()
        {
            //Set Default WebChannel, Need to Change.
            this.WebChannelID = "1";
        }

        private ObservableCollection<int?> stockSysNo;
        public ObservableCollection<int?> StockSysNo
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

        private List<string> stockName;
        public List<string> StockName
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

        private List<WebChannelVM> webChannelList;
        public List<WebChannelVM> WebChannelList
        {
            get
            {
                return webChannelList;
            }
            set
            {
                base.SetValue("WebChannelList", ref webChannelList, value);
            }

        }

        private List<StockVM> stockList;
        public List<StockVM> StockList
        {
            get
            {
                return stockList;
            }
            set
            {
                base.SetValue("StockList", ref stockList, value);
            }

        }
    }
}
