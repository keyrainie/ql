using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;
using System.Transactions;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(SaleAdvAppService))]
    public class SaleAdvAppService
    {
        private readonly SaleAdvertisementProcessor processor = ObjectFactory<SaleAdvertisementProcessor>.Instance;

        public virtual SaleAdvertisement LoadBySysNo(int sysNo)
        {            
            return processor.LoadBySysNo(sysNo);
        }        
       
        public virtual SaleAdvertisement Create(SaleAdvertisement entity)
        {
            return processor.Create(entity);
        }

        public virtual void Update(SaleAdvertisement entity)
        {
            processor.Update(entity);
        }

        public virtual void UpdateIsHoldSaleAdvertisementBySysNo(SaleAdvertisement entity)
        {
            processor.UpdateIsHoldSaleAdvertisementBySysNo(entity);
        }

        public virtual SaleAdvertisementItem CreateItem(SaleAdvertisementItem entity)
        {
            return processor.CreateItem(entity);
        }

        public virtual void UpdateItem(SaleAdvertisementItem entity)
        {
            processor.UpdateItem(entity);
        }

        public virtual void BatchUpdateItem(List<SaleAdvertisementItem> list)
        {
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                list.ForEach(p =>
                {
                    processor.UpdateItem(p);
                });

                scope.Complete();
            }
        }

        public virtual void UpdateItemStatus(SaleAdvertisementItem entity)
        {
            processor.UpdateItemStatus(entity);
        }

        public virtual void BatchUpdateItemStatus(List<SaleAdvertisementItem> list)
        {
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                list.ForEach(p =>
                {
                    processor.UpdateItemStatus(p);
                });

                scope.Complete();
            }
        }

        public virtual void DeleteItem(int sysNo)
        {
            processor.DeleteItem(sysNo);
        }

        public virtual void BatchDeleteItem(List<int> list)
        {
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                list.ForEach(p =>
                {
                    processor.DeleteItem(p);
                });

                scope.Complete();
            }
        }

        public virtual SaleAdvertisementGroup CreateSaleAdvGroup(SaleAdvertisementGroup entity)
        {
            return processor.CreateSaleAdvGroup(entity);
        }

        public virtual SaleAdvertisementGroup UpdateSaleAdvGroup(SaleAdvertisementGroup entity)
        {
            return processor.UpdateSaleAdvGroup(entity);            
        }

        public virtual void DeleteSaleAdvGroup(int sysNo)
        {
            processor.DeleteSaleAdvGroup(sysNo);
        }
    }
}
