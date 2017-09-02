using System;
using System.Linq;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.BizEntity;
using System.ComponentModel.Composition;
using ECCentral.Service.EventMessage;
using System.Collections.Generic;

namespace ECCentral.Service.SO.BizProcessor
{
    /// <summary>
    /// 锁定订单
    /// </summary>
    [Export(typeof(SOHolder))]
    [VersionExport(typeof(SOAction))]
    public class SOHolder : SOAction
    {
        private ISODA _soDA;
        protected ISODA SODA
        {
            get
            {
                _soDA = _soDA ?? ObjectFactory<ISODA>.Instance;
                return _soDA;
            }
        }
        /// <summary>
        /// 当前订单编号
        /// </summary>
        protected int SOSysNo
        {
            get
            {
                return CurrentSO.SysNo.Value;
            }
        }

        public override void Do()
        {
            Hold();
        }

        #region 订单锁定

        /// <summary>
        /// 判断订单是否已经通过网站被锁定
        /// </summary>
        public void CheckSOIsWebHold()
        {
            switch (CurrentSO.BaseInfo.HoldStatus)
            {
                case SOHoldStatus.WebHold:
                    BizExceptionHelper.Throw("SO_Hold_WebHold");
                    break;
                case SOHoldStatus.Processing:
                    BizExceptionHelper.Throw("SO_Hold_Processing");
                    break;
            }
        }

        /// <summary>
        /// 订单锁定前的检查
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <param name="reason">订单锁定原因</param>
        private void HoldPreCheck(SOHoldReason reason)
        {
            switch (CurrentSO.BaseInfo.Status)
            {
                case SOStatus.Origin:
                    CheckSOIsWebHold();
                    break;
                case SOStatus.Split:
                    BizExceptionHelper.Throw("SO_Hold_SplitComplete");
                    break;
                case SOStatus.Abandon:
                    BizExceptionHelper.Throw("SO_Hold_Abandoned");
                    break;
                case SOStatus.OutStock:
                    BizExceptionHelper.Throw("SO_Hold_OutStock");
                    break;
            }
            if (reason != SOHoldReason.CancelAuditOrder && CurrentSO.BaseInfo.HoldStatus.Value == SOHoldStatus.BackHold)
            {
                BizExceptionHelper.Throw("SO_Hold_BackHold");
            }

            // 检查是否出库
            if (SODA.IsNeweggOutStock(SOSysNo))
            {
                BizExceptionHelper.Throw("SO_Hold_OutStock");
            }

            // 检查订单是否正在处理 
            if (ObjectFactory<IOPCDA>.Instance.SOIsProcess(SOSysNo))
            {
                BizExceptionHelper.Throw("SO_Hold_IsAsyncHold");
            }

        }

        /// <summary>
        /// 直接锁住订单
        /// </summary>
        /// <param name="soBaseInfo"></param>
        /// <returns></returns>
        public virtual void DirectHoldSO()
        {
            CurrentSO.BaseInfo.HoldStatus = SOHoldStatus.BackHold;
            CurrentSO.BaseInfo.HoldTime = DateTime.Now;
            CurrentSO.BaseInfo.HoldUser = ServiceContext.Current.UserSysNo;

            //  1.  锁定订单操作
            SODA.UpdateSOHoldInfo(CurrentSO.BaseInfo);

            //  2.  添加订单操作日志
            WriteLog(ECCentral.BizEntity.Common.BizLogType.Sale_SO_HoldSO, "订单锁定");
        }

        /// <summary>
        /// 锁定订单。返回true表示同步锁定订单，false表示异步锁定订单
        /// </summary>
        /// <param name="soBaseInfo">订单基本信息</param>
        /// <returns>是不是已经同步锁定了订单，true 表示是，false 表示异步锁定订单（订单现在还没有锁定）</returns>
        public virtual bool Hold(SOHoldReason holdType, OPCCallBackType callBackType)
        {
            //  1.  锁定前的业务检查，订单处于以下状态时不能被锁定：1.已作废，已拆分，已出库；2.已经被锁定（前台网站和后台管理锁定后都不能再次锁定）
            HoldPreCheck(holdType);

            //  2.  判断WMS是否下载订单，如果下载则要Hold WMS
            bool wmsIsDownload = SODA.WMSIsDownloadSO(SOSysNo);

            //  3.  仓库锁定订单
            if (wmsIsDownload)
            {
                List<int> stockSysNoList = (from item in CurrentSO.Items
                                            where item.StockSysNo.HasValue && item.ProductType != SOProductType.Coupon && item.ProductType != SOProductType.ExtendWarranty
                                            select item.StockSysNo.Value).Distinct().ToList();
                try
                {
                    //同步锁定订单
                    WMSHoldMessage message = new WMSHoldMessage
                    {
                        SOSysNo = SOSysNo,
                        ActionType = ECCentral.Service.EventMessage.WMSActionType.Hold,
                        UserSysNo = ServiceContext.Current.UserSysNo,
                        WarehouseSysNoList = stockSysNoList,
                        Reason = CurrentSO.BaseInfo.HoldReason
                    };
                    EventPublisher.Publish<WMSHoldMessage>(message);
                }
                catch (ThirdPartBizException biz_ex)
                {
                    throw new BizException(biz_ex.Message);
                }
                catch
                {
                    //异步锁定订单
                    WMSAction action = WMSAction.Hold;
                    switch (holdType)
                    {
                        case SOHoldReason.AbandonOrder:
                            action = WMSAction.AbandonHold;
                            break;
                        case SOHoldReason.CancelAuditOrder:
                            action = WMSAction.CancelAuditHold;
                            break;
                    }
                    ObjectFactory<OPCProcessor>.Instance.SendMessageToWMS(CurrentSO, action, callBackType); //ExternalDomainBroker.SendMessageToWMS(CurrentSO, action, OPCCallBackType.HoldCallBack);

                    BizExceptionHelper.Throw("SO_Hold_NetErrorSysncHolding");
                }
            }
            //  3.  本地锁定订单
            DirectHoldSO();
            return true;
        } 
        /// <summary>
        /// 普通锁定订单
        /// </summary>
        /// <param name="soBaseInfo">订单基本信息</param>
        /// <returns>是不是同步锁定订单，true 表示是</returns>
        private void Hold()
        {
            if (!Hold(SOHoldReason.Hold, OPCCallBackType.HoldCallBack))
            {
                BizExceptionHelper.Throw("SO_Hold_AsyncHolding");
            }
        }

