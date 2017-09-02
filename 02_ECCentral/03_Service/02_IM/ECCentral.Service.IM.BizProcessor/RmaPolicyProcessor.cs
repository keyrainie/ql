using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(RmaPolicyProcessor))]
    public class RmaPolicyProcessor
    {
        /// <summary>
        /// 创建退换货信息
        /// </summary>
        /// <param name="info"></param>
        public void CreateRmaPolicy(RmaPolicyInfo info)
        {
            if (info.RmaType == RmaPolicyType.StandardType)
            {
                if (ObjectFactory<IRmaPolicyDA>.Instance.IsExistsRmaPolicy())
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "CreateRmaPolicyResult"));
                }
            }
            using (TransactionScope scope = new TransactionScope())
            {
                ObjectFactory<IRmaPolicyDA>.Instance.CreateRmaPolicy(info);
                //创建日志
                info.Status = RmaPolicyStatus.Active;
                ObjectFactory<RmaPolicyLogProcessor>.Instance.CreateRMAPolicyLog(info, RmaLogActionType.Create);
                scope.Complete();
            }
        }
        /// <summary>
        /// 更新退换货信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateRmaPolicy(RmaPolicyInfo info)
        {
            if (info.RmaType == RmaPolicyType.StandardType)
            {
                if (ObjectFactory<IRmaPolicyDA>.Instance.IsExistsRmaPolicy((int)info.SysNo))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateRmaPolicyResult"));
                }
            }
            using (TransactionScope scope = new TransactionScope())
            {
                ObjectFactory<IRmaPolicyDA>.Instance.UpdateRmaPolicy(info);
                //创建日志
                ObjectFactory<RmaPolicyLogProcessor>.Instance.CreateRMAPolicyLog(info, RmaLogActionType.Edit);
                scope.Complete();
            }
        }

        /// <summary>
        ///作废
        /// </summary>
        /// <param name="sysNo"></param>
        public void DeActiveRmaPolicy(List<RmaPolicyInfo> list)
        {
            List<string> errorlist = new List<string>();
            foreach (var item in list)
            {
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        ObjectFactory<IRmaPolicyDA>.Instance.DeActiveRmaPolicy((int)item.SysNo,item.User);
                        item.Status = RmaPolicyStatus.DeActive;
                        //创建日志
                        ObjectFactory<RmaPolicyLogProcessor>.Instance.CreateRMAPolicyLog(item, RmaLogActionType.DeActive);
                        scope.Complete();
                    }
                }
                catch (Exception)
                {

                    errorlist.Add(item.RMAPolicyName);
                }

            }
            if (errorlist.Count > 0)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "DeActiveRmaPolicyResult"), errorlist.Count, errorlist.Join(";")));
            }

        }

        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="sysNo"></param>
        public void ActiveRmaPolicy(List<RmaPolicyInfo> list)
        {

            List<string> errorlist = new List<string>();
            foreach (var item in list)
            {
                if (item.RmaType == RmaPolicyType.StandardType)
                {
                    if (ObjectFactory<IRmaPolicyDA>.Instance.IsExistsRmaPolicy())
                    {
                        errorlist.Add(item.RMAPolicyName);
                        continue;
                    }
                }
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        ObjectFactory<IRmaPolicyDA>.Instance.ActiveRmaPolicy((int)item.SysNo, item.User);
                        //创建日志
                        item.Status = RmaPolicyStatus.Active;
                        ObjectFactory<RmaPolicyLogProcessor>.Instance.CreateRMAPolicyLog(item, RmaLogActionType.Active);
                        scope.Complete();
                    }
                }
                catch (Exception)
                {

                    errorlist.Add(item.RMAPolicyName);
                }

            }
            if (errorlist.Count > 0)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "ActiveRmaPolicyResult"), errorlist.Count, errorlist.Join(";")));
            }

        }
        /// <summary>
        /// 组装商品退换货保修信息
        /// </summary>
        /// <param name="c3sysno"></param>
        /// <param name="brandsysno"></param>
        /// <returns></returns>
        public ProductRMAPolicyInfo MakeRMAPolicyEntity(int c3sysno, int brandsysno)
        {
            ProductRMAPolicyInfo entity = new ProductRMAPolicyInfo();
            entity.IsBrandWarranty = "N";
            //得到默认退换货
            DefaultRMAPolicyInfo dpolicyentity = ObjectFactory<IDefaultRMAPolicy>.Instance.GetDefaultRMAPolicy(c3sysno, brandsysno);
            if (dpolicyentity == null)
            {
                //得到标准退换货信息
                RmaPolicyInfo policyentity = ObjectFactory<IRmaPolicyQueryDA>.Instance.GetStandardRmaPolicy();
                if (policyentity == null) //没有默认也没有标准则不能创建商品
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "MakeRMAPolicyEntityResult"));
                }
                entity.RMAPolicyMasterSysNo = policyentity.SysNo;
                entity.ReturnDate = policyentity.ReturnDate;
                entity.ChangeDate = policyentity.ChangeDate;
                entity.IsOnlineRequst = policyentity.IsOnlineRequest == IsOnlineRequst.YES ? "Y" : "N";
                
              }
            else
            {
                //得到退换货详细信息
                RmaPolicyInfo info = ObjectFactory<IRmaPolicyQueryDA>.Instance.QueryRmaPolicyBySysNo((int)dpolicyentity.RMAPolicySysNo);
                entity.RMAPolicyMasterSysNo = info.SysNo;
                entity.ReturnDate = info.ReturnDate;
                entity.ChangeDate = info.ChangeDate;
                entity.IsOnlineRequst = info.IsOnlineRequest == IsOnlineRequst.YES ? "Y" : "N";
            }

            ProductBrandWarrantyInfo warrantyentity = ObjectFactory<IProductBrandWarrantyDA>.Instance.GetBrandWarranty(c3sysno,brandsysno);
            if (warrantyentity!=null) 
            {
                entity.IsBrandWarranty = "Y";
            }

            return entity;
        }
    }
}
