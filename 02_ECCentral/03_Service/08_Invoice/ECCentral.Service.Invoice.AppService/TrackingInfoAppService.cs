using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(TrackingInfoAppService))]
    public class TrackingInfoAppService
    {
        #region TrackingInfo

        /// <summary>
        /// 添加跟踪单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TrackingInfo CreateTrackingInfo(TrackingInfo entity)
        {
            return ObjectFactory<TrackingInfoProcessor>.Instance.CreateTrackingInfo(entity);
        }

        /// <summary>
        /// 批量创建跟踪单
        /// </summary>
        /// <param name="trackingInfoList"></param>
        /// <returns></returns>
        public virtual string BatchCreateTrackingInfo(List<TrackingInfo> trackingInfoList)
        {
            var request = trackingInfoList.Select(x => new BatchActionItem<TrackingInfo>()
            {
                ID = x.OrderSysNo.ToString(),
                Data = x
            }).ToList();

            var trackingInfoBL = ObjectFactory<TrackingInfoProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, trackingInfo =>
            {
                trackingInfoBL.CreateTrackingInfo(trackingInfo);
            });

            return result.PromptMessage;
        }

        /// <summary>
        /// 更新跟踪单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void UpdateTrackingInfo(TrackingInfo entity)
        {
            ObjectFactory<TrackingInfoProcessor>.Instance.UpdateTrackingInfo(entity);
        }

        /// <summary>
        /// 提交报损
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual void SubmitTrackingInfo(int sysNo)
        {
            ObjectFactory<TrackingInfoProcessor>.Instance.SubmitTrackingInfo(sysNo);
        }

        /// <summary>
        /// 批量提交报损
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchSubmitTrackingInfo(List<int> sysNoList)
        {
            var request = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var trackingInfoBL = ObjectFactory<TrackingInfoProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, sysNo =>
            {
                trackingInfoBL.SubmitTrackingInfo(sysNo);
            });

            return result.PromptMessage;
        }

        /// <summary>
        /// 关闭跟踪单
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual void CloseTrackingInfo(int sysNo)
        {
            ObjectFactory<TrackingInfoProcessor>.Instance.CloseTrackingInfo(sysNo);
        }

        /// <summary>
        /// 批量关闭跟踪单
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchCloseTrackingInfo(List<int> sysNoList)
        {
            var request = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var trackingInfoBL = ObjectFactory<TrackingInfoProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, sysNo =>
            {
                trackingInfoBL.CloseTrackingInfo(sysNo);
            });

            return result.PromptMessage;
        }

        /// <summary>
        /// 批量设置跟踪单责任人姓名
        /// </summary>
        /// <param name="trackingInfoList"></param>
        /// <returns></returns>
        public virtual string BatchUpdateTrackingInfoResponsibleUserName(List<TrackingInfo> trackingInfoList)
        {
            var request = trackingInfoList.Select(x => new BatchActionItem<TrackingInfo>()
            {
                ID = x.SysNo.ToString(),
                Data = x
            }).ToList();

            var trackingInfoBL = ObjectFactory<TrackingInfoProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, trackingInfo =>
            {
                trackingInfoBL.UpdateTrackingInfoResponsibleUserName(trackingInfo, "");
            });

            return result.PromptMessage;
        }

        /// <summary>
        /// 批量设置跟踪单损失类型
        /// </summary>
        /// <param name="trackingInfoList"></param>
        /// <returns></returns>
        public string BatchUpdateTrackingInfoLossType(List<TrackingInfo> trackingInfoList)
        {
            var request = trackingInfoList.Select(x => new BatchActionItem<TrackingInfo>()
            {
                ID = x.SysNo.ToString(),
                Data = x
            }).ToList();

            var trackingInfoBL = ObjectFactory<TrackingInfoProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, trackingInfo =>
            {
                trackingInfoBL.UpdateTrackingInfoLossType(trackingInfo);
            });

            return result.PromptMessage;
        }

        #endregion TrackingInfo

        #region ResponsibleUser

        public void CreateOrUpdateResponsibleUser(ResponsibleUserInfo entity, bool overrideWhenCreate)
        {
            var existed = GetExistedResponsibleUser(entity);

            if (!overrideWhenCreate)
            {
                if (existed != null)
                {
                    throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.TrackingInfo, "TrackingInfo_CreateOrUpdateResponsibleUser_Exist"));
                }
                ObjectFactory<TrackingInfoProcessor>.Instance.CreateResponsibleUser(entity);
            }
            else
            {
                if (existed == null)
                {
                    throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.TrackingInfo, "TrackingInfo_CreateOrUpdateResponsibleUser_NotFound"));
                }
                entity.SysNo = existed.SysNo;
                ObjectFactory<TrackingInfoProcessor>.Instance.UpdateResponsibleUser(entity);
            }
        }

        public ResponsibleUserInfo GetExistedResponsibleUser(ResponsibleUserInfo entity)
        {
            return ObjectFactory<TrackingInfoProcessor>.Instance.GetExistedResponsibleUser(entity);
        }

        public string BatchAbandonResponsibleUser(List<int> sysNoList)
        {
            var Request = sysNoList.Select(s => new BatchActionItem<int>()
            {
                ID = s.ToString(),
                Data = s
            }).ToList();

            var BL = ObjectFactory<TrackingInfoProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(Request, sysNo =>
            {
                BL.AbandonResponsibleUser(sysNo);
            });
            return result.PromptMessage;
        }

        #endregion ResponsibleUser
    }
}