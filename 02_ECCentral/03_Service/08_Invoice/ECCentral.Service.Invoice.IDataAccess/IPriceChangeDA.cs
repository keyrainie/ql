using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.PriceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IPriceChangeDA
    {
        int SavePriceChangeMaster(PriceChangeMaster item);

        void SavePriceChangeItem(PriceChangeItem item);

        void DeletePriceChangeItemByMasterSysNo(int sysNo);

        PriceChangeMaster UpdatePriceChangeMaster(PriceChangeMaster item);

        PriceChangeMaster GetPriceChangeBySysNo(int sysNo);

        List<PriceChangeMaster> GetPriceChangeByStatus(RequestPriceStatus status);

        void UpdatePriceChangeStatus(PriceChangeMaster item);

        bool IsExistsAuditedOrRuningProduct(PriceChangeItem item);

        void UpdateRealBeginDate(int sysNo);

         /// <summary>
        /// 新单压就单，使旧单的状态为不可用状态
        /// </summary>
        /// <param name="itmeSysNo"></param>
        void DisableOldChangeItemStatusByNewItemSysNo(int itmeSysNo);
    }
}
