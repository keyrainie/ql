using System;
using System.Collections.Specialized;
using System.Text;

using ECCentral.BizEntity.RMA;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.RMA.AppService
{
    public class RequestPrintData : IPrintDataBuild
    {
        #region IPrintDataBuild Members

        public void BuildData(NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();
            string sysNo = requestPostData["SysNo"];
            if (!string.IsNullOrEmpty(sysNo))
            {
                StringBuilder sbRegisterSysNo = new StringBuilder();
                StringBuilder sbCustomerDesc = new StringBuilder();
                StringBuilder sbBriefNames = new StringBuilder();
                CustomerInfo customer;
                SOBaseInfo soBaseInfo;
                DeliveryInfo deliveryInfo;
                int requestSysNo;
                string deliveryUserName, businessModel;

                if (int.TryParse(sysNo, out requestSysNo))
                {
                    var request = ObjectFactory<RequestAppService>.Instance.LoadWithRegistersBySysNo(requestSysNo, out customer, out soBaseInfo, out deliveryInfo, out deliveryUserName, out businessModel);
                    if (request.Registers != null)
                    {
                        foreach (RMARegisterInfo register in request.Registers)
                        {
                            sbRegisterSysNo.Append(string.Format("{0}, ", register.SysNo.Value.ToString()));
                            sbBriefNames.Append(string.Format("{0}<br />", register.BasicInfo.ProductName));
                            sbCustomerDesc.Append(string.Format("{0}<br />", register.BasicInfo.CustomerDesc));
                        }
                    }
                    variables.Add("Now", DateTime.Now.ToString("yyyy-MM-dd"));
                    variables.Add("SOSysNo", request.SOSysNo);
                    variables.Add("Contact", request.Contact);
                    variables.Add("Phone", request.Phone);
                    variables.Add("Address", request.Address);
                    variables.Add("RegisterSysNoString", sbRegisterSysNo.ToString().Substring(0, sbRegisterSysNo.Length - 2));
                    variables.Add("BriefNameString", sbBriefNames.ToString());
                    variables.Add("CustomerDescString", sbCustomerDesc.ToString());
                }
            }
        }

        #endregion
    }
}
