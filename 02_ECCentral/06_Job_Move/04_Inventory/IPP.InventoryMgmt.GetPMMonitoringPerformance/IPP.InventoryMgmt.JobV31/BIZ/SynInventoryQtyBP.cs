using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.Taobao.JobV31.BusinessEntities;
using IPP.InventoryMgmt.Taobao.JobV31.Provider;
using Newegg.Oversea.Framework.Entity;
using IPP.ThirdPart.Interfaces.Contract;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using IPP.ThirdPart.Interfaces.Interface;
using Newegg.Oversea.Framework.Contract;
using IPP.InventoryMgmt.Taobao.JobV31.Common;
using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.ExceptionHandler;

namespace IPP.InventoryMgmt.Taobao.JobV31.BIZ
{
    internal class SynInventoryQtyBP : SynInventoryQtyBase
    {
        private void SynInvnetoryQty(List<InventoryQtyEntity> inventoryQtyList)
        {
            TaoBaoRequest request = new TaoBaoRequest();
            request.TaoBaoSKUList = new List<TaoBaoSKUMsg>();

            foreach (InventoryQtyEntity entity in inventoryQtyList)
            {
                TaoBaoSKUMsg skuMsg = new TaoBaoSKUMsg();
                skuMsg.Quantity = entity.SynInventoryQty.ToString();
                skuMsg.Type = "2";
                skuMsg.SKU = entity.SKU;                
                request.TaoBaoSKUList.Add(skuMsg);
            }
            request.Header = Util.CreateServiceHeader();
            ITaoBaoMaintain service = ServiceBroker.FindService<ITaoBaoMaintain>();
            try
            {
                TaoBaoResponse response = service.TaoBaoItemQantityUpdate(request);
                
                if (response != null && response.Faults != null && response.Faults.Count > 0)
                {
                    MessageFault msg = response.Faults[0];
                    BusinessException ex = new BusinessException(msg.ErrorCode, string.Format("{0}\r\n{1}", msg.ErrorDescription, msg.ErrorDetail), true);

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
                throw ex;
            }
            finally
            {
                ServiceBroker.DisposeService<ITaoBaoMaintain>(service);
            }
        }

        protected override void On_RunningBefor(object sender, InventoryQtyArgs args)
        {
            SynInvnetoryQty(args.InventoryQtyEntityList);
        }
    }




}
