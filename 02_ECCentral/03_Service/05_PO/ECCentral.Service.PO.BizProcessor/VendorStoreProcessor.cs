using ECCentral.BizEntity;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity.PO.Vendor;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;

namespace ECCentral.Service.PO.BizProcessor
{
    [VersionExport(typeof(VendorStoreProcessor))]
    public class VendorStoreProcessor
    {
        private IVendorStoreDA da = ObjectFactory<IVendorStoreDA>.Instance;

        public VendorStoreInfo Create(VendorStoreInfo entity)
        {
            PreCheck(entity);

            if (entity.VendorSysNo == null)
            {
                throw new BizException("供应商编号不能为空！");
            }

            return da.Create(entity);
        }

        public VendorStoreInfo Update(VendorStoreInfo entity)
        {
            PreCheck(entity);

            return da.Update(entity);
        }

        private void PreCheck(VendorStoreInfo entity)
        {
            if (entity.AreaSysNo == null)
            {
                throw new BizException("地区不能为空！");
            }
            if (string.IsNullOrEmpty(entity.Name))
            {
                throw new BizException("门店名称不能为空！");
            }
            if (da.CheckVendorStoreNameExists(entity.SysNo ?? 0, entity.Name, entity.VendorSysNo.Value))
            {
                throw new BizException(string.Format("该供应商下的门店名称[{0}]已经存在！", entity.Name));
            }
        }

        public void UpdateCommissionRuleTemplate(BizEntity.PO.Vendor.CommissionRuleTemplateInfo info)
        {
            if (info.BrandSysNos == null || info.BrandSysNos.Count == 0)
            {
                throw new BizException("请选择品牌！");
            }
            if (info.C3SysNos == null || info.C3SysNos.Count == 0)
            {
                throw new BizException("请选择类别！");
            }

            using (TransactionScope scope = TransactionScopeFactory.CreateTransactionScope())
            {
                foreach (var c3 in info.C3SysNos)
                {
                    foreach (var brand in info.BrandSysNos)
                    {
                        info.C1SysNo = c3.C1;
                        info.C2SysNo = c3.C2;
                        info.C3SysNo = c3.C3;
                        info.BrandSysNo = brand;
                        info.SalesRule = SerializationUtility.XmlSerialize(info.SaleRuleEntity);
                        da.UpdateCommissionRuleTemplate(info);
                    }
                }
                scope.Complete();
            }
        }

        public void BatchSetCommissionRuleStatus(string sysnos, CommissionRuleStatus commissionRuleStatus)
        {
            da.SetCommissionRuleStatus(sysnos, commissionRuleStatus);
        }

        public CommissionRuleTemplateInfo GetCommissionRuleTemplateInfo(int sysno)
        {
            return da.GetCommissionRuleTemplateInfo(sysno);
        }
        
        public VendorPresetContent GetVendorPresetContent()
        {
            return da.GetVendorPresetContent();
        }

        public void SecondDomainAuditThrough(int sysno)
        {
            if (ObjectFactory<IVendorStoreQueryDA>.Instance.CheckSecondDomainStatus(sysno))
            {
                ObjectFactory<IVendorStoreQueryDA>.Instance.ChangeSecondDomainStatus(sysno, 1);
            }
            else
            {
                throw new BizException("状态不是待审核，无法审核通过！");
            }
        }

        public void SecondDomainAuditThroughNot(int sysno)
        {
            if (ObjectFactory<IVendorStoreQueryDA>.Instance.CheckSecondDomainStatus(sysno))
            {
                ObjectFactory<IVendorStoreQueryDA>.Instance.ChangeSecondDomainStatus(sysno, -1);
            }
            else
            {
                throw new BizException("状态不是待审核，无法审核不通过！");
            }
        }


    }
}
