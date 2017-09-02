using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.RMA.BizProcessor
{
    [VersionExport(typeof(OutBoundProcessor))]
    public class OutBoundProcessor
    {
        private IOutBoundDA outBoundDA = ObjectFactory<IOutBoundDA>.Instance;
        public virtual void SendDunEmail(int OutboundSysNo, int RegisterSysNo, int SendMailCount, int SOSysNo)
        {
            if (outBoundDA.UpdateOutboundItemSendEmailCount(OutboundSysNo, RegisterSysNo, SendMailCount))
            {
                #region 构建邮件模板，调用EmailService发送邮件

                RMARegisterInfo registerInfo = ObjectFactory<IRegisterDA>.Instance.LoadBySysNo(RegisterSysNo);
                int productSysNo = registerInfo.BasicInfo.ProductSysNo.Value;

                ProductManagerInfo pmInfo = ExternalDomainBroker.GetPMInfoByProductSysNo(productSysNo);
                ProductInfo product = ExternalDomainBroker.GetProductInfo(productSysNo);

                DataRow dr = outBoundDA.GetOutboundBySysNo(OutboundSysNo);
                string vendorName = string.Empty;
                DateTime outTime = DateTime.Now;
                if (dr != null)
                {
                    vendorName = dr["VendorName"].ToString();
                    outTime = Convert.ToDateTime(dr["OutTime"]);
                }
                int vendorSysNo = Convert.ToInt32(dr["VendorSysNo"]);
                VendorInfo vendorInfo = ExternalDomainBroker.GetVendorFinanceInfoByVendorSysNo(vendorSysNo);

                SOInfo soInfo = ExternalDomainBroker.GetSOInfo(SOSysNo);
                string SODate = soInfo.BaseInfo.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                string Warranty = soInfo.Items.Where(p => p.ProductSysNo == productSysNo).FirstOrDefault().Warranty;

                //填充邮件模板
                KeyValueVariables vars = new KeyValueVariables();
                vars.Add("CurrentYear", DateTime.Now.Year);
                vars.Add("CurrentMonth", DateTime.Now.Month);
                vars.Add("CurrentDay", DateTime.Now.Day);
                vars.Add("CurrentTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                ///Load单件填充的数据
                vars.Add("RegisterSysNo", registerInfo.SysNo.Value);
                vars.Add("ProductID", product.ProductID);
                vars.Add("ProductName", product.ProductName);
                vars.Add("Memo", registerInfo.BasicInfo.Memo);
                vars.Add("RefundStatus", registerInfo.BasicInfo.RefundStatus);
                vars.Add("RevertStatus", registerInfo.RevertInfo.RevertStatus);
                vars.Add("ProductCost", registerInfo.BasicInfo.Cost);

                vars.Add("PMName", pmInfo.UserInfo.UserDisplayName);

                vars.Add("Day", GetTimeSpan(outTime).Days);
                vars.Add("Hour", GetTimeSpan(outTime).Hours);
                vars.Add("Secend", GetTimeSpan(outTime).Minutes);
                vars.Add("VendorName", vendorName);
                vars.Add("OutTime", outTime.ToString("yyyy-MM-dd HH:mm:ss"));

                vars.Add("SOSysNo", SOSysNo);
                vars.Add("SODate", SODate);
                vars.Add("Warranty", Warranty);

                vars.Add("IsContact", IsContactDesc(vendorInfo));
                vars.Add("PayPeriodType", vendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsName);

                string ccAddress = AppSettingManager.GetSetting("RMA", "OutBoundNotReturnEmails");
                EmailHelper.SendEmailByTemplate(pmInfo.UserInfo.EmailAddress, ccAddress, "", "SendDunMailForOutBoundNotReturn", vars, null, true, true);
                #endregion
            }
        }
        private TimeSpan GetTimeSpan(DateTime? OutTime)
        {
            return DateTime.Now.Subtract(OutTime.HasValue ? OutTime.Value : DateTime.Now);
        }
        private string IsContactDesc(VendorInfo vendorInfo)
        {
            if (vendorInfo != null && vendorInfo.VendorBasicInfo.VendorStatus == VendorStatus.Available)
            {
                DateTime now = DateTime.Now;
                if (vendorInfo.VendorFinanceInfo.CooperateValidDate.HasValue &&
                    vendorInfo.VendorFinanceInfo.CooperateExpiredDate.HasValue &&
                    now >= vendorInfo.VendorFinanceInfo.CooperateValidDate.Value &&
                    now <= vendorInfo.VendorFinanceInfo.CooperateExpiredDate.Value)
                {
                    return "Y";
                }
                else if (vendorInfo.VendorFinanceInfo.TotalPOAmt.HasValue &&
                         vendorInfo.VendorFinanceInfo.TotalPOAmt.Value > (vendorInfo.VendorFinanceInfo.CooperateAmt.HasValue ? vendorInfo.VendorFinanceInfo.CooperateAmt.Value : 0))
                {
                    return "Y";
                }
            }
            return "N";
        }

        #region For PO Domain

        public virtual List<int> GetOutBoundSysNoListByRegisterSysNoList(string registerSysNoList)
        {
            return ObjectFactory<IOutBoundDA>.Instance.GetOutBoundSysNoListByRegisterSysNoList(registerSysNoList);
        }

        public virtual bool UpdateOutBounds(string outBoundSysNoList)
        {
            return ObjectFactory<IOutBoundDA>.Instance.UpdateOutBounds(outBoundSysNoList);
        }
        #endregion
    }
}
