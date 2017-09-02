using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage.Inventory;
using System.Xml;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(ExperienceHallProcessor))]
    public class ExperienceHallProcessor
    {
        private IExperienceHallDA ExperienceDA = ObjectFactory<IExperienceHallDA>.Instance;

        /// <summary>
        /// 根据SysNo获取调拨单信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual ExperienceInfo GetExperienceInfoBySysNo(int requestSysNo)
        {
            ExperienceInfo Experience = ExperienceDA.GetExperienceInfoBySysNo(requestSysNo);
            if (Experience != null)
            {
                Experience.ExperienceItemInfoList = ExperienceDA.GetExperienceItemListByRequestSysNo(requestSysNo);
            }

            return Experience;
        }

        ///// <summary>
        ///// 根据商品编号得到其所属产品线
        ///// </summary>
        ///// <param name="sysNo"></param>
        ///// <returns></returns>
        //public virtual ExperienceInfo GetProductLineInfo(int ProductSysNo)
        //{
        //    ExperienceInfo Experience = ExperienceDA.GetProductLineInfo(ProductSysNo);
        //    return Experience;
        //}

        /// <summary>
        /// 创建调拨单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ExperienceInfo CreateRequest(ExperienceInfo entityToCreate)
        {
            ExperienceInfo result;
            var newItems = entityToCreate.ExperienceItemInfoList;

            CheckedCondition(entityToCreate);

            using (var scope = new TransactionScope())
            {
                entityToCreate.Status = ExperienceHallStatus.Created;
                entityToCreate.InDate = DateTime.Now;

                //int requestSysNo = ExperienceDA.GetExperienceSequence();
                //entityToCreate.SysNo = requestSysNo;

                result = ExperienceDA.CreateExperience(entityToCreate);

                if (newItems != null && newItems.Count > 0)
                {
                    ExperienceDA.DeleteExperienceItemByRequestSysNo(result.SysNo.Value);

                    newItems.ForEach(item =>
                    {
                        ExperienceDA.CreateExperienceItem(item, result.SysNo.Value);
                    });
                }

                scope.Complete();
            }

            return result;
        }

        private void CheckedCondition(ExperienceInfo entity)
        {
            var newItems = entity.ExperienceItemInfoList;

            if (newItems.Any(x => { return x.AllocateQty <= 0; }))
            {
                throw new BizException("商品调拨数量必须大于0");
            }

            if (entity.AllocateType == AllocateType.ExperienceOut)
            {
                string error = string.Empty;
                newItems.ForEach(x =>
                {
                    if (ExperienceDA.GetValidStockQty(x.ProductSysNo.Value) < x.AllocateQty)
                    {
                        error += "商品 " + x.ProductSysNo.Value + " 体验厅库存不足" + Environment.NewLine;
                    }
                });

                if (!string.IsNullOrEmpty(error))
                {
                    throw new BizException(error);
                }
            }
        }
            
        /// <summary>
        /// 更新调拨单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ExperienceInfo UpdateRequest(ExperienceInfo entityToUpdate)
        {
            var newItems = entityToUpdate.ExperienceItemInfoList;

            CheckedCondition(entityToUpdate);

            using (var scope = new TransactionScope())
            {
                ExperienceDA.UpdateExperience(entityToUpdate);

                if (newItems != null && newItems.Count > 0)
                {
                    ExperienceDA.DeleteExperienceItemByRequestSysNo(entityToUpdate.SysNo.Value);

                    newItems.ForEach(item =>
                    {
                        ExperienceDA.CreateExperienceItem(item, entityToUpdate.SysNo.Value);
                    });
                }

                scope.Complete();
            }

            return entityToUpdate;
        }

        /// <summary>
        /// 审核调拨单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void AuditExperience(ExperienceInfo entityToUpdate)
        {
            List<ExperienceItemInfo> items = ExperienceDA.GetExperienceItemListByRequestSysNo(entityToUpdate.SysNo.Value);

            ExperienceInfo Experience = GetExperienceInfoBySysNo(entityToUpdate.SysNo.Value);
            CheckedCondition(Experience);

            ExperienceDA.UpdateExperienceStatus(entityToUpdate, (int)ExperienceHallStatus.Audit);

            //using (var scope = new TransactionScope())
            //{
            //    if (items != null && items.Count > 0)
            //    {
            //        items.ForEach(x =>
            //        {
            //            ExperienceDA.AuditExperienceInOrOut(x, entityToUpdate.SysNo.Value);
            //        });
            //    }

            //    ExperienceDA.UpdateExperienceStatus(entityToUpdate, (int)ExperienceHallStatus.Experienced);

            //    scope.Complete();
            //}
        }

        public virtual void ExperienceInOrOut(ExperienceInfo entityToUpdate)
        {
            List<ExperienceItemInfo> items = ExperienceDA.GetExperienceItemListByRequestSysNo(entityToUpdate.SysNo.Value);

            ExperienceInfo Experience = GetExperienceInfoBySysNo(entityToUpdate.SysNo.Value);
            CheckedCondition(Experience);

            using (var scope = new TransactionScope())
            {
                if (items != null && items.Count > 0)
                {
                    items.ForEach(x =>
                    {
                        ExperienceDA.AuditExperienceInOrOut(x, entityToUpdate.SysNo.Value);
                    });
                }

                ExperienceDA.UpdateExperienceStatus(entityToUpdate, (int)ExperienceHallStatus.Experienced);

                scope.Complete();
            }
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="sysno"></param>
        public virtual void CancelAuditExperience(ExperienceInfo entityToUpdate)
        {
            ExperienceDA.UpdateExperienceStatus(entityToUpdate, (int)ExperienceHallStatus.Created);

            //List<ExperienceItemInfo> items = ExperienceDA.GetExperienceItemListByRequestSysNo(entityToUpdate.SysNo.Value);
            //using (var scope = new TransactionScope())
            //{
            //    if (items != null && items.Count > 0)
            //    {
            //        items.ForEach(x =>
            //        {
            //            ExperienceDA.CancelAuditExperienceInOrOut(x, entityToUpdate.SysNo.Value);
            //        });
            //    }

            //    ExperienceDA.UpdateExperienceStatus(entityToUpdate, (int)ExperienceHallStatus.Created);

            //    scope.Complete();
            //}
        }

        /// <summary>
        /// 作废调拨单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void AbandonExperience(ExperienceInfo entityToUpdate)
        {
            ExperienceDA.UpdateExperienceStatus(entityToUpdate, (int)ExperienceHallStatus.Abandon);
        }
    }
}
