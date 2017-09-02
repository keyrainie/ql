using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.BizProcessor;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(GiftCardAppService))]
    public class GiftCardAppService
    {
        /// <summary>
        /// 批量强制失效
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetGiftCardInvalid(List<GiftCardInfo> items)
        {
            ObjectFactory<GiftCardProcessor>.Instance.BatchSetGiftCardInvalid(items);
        }

        /// <summary>
        /// 强制失效
        /// </summary>
        /// <param name="items"></param>
        public virtual void SetGiftCardInvalid(GiftCardInfo item)
        {
            ObjectFactory<GiftCardProcessor>.Instance.SetGiftCardInvalid(item);
        }

        /// <summary>
        /// 批量锁定
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchLockGiftCard(List<int> items)
        {
            ObjectFactory<GiftCardProcessor>.Instance.BatchLockGiftCard(items);
        }

        /// <summary>
        /// 批量解锁
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchUnLockGiftCard(List<int> items)
        {
            ObjectFactory<GiftCardProcessor>.Instance.BatchUnLockGiftCard(items);
        }

        /// <summary>
        /// 批量激活
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchActiveGiftCard(List<int> sysNoList)
        {
            List<BatchActionItem<int>> items = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var bl = ObjectFactory<GiftCardProcessor>.Instance;

            var result = BatchActionManager.DoBatchAction<int, BizException>(items, (sysno) =>
            {
                bl.ActiveGiftCard(sysno);
            });

            return result.PromptMessage;
        }

        /// <summary>
        /// 审核礼品券商品
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchAuditVoucherProduct(List<int> sysNoList)
        {
            List<BatchActionItem<GiftVoucherProduct>> items = sysNoList.Select(x => new BatchActionItem<GiftVoucherProduct>()
            {
                ID = x.ToString(),
                Data = new GiftVoucherProduct() 
                {
                    SysNo = x
                }
            }).ToList();

            var bl = ObjectFactory<GiftCardProcessor>.Instance;

            var resutl = BatchActionManager.DoBatchAction<GiftVoucherProduct, BizException>(items, (p) => 
            {
                bl.AuditVoucherProduct(p);
            });

            return resutl.PromptMessage;
        }

        /// <summary>
        /// 拒绝审核礼品券商品
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchCancelAuditVoucherProduct(List<int> sysNoList)
        {
            List<BatchActionItem<GiftVoucherProduct>> items = sysNoList.Select(x => new BatchActionItem<GiftVoucherProduct>()
            {
                ID = x.ToString(),
                Data = new GiftVoucherProduct()
                {
                    SysNo = x
                }
            }).ToList();

            var bl = ObjectFactory<GiftCardProcessor>.Instance;

            var resutl = BatchActionManager.DoBatchAction<GiftVoucherProduct, BizException>(items, (p) =>
            {
                bl.CancelAuditVoucherProduct(p);
            });

            return resutl.PromptMessage;
        }

        /// <summary>
        /// 作废礼品券商品
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchVoidVoucherProduct(List<int> sysNoList)
        {
            List<BatchActionItem<GiftVoucherProduct>> items = sysNoList.Select(x => new BatchActionItem<GiftVoucherProduct>()
            {
                ID = x.ToString(),
                Data = new GiftVoucherProduct()
                {
                    SysNo = x
                }
            }).ToList();

            var bl = ObjectFactory<GiftCardProcessor>.Instance;

            var resutl = BatchActionManager.DoBatchAction<GiftVoucherProduct, BizException>(items, (p) =>
            {
                bl.VoidVoucherProduct(p);
            });

            return resutl.PromptMessage;
        }


        /// <summary>
        /// 审核礼品券商品关联请求
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchAuditVoucherRequest(List<int> sysNoList)
        {
            List<BatchActionItem<GiftVoucherProductRelationRequest>> items = sysNoList.Select(x => new BatchActionItem<GiftVoucherProductRelationRequest>()
            {
                ID = x.ToString(),
                Data = new GiftVoucherProductRelationRequest()
                {
                    SysNo = x
                }
            }).ToList();

            var bl = ObjectFactory<GiftCardProcessor>.Instance;

            var resutl = BatchActionManager.DoBatchAction<GiftVoucherProductRelationRequest, BizException>(items, (p) =>
            {
                bl.AuditVoucherRelationRequest(p);
            });

            return resutl.PromptMessage;
        }

        /// <summary>
        /// 审核礼品券商品关联请求
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchCancelAuditVoucherRequest(List<int> sysNoList)
        {
            List<BatchActionItem<GiftVoucherProductRelationRequest>> items = sysNoList.Select(x => new BatchActionItem<GiftVoucherProductRelationRequest>()
            {
                ID = x.ToString(),
                Data = new GiftVoucherProductRelationRequest()
                {
                    SysNo = x
                }
            }).ToList();

            var bl = ObjectFactory<GiftCardProcessor>.Instance;

            var resutl = BatchActionManager.DoBatchAction<GiftVoucherProductRelationRequest, BizException>(items, (p) =>
            {
                bl.CancelAuditVoucherRelationRequest(p);
            });

            return resutl.PromptMessage;
        }

        /// <summary>
        /// 获取礼品卡操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual List<ECCentral.BizEntity.IM.GiftCardOperateLog> GetGiftCardOperateLogByCode(string code)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GetGiftCardOperateLogByCode(code);
        }

        /// <summary>
        /// 获取礼品卡操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual List<ECCentral.BizEntity.IM.GiftCardRedeemLog> GetGiftCardRedeemLogJoinSOMaster(string code)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GetGiftCardRedeemLogJoinSOMaster(code);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateGiftCardInfo(ECCentral.BizEntity.IM.GiftCardInfo item)
        {
            ObjectFactory<GiftCardProcessor>.Instance.UpdateGiftCardInfo(item);
        }

        public virtual List<ECCentral.BizEntity.IM.GiftCardFabrication> GetGiftCardFabricationItem(int sysNo)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GetGiftCardFabricationItem(sysNo);
        }
        public virtual DataTable GetGiftCardFabricationItemSum(int sysNo)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GetGiftCardFabricationItemSum(sysNo);
        }

        /// <summary>
        /// 更新礼品卡主体信息及其子项信息
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateGiftCardFabrications(GiftCardFabricationMaster item)
        {
            ObjectFactory<GiftCardProcessor>.Instance.UpdateGiftCardFabrications(item);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="item"></param>
        public virtual void DeleteGiftCardFabrication(int sysNo)
        {
            ObjectFactory<GiftCardProcessor>.Instance.DeleteGiftCardFabrication(sysNo);
        }

        public virtual int CreatePOGiftCardFabrication(GiftCardFabricationMaster item)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.CreatePOGiftCardFabrication(item);
        }

        /// <summary>
        /// 获取当前生成的需要导出的礼品卡信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual DataTable GetGiftCardInfoByGiftCardFabricationSysNo(int sysNo)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GetGiftCardInfoByGiftCardFabricationSysNo(sysNo);
        }

        /// <summary>
        /// 获取生成的礼品卡信息及相应密码卡号
        /// </summary>
        /// <param name="item"></param>
        public virtual bool GetAddGiftCardInfoList(int sysNo)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GetAddGiftCardInfoList(sysNo);
        }

        /// <summary>
        /// 添加礼品券商品
        /// </summary>
        /// <param name="entity"></param>
        public virtual int AddGiftCardProductInfo(GiftVoucherProduct entity)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.AddGiftVoucher(entity);
        }

        /// <summary>
        /// 加载礼品券商品
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual GiftVoucherProduct GetGiftVoucherProductInfo(int sysNo)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GetVoucherProductBySysNo(sysNo);
        }

        /// <summary>
        /// 更新礼品券商品
        /// </summary>
        /// <param name="entity"></param>
        public virtual GiftVoucherProduct UpdateVoucherProductInfo(GiftVoucherProduct entity)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.UpdateVoucherProductInfo(entity);
        }

        /// <summary>
        /// 获取关联请求
        /// </summary>
        /// <param name="relationSysNo"></param>
        /// <returns></returns>
        public virtual List<GiftVoucherProductRelationRequest> GetGiftVoucherProductRelationRequest(int relationSysNo)
        {
            return ObjectFactory<GiftCardProcessor>.Instance.GetGiftVoucherProductRelationRequest(relationSysNo);
        }
    }
}
