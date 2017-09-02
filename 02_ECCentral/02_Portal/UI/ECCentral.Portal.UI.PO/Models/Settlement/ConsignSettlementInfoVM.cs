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
using ECCentral.Portal.UI.PO.Models.Settlement;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Models
{
    public class ConsignSettlementInfoVM : SettlementRequestBaseVM
    {

        public ConsignSettlementInfoVM()
        {
            TaxRateData = PurchaseOrderTaxRate.Percent017;
            consignSettlementItemInfoList = new List<ConsignSettlementItemInfoVM>();
            eIMSInfo = new ConsignSettlementEIMSInfoVM();
        }

        #region [返点信息]

        private int? returnPointC3SysNo;

        public int? ReturnPointC3SysNo
        {
            get { return returnPointC3SysNo; }
            set { base.SetValue("ReturnPointC3SysNo", ref returnPointC3SysNo, value); }
        }

        private int? pM_ReturnPointSysNo;

        public int? PM_ReturnPointSysNo
        {
            get { return pM_ReturnPointSysNo; }
            set { base.SetValue("PM_ReturnPointSysNo", ref pM_ReturnPointSysNo, value); }
        }

        private ConsignSettlementEIMSInfoVM eIMSInfo;

        public ConsignSettlementEIMSInfoVM EIMSInfo
        {
            get { return eIMSInfo; }
            set { base.SetValue("EIMSInfo", ref eIMSInfo, value); }
        }

        #endregion

        /// <summary>
        /// 代销结算商品财务记录列表
        /// </summary>
        private List<ConsignSettlementItemInfoVM> consignSettlementItemInfoList;

        public List<ConsignSettlementItemInfoVM> ConsignSettlementItemInfoList
        {
            get { return consignSettlementItemInfoList; }
            set { base.SetValue("ConsignSettlementItemInfoList", ref consignSettlementItemInfoList, value); }
        }

    }
}
