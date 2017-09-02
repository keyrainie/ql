using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using System.Text;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductLineProcessor))]
    public class ProductLineProcessor
    {
        private IProductLineDA da = ObjectFactory<IProductLineDA>.Instance;

        public virtual ProductLineInfo LoadBySysNo(int sysno)
        {
            return da.GetProductLine(sysno);
        }

        public virtual ProductLineInfo Create(ProductLineInfo entity) 
        {
            ValidateProductLine(entity);
            using (TransactionScope scope = new TransactionScope())
            {
                entity = da.Create(entity);
                da.CreateChangePool(entity.SysNo.Value,null,null);
                scope.Complete();
            }
            return entity;
        }

        public virtual ProductLineInfo Update(ProductLineInfo entity)
        {
            ValidateProductLine(entity);
            using (TransactionScope scope = new TransactionScope())
            {
                entity = da.Update(entity);
                if (!da.HasSameChange(entity.SysNo.Value, entity.PMUserSysNo))
                {
                    da.CreateChangePool(entity.SysNo.Value, null, null);
                }
                scope.Complete();
            }
            return entity;
        }

        public virtual void Delete(int sysno)
        {
            ProductLineInfo entity = LoadBySysNo(sysno);
            if (entity==null)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "ProductLineDeleteResult1"), sysno));
            }
            bool islosepm = da.IsProductLosePM(entity.C2SysNo.Value,entity.BrandSysNo);
            if (islosepm)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductLineDeleteResult2"));
            }
            else
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (!da.HasSameChange(sysno, entity.PMUserSysNo))
                    {
                        da.CreateChangePool(sysno, null, null);
                    }
                    da.Delete(sysno);
                    scope.Complete();
                }
            }
        }

        private void ValidateProductLine(ProductLineInfo entity)
        {
            if (entity == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ValidateProductLineResult1"));
            }
            if (!entity.C2SysNo.HasValue|| entity.C2SysNo <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ValidateProductLineResult2"));
            }
            if (entity.PMUserSysNo <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ValidateProductLineResult3"));
            }
            if (entity.MerchandiserSysNo <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ValidateProductLineResult4"));
            }
            var message = new List<string>();
            if (!da.C2IsValid(entity.C2SysNo.Value))
            {
                message.Add(ResouceManager.GetMessageString("IM.Product", "ValidateProductLineResult5"));
            }
            if (entity.BrandSysNo.HasValue && !da.BrandIsValid(entity.BrandSysNo.Value))
            {
                message.Add(ResouceManager.GetMessageString("IM.Product", "ValidateProductLineResult6"));
            }
            if (!da.PMIsValid(entity.PMUserSysNo))
            {
                message.Add(ResouceManager.GetMessageString("IM.Product", "ValidateProductLineResult7"));
            }
            if (!da.PMIsValid(entity.MerchandiserSysNo))
            {
                message.Add(ResouceManager.GetMessageString("IM.Product", "ValidateProductLineResult8"));
            }
            if (message.Any())
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ValidateProductLineResult9") + message.Join(","));
            }
            if (!entity.SysNo.HasValue || entity.SysNo <= 0)
            {
                var isexists = da.IsExists(entity.C2SysNo.Value, entity.BrandSysNo);

                //是否会导致新分类重复
                if (isexists)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "ValidateProductLineResult10"));
                }
            }
        }

        public ProductManagerInfo GetPMByC3(int c3SysNo, int brandSysNo)
        {
            return da.GetPMByC3(c3SysNo, brandSysNo);
        }

        public bool HasRightByPMUser(ProductLineInfo entity) 
        {
            return da.HasRightByPMUser(entity);
        }

        public void BatchUpdate(BatchUpdatePMEntity entity) 
        {
            if(entity==null)
            {
                throw new Exception("Argument is null");
            }

            if (entity.ProductLineList == null || entity.ProductLineList.Count == 0)
            {
                string error = string.Empty;
                if(entity.IsEmptyC2Create)
                {
                    error = ResouceManager.GetMessageString("IM.Product", "ProductLineBatchUpdateResult1");
                }
                else
                {
                    error = ResouceManager.GetMessageString("IM.Product", "ProductLineBatchUpdateResult2");
                }
                throw new BizException(error);
            }
            StringBuilder failtstring = new StringBuilder();
            foreach (var item in entity.ProductLineList)
            {
                if(entity.IsEmptyC2Create)
                {
                    item.PMUserSysNo = entity.PMUserSysNo.Value;
                    item.MerchandiserSysNo = entity.MerchandiserSysNo.Value;
                    item.BackupPMSysNoList = entity.BackupPMSysNoList;
                    item.InUser = entity.InUser;
                    item.CompanyCode = entity.CompanyCode;
                    item.LanguageCode = entity.LanguageCode;
                    try
                    {
                        Create(item);
                    }
                    catch (BizException ex)
                    {
                        failtstring.AppendLine(string.Format(ResouceManager.GetMessageString("IM.Product", "ProductLineBatchUpdateResult3"), item.C2Name, ex.Message));
                    }
                    
                }
                else
                {
                    ProductLineInfo olditem = LoadBySysNo(item.SysNo.Value);
                    if (entity.PMUserSysNo.HasValue)
                    {
                        olditem.PMUserSysNo = entity.PMUserSysNo.Value;
                    }
                    if (entity.MerchandiserSysNo.HasValue)
                    {
                        olditem.MerchandiserSysNo = entity.MerchandiserSysNo.Value;
                    }
                    olditem.EditUser = entity.InUser;
                    try
                    {
                        olditem.BackupPMSysNoList = ProcessBakPMList(olditem.BackupPMSysNoList, entity.BackupPMList, entity.BakPMUpdateType);
                        Update(olditem);
                    }
                    catch (BizException ex)
                    {
                        failtstring.AppendLine(string.Format(ResouceManager.GetMessageString("IM.Product", "ProductLineBatchUpdateResult4"), item.SysNo, ex.Message));
                    }
                }
            }
            if(failtstring.Length>0)
            {
                throw new BizException(failtstring.ToString());
            }
        }
        private string ProcessBakPMList(string oldBakPMs,List<int?> newBakPMs,string operatortype)
        {
            List<int?> oldbakpms = new List<int?>();
            if (!string.IsNullOrEmpty(oldBakPMs))
            {
                oldbakpms = oldBakPMs.Split(';').Select<string, int?>(s => {
                    int result;
                    int.TryParse(s,out result);
                    return result;
                }).ToList<int?>();
            }

            if(operatortype=="Append")
            {
                oldbakpms.AddRange(newBakPMs.ToArray<int?>());
                oldbakpms = oldbakpms.Distinct().ToList();
            }
            else if(operatortype=="Remove")
            {
                foreach (var item in newBakPMs)
                {
                    oldbakpms.RemoveAll(pm =>
                    {
                        if (pm == item)
                        {
                            return true;
                        }
                        return false;
                    });
                }
                oldbakpms = oldbakpms.Distinct().ToList();
            }
            oldbakpms = oldbakpms.Where(args => args > 0).ToList();
            return oldbakpms.Join(";");
        }

        public bool DeleteByPMUserSysNo(int pmusersysno)
        {
            bool result = false;
            using (TransactionScope scope = new TransactionScope())
            {
                da.DeleteByPMUserSysNo(pmusersysno);

                List<ProductLineInfo> list = da.GetByBakPMUserSysNo(pmusersysno);
                if (list.Count > 0)
                {
                    BatchUpdatePMEntity entity = new BatchUpdatePMEntity();
                    entity.IsEmptyC2Create = false;
                    entity.BackupPMSysNoList = pmusersysno.ToString();
                    entity.ProductLineList = list;
                    entity.BakPMUpdateType = "Remove";
                    BatchUpdate(entity);
                }
                scope.Complete();
                result = true;
            }
            return result;
        }

        public bool HasProductByPMUserSysNo(int pmusersysno)
        {
            bool result = false;
            List<ProductLineInfo> list = da.GetProductLineByPMSysNo(pmusersysno);
            foreach (var item in list)
            {
                result = da.HasProduct(item.C2SysNo, item.BrandSysNo);
                if (result)
                {
                    break;
                }
            }
            return result;
        }
    }
}
