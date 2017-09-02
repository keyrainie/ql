using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using IPP.OrderMgmt.JobV31.Dac;
using IPP.OrderMgmt.JobV31.ServiceAdapter;
using System.Threading;
using Newegg.Oversea.Framework.ExceptionHandler;

namespace IPP.OrderMgmt.JobV31.Biz
{
    public class AbandonGroupBuySOV2
    {
        private static string UserDisplayName;
        private static string UserLoginName;
        private static string CompanyCode;
        private static string StoreCompanyCode;
        private static string StoreSourceDirectoryKey;

        private static void GetAutoAuditInfo()
        {
            UserDisplayName = ConfigurationManager.AppSettings["UserDisplayName"];
            UserLoginName = ConfigurationManager.AppSettings["UserLoginName"];
            CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
            StoreCompanyCode = ConfigurationManager.AppSettings["StoreCompanyCode"];
            StoreSourceDirectoryKey = ConfigurationManager.AppSettings["StoreSourceDirectoryKey"];
        }

        /// <summary>
        /// 处理团购
        /// </summary>
        public static void Process()
        {
            GetAutoAuditInfo();

            //获取SO
            List<int> soSysNolist = SODA.GetGroupBuySONotPayInTimeV2(CompanyCode);

            foreach (int soSysNo in soSysNolist)
            {
                Console.WriteLine(string.Format("订单:{0}", soSysNo));
                //SOEntity soEntity = SODA.GetSOEntity(soSysNo, CompanyCode);
                try
                {
                    AbandonSO(soSysNo);
                }
                catch (Exception e)
                {
                    Dealfault(e);
                    continue;
                }
            }
        }

        public static void AbandonSO(int soSysNo)
        {
            OrderServiceAdapter.AbandonSO(soSysNo);
        }

        public static void Dealfault(Exception ex)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                ExceptionHelper.HandleException(ex);
            });

        }








    }
}
