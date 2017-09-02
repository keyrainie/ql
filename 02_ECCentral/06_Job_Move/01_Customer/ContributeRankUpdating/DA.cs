using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using System.Xml.Linq;
using System.IO;
using Newegg.Oversea.Framework.Utilities;

namespace ContributeRankUpdating
{
    internal class DA
    {
        public static List<CustomerSysnoList> GetCustomerSysnoList()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCustomerSysnoList");

            List<CustomerSysnoList> result = command.ExecuteEntityList<CustomerSysnoList>();
            return result;
        }

        public static int updateCustomer_Extend(int customerSysno, string contributeRank)
        {
            DataCommand command = DataCommandManager.GetDataCommand("updateCustomer_Extend");
            command.SetParameterValue("@CustomerSysno", customerSysno);
            command.SetParameterValue("@ContributeRank", contributeRank);
            int count = 0;
            count = command.ExecuteNonQuery();
            return count;
        }

        public static void GetCustomerContributeInfo(int customerSysNo,string companyCode,out int guideCount,out int reviewCount,out int consultReplyCount)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCustomerContributeInfo");
            command.SetParameterValue("@CustomerSysno", customerSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);

            command.ExecuteNonQuery();

            guideCount = Convert.ToInt32(command.GetParameterValue("@GuideCount"));
            reviewCount = Convert.ToInt32(command.GetParameterValue("@ReviewCount"));
            consultReplyCount = Convert.ToInt32(command.GetParameterValue("@ConsultReplyCount"));
        }
    }
}