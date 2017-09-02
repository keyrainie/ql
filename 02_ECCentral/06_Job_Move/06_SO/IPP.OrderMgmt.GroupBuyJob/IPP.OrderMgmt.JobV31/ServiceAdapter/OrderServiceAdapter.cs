using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.OrderMgmt.ServiceInterfaces;
using IPP.Oversea.CN.OrderMgmt.ServiceInterfaces.DataContracts;
using IPP.Oversea.CN.OrderMgmt.ServiceInterfaces.ServiceContracts;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using Newegg.Oversea.Framework.Contract;


namespace IPP.OrderMgmt.JobV31.ServiceAdapter
{
    public class OrderServiceAdapter
    {
        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="soSysNo"></param>
        public static void UpdateSO4GroupBuying(int soSysNo)
        {
            
            SimpleTypeDataContract<int> SysNo=new SimpleTypeDataContract<int>
            {
                Value=soSysNo,
                Header = ServiceAdapterHelper.ApplyMessageHeader()
            };


            IMaintainSOV31 service = ServiceBroker.FindService<IMaintainSOV31>();

            try
            {
                DefaultDataContract msg = service.UpdateSO4GroupBuying(SysNo);
                ServiceAdapterHelper.DealServiceFault(msg.Faults);
            }
            finally
            {
                ServiceBroker.DisposeService<IMaintainSOV31>(service);
            }
        }

        public static void UpdateSO4GroupBuyingV2(int soSysNo,int gourpBuySysNo,int productSysNo)
        {
            SOGroupBuyAjustV31 message = new SOGroupBuyAjustV31() { Body = new SOGroupBuyAjustMsg(), Header = ServiceAdapterHelper.ApplyMessageHeader()};
            message.Body.SOSysNo=soSysNo;
            message.Body.GroupBuySysNo=gourpBuySysNo;
            message.Body.ProductSysNo=productSysNo;

            IMaintainSOV31 service = ServiceBroker.FindService<IMaintainSOV31>();
            try
            {
                DefaultDataContract msg = service.UpdateSO4GroupBuyingV2(message);
                ServiceAdapterHelper.DealServiceFault(msg.Faults);
            }
            finally
            {
                ServiceBroker.DisposeService<IMaintainSOV31>(service);
            }
 
        }

        /// <summary>
        /// 作废订单
        /// </summary>
        /// <param name="soNumber"></param>
        public static void AbandonSO(int soNumber)
        {
            SOV31 msg = new SOV31()
            {
                Body = new SOMsg()
                {
                    SOMaster = new SOMasterMsg()
                    {
                        SystemNumber = soNumber
                    }
                },
                Header=ServiceAdapterHelper.ApplyMessageHeader()
            };


            IMaintainSOV31 service = ServiceBroker.FindService<IMaintainSOV31>();

            try
            {
                
                msg = service.SystemAbandonSO(msg);
                ServiceAdapterHelper.DealServiceFault(msg.Faults);
               
            }
            finally
            {
                ServiceBroker.DisposeService<IMaintainSOV31>(service);
            }
 
        }

    }
}
