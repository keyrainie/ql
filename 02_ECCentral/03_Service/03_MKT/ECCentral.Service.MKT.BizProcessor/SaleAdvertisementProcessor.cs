using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(SaleAdvertisementProcessor))]
    public class SaleAdvertisementProcessor
    {
        private readonly ISaleAdvertisementDA da = ObjectFactory<ISaleAdvertisementDA>.Instance;

        public virtual SaleAdvertisement LoadBySysNo(int sysNo)
        {
            var result = da.LoadBySysNo(sysNo);
            if (result == null)
            {
                //throw new BizException("页面促销模板信息不存在！");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleAdvertisement", "SaleAdvertisement_NotExistAdvertise"));
            }
            result.Groups = da.LoadSaleAdvGroupsBySaleAdvSysNo(sysNo);
            result.Items = da.GetSaleAdvItems(sysNo);

            if (result.Groups != null && result.Items != null)
            {
                result.Groups.ForEach(p =>
                {
                    p.ItemsCount = result.Items.Count(i => i.GroupSysNo == p.SysNo);
                });
            }
            if (result.Items != null)
            {
                //获取商品和库存的相关信息            
                var productSysNoList = result.Items.Select(p => p.ProductSysNo.Value).ToList();

                //var products = ExternalDomainBroker.GetProductInfoListByProductSysNoList(productSysNoList);
                var inventorys = ExternalDomainBroker.GetProductInventoryInfoByProductSysNoList(productSysNoList);
                result.Items.ForEach(i =>
                {
                    //ProductInfo product = products.FirstOrDefault(p => p.SysNo == i.ProductSysNo);
                    ProductInfo product = ExternalDomainBroker.GetSimpleProductInfo(i.ProductSysNo.Value);
                    if (product != null)
                    {
                        i.ProductID = product.ProductID;
                        i.ProductName = product.ProductName;
                        i.ProductStatus = product.ProductStatus;
                    }

                    var inventory = inventorys.FirstOrDefault(p => p.ProductSysNo == i.ProductSysNo);
                    if (inventory != null)
                    {
                        i.OnlineQty = inventory.OnlineQty;
                    }
                });
            }
            return result;
        }

        public virtual SaleAdvertisement Create(SaleAdvertisement entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (string.IsNullOrEmpty(entity.Name.Content))
            {
                //throw new BizException("页面名称不能为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleAdvertisement", "SaleAdvertisement_PageNameNotNull"));
            }
            if (string.IsNullOrEmpty(entity.Header))
            {
                //throw new BizException("页面头部不能为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleAdvertisement", "SaleAdvertisement_PageTitleNotNull"));
            }
            if (string.IsNullOrEmpty(entity.CssPath))
            {
                entity.CssPath = null;
            }
            if (string.IsNullOrEmpty(entity.Footer))
            {
                entity.Footer = null;
            }

            return da.CreateSaleAdv(entity);
        }

        public virtual void Update(SaleAdvertisement entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.SysNo <= 0)
            {
                //throw new BizException("系统编号不能为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleAdvertisement", "SaleAdvertisement_SysNoNotNull"));
            }
            if (string.IsNullOrEmpty(entity.Name.Content))
            {
                //throw new BizException("页面名称不能为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleAdvertisement", "SaleAdvertisement_PageNameNotNull"));
            }
            if (string.IsNullOrEmpty(entity.Header))
            {
                //throw new BizException("页面头部不能为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleAdvertisement", "SaleAdvertisement_PageTitleNotNull"));
            }

            if (string.IsNullOrEmpty(entity.CssPath))
            {
                entity.CssPath = null;
            }
            if (string.IsNullOrEmpty(entity.Footer))
            {
                entity.Footer = null;
            }

            da.UpdateSaleAdv(entity);
        }

        public void UpdateIsHoldSaleAdvertisementBySysNo(SaleAdvertisement entity)
        {
            da.UpdateIsHoldSaleAdvBySysNo(entity);
        }

        public virtual SaleAdvertisementItem CreateItem(SaleAdvertisementItem entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (string.IsNullOrEmpty(entity.Introduction))
            {
                entity.Introduction = null;
            }
            //UI上可以不选择Group,如果GroupSysNo写入DB为null的话则查不出来
            if (entity.GroupSysNo == null)
            {
                entity.GroupName = null;
                entity.GroupSysNo = 0;
            }


            ProductInfo product = null;
            if (entity.ProductSysNo == null)
            {
                product = ExternalDomainBroker.GetProductInfo(entity.ProductID);
            }
            else
            {
                product = ExternalDomainBroker.GetProductInfo(entity.ProductSysNo.Value);
            }
            if (product == null)
            {
                //throw new BizException("商品无效！");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleAdvertisement", "SaleAdvertisement_InvalidProduct"));
            }
            else
            {
                entity.MarketPrice = product.ProductPriceInfo.BasicPrice;
                entity.ProductSysNo = product.SysNo;
                entity.ProductID = product.ProductID;
                entity.ProductName = product.ProductName;
                entity.ProductStatus = product.ProductStatus;
            }
            if (da.CheckSaleAdvItemDuplicate(entity))
            {
                //throw new BizException("你要创建或修改的记录已经存在了！");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleAdvertisement", "SaleAdvertisement_RecordAreadyExsist"));
            }
#warning To do 获取京东价
            var inventoryInfo = ExternalDomainBroker.GetProductInventoryInfo(entity.ProductSysNo.Value);
            entity.OnlineQty = inventoryInfo.Sum(p => p.OnlineQty);

            entity.Status = ADStatus.Active;
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope())
            {
                da.CreateItem(entity);

                da.CreateSaleAdvItemLog(entity, "A");

                scope.Complete();
            }

            return entity;
        }

        public virtual void UpdateItem(SaleAdvertisementItem entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (string.IsNullOrEmpty(entity.Introduction))
            {
                entity.Introduction = null;
            }

            //UI上可以不选择Group,如果GroupSysNo写入DB为null的话则查不出来
            if (entity.GroupSysNo == null)
            {
                entity.GroupName = null;
                entity.GroupSysNo = 0;
            }

            da.UpdateItem(entity);
        }

        public virtual void UpdateItemStatus(SaleAdvertisementItem entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            da.UpdateItemStatus(entity);
        }

        public virtual void DeleteItem(int sysNo)
        {
            var orgin = da.LoadSaleAdvItemBySysNo(sysNo);
            if (orgin == null)
            {
                //throw new BizException("促销商品不存在！");
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleAdvertisement", "SaleAdvertisement_ProductNotExsist"));
            }

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                da.DeleteItem(sysNo);

                da.CreateSaleAdvItemLog(orgin, "D");//添加日志

                scope.Complete();
            }
        }

        public virtual SaleAdvertisementGroup CreateSaleAdvGroup(SaleAdvertisementGroup entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.SaleAdvSysNo == null)
            {
                throw new ArgumentNullException("entity.SaleAdvSysNo");
            }
            var list = da.LoadSaleAdvGroupsBySaleAdvSysNo(entity.SaleAdvSysNo.Value);
            if (list != null && list.FirstOrDefault(p => p.GroupName == entity.GroupName) != null)
            {
                //throw new BizException(string.Format("创建失败！原因：组名{0}已存在！", entity.GroupName));
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleAdvertisement", "SaleAdvertisement_CreateFailed"));
            }
            entity.Status = ADStatus.Active;

            da.CreateSaleAdvGroup(entity);

            return da.LoadSaleAdvGroupBySysNo(entity.SysNo.Value);
        }

        public virtual SaleAdvertisementGroup UpdateSaleAdvGroup(SaleAdvertisementGroup entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            var list = da.LoadSaleAdvGroupsBySaleAdvSysNo(entity.SaleAdvSysNo.Value);
            if (list != null && list.FirstOrDefault(p => p.GroupName == entity.GroupName && p.SysNo != entity.SysNo) != null)
            {
                //throw new BizException(string.Format("更新失败！原因：组名{0}已存在！", entity.GroupName));
                throw new BizException(ResouceManager.GetMessageString("MKT.SaleAdvertisement", "SaleAdvertisement_UpdateFailed"));
            }

            da.UpdateSaleAdvGroup(entity);

            return da.LoadSaleAdvGroupBySysNo(entity.SysNo.Value);
        }

        public virtual void DeleteSaleAdvGroup(int sysNo)
        {
            da.DeleteSaleAdvGroup(sysNo);
        }
    }
}
