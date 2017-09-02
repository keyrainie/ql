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

namespace ECCentral.Service.Inventory.BizProcessor
{
    /// <summary>
    /// 移仓单
    /// </summary>
    [VersionExport(typeof(ShiftRequestMemoProcessor))]
    public class ShiftRequestMemoProcessor
    {
        private IShiftRequestMemoDA shiftRequestMemoDA = ObjectFactory<IShiftRequestMemoDA>.Instance;

        /// <summary>
        /// 根据RequestSysNo获取跟进日志信息列表
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual List<ShiftRequestMemoInfo> GetShiftRequestMemoInfoListByRequestSysNo(int requestSysNo)
        {
            return shiftRequestMemoDA.GetShiftRequestMemoInfoListByRequestSysNo(requestSysNo);
        }

        /// <summary>
        /// 根据MemoSysNo获取跟进日志信息
        /// </summary>
        /// <param name="memoSysNo"></param>
        /// <returns></returns>
        public virtual ShiftRequestMemoInfo GetShiftRequestMemoInfoBySysNo(int memoSysNo)
        {
            return shiftRequestMemoDA.GetShiftRequestMemoInfoBySysNo(memoSysNo);
        }


        /// <summary>
        /// 创建跟进日志
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual List<ShiftRequestMemoInfo> CreateShiftRequestMemo(List<ShiftRequestMemoInfo> entityToCreate)
        {
            List<ShiftRequestMemoInfo> result = new List<ShiftRequestMemoInfo>();
            entityToCreate.ForEach(x => {
                PreCheckShiftRequestMemoInfoForCreate(x);

                if (x.MemoStatus == ShiftRequestMemoStatus.Finished)
                {
                    x.EditDate = DateTime.Now;
                }

                result.Add(shiftRequestMemoDA.CreateShiftRequestMemo(x));    
            
            });

            return result;
        }


        /// <summary>
        /// 批量更新跟进日志
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual List<ShiftRequestMemoInfo> UpdateShiftRequestMemoList(List<ShiftRequestMemoInfo> entityListToUpdate)
        {
            List<ShiftRequestMemoInfo> results = new List<ShiftRequestMemoInfo>();
            using (var scope = new TransactionScope())
            {
                foreach (ShiftRequestMemoInfo shiftMemo in entityListToUpdate)
                {
                    PreCheckShiftRequestInfoForUpdate(shiftMemo);
                    ShiftRequestMemoInfo result = shiftRequestMemoDA.UpdateShiftRequestMemo(shiftMemo);
                    results.Add(result);
                }

                scope.Complete();
            }

            return results;
        }

        #region 私有方法

        private void PreCheckShiftRequestMemoInfoForCreate(ShiftRequestMemoInfo entityToCreate)
        {
            if (entityToCreate == null)
            {
                BizExceptionHelper.Throw("ShiftInternalMemo_ItemContentNotBeNull");
                //throw new BizException("WarningMessage.ShiftInternalMemo_ItemContentNotBeNullValue");
            }

            if (entityToCreate.Content == null)
            {
                BizExceptionHelper.Throw("ShiftInternalMemo_ItemContentNotBeNull");
                //throw new BizException("WarningMessage.ShiftInternalMemo_ItemContentNotBeNullValue");
            }

            if (entityToCreate.MemoStatus == ShiftRequestMemoStatus.FollowUp)
            {
                if (entityToCreate.RemindTime == null)
                {
                    BizExceptionHelper.Throw("ShiftInternalMemo_ItemRemindTimeNotBeNull");
                    //throw new BizException("WarningMessage.ShiftInternalMemo_ItemRemindTimeNotBeNullValue");
                }
                else if (DateTime.Compare(entityToCreate.RemindTime.Value, DateTime.Now.AddHours(1)) < 0)
                {
                    BizExceptionHelper.Throw("ShiftInternalMemo_ItemRemindTimeShouldBeGreaterThanNowOneHour");
                    //throw new BizException("WarningMessage.ShiftInternalMemo_ItemRemindTimeShouldBeGreaterThanNowOneHourValue");
                }
            }           

        }

        private void PreCheckShiftRequestInfoForUpdate(ShiftRequestMemoInfo entityToUpdate)
        {
            if (entityToUpdate == null)
            {
                BizExceptionHelper.Throw("ShiftInternalMemo_ItemContentNotBeNull");
                //throw new BizException("WarningMessage.ShiftInternalMemo_ItemNoteNotBeNullValue");
            }

            if (entityToUpdate.Note == null)
            {
                BizExceptionHelper.Throw("ShiftInternalMemo_ItemContentNotBeNull");
                //throw new BizException("WarningMessage.ShiftInternalMemo_ItemNoteNotBeNullValue");
            }
        }

        #endregion 私有方法
    }
}
