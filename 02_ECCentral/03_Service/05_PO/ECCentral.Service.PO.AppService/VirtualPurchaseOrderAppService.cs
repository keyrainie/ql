using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(VirtualPurchaseOrderAppService))]
    public class VirtualPurchaseOrderAppService
    {
        #region [Fields]
        private VSPOProcessor m_VSPOProcessor;
        private ISOBizInteract m_SOBizInteract;

        public ISOBizInteract SOBizInteract
        {
            get
            {
                if (null == m_SOBizInteract)
                {
                    m_SOBizInteract = ObjectFactory<ISOBizInteract>.Instance;
                }
                return m_SOBizInteract;
            }
        }

        public VSPOProcessor VSPOProcessor
        {
            get
            {
                if (null == m_VSPOProcessor)
                {
                    m_VSPOProcessor = ObjectFactory<VSPOProcessor>.Instance;
                }
                return m_VSPOProcessor;
            }
        }
        #endregion

        public virtual VirtualStockPurchaseOrderInfo LoadVirtualPurchaseOrderInfoBySysNo(int vspoSysNo)
        {
            return VSPOProcessor.LoadVSPOInfo(vspoSysNo);
        }

        public virtual void UpdateVirtualPurchaseInfo(VirtualStockPurchaseOrderInfo vspoInfo)
        {
            VSPOProcessor.UpdateVSPO(vspoInfo);
        }

        public virtual void AbandonVirtualPurchaseInfo(VirtualStockPurchaseOrderInfo info)
        {
            VSPOProcessor.AdandonVSPO(info);
        }

        public virtual void UpdateVirtualPurchaseInfoCSMemo(VirtualStockPurchaseOrderInfo info)
        {
            VSPOProcessor.UpdateCSMemoForVSPO(info);
        }

        public virtual VirtualStockPurchaseOrderInfo LoadVirtualPurchaseInfoBySOItemSysNo(int soSysNo, int productSysNo)
        {
            return VSPOProcessor.LoadVirtualPurchaseInfoBySOItemSysNo(soSysNo, productSysNo);
        }

        public virtual VirtualStockPurchaseOrderInfo CreateVSPO(VirtualStockPurchaseOrderInfo info)
        {
            return VSPOProcessor.CreateVSPO(info);
        }

        public virtual bool IsVSPOItemPriceLimited(int soSysNo, int productSysNo, int purchaseQty)
        {
            return VSPOProcessor.IsVSPOItemPriceLimited(soSysNo, productSysNo, purchaseQty);
        }
    }
}
