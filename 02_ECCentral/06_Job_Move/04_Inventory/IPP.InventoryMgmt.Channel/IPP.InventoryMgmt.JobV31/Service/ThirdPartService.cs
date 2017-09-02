using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.InventoryMgmt.JobV31.BusinessEntities;
using IPP.ThirdPart.Interfaces.Interface;
using IPP.ThirdPart.Interfaces.Contract;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using Newegg.Oversea.Framework.ExceptionBase;
using IPP.Oversea.CN.InventoryMgmt.JobV31.Common;

namespace IPP.Oversea.CN.InventoryMgmt.JobV31.Service
{
    public class ThirdPartService
    {
        public static void Sync(ChannelProductEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
#if DEBUG
            ISyncChannelInventoryV31 service = ServiceBroker.FindService<ISyncChannelInventoryV31>("CN.InventoryMgmt.UI.IPP01", "LocalDev");
#else
            ISyncChannelInventoryV31 service = ServiceBroker.FindService<ISyncChannelInventoryV31>();
#endif
            ChannelInventoryRequestV31 request = new ChannelInventoryRequestV31();
            request.Body = new List<ChannelInventoryMsg>();
            ChannelInventoryMsg msg = new ChannelInventoryMsg
            {
                ChannelSysNo = entity.ChannelSysNo,
                ChannelInventory = entity.SynChannelQty,
                ProductSysNo = entity.ProductSysNo,
                SynProductID = entity.SynProductID,
                SkuID=entity.SkuID
                
            };
            request.Body.Add(msg);

            request.Header = Util.CreateMessageHeader();

            ChannelInventoryResponseV31 response = service.Sync(request);

            if (response == null)
            {
                throw new BusinessException("SyncChannelInventoryJobError",
                    string.Format("ThirdPart Service返回NULL值,product:{0},channel:{1}({2})  同步失败",
                    entity.ProductSysNo, entity.ChannelSysNo, entity.ChannelCode));
            }
            if (response.Faults != null
                && response.Faults.Count > 0)
            {
                var fault = response.Faults[0];
                throw new BusinessException(fault.ErrorCode, string.IsNullOrEmpty(fault.ErrorDetail) ? fault.ErrorDescription : fault.ErrorDetail);
            }
            if (response.Body == null
                || response.Body.Count == 0)
            {
                throw new BusinessException("SyncChannelInventoryJobError",
                    string.Format("ThirdPart Service返回未知错误,product:{0},channel:{1}({2})  同步失败",
                    entity.ProductSysNo, entity.ChannelSysNo, entity.ChannelCode));
            }
            msg = response.Body[0];
            if (msg.FaultMsg != null)
            {
                BusinessException ex = new BusinessException(msg.FaultMsg.ErrorCode,
                    string.Format("product:{0},channel:{1}({2}) 同步失败\r\n{3}\r\n{4}",
                    entity.ProductSysNo,
                    entity.ChannelSysNo,
                    entity.ChannelCode,
                    msg.FaultMsg.ErrorDescription,
                    msg.FaultMsg.ErrorDetail));
                throw ex;
            }
        }
    }
}
