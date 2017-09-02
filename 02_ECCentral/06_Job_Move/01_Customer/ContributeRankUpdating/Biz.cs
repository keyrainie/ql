using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using Newegg.Oversea.Framework.Biz;

namespace ContributeRankUpdating
{
    public class Biz : IJobAction
    {
        #region IJobAction Members
        public void ShowMessage(JobContext context,string message) 
        {
            context.Message = message;
           // Console.WriteLine(context.Message);
        }
        public void Run(JobContext context)
        {
            ShowMessage(context,"JobStart");
            List<CustomerSysnoList> list = DA.GetCustomerSysnoList();
            
            if (list != null && list.Count > 0)
            {
                ShowMessage(context, "Get Customer Count:" + list.Count);
                int updateCount = 0;
                try
                {
                    foreach (var Customer in list)
                    {
                        string ContributeRank = GetContributeRankByUserSysNo(Customer.CustomerSysno, "8601");  //BusinessContext.Current.CompanyCode
                        if (!string.IsNullOrEmpty(ContributeRank))
                        {
                            DA.updateCustomer_Extend(Customer.CustomerSysno, ContributeRank);
                            updateCount++;
                        }
                    }
                    ShowMessage(context, "Update Customer Count:" + updateCount);
                }
                catch (Exception e)
                {
                    ShowMessage(context, e.Message);
                }
            }
            
            //ShowMessage(context,"JobSuccess");
           // Console.ReadLine();
        }        

        #endregion

        private string GetContributeRankByUserSysNo(int customerSysNo, string companyCode)
        {
            int guideCount,reviewCount,consultReplyCount;
            DA.GetCustomerContributeInfo(customerSysNo, companyCode, out guideCount, out reviewCount, out consultReplyCount);
            return GetRankString(guideCount, reviewCount, consultReplyCount);
        }

        public string GetRankString(int customerGuideCount, int reviewCount, int consultReplyCount)
        {
            if (customerGuideCount == 0)
            {
                return "";
            }
            else if (customerGuideCount > 0 && customerGuideCount < 5)
            {
                if ((ExchangeCustomerGuideCount(reviewCount, 1) + customerGuideCount >= 5)
                    || (ExchangeCustomerGuideCount(consultReplyCount, 2) + customerGuideCount >= 5))
                {
                    return "T";
                }
                else
                {
                    return "";
                }
            }
            else if (customerGuideCount >= 5 && customerGuideCount < 20)
            {
                if ((ExchangeCustomerGuideCount(reviewCount, 1) + customerGuideCount >= 20)
                    || (ExchangeCustomerGuideCount(consultReplyCount, 2) + customerGuideCount >= 20))
                {
                    return "L";
                }
                else
                {
                    return "T";
                }
            }
            else if (customerGuideCount >= 20 && customerGuideCount < 50)
            {
                if ((ExchangeCustomerGuideCount(reviewCount, 1) + customerGuideCount >= 50)
                    || (ExchangeCustomerGuideCount(consultReplyCount, 2) + customerGuideCount >= 50))
                {
                    return "A";
                }
                else
                {
                    return "L";
                }
            }
            else if (customerGuideCount >= 50 && customerGuideCount < 100)
            {
                if ((ExchangeCustomerGuideCount(reviewCount, 1) + customerGuideCount >= 100)
                    || (ExchangeCustomerGuideCount(consultReplyCount, 2) + customerGuideCount >= 100))
                {
                    return "P";
                }
                else
                {
                    return "A";
                }
            }
            else
            {
                return "P";
            }
        }

        private int ExchangeCustomerGuideCount(int count, int type)
        {
            if (type == 1)
            {
                return count / 10;
            }
            else
            {
                return count / 20;
            }
        }
    }
}