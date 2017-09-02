using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using IPP.OrderMgmt.JobV31.BusinessEntities;
using IPP.OrderMgmt.JobV31.Dac;
using IPP.OrderMgmt.JobV31.ServiceAdapter;
using System.Threading;
using Newegg.Oversea.Framework.ExceptionHandler;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.OrderMgmt.JobV31.Biz
{
    public class GroupBuyProcessV2
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

            //获取团购信息
            List<ProductGroupBuyingEntity> groupList = SODA.GetGroupBuyNeedProcess(CompanyCode);


            if (groupList == null || groupList.Count == 0)
            {
                Console.WriteLine("没有符合的团购信息");
                return;
            }


            Console.WriteLine(string.Format("本次获取团购数量：{0}", groupList.Count));

            foreach (ProductGroupBuyingEntity group in groupList)
            {

                //团购结束未处理
                if (group.Status == "F" && group.IsSettlement == "N" && group.SuccessDate == null)
                {
                    //团购失败
                    FailedGroupBuyProcess(group, context);
                }
                else if ((group.Status == "F" && group.IsSettlement == "N" && group.SuccessDate != null) || group.IsSettlement == "C")
                {
                    //团购成功
                    SuccessfulGroupBuyProcess(group, context);
                }

               
            }


        }



        private static void FailedGroupBuyProcess(ProductGroupBuyingEntity group,JobContext context)
        {
            List<GroupBuyItemEntity> GroupBuyItemList = GroupBuyProcessDA.GetGroupBuyItems(group.SystemNumber);

            if (GroupBuyItemList == null || GroupBuyItemList.Count == 0)
            {
                Console.WriteLine("没有团购商品");
            }

            Console.WriteLine(string.Format("团购号:{0}", group.SystemNumber));

            foreach (GroupBuyItemEntity groupBuyItem in GroupBuyItemList)
            {
                Console.WriteLine(string.Format("订单:{0},商品{1}", groupBuyItem.SOSysNo, groupBuyItem.ProductSysNo));

                int ItemExSysNo = groupBuyItem.SysNo;
                int soSysNo = groupBuyItem.SOSysNo;
                int groupBuySysNo = groupBuyItem.ReferenceSysNo;
                int productSysNo = groupBuyItem.ProductSysNo;
                //作废或标记订单为P
                try
                {
                    UpdateItemSettlementStatus(ItemExSysNo, "P");
                }
                catch (Exception ex)
                {
                    UpdateItemSettlementStatus(ItemExSysNo, "F");
                    UpdateSOGroupBuyStatus(soSysNo, "F");
                    context.Message += ex.ToString();
                    Dealfault(ex);
                    continue;
                }
            }

            EndGroupBuying(group.SystemNumber);
        }

        private static void SuccessfulGroupBuyProcess(ProductGroupBuyingEntity group, JobContext context)
        {
            List<GroupBuyItemEntity> GroupBuyItemList = GroupBuyProcessDA.GetGroupBuyItems(group.SystemNumber);

            if (GroupBuyItemList == null || GroupBuyItemList.Count == 0)
            {
                Console.WriteLine("没有团购商品");
            }

            Console.WriteLine(string.Format("团购号:{0}", group.SystemNumber));

            foreach (GroupBuyItemEntity groupBuyItem in GroupBuyItemList)
            {
                Console.WriteLine(string.Format("订单:{0},商品{1}", groupBuyItem.SOSysNo, groupBuyItem.ProductSysNo));

                int ItemExSysNo=groupBuyItem.SysNo;
                int soSysNo = groupBuyItem.SOSysNo;
                int groupBuySysNo = groupBuyItem.ReferenceSysNo;
                int productSysNo=groupBuyItem.ProductSysNo;
                //更新订单
                try
                {
                    //修改团购商品价格
                    UpdateSO(soSysNo, groupBuySysNo, productSysNo);

                    //修改团购Item状态
                    UpdateItemSettlementStatus(ItemExSysNo, "S");

                }
                catch (Exception ex)
                {
                    UpdateItemSettlementStatus(ItemExSysNo, "F");
                    UpdateSOGroupBuyStatus(soSysNo, "F");
                    context.Message += ex.ToString();
                    Dealfault(ex);
                    continue;
                }
            }
            

            if (group.Status == "F")
            {
                EndGroupBuying(group.SystemNumber);
            }

        }


        public static void UpdateSO(int soSysNo, int groupBuySysNo, int productSysNo)
        {
            //OrderServiceAdapter.UpdateSO4GroupBuying(soSysNo);
            OrderServiceAdapter.UpdateSO4GroupBuyingV2(soSysNo, groupBuySysNo, productSysNo);
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
            Console.WriteLine(ex.ToString());

            ThreadPool.QueueUserWorkItem(o =>
            {
              ExceptionHelper.HandleException(ex);
            });

        }

        public static int UpdateItemSettlementStatus(int itemSysNo,string settlementStatus)
        {
            return GroupBuyProcessDA.UpdateItemSettlementStatus(itemSysNo, settlementStatus);
 
        }

        public static bool ExistsNetPay(int soSysNo)
        {
            return CommonDA.ExistsNetPay(soSysNo, CompanyCode);
        }

        public static bool IsItemAllSettled(int soSysNo)
        {
            return GroupBuyProcessDA.IsItemAllSettled(soSysNo);
 
        }

        public static void UpdateSOGroupBuyStatus(int soSysNo, string status)
        {
            SODA.UpdateSOGroupBuyStatus(soSysNo, status, CompanyCode);
        }

        public static void EndGroupBuying(int groupBuyingSysNo)
        {
            CommonDA.ChangeGroupBuySettlement(groupBuyingSysNo, "Y", CompanyCode);

        }

        public static bool IsOnlyHaveFailItem(int soSysNo, int productSysNo)
        {
            return GroupBuyProcessDA.IsOnlyHaveFailItem(soSysNo,productSysNo);
        }

        public static bool IsPartlyFail(int soSysNo)
        {
            return GroupBuyProcessDA.IsPartlyFail(soSysNo);
 
        }


    }
}
