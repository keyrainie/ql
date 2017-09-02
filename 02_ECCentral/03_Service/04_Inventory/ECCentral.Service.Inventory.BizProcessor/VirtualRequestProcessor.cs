using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Utility;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(VirtualRequestProcessor))]
    public class VirtualRequestProcessor
    {
        private IVirtualRequestDA virtualRequestDA = ObjectFactory<IVirtualRequestDA>.Instance;


        /// <summary>
        /// 根据SysNo获取虚库申请单信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual VirtualRequestInfo GetVirtualRequestInfoBySysNo(int requestSysNo)
        {
            VirtualRequestInfo virtualRequest = virtualRequestDA.GetVirtualRequestInfoBySysNo(requestSysNo);
            if (virtualRequest != null && virtualRequest.VirtualProduct != null && virtualRequest.VirtualProduct.SysNo > 0)
            {
                virtualRequest.VirtualProduct = ExternalDomainBroker.GetProductInfoByProductSysNo(virtualRequest.VirtualProduct.SysNo);
            }

            return virtualRequest;
        }


        public virtual void AuditRequest(VirtualRequestInfo entityToUpdate)
        {
            VirtualRequestInfo originEntity = GetVirtualRequestInfoBySysNo((int)entityToUpdate.SysNo);

            PreCheckOriginVirtualRequestInfo(originEntity, InventoryAdjustSourceAction.Audit);

        }


        public void ApplyRequestBatch(bool canOperateItemOfLessThanPrice, bool canOperateItemOfSecondHand, List<VirtualRequestInfo> requestList)
        {
            //check less than point price right and second-hand right
            List<int> productSysNoList = new List<int>();
            requestList.ForEach(item =>
            {
                productSysNoList.Add(item.VirtualProduct.SysNo);
            });
            List<ProductInfo> items = ExternalDomainBroker.GetProductInfoListByProductSysNoList(productSysNoList);

            requestList.ForEach(entity =>
            {
                ProductInfo product = items.Find(p => { return p.SysNo == entity.VirtualProduct.SysNo; });
                if (product == null)
                {
                    throw new BizException(string.Format("操作失败，不能找到编号为{0}的商品的价格信息", entity.VirtualProduct.SysNo));
                }
                if (!canOperateItemOfLessThanPrice)
                {
                    if (product.ProductPriceInfo.UnitCost < 800)
                    {
                        throw new BizException(String.Format("商品编号：{0}，错误原因：该商品的单价已经低于了800RMB，您没该商品的虚库操作权限", entity.VirtualProduct.SysNo));
                    }
                }
                if (!canOperateItemOfSecondHand)
                {
                    if (product.ProductBasicInfo.ProductType == ProductType.OpenBox)
                    {
                        throw new BizException(String.Format("商品编号：{0}，错误原因：该商品为二手商品，您没该商品的虚库操作权限", entity.VirtualProduct.SysNo));
                    }
                }
            });

            using (TransactionScope scope = new TransactionScope())
            {
                requestList.ForEach(request =>
                  {
                      //****************************（2011年6月20日新需求 要求 创建单据时 不关闭之前虚库申请单据************                          
                      List<VirtualRequestInfo> oldRequestList = virtualRequestDA.ExistNeedCloseRequestByStockAndItem(request.Stock.SysNo.Value, request.VirtualProduct.SysNo, request.CompanyCode);

                      if (oldRequestList != null && oldRequestList.Count > 0)
                      {
                          foreach (VirtualRequestInfo r in oldRequestList)
                          {
                              switch (r.RequestStatus)//存在 运行中 或 关闭中的单据  需 先进性虚库判断
                              {
                                  case VirtualRequestStatus.Origin:
                                  case VirtualRequestStatus.Approved:
                                  case VirtualRequestStatus.Running:
                                  case VirtualRequestStatus.Closing:
                                      if (request.ActiveVirtualQuantity > 0 && request.VirtualQuantity < request.ActiveVirtualQuantity)//调整的虚库数量大于生效的虚库数量 关闭虚库申请单 调整库存 负责无法创建虚库申请单
                                      {
                                          throw new BizException("系统编号为" + request.VirtualProduct.SysNo + "的商品设定的虚库数量小于生效虚库数量，无法创建虚库申请单！");
                                      }
                                      break;
                              }
                          }
                      }
                      request.CreateDate = DateTime.Now;
                      if (!request.StartDate.HasValue)
                      {
                          request.StartDate = request.CreateDate;
                          request.EndDate = new DateTime(request.StartDate.Value.Year, request.StartDate.Value.Month, request.StartDate.Value.Day).AddDays(4).Subtract(new TimeSpan(0, 0, 1));
                      }
                      request.CreateUser = request.CreateUser == null || request.CreateUser.SysNo == null ? new UserInfo { SysNo = ServiceContext.Current.UserSysNo } : request.CreateUser;
                      request.SysNo = virtualRequestDA.Apply(request);
                  });
                scope.Complete();
            }
        }

        public virtual void CreateRequestBatch(List<VirtualRequestInfo> entities)
        {

        }


        /// <summary>
        /// 审核  - 同意
        /// </summary>
        public virtual void ApproveRequest(VirtualRequestInfo info)
        {
            VirtualRequestInfo oldInfo = ObjectFactory<IVirtualRequestDA>.Instance.GetVirtualRequestInfoBySysNo(info.SysNo.Value);
            if (oldInfo.RequestStatus != VirtualRequestStatus.Origin)
            {
                throw new BizException("该单据不是原始状态的单据，不能进行审核操作!");
            }

            //if (oldInfo.CreateUser.SysNo == ServiceContext.Current.UserSysNo)
            //{
            //    throw new BizException("创建人和审核人不能相同");
            //}

            oldInfo.AuditNote = info.AuditNote;
            info.RequestStatus = VirtualRequestStatus.Approved;
            using (TransactionScope scope = new TransactionScope())
            {
                int checkVirtualResult = ObjectFactory<IVirtualRequestDA>.Instance.CheckVirtualQty(oldInfo.VirtualProduct.SysNo, oldInfo.VirtualQuantity, oldInfo.Stock.SysNo.Value, oldInfo.CompanyCode);

                switch (checkVirtualResult)
                {
                    case 1:
                        throw new BizException("审核失败！调整可用库存后不能使可卖数量变为负数，可卖数量=可用库存+虚拟库存+代销库存.");
                    case 2:
                        throw new BizException("审核失败！该类商品中的混合型商品（即有可用库存又有虚拟库存）种类在该类所有商品种类中的比例已经超过限制.");
                    case 3:
                        throw new BizException("审核失败！该类商品中的纯虚拟库存商品种类数量已经超过限制.");
                    default:
                        break;
                }

                ObjectFactory<IVirtualRequestDA>.Instance.UpdateProductExtension((int)oldInfo.VirtualType, oldInfo.VirtualProduct.SysNo, info.CompanyCode);
                bool Verify = ObjectFactory<IVirtualRequestDA>.Instance.Verify(info);
                #region (2011年6月20日新需求)审核通过后关闭之前的虚库申请单据
                if (Verify && info.RequestStatus == VirtualRequestStatus.Approved)
                {
                    List<VirtualRequestInfo> applySysNumberList = ObjectFactory<IVirtualRequestDA>.Instance.ExistNeedCloseRequestByStockAndItem(oldInfo.Stock.SysNo.Value, oldInfo.VirtualProduct.SysNo, oldInfo.CompanyCode);
                    if (applySysNumberList != null && applySysNumberList.Count > 0)
                    {
                        int closeResult = 0;
                        int IsAdjustVirtualQty = 1;//是否调整库存
                        foreach (VirtualRequestInfo item in applySysNumberList)
                        {
                            if (item.SysNo == info.SysNo)//除过前申请单外，其他申请单据 全部进行关闭。
                            {
                                continue;
                            }
                            VirtualRequestStatus currentStatus = item.RequestStatus;

                            if (currentStatus == VirtualRequestStatus.Running || currentStatus == VirtualRequestStatus.Closing)
                            {
                                IsAdjustVirtualQty = 1;//运行中或关闭中的单据需要调整库存

                            }
                            else if (currentStatus == VirtualRequestStatus.Origin || currentStatus == VirtualRequestStatus.Approved)
                            {
                                IsAdjustVirtualQty = 0;//待审核或已审核单据不需要调整库存                                   
                            }
                            closeResult = ObjectFactory<IVirtualRequestDA>.Instance.CloseRequest(item.SysNo.Value, currentStatus, IsAdjustVirtualQty, info.CompanyCode);
                        }
                    }
                }
                #endregion
                scope.Complete();
            }
        }

        /// <summary>
        /// 审核 - 拒绝
        /// </summary>
        public virtual void RejectRequest(VirtualRequestInfo info)
        {
            VirtualRequestInfo oldInfo = ObjectFactory<IVirtualRequestDA>.Instance.GetVirtualRequestInfoBySysNo(info.SysNo.Value);
            if (oldInfo.RequestStatus != VirtualRequestStatus.Origin)
            {
                throw new BizException("该单据不是原始状态的单据，不能进行审核操作!");
            }
            oldInfo.AuditNote = info.AuditNote;
            info.RequestStatus = VirtualRequestStatus.Rejected;
            using (TransactionScope scope = new TransactionScope())
            {
                ObjectFactory<IVirtualRequestDA>.Instance.CheckVirtualQty(oldInfo.VirtualProduct.SysNo, oldInfo.VirtualQuantity, oldInfo.Stock.SysNo.Value, oldInfo.CompanyCode);
                ObjectFactory<IVirtualRequestDA>.Instance.UpdateProductExtension((int)oldInfo.VirtualType, oldInfo.VirtualProduct.SysNo, info.CompanyCode);
                bool Verify = ObjectFactory<IVirtualRequestDA>.Instance.Verify(info);
                #region (2011年6月20日新需求)审核通过后关闭之前的虚库申请单据
                if (Verify && info.RequestStatus == VirtualRequestStatus.Approved)
                {
                    List<VirtualRequestInfo> applySysNumberList = ObjectFactory<IVirtualRequestDA>.Instance.ExistNeedCloseRequestByStockAndItem(oldInfo.Stock.SysNo.Value, oldInfo.VirtualProduct.SysNo, oldInfo.CompanyCode);
                    if (applySysNumberList != null && applySysNumberList.Count > 0)
                    {
                        int closeResult = 0;
                        int IsAdjustVirtualQty = 1;//是否调整库存
                        foreach (VirtualRequestInfo item in applySysNumberList)
                        {
                            if (item.SysNo == info.SysNo)//除过前申请单外，其他申请单据 全部进行关闭。
                            {
                                continue;
                            }
                            VirtualRequestStatus currentStatus = item.RequestStatus;

                            if (currentStatus == VirtualRequestStatus.Running || currentStatus == VirtualRequestStatus.Closing)
                            {
                                IsAdjustVirtualQty = 1;//运行中或关闭中的单据需要调整库存

                            }
                            else if (currentStatus == VirtualRequestStatus.Origin || currentStatus == VirtualRequestStatus.Approved)
                            {
                                IsAdjustVirtualQty = 0;//待审核或已审核单据不需要调整库存                                   
                            }
                            closeResult = ObjectFactory<IVirtualRequestDA>.Instance.CloseRequest(item.SysNo.Value, currentStatus, IsAdjustVirtualQty, info.CompanyCode);
                        }
                    }
                }
                #endregion
                scope.Complete();
            }
        }

        #region 私有方法

        private void PreCheckVirtualRequestInfoForCreate(VirtualRequestInfo entityToCreate)
        {

        }

        private void PreCheckVirtualRequestInfoForAudit(VirtualRequestInfo entityToUpdate)
        {

        }

        private void PreCheckVirtualRequestInfoForApply(VirtualRequestInfo entityToUpdate)
        {

        }

        private void PreCheckOriginVirtualRequestInfo(VirtualRequestInfo entity, InventoryAdjustSourceAction actionType)
        {
            if (entity == null)
            {
                throw new BizException("WarningMessage.VirtualRequestd_cannotFindOriginalVirtualRequestValue");
            }

            //借货单当前状态检查
            if ((actionType == InventoryAdjustSourceAction.Update || actionType == InventoryAdjustSourceAction.Audit)
                && entity.RequestStatus != VirtualRequestStatus.Origin)
            {
                throw new BizException("WarningMessage.VirtualRequest_CanNotVerifyCode, WarningMessage.VirtualRequest_CanNotVerifyValue");
            }
        }
        #endregion 私有方法
    }
}
