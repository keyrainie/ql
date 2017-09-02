using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice.Report;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Invoice.InvoiceReport;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IInvoiceReportDA))]
    public class InvoiceReportDA : IInvoiceReportDA
    {
        #region IInvoiceReportDA Members

        public TrackingNumberInfo CreateTrackingNumber(TrackingNumberInfo entity)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("InsertTrackingNumber");
            dataCommand.SetParameterValue(entity);

            var suffix = GetStockSuffix(entity.StockSysNo.Value);
            dataCommand.CommandText = dataCommand.CommandText
                .Replace("Invoice_TrackingNumber", "Invoice_TrackingNumber_" + suffix);

            entity.SysNo = Convert.ToInt32(dataCommand.ExecuteScalar());
            return entity;
        }

        #endregion IInvoiceReportDA Members

        private Hashtable _stockSuffixMap;

        [Caching("local", ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        private string GetStockSuffix(int stockSysNo)
        {
            string cfg = AppSettingManager.GetSetting("Invoice", "InvoiceReportStockSuffixMap");
            if (_stockSuffixMap == null)
            {
                _stockSuffixMap = new Hashtable();
                if (!string.IsNullOrEmpty(cfg))
                {
                    string[] groups = cfg.Split(';');
                    foreach (var g in groups)
                    {
                        var pair = g.Split('-');
                        _stockSuffixMap.Add(pair[0], pair[1]);
                    }
                }
            }
            return (string)_stockSuffixMap[stockSysNo.ToString()];
        }

        #region 发票打印相关

        /// <summary>
        /// 获取订单发票的Master信息
        /// </summary>
        /// <param name="soNo">SO编号</param>
        /// <param name="stockSysNo">分仓编号</param>
        /// <param name="strNEG">NEG</param>
        /// <param name="strMET">MET</param>
        /// <returns></returns>
        public SOInvoiceMaster GetSOInvoiceMaster(int soNo, int stockSysNo, string strNEG, string strMET)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("InvoiceReport.GetSOInvoiceMaster");
            dataCommand.SetParameterValue("@SOSysNo", soNo);
            dataCommand.SetParameterValue("@StockSysNo", stockSysNo);
            dataCommand.SetParameterValue("@NEG", strNEG);
            dataCommand.SetParameterValue("@MET", strMET);

            var result = dataCommand.ExecuteEntity<SOInvoiceMaster>();

            return result;
        }
        
        public InvoiceInfo GetInvoicePrintHead(int soNo, int stockSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("InvoiceReport.GetInvoicePrintHead");
            dataCommand.SetParameterValue("@SOSysNo", soNo);
            dataCommand.SetParameterValue("@StockSysNo", stockSysNo);

            var result = dataCommand.ExecuteEntity<InvoiceInfo>();

            return result;
        }

        public List<InvoiceItem> GetSOInvoiceProductItem(int soNo, int stockSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("InvoiceReport.GetSOInvoiceProductItem");
            dataCommand.SetParameterValue("@SOSysNo", soNo);
            dataCommand.SetParameterValue("@StockSysNo", stockSysNo);

            return dataCommand.ExecuteEntityList<InvoiceItem>();
        }

        public List<InvoiceItem> GetSOExtendWarrantyItem(int soNo, int stockSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("InvoiceReport.GetSOExtendWarrantyItem");
            dataCommand.SetParameterValue("@SOSysNo", soNo);
            dataCommand.SetParameterValue("@StockSysNo", stockSysNo);

            return dataCommand.ExecuteEntityList<InvoiceItem>();
        }

        public List<InvoiceSub> GetInvoiceSub(int soNo, int stockSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("InvoiceReport.GetInvoiceSub");
            dataCommand.SetParameterValue("@SOSysNo", soNo);
            dataCommand.SetParameterValue("@StockSysNo", stockSysNo);

            return dataCommand.ExecuteEntityList<InvoiceSub>();
        }

        [Caching("local", ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public string GetSysConfiguration(string Key)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("InvoiceReport.GetSysConfiguration");
            dataCommand.SetParameterValue("@key", Key);

            return dataCommand.ExecuteScalar().ToString();
        }

        #endregion
    }
}