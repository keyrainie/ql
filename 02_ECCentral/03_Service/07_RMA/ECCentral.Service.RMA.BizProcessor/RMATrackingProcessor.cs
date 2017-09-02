using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using System.Transactions;

namespace ECCentral.Service.RMA.BizProcessor
{
    [VersionExport(typeof(RMATrackingProcessor))]
    public class RMATrackingProcessor
    {
        private IRMATrackingDA DA = ObjectFactory<IRMATrackingDA>.Instance;

        /// <summary>
        /// 创建RMA跟进日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual InternalMemoInfo CreateRMATracking(InternalMemoInfo entity)
        {
            InternalMemoInfo result = null;
            //检查单件号是否有效
            if (!DA.IsExistRegisterSysNo(entity.RegisterSysNo.Value))
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "RegisterNotExists");
                msg = string.Format(msg, entity.RegisterSysNo);
                throw new BizException(msg);
            }
            string currentUserName = ExternalDomainBroker.GetUserInfoBySysNo(ServiceContext.Current.UserSysNo);
            //更新Register的备注信息
            RegisterBasicInfo registerEntity = new RegisterBasicInfo()
            {
                SysNo = entity.RegisterSysNo,
                Memo = String.Format("{0}[{1} {2}]", entity.Content, currentUserName, DateTime.Now)
            };

            using (TransactionScope scope = new TransactionScope())
            {
                result = DA.Create(entity);
                ObjectFactory<IRegisterDA>.Instance.UpdateMemo(registerEntity);
                scope.Complete();
            }

            return result;
        }
        /// <summary>
        /// 关闭RMA跟进单
        /// </summary>
        /// <param name="msg"></param>
        public virtual void CloseRMATracking(InternalMemoInfo msg)
        {
            //关闭RMA跟进单的同时需要关闭客户来电记录
            ExternalDomainBroker.CloseCustomerCalling(msg.SysNo.Value,msg.Note);
            DA.Close(msg);
        }

        /// <summary>
        /// 派发RMA跟进单
        /// </summary>
        /// <param name="msg"></param>
        public virtual void DispatchRMATracking(List<int> sysNoList, int handlerSysNo)
        {
            string error1 = ResouceManager.GetMessageString("RMA.RMATracking", "CannotDispatchDuplicateTracking");
            string error2 = ResouceManager.GetMessageString("RMA.RMATracking", "CannotDispatchComplateTracking");
            List<string> error1List = new List<string>();
            List<string> error2List = new List<string>();

            foreach (int sysNo in sysNoList)
            {
                var temp = DA.GetRMATrackingBySysNo(sysNo);
                if (temp.Status == InternalMemoStatus.Close)
                {
                    error2List.Add(temp.SysNo.ToString());
                    continue;
                }
                if (!DA.Dispatch(sysNo, handlerSysNo))
                {
                    error1List.Add(temp.SysNo.ToString());
                }
            }
            HandleDispatchError(error1, error2, error1List, error2List);
        }
        /// <summary>
        /// 取消派发RMA跟进单
        /// </summary>
        /// <param name="msg"></param>
        public virtual void CancelDispatchRMATracking(List<int> sysNoList)
        {
            string error1 = ResouceManager.GetMessageString("RMA.RMATracking", "CannotCancelDispatchOriginalTracking");
            string error2 = ResouceManager.GetMessageString("RMA.RMATracking", "CannotCancelDispatchComplateTracking");
            List<string> error1List = new List<string>();
            List<string> error2List = new List<string>();

            foreach (int sysNo in sysNoList)
            {
                var temp = DA.GetRMATrackingBySysNo(sysNo);
                if (temp.Status == InternalMemoStatus.Close)
                {
                    error2List.Add(temp.SysNo.ToString());
                    continue;
                }
                if (!DA.CancelDispatch(sysNo))
                {
                    error1List.Add(temp.SysNo.ToString());
                }
            }
            HandleDispatchError(error1, error2, error1List, error2List);
        }
        /// <summary>
        /// 返回操作成功/失败提示信息
        /// </summary>
        /// <param name="error1"></param>
        /// <param name="error2"></param>
        /// <param name="error1List"></param>
        /// <param name="error2List"></param>
        private void HandleDispatchError(string error1, string error2, List<string> error1List, List<string> error2List)
        {
            if (error1List.Count != 0 || error2List.Count != 0)
            {
                string error = string.Empty;
                if (error1List.Count != 0)
                    error += string.Format(error1, string.Join(",", error1List.ToArray()));
                if (error2List.Count != 0)
                    error += string.Format(error2, string.Join(",", error2List.ToArray()));
                throw new BizException(error);
            }
        }
       
    }
}
