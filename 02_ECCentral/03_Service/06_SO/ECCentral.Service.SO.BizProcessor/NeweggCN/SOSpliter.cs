using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using System.Text;

namespace ECCentral.Service.SO.BizProcessor
{
    /// <summary>
    /// 作废订单
    /// 属性 Parameter 说明:
    /// Parameter[0] : bool , 是否自动拆分订单,默认为true；
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "SIM", "Split" })]
    public class SIMSOSpliter : SOSpliter
    {
        public SIMSOSpliter()
            : base()
        {
            SubSOAssign += new Action<SOInfo>(SIMSOSpliter_SubSOAssign);
        }

        void SIMSOSpliter_SubSOAssign(SOInfo subSOInfo)
        {
            //子单是否是SIM卡订单 
            bool isSIMSO = false;
            string[] simCardProductsPre = ExternalDomainBroker.Config_SIMCardItemList_Pre(CurrentSO.CompanyCode);
            string[] simCardProductsAft = ExternalDomainBroker.Config_SIMCardItemList_Aft(CurrentSO.CompanyCode);

            string[] simCardProducts = new string[simCardProductsPre.Length + simCardProductsAft.Length];

            simCardProductsPre.CopyTo(simCardProducts, 0);
            simCardProductsAft.CopyTo(simCardProducts, simCardProductsPre.Length);

            if (simCardProducts != null && simCardProducts.Length > 0)
            {
                foreach (SOItemInfo soItem in subSOInfo.Items)
                {
                    if (simCardProducts.Contains(soItem.ProductSysNo.ToString()))
                        isSIMSO = true;
                }
            }

            if (isSIMSO)
            {
                subSOInfo.BaseInfo.SOType = SOType.SIM;
                subSOInfo.SIMCardAndContractPhoneInfo = CurrentSO.SIMCardAndContractPhoneInfo;
                subSOInfo.SIMCardAndContractPhoneInfo.SOSysNo = subSOInfo.SysNo;
            }
            else
                subSOInfo.BaseInfo.SOType = SOType.General;
        }

        protected override void SaveSubSO(SOInfo subSOInfo)
        {
            base.SaveSubSO(subSOInfo);
            if (subSOInfo.SIMCardAndContractPhoneInfo != null)
            {
                SODA.InsertSOSIMCard(subSOInfo.SIMCardAndContractPhoneInfo);
            }
        }
    }

    [VersionExport(typeof(SOAction), new string[] { "ContractPhone", "Split" })]
    public class ContractPhoneSOSpliter : SOSpliter
    {
        public ContractPhoneSOSpliter()
            : base()
        {
            SubSOAssign += new Action<SOInfo>(ContractPhoneSOSpliter_SubSOAssign);
        }

        void ContractPhoneSOSpliter_SubSOAssign(SOInfo subSOInfo)
        {
            if (CurrentSO.SIMCardAndContractPhoneInfo != null)
            {
                subSOInfo.SIMCardAndContractPhoneInfo = SerializationUtility.DeepClone(CurrentSO.SIMCardAndContractPhoneInfo);
                subSOInfo.SIMCardAndContractPhoneInfo.SOSysNo = subSOInfo.SysNo;
            }
        }


        protected override void SaveSubSO(SOInfo subSOInfo)
        {
            base.SaveSubSO(subSOInfo);
            if (subSOInfo.SIMCardAndContractPhoneInfo != null)
            {
                SODA.InsertSOSIMCard(subSOInfo.SIMCardAndContractPhoneInfo);
            }
        }
    }
}