        #endregion


        #region

        /// <summary>
        /// 锁定原因
        /// </summary>
        public enum SOHoldReason
        {
            /// <summary>
            /// 普通的锁定
            /// </summary>
            Hold = 0,
            /// <summary>
            /// 取消审核订单时的锁定
            /// </summary>
            CancelAuditOrder = 1,
            /// <summary>
            /// 作废订单时的锁定
            /// </summary>
            AbandonOrder = 2,
        }

        #endregion
    }

    [VersionExport(typeof(SOAction), new string[] { "General", "Hold" })]
    public class GeneralSOHolder : SOHolder
    {
    }

    [Export(typeof(SOUnholder))]
    [VersionExport(typeof(SOAction))]
    public class SOUnholder : SOAction
    {
        private ISODA _soDA;
        protected ISODA SODA
        {
            get
            {
                _soDA = _soDA ?? ObjectFactory<ISODA>.Instance;
                return _soDA;
            }
        }
        /// <summary>
        /// 当前订单编号
        /// </summary>
        protected int SOSysNo
        {
            get
            {
                return CurrentSO.SysNo.Value;
            }
        }

        #region 订单解锁
        protected virtual void UnholdPreCheck(SOUnholdReason reason)
        {
            switch (CurrentSO.BaseInfo.Status.Value)
            {
                case SOStatus.Origin:
                case SOStatus.WaitingManagerAudit:
                    if (reason == SOUnholdReason.User && CurrentSO.BaseInfo.HoldStatus == SOHoldStatus.BackHold)
                    {
                        BizExceptionHelper.Throw("SO_Hold_SOIsWaitingManagerAudit");
                    }
                    break;
                case SOStatus.Abandon:
                    BizExceptionHelper.Throw("SO_Hold_Abandoned");
                    break;
                case SOStatus.OutStock:
                    BizExceptionHelper.Throw("SO_Hold_OutStock");
                    break;
                case SOStatus.Split:
                    BizExceptionHelper.Throw("SO_Hold_SplitComplete");
                    break;
            }
            if (CurrentSO.BaseInfo.Status != SOStatus.Origin && CurrentSO.BaseInfo.HoldStatus == SOHoldStatus.WebHold)
            {
                BizExceptionHelper.Throw("SO_Hold_SOIsNotOriginalStatus");
            }
        }

        /// <summary>
        /// 订单解锁
        /// </summary>
        /// <param name="soBaseInfo">订单基本信息</param>
        /// <param name="reason">解锁原因</param>
        internal virtual void Unhold(SOUnholdReason reason)
        {
            //  1.  解锁前的业务检查
            UnholdPreCheck(reason);

            CurrentSO.BaseInfo.HoldTime = DateTime.Now;
            CurrentSO.BaseInfo.HoldStatus = SOHoldStatus.Unhold;
            CurrentSO.BaseInfo.HoldUser = ServiceContext.Current.UserSysNo;
            //  2.  保存订单解锁
            SODA.UpdateSOHoldInfo(CurrentSO.BaseInfo);
            //  3.  添加订单更改日志，通知WMS重新Download订单
            ObjectFactory<SOLogProcessor>.Instance.WriteSOChangeLog(new SOChangeLog
            {
                ChangeTime = DateTime.Now,
                ChangeType = 3,
                CompanyCode = CurrentSO.CompanyCode,
                CustomerSysNo = CurrentSO.BaseInfo.CustomerSysNo.Value,
                Guid = Guid.NewGuid(),
                SOSysNo = SOSysNo,
            });
            //  4.  订单操作日志   
            WriteLog(BizEntity.Common.BizLogType.Sale_SO_UnHoldSO, "取消锁定");
        }

        public virtual void Unhold()
        {
            Unhold(SOUnholdReason.User);
        }
        #endregion

        #region 内部定义

        /// <summary>
        /// 解锁原因
        /// </summary>
        public enum SOUnholdReason
        {
            /// <summary>
            /// 用户直接解锁
            /// </summary>
            User,
            /// <summary>
            /// 审核订单解锁
            /// </summary>
            Audit
        }
        #endregion

        public override void Do()
        {
            Unhold();
        }
    }


    [VersionExport(typeof(SOAction), new string[] { "General", "Unhold" })]
    public class GeneralSOUnholder : SOUnholder
    {
    }

}
