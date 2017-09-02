using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using IPP.OrderMgmt.JobV31.Dac;
using IPP.OrderMgmt.JobV31.BusinessEntities;
using IPP.OrderMgmt.JobV31.ServiceAdapter;
using System.Threading;
using Newegg.Oversea.Framework.ExceptionHandler;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.OrderMgmt.JobV31.Biz
{
    public class GroupBuySOMarker
    {

        private static string UserDisplayName;
        private static string UserLoginName;
        private static string CompanyCode;
        private static string StoreCompanyCode;
        private static string StoreSourceDirectoryKey;

        private static void GetConfig()
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
        public static void Process(JobContext context)
        {

            GetConfig();

            List<int> soList = GroupBuyProcessDA.GetSOSysNosNeedMark();

            if (soList != null)
            {
                foreach (int soSysNo in soList)
                {
                    Console.WriteLine(string.Format("订单：{0}",soSysNo));
                    try
                    {
                        //获取所有Item
                        List<GroupBuyItemEntity> items = GroupBuyProcessDA.GetGroupBuyItemBySOSysNo(soSysNo);

                        if (items == null || items.Count == 0)
                            continue;

                        //如果含有未处理的Item就过
                        if (items.Exists(x => x.ReferenceSysNo != 0 && string.IsNullOrEmpty(x.SettlementStatus)))
                        {
                            continue;
                        }
                        //如果含有F
                        else if (items.Exists(x => x.SettlementStatus == "F"))
                        {
                            //更新Checkshipping
                            UpdateSOGroupBuyStatus(soSysNo, "F");
                        }
                        //如果含有P
                        else if (items.Exists(x => x.SettlementStatus == "P"))
                        {
                            //只含P,作废
                            if (!items.Exists(x => x.SettlementStatus != "P"))
                            {
                                if (ExistsNetPay(soSysNo))
                                {
                                    AuditNetPay(soSysNo);

                                    CreateAO(soSysNo);
                                }

                                AbandonSO(soSysNo);

                            }
                            //还有其他商品
                            else
                            {
                                //更新Checkshipping
                                UpdateSOGroupBuyStatus(soSysNo, "P");

                            }
                        }
                        //全S
                        else if (!items.Exists(x => x.ReferenceSysNo != 0 && x.SettlementStatus != "S"))
                        {

                            //审核NetPay
                            if (ExistsNetPay(soSysNo))
                            {
                                AuditNetPay(soSysNo);
                            }

                            //修改CheckShippingSettlement
                            UpdateSOGroupBuyStatus(soSysNo, "S");

                        }
                    }
                    catch (Exception ex)
                    {
                        
                        UpdateSOGroupBuyStatus(soSysNo, "F");

                        context.Message += ex.ToString();
                        Dealfault(ex);
                        continue;
                    }

                }

            }

        }

        public static void Dealfault(Exception ex)
        {
            Console.WriteLine(ex.ToString());

            ThreadPool.QueueUserWorkItem(o =>
            {
                ExceptionHelper.HandleException(ex);
            });

        }

        public static void UpdateSOGroupBuyStatus(int soSysNo, string status)
        {
            SODA.UpdateSOGroupBuyStatus(soSysNo, status, CompanyCode);
        }

        public static bool ExistsNetPay(int soSysNo)
        {
            return CommonDA.ExistsNetPay(soSysNo, CompanyCode);
        }

        public static void AuditNetPay(int soSysNo)
        {
            InvoiceServiceAdapter.AuditNetPay(soSysNo);
        }

        public static void CreateAO(int soSysNo)
        {
            InvoiceServiceAdapter.CreateAO(soSysNo);

        }

        public static void AbandonSO(int soSysNo)
        {
            OrderServiceAdapter.AbandonSO(soSysNo);
        }



    }
}
