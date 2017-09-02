using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(ECDynamicCategoryProcessor))]
    public class ECDynamicCategoryProcessor
    {
        private IECDynamicCategoryDA da = ObjectFactory<IECDynamicCategoryDA>.Instance;

        public ECDynamicCategory Create(ECDynamicCategory entity)
        {
            PreCheck(entity);

            return da.Create(entity);
        }

        public void Update(ECDynamicCategory entity)
        {
            PreCheck(entity);

            da.Update(entity);
        }

        public void Delete(int sysNo)
        {
            if (!da.CheckSubCategoryExists(sysNo))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    da.Delete(sysNo);
                    da.DeleteCategoryMapping(sysNo);

                    scope.Complete();
                }
            }
            else
            {
                //throw new BizException("该类别下存在子类别，不能删除！");
                throw new BizException(ResouceManager.GetMessageString("MKT.ECCategory", "ECCategory_HasChildCategory"));
            }
        }

        public void InsertCategoryProductMapping(int dynamicCategorySysNo, List<int> productSysNoList)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                if (productSysNoList != null)
                {
                    productSysNoList.ForEach(p =>
                    {
                        da.InsertCategoryProductMapping(dynamicCategorySysNo, p);                        
                    });
                }
                scope.Complete();
            }
        }       

        public void DeleteCategoryProductMapping(int dynamicCategorySysNo, List<int> productSysNoList)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                if (productSysNoList != null)
                {
                    productSysNoList.ForEach(p =>
                    {
                        da.DeleteCategoryProductMapping(dynamicCategorySysNo, p);
                    });
                }
                scope.Complete();
            }
        }

        public ECDynamicCategory GetCategoryTree(DynamicCategoryStatus? status, DynamicCategoryType? categoryType)
        {
            var list = da.GetDynamicCategories(status,categoryType);
            ECDynamicCategory root = new ECDynamicCategory();
            root.Name = "Root";
            root.SysNo = 0;
            root.ParentSysNo = 0;
            root.CategoryType = DynamicCategoryType.Standard;
            root.Status = DynamicCategoryStatus.Active;
            BuildTree(root, list);
            return root;
        }

        private void PreCheck(ECDynamicCategory entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
            {
                //throw new BizException("名称不能为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.ECCategory", "ECCategory_NameIsNull"));
            }
            if (entity.CategoryType == null)
            {
                //throw new BizException("类别不能为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.ECCategory", "ECCategory_CategoryIsNull"));
            }
            if (da.CheckNameDuplicate(entity.Name, entity.SysNo ?? 0, entity.Level.Value, entity.CompanyCode))
            {
                //throw new BizException(string.Format("该级别下类别名称[{0}]已经存在！", entity.Name));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.ECCategory", "ECCategory_ExsistCategoryName"), entity.Name));
            }
        }

        private void BuildTree(ECDynamicCategory parent, List<ECDynamicCategory> list)
        {
            parent.SubCategories = list.Where(item => (item.ParentSysNo ?? 0) == parent.SysNo).OrderByDescending(item => item.Priority).ToList();
            foreach (var c in parent.SubCategories)
            {
                BuildTree(c, list);
            }
        }
    }
}
