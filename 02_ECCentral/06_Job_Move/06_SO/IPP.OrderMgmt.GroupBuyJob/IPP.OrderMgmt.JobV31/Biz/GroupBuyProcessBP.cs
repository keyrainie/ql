using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.OrderMgmt.JobV31.BusinessEntities;
using System.Configuration;
using IPP.OrderMgmt.JobV31.Dac;
using IPP.OrderMgmt.JobV31.ServiceAdapter;
using System.Threading;
using Newegg.Oversea.Framework.ExceptionHandler;

namespace IPP.OrderMgmt.JobV31.Biz
{
    public class GroupBuyProcessBP
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

            //获取团购信息
            List<ProductGroupBuyingEntity> groupList=SODA.GetGroupBuyNeedProcess(CompanyCode);

            if (groupList == null || groupList.Count == 0)
            {
                Console.WriteLine("没有符合的团购信息");
                return;
            }

            Console.WriteLine(string.Format("本次获取团购数量：{0}",groupList.Count));

            foreach (ProductGroupBuyingEntity group in groupList)
            {

                //团购结束未处理
                if (group.Status=="F" && group.IsSettlement=="N" && group.SuccessDate == null)
                {
                    //团购失败
                    FailedGroupBuyProcess(group);
                }
                else if (group.Status == "F" && group.IsSettlement == "N" && group.SuccessDate != null)
                {
                    //团购成功
                    SuccessfulGroupBuyProcess(group);
                }

                //团购到达峰值
                if (group.IsSettlement == "C")
                {
                    
                    FullGroupBuyProcess(group);
 
                }
            }

        }


        public static void FullGroupBuyProcess(ProductGroupBuyingEntity group)
        {
            List<int> soSysNolist = SODA.GetSOSysNoListByReferenceSysno(group.SystemNumber, CompanyCode);

            Console.WriteLine(string.Format("团购号:{0}", group.SystemNumber));

            foreach (int soSysNo in soSysNolist)
            {
                Console.WriteLine(string.Format("订单:{0}", soSysNo));

                SOEntity soEntity = SODA.GetSOEntity(soSysNo, CompanyCode);


                //更新订单
                try
                {
                    UpdateSO(soSysNo);

                    if (ExistsNetPay(soSysNo))
                    {
                        AuditNetPay(soSysNo);
                    }
                }
                catch (Exception ex)
                {
                    UpdateSOGroupBuyStatus(soSysNo, "F");
                    Dealfault(ex);
                    continue;
                }

                UpdateSOGroupBuyStatus(soSysNo, "S");

            }

            if (group.Status == "F")
            {
                //EndGroupBuying(group.SystemNumber);
            }

        }


        public static void SuccessfulGroupBuyProcess(ProductGroupBuyingEntity group)
        {
            //获取需要处理的订单号
            List<int> soSysNolist = SODA.GetSOSysNoListByReferenceSysno(group.SystemNumber, CompanyCode);

            Console.WriteLine(string.Format("团购号:{0}",group.SystemNumber));

            foreach(int soSysNo in soSysNolist)
            {
                Console.WriteLine(string.Format("订单:{0}", soSysNo));

                SOEntity soEntity = SODA.GetSOEntity(soSysNo, CompanyCode);


                //更新订单
                try 
                {
                    UpdateSO(soSysNo);

                    if (ExistsNetPay(soSysNo))
                    {
                        AuditNetPay(soSysNo);
                    }
                }
                catch (Exception ex)
                {
                    UpdateSOGroupBuyStatus(soSysNo, "F");
                    Dealfault(ex);
                    continue;
                }

                UpdateSOGroupBuyStatus(soSysNo, "S");

            }

            //EndGroupBuying(group.SystemNumber);
   

        }


        public static void FailedGroupBuyProcess(ProductGroupBuyingEntity group)
        {
            //获取需要处理的订单号
            List<int> soSysNolist = SODA.GetSOSysNoListByReferenceSysno(group.SystemNumber, CompanyCode);

            Console.WriteLine(string.Format("团购号:{0}", group.SystemNumber));

            foreach (int soSysNo in soSysNolist)
            {
                Console.WriteLine(string.Format("订单:{0}", soSysNo));

                SOEntity soEntity = SODA.GetSOEntity(soSysNo, CompanyCode);


                //审核NetPay
                try
                {
                    if (ExistsNetPay(soSysNo))
                    {
                        AuditNetPay(soSysNo);

                        CreateAO(soSysNo);
                    }

                    AbandonSO(soSysNo);
                    
                }
                catch (Exception ex)
                {
                    UpdateSOGroupBuyStatus(soSysNo, "F");
                    Dealfault(ex);
                    continue;
                }

                UpdateSOGroupBuyStatus(soSysNo, "S");

                SendFailedMail(soEntity,group);

            }

            //EndGroupBuying(group.SystemNumber);

        }


        public static void UpdateSO(int soSysNo)
        {
            OrderServiceAdapter.UpdateSO4GroupBuying(soSysNo);
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

        public static void Dealfault(Exception ex)
        {
            Console.WriteLine(string.Format("{0}|{1}",ex.Message, ex.StackTrace));

            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    ExceptionHelper.HandleException(ex);
                }
                catch (Exception e)
                {
                   
                }
            });

        }

        public static bool ExistsNetPay(int soSysNo)
        {
            return CommonDA.ExistsNetPay(soSysNo, CompanyCode);
        }


        public static void WriteLog(int sysNo, string type,string memo)
        {
            CommonDA.WriteProcessLog(sysNo,type,memo);
        }

        public static void SendFailedMail(SOEntity soEntity, ProductGroupBuyingEntity group)
        {

           ThreadPool.QueueUserWorkItem(o =>
           {
               SendMailBP.SendFailedMail4SO(soEntity, group);
           });
        }


        public static void UpdateSOGroupBuyStatus(int soSysNo,string status)
        {
            SODA.UpdateSOGroupBuyStatus(soSysNo, status, CompanyCode);
        }

        public static void EndGroupBuying( int groupBuyingSysNo)
        {
            CommonDA.ChangeGroupBuySettlement(groupBuyingSysNo, "Y", CompanyCode);

        }

    }
}
