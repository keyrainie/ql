using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;

using IPP.OrderMgmt.JobV31.DataAccess;
using System.Threading;
using Newegg.Oversea.Framework.ExceptionHandler;
using WMSWaitInStock.ServiceAdapter;
using IPP.Oversea.CN.POASNMgmt.BusinessEntities;

namespace WMSWaitInStock
{
    public class WMSInStockJob : IJobAction
    {
        public void Run(JobContext context)
        {
            //获取待入库的采购单列表
            var list = PODA.GetPOList();

            
            foreach (var item in list)
            {
                try
                {
                    FillItemAndPOInfo(item);

                    //Validation(so);

                    InStock(item);
                   
                }
                catch (Exception ex)
                {

                    Dealfault(ex);
                    continue;

                }
            }
        }

        public static void FillItemAndPOInfo(POEntity po)
        {

            po.POItems = PODA.GetPOItemListByPOSysNo(po.SysNo.Value);
 
        }

        public static void Dealfault(Exception ex)
        {
            Console.WriteLine("调服务异常" + ex.ToString());

            ThreadPool.QueueUserWorkItem(o =>
            {
                ExceptionHelper.HandleException(ex);
            });

        }

        ///// <summary>
        ///// 校验so是否满足出库条件
        ///// </summary>
        ///// <param name="so"></param>
        //private bool Validation(SOEntity so)
        //{

        //    if (so.SOItemList == null || so.SOItemList.Count == 0)
        //    {
        //        throw new Exception("没有商品信息"); 
        //    }
            
        //    //支付方式
        //    if (so.PaymentMethod == 0 && so.IsPayWhenRecv != 0)
        //    {
        //        throw new Exception("支付方式异常");
        //    }

        //    //应收款验证
        //    if(so.ReceivableAmt==0)
        //    {
        //        if (so.PaymentMethod.Value != 0 
        //            || so.PayTypeSysNo==null)
        //        {
        //            throw new Exception("代收款异常");
 
        //        }

        //    }

        //    //


        //    return true;
        //}

        /// <summary>
        /// 调用WMS接口,导入入库单
        /// </summary>
        /// <param name="so"></param>
        private void InStock(POEntity po)
        {
            ThirdPartServiceAdapter.InStock(po);
            
        }
    }
}
