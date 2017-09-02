using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.DataAccess;
using IPP.InventoryMgmt.JobV31.Common;
using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.Contract;
using IPP.ThirdPart.Interfaces.Contract;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using IPP.ThirdPart.Interfaces.Interface;
using Newegg.Oversea.Framework.ExceptionHandler;
using IPP.InventoryMgmt.JobV31.BusinessEntities;

namespace IPP.InventoryMgmt.JobV31.BIZ
{
    public class ThirdPartInventoryBP : ThirdPartInventoryBPBase
    {
        public ThirdPartInventoryBP(CommonConst Common) : base(Common) { }

        public ThirdPartInventoryBP() : base() { }

        private void SynInvnetoryQty(List<TaoBaoSKUMsg> TaoBaoSKUList)
        {
            TaoBaoRequest request = new TaoBaoRequest();
            request.TaoBaoSKUList = TaoBaoSKUList;
            request.Header = Util.CreateServiceHeader(Common);
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
            finally
            {
                ServiceBroker.DisposeService<ITaoBaoMaintain>(service);
            }
        }

        protected override void On_Running(object sender, ThirdPartInventoryArgs args)
        {
            List<ThirdPartInventoryEntity> qtyList = args.ThirdPartInventoryList;
            List<TaoBaoSKUMsg> list = new List<TaoBaoSKUMsg>();

            foreach (ThirdPartInventoryEntity entity in qtyList)
            {
                TaoBaoSKUMsg skuMsg = new TaoBaoSKUMsg();
                int qty = Addapter.CalculateInventoryQty.CalculateSynQty(entity);

                #region 2011-10-26 Midify by Kilin 新商品同步库存时，淘宝库存默认为一个
                if (!entity.InventoryAlamQty.HasValue)
                {
                    qty -= 1;
                }
                #endregion
                skuMsg.Quantity = qty.ToString();
                skuMsg.Type = "2";
                skuMsg.SKU = entity.SKU;
                list.Add(skuMsg);
            }

            SynInvnetoryQty(list);
        }
    }
}
