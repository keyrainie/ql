using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using System.Transactions;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(TopItemProcessor))]
    public class TopItemProcessor
    {

        public virtual void SetTopItem(TopItemInfo entity)
        {
            Dictionary<int, int> affectCategory = new Dictionary<int, int>();//用于排重
            affectCategory.Add(entity.CategorySysNo.Value, 0);
            //扩展生效的处理
            if (entity.IsExtend.HasValue && entity.IsExtend.Value)
            {
                PageTypePresentationType currentType = PageTypeUtil.ResolvePresentationType(ModuleType.TopItem, entity.CategoryType.Value.ToString());
                if (currentType == PageTypePresentationType.Category3)
                {
                    var sameC3List = ObjectFactory<ECCategoryProcessor>.Instance.GetRelatedECCategory3SysNo(entity.CategorySysNo.Value);
                    sameC3List.ForEach(item =>
                    {
                        affectCategory.Add(item.SysNo.Value, 0);
                    });
                }
            }
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (var item in affectCategory)
                {
                    entity.CategorySysNo = item.Key;
                    var orgain = ObjectFactory<ITopItemDA>.Instance.QueryTopItem(entity.CategoryType.Value, entity.CategorySysNo.Value);
                    if (orgain != null && orgain.Where(p => p.ProductSysNo.Value == entity.ProductSysNo).Count() > 0)
                    {
                        ObjectFactory<ITopItemDA>.Instance.UpdateTopItemPriority(entity);
                    }
                    else
                    {
                        ObjectFactory<ITopItemDA>.Instance.CreateTopItem(entity);
                    }
                }
                ReSetPriority(entity.CategoryType.Value, entity.CategorySysNo.Value);
                scope.Complete();
            }
        }

        public virtual void CancleTopItem(List<TopItemInfo> list)
        {
            if (list.Count > 0)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    list.ForEach(entity =>
                    {
                        List<TopItemInfo> waitingHandleList = new List<TopItemInfo>();
                        waitingHandleList.Add(entity);
                        PageTypePresentationType currentType = PageTypeUtil.ResolvePresentationType(ModuleType.TopItem, entity.CategoryType.Value.ToString());
                        if (currentType == PageTypePresentationType.Category3 && entity.IsExtend.HasValue && entity.IsExtend.Value)
                        {
                            //处理扩展生效
                            var extendlist = ObjectFactory<ECCategoryProcessor>.Instance.GetRelatedECCategory3SysNo(entity.CategorySysNo.Value);
                            foreach (var item in extendlist)
                            {
                                entity.CategorySysNo = item.SysNo;
                                waitingHandleList.Add(entity);
                            }
                        }
                        waitingHandleList.ForEach(item =>
                        {
                            ObjectFactory<ITopItemDA>.Instance.DeleteTopItem(item);
                        });
                    });
                    scope.Complete();
                }
                ReSetPriority(list[0].CategoryType.Value, list[0].CategorySysNo.Value);
            }
        }

        private void ReSetPriority(int PageType, int RefSysNo)
        {
            //所有置顶的商品
            List<TopItemInfo> all = ObjectFactory<ITopItemDA>.Instance.QueryTopItem(PageType, RefSysNo);
            //已经下架商品列表
            List<ProductInfo> products = ExternalDomainBroker.GetProductInfoListByProductSysNoList(all.Select(p => p.ProductSysNo.Value).ToList<int>()).Where(p => p.ProductStatus == ProductStatus.InActive_Show).ToList();//原代码写的status=0
            List<TopItemInfo> removedList = new List<TopItemInfo>();
            products.ForEach(item =>
            {
                removedList.Add(new TopItemInfo() { ProductSysNo = item.SysNo, CategorySysNo = RefSysNo, CategoryType = PageType });
            });
            CancleTopItem(removedList);
            all = ObjectFactory<ITopItemDA>.Instance.QueryTopItem(PageType, RefSysNo);
            for (int i = 0; i < all.Count; i++)
            {
                TopItemInfo entity = all[i];
                entity.Priority = i + 1;
                ObjectFactory<ITopItemDA>.Instance.UpdateTopItemPriority(entity);
            }
        }

        public virtual void TopItemConfigUpdate(TopItemConfigInfo entity)
        {
            TopItemConfigInfo item = LoadTopItemConfig(entity.PageType.Value, entity.RefSysNo.Value);
            if (item != null)//update
            {
                ObjectFactory<ITopItemDA>.Instance.UpdateTopItemConfig(entity);
            }
            else //create
            {
                ObjectFactory<ITopItemDA>.Instance.CreateTopItemConfig(entity);
            }
        }

        public virtual TopItemConfigInfo LoadTopItemConfig(int PageType, int RefSysNo)
        {
            return ObjectFactory<ITopItemDA>.Instance.LoadItemConfig(PageType, RefSysNo);
        }
    }
}
