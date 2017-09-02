using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;

using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.RMA.AppService
{
    public class Register
    {
        public string RegisterSysNoPrintCode { get; set; }
    }

    public class RequestLabelPrintData : IPrintDataBuild
    {
        #region IPrintDataBuild Members

        public void BuildData(NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();

            int sysNo = 0;
            string sysNoString = requestPostData["SysNo"];
            bool isNum = int.TryParse(sysNoString, out sysNo);
            if (isNum && sysNo > 0)
            {
                ObjectFactory<RequestAppService>.Instance.PrintLabels(sysNo);

                CustomerInfo customer;
                SOBaseInfo soBaseInfo;
                DeliveryInfo deliveryInfo;
                string deliveryUserName, businessModel;

                var requestView = ObjectFactory<RequestAppService>.Instance.LoadWithRegistersBySysNo(sysNo, out customer, out soBaseInfo, out deliveryInfo, out deliveryUserName, out businessModel);
                if (requestView != null && requestView.Registers != null && requestView.Registers.Count > 0)
                {
                    List<Register> regiserSysNoBarCodeList = new List<Register>();
                    requestView.Registers.ForEach(item =>
                    {
                        string barCode = Convert.ToBase64String(BarCode39Helper.DrawImage(13, item.SysNo.ToString().PadLeft(12, '0')));

                        regiserSysNoBarCodeList.Add(new Register
                        {
                            RegisterSysNoPrintCode = barCode
                        });
                    });

                    if (requestView.InvoiceType.HasValue && requestView.StockType.HasValue && requestView.ShippingType.HasValue)
                    {
                        if (requestView.InvoiceType != InvoiceType.SELF || requestView.StockType != StockType.SELF || requestView.ShippingType != ECCentral.BizEntity.Invoice.DeliveryType.SELF)
                        {
                            var merchantInfo = ObjectFactory<IPOBizInteract>.Instance.GetVendorInfoSysNo(requestView.MerchantSysNo.Value);

                            string briefName = merchantInfo.VendorBasicInfo.VendorBriefName ?? (merchantInfo.VendorBasicInfo.VendorNameLocal ?? "");
                            if (briefName.Length > 5)
                            {
                                briefName = briefName.Substring(0, 6);
                            }

                            DataTable t = new DataTable();
                            t.Columns.Add("RegisterSysNoPrintCode");

                            regiserSysNoBarCodeList.ForEach(p=>{
                                t.Rows.Add(p.RegisterSysNoPrintCode);
                            });

                            variables.Add("RequestID", requestView.RequestID);
                            variables.Add("MerchantName", briefName);

                            tableVariables.Add("RegisterView", t);                           
                        }
                    }
                }
            }
        }

        #endregion
    }
}
