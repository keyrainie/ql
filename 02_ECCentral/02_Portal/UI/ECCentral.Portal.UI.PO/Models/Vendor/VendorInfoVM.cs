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
using ECCentral.BizEntity.PO;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorInfoVM : ModelBase
    {
        public VendorInfoVM()
        {
            vendorBasicInfo = new VendorBasicInfoVM(this);
            vendorFinanceInfo = new VendorFinanceInfoVM();
            vendorCustomsInfo = new VendorCustomsInfoVM();
            vendorServiceInfo = new VendorServiceInfoVM();
            vendorHistoryLog = new List<VendorHistoryLogVM>();
            vendorAttachInfo = new VendorAttachInfoVM();
            vendorAgentInfoList = new List<VendorAgentInfoVM>();
            vendorDeductInfo = new VendorDeductInfoVM();
            VendorStoreInfoList = new ObservableCollection<VendorStoreInfoVM>();
        }

        /// <summary>
        /// 系统编号
        /// </summary>
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
        /// <summary>
        /// 供应商基本信息
        /// </summary>
        private VendorBasicInfoVM vendorBasicInfo;
        public VendorBasicInfoVM VendorBasicInfo
        {
            get { return vendorBasicInfo; }
            set { base.SetValue("VendorBasicInfo", ref vendorBasicInfo, value); }
        }

        /// <summary>
        /// 供应商财务信息
        /// </summary>
        private VendorFinanceInfoVM vendorFinanceInfo;

        public VendorFinanceInfoVM VendorFinanceInfo
        {
            get { return vendorFinanceInfo; }
            set { base.SetValue("VendorFinanceInfo", ref vendorFinanceInfo, value); }
        }
        /// <summary>
        /// 关务对接信息
        /// </summary>
        private VendorCustomsInfoVM vendorCustomsInfo;

        public VendorCustomsInfoVM VendorCustomsInfo
        {
            get { return vendorCustomsInfo; }
            set { base.SetValue("VendorCustomsInfo", ref vendorCustomsInfo, value); }
        }
        /// <summary>
        /// 供应商售后服务信息
        /// </summary>
        private VendorServiceInfoVM vendorServiceInfo;

        public VendorServiceInfoVM VendorServiceInfo
        {
            get { return vendorServiceInfo; }
            set { base.SetValue("VendorServiceInfo", ref vendorServiceInfo, value); }
        }

        /// <summary>
        /// 供应商附件信息
        /// </summary>
        private VendorAttachInfoVM vendorAttachInfo;

        public VendorAttachInfoVM VendorAttachInfo
        {
            get { return vendorAttachInfo; }
            set { base.SetValue("VendorAttachInfo", ref vendorAttachInfo, value); }
        }


        /// <summary>
        /// 供应商代理信息
        /// </summary>
        private List<VendorAgentInfoVM> vendorAgentInfoList;

        public List<VendorAgentInfoVM> VendorAgentInfoList
        {
            get { return vendorAgentInfoList; }
            set { base.SetValue("VendorAgentInfoList", ref vendorAgentInfoList, value); }
        }

        /// <summary>
        /// 供应商历史信息
        /// </summary>
        private List<VendorHistoryLogVM> vendorHistoryLog;

        public List<VendorHistoryLogVM> VendorHistoryLog
        {
            get { return vendorHistoryLog; }
            set { base.SetValue("VendorHistoryLog", ref vendorHistoryLog, value); }
        }

        public ObservableCollection<VendorStoreInfoVM> VendorStoreInfoList { get; set; }
        private VendorDeductInfoVM vendorDeductInfo;
        public VendorDeductInfoVM VendorDeductInfo
        {
            get { return vendorDeductInfo; }
            set { base.SetValue("VendorDeductInfo", ref vendorDeductInfo, value); }
        }
    }
}
