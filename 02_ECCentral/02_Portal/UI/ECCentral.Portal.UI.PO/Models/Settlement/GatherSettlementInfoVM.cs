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
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Models.Settlement
{
    public class GatherSettlementInfoVM : SettlementRequestBaseVM
    {
        public GatherSettlementInfoVM()
        {
            gatherSettlementItemInfoList = new List<GatherSettlementItemInfoVM>();
        }

        private GatherSettleStatus? settleStatus;

        public GatherSettleStatus? SettleStatus
        {
            get { return settleStatus; }
            set { this.SetValue("SettleStatus", ref settleStatus, value); }
        }

        /// <summary>
        /// 代收结算商品列表
        /// </summary>
        private List<GatherSettlementItemInfoVM> gatherSettlementItemInfoList;

        public List<GatherSettlementItemInfoVM> GatherSettlementItemInfoList
        {
            get { return gatherSettlementItemInfoList; }
            set { this.SetValue("GatherSettlementItemInfoList", ref gatherSettlementItemInfoList, value); }
        }
    }
}
