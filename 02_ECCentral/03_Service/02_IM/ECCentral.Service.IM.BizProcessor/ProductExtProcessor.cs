using System;
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductExtProcessor))]
    public class ProductExtProcessor
    {
        private readonly IProductExtDA productExtDA = ObjectFactory<IProductExtDA>.Instance;
        /// <summary>
        /// 批量设置是否可以退货
        /// </summary>
        /// <param name="list"></param>
        public virtual void UpdatePermitRefund(List<ProductExtInfo> list)
        {
            foreach (ProductExtInfo item in list)
            {
                productExtDA.UpdatePermitRefund(item);
            }
        }


        public bool UpdateProductExKeyKeywords(int sysNo, string keywords, string keywords0, int editUserSysNo, string companyCode)
        {
            int rtn = 0;
            rtn = productExtDA.UpdateProductExKeyKeywords(sysNo, keywords, keywords0, editUserSysNo, companyCode);

            if (rtn > 0)
                return true;
            else
                return false;
        }

        public ProductBatchManagementInfo UpdateIsBatch(ProductBatchManagementInfo batchManagementInfo)
        {
            using (var scope = TransactionScopeFactory.CreateTransactionScope())
            {
                productExtDA.UpdateIsBatch(batchManagementInfo);

                var batch = productExtDA.GetProductBatchManagementInfo(batchManagementInfo.ProductSysNo.Value);

                if (!string.IsNullOrEmpty(batchManagementInfo.Note) && batchManagementInfo.IsBatch.Value)
                {
                    var log = new ProductBatchManagementInfoLog { Note = batchManagementInfo.Note, BatchManagementSysNo = batch.SysNo };
                    productExtDA.InsertProductBatchManagementLog(log);
                }

                scope.Complete();
            }

            return productExtDA.GetProductBatchManagementInfo(batchManagementInfo.ProductSysNo.Value);
        }
    }
}
