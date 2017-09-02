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
using System.Collections.Generic;

namespace ECCentral.Portal.UI.PO.Models
{
    public class CostChangeInfoVM : ModelBase
    {
        public CostChangeInfoVM()
        {
            costChangeBasicInfo = new CostChangeBasicInfoVM();
            costChangeItems = new List<CostChangeItemInfoVM>();
        }

        private int? ccSysNo;

        public int? CCSysNo
        {
            get { return ccSysNo; }
            set { this.SetValue("CCSysNo", ref ccSysNo, value); }
        }
        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { this.SetValue("CompanyCode", ref companyCode, value); }
        }

        private bool? isEnabled;

        public bool? IsEnabled
        {
            get { return isEnabled; }
            set { this.SetValue("IsEnabled", ref isEnabled, value); }
        }

        /// <summary>
        /// 成本变价单基本信息
        /// </summary>
        private CostChangeBasicInfoVM costChangeBasicInfo;

        public CostChangeBasicInfoVM CostChangeBasicInfo
        {
            get { return costChangeBasicInfo; }
            set { this.SetValue("CostChangeBasicInfo", ref costChangeBasicInfo, value); }
        }

        /// <summary>
        /// 商品变价明细
        /// </summary>
        private List<CostChangeItemInfoVM> costChangeItems;

        public List<CostChangeItemInfoVM> CostChangeItems
        {
            get { return costChangeItems; }
            set { this.SetValue("CostChangeItems", ref costChangeItems, value); }
        }

    }
}
