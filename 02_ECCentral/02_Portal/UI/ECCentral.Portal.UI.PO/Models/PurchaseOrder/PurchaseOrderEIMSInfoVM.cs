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
    public class PurchaseOrderEIMSInfoVM : ModelBase
    {

        public PurchaseOrderEIMSInfoVM()
        {
            eIMSInfoList = new List<EIMSInfoVM>();
        }

        /// <summary>
        /// 合同编号
        /// </summary>
        private string contractNumber;

        public string ContractNumber
        {
            get { return contractNumber; }
            set { this.SetValue("ContractNumber", ref contractNumber, value); }
        }

        /// <summary>
        /// 使用返点金额
        /// </summary>
        private decimal? totalEIMS;

        public decimal? TotalEIMS
        {
            get { return totalEIMS; }
            set { totalEIMS = value; }
        }

        /// <summary>
        /// 已扣减返点
        /// </summary>
        private decimal? totalUsedEIMS;

        public decimal? TotalUsedEIMS
        {
            get { return totalUsedEIMS; }
            set { totalUsedEIMS = value; }
        }

        /// <summary>
        ///  返点信息
        /// </summary>
        private List<EIMSInfoVM> eIMSInfoList;

        public List<EIMSInfoVM> EIMSInfoList
        {
            get { return eIMSInfoList; }
            set { this.SetValue("EIMSInfoList", ref eIMSInfoList, value); }
        }
    }
}
