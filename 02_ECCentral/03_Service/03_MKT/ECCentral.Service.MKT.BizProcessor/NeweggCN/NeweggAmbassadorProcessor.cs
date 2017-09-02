using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity;
using System.Transactions;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(NeweggAmbassadorProcessor))]
    public class NeweggAmbassadorProcessor
    {
        private INeweggAmbassadorDA _neweggAmbassadorDA = ObjectFactory<INeweggAmbassadorDA>.Instance;

        private string m_UnActiveString = "未激活（1）";
        private string m_ActiveString = "已激活（2）";
        private string m_CancelString = "已取消（3）";
        private string m_symbol = "=>";

        /// <summary>
        /// 更新泰隆优选大使的状态信息，即激活或取消泰隆优选大使。
        /// </summary>
        /// <param name="batchInfo"></param>
        public void MaintainNeweggAmbassadorStatus(NeweggAmbassadorBatchInfo batchInfo)
        {
            if (batchInfo == null || batchInfo.NeweggAmbassadors == null || batchInfo.NeweggAmbassadors.Count <= 0)
            {
                //throw new BizException("请至少选择一条泰隆优选大使信息！");
                throw new BizException(ResouceManager.GetMessageString("MKT.KeyWords", "Keywords_MustSelectUser"));
            }

            using (TransactionScope scope = new TransactionScope())
            {
                foreach (NeweggAmbassadorSatusInfo ambassadorStatusInfo in batchInfo.NeweggAmbassadors)
                {

                    //Log信息。
                    NeweggAmbassadorMaintainLogInfo logInfo = new NeweggAmbassadorMaintainLogInfo();
                    logInfo.AmbassadorSysNo = ambassadorStatusInfo.AmbassadorSysNo.Value;
                    logInfo.CompanyCode = batchInfo.CompanyCode;
                    

                    if (ambassadorStatusInfo.OrignCustomerMark == AmbassadorStatus.Active)
                    {
                        logInfo.Note = m_ActiveString + m_symbol + m_CancelString;
                        ambassadorStatusInfo.OrignCustomerMark = AmbassadorStatus.Canceled;
                    }
                    else
                    {
                        if (ambassadorStatusInfo.OrignCustomerMark == AmbassadorStatus.UnActive)
                        {
                            logInfo.Note = m_UnActiveString + m_symbol + m_ActiveString;
                        }
                        //已取消
                        else if (ambassadorStatusInfo.OrignCustomerMark == AmbassadorStatus.Canceled)
                        {
                            logInfo.Note = m_CancelString + m_symbol + m_ActiveString;
                        }
                        ambassadorStatusInfo.OrignCustomerMark = AmbassadorStatus.Active;
                    }

                    if (ambassadorStatusInfo.OrignCustomerMark == AmbassadorStatus.Active)
                    {
                        //激活泰隆优选大使   
                        _neweggAmbassadorDA.MaintainNeweggAmbassadorStatusActive(ambassadorStatusInfo);
                    }
                    else
                    {
                        //取消泰隆优选大使   
                        _neweggAmbassadorDA.MaintainNeweggAmbassadorStatusCancel(ambassadorStatusInfo);
                    }

                    switch (ambassadorStatusInfo.OrignCustomerMark)
                    {
                        case null:
                            logInfo.Status = "E";
                            break;
                        case AmbassadorStatus.Active:
                            logInfo.Status = "A";
                            break;
                        default:
                            logInfo.Status = "D";
                            break;
                    }

                    //记录下激活和取消的泰隆优选大使Log
                    _neweggAmbassadorDA.LogNeweggAmbassadorMaintainInfo(logInfo);
                }

                scope.Complete();
            }
        }

        /// <summary>
        /// 尝试更新泰隆优选大使的状态，返回需要确认的泰隆优选大使。
        /// </summary>
        /// <param name="batchInfo"></param>
        /// <returns></returns>
        public NeweggAmbassadorBatchInfo TryUpdateAmbassadorStatus(NeweggAmbassadorBatchInfo batchInfo)
        {
            //需要确认的泰隆优选大使
            NeweggAmbassadorBatchInfo confirmBatch = new NeweggAmbassadorBatchInfo();
            confirmBatch.CompanyCode = batchInfo.CompanyCode;

            //直接更新的泰隆优选大使
            NeweggAmbassadorBatchInfo updateBatch = new NeweggAmbassadorBatchInfo();
            updateBatch.CompanyCode = batchInfo.CompanyCode;

            foreach (NeweggAmbassadorSatusInfo statusInfo in batchInfo.NeweggAmbassadors)
            {
                //状态为未激活
                if (statusInfo.OrignCustomerMark == AmbassadorStatus.UnActive)
                {
                    updateBatch.NeweggAmbassadors.Add(statusInfo);
                }
                //状态为已取消
                else if (statusInfo.OrignCustomerMark == AmbassadorStatus.Canceled)
                {
                    int Flag = 0;

                    Flag = _neweggAmbassadorDA.CheckCustomerStatus(statusInfo);
                    //如果是三个月内的需要确认
                    if (Flag > 0)
                    {
                        confirmBatch.NeweggAmbassadors.Add(statusInfo);
                    }
                    else
                    {
                        updateBatch.NeweggAmbassadors.Add(statusInfo);
                    }
                }
            }

            if (updateBatch.NeweggAmbassadors != null && updateBatch.NeweggAmbassadors.Count > 0)
            {
                try
                {

                    MaintainNeweggAmbassadorStatus(updateBatch);
                }
                catch (Exception e)
                {
                    confirmBatch.NeweggAmbassadors = new List<NeweggAmbassadorSatusInfo>();

                    throw e;
                }
            }
            return confirmBatch;
        }

        /// <summary>
        /// 取消申请。
        /// </summary>
        /// <param name="batchInfo"></param>
        public void CancelRequestNeweggAmbassadorStatus(NeweggAmbassadorBatchInfo batchInfo)
        {

            if (batchInfo == null || batchInfo.NeweggAmbassadors==null||batchInfo.NeweggAmbassadors.Count<=0)
            {
                return;
            }
            foreach (NeweggAmbassadorSatusInfo statusInfo in batchInfo.NeweggAmbassadors)
            {
                if (statusInfo != null)
                {
                    NeweggAmbassadorEntity entity = new NeweggAmbassadorEntity();
                    entity.AmbassadorSysNo = statusInfo.AmbassadorSysNo;
                    entity.CompanyCode = statusInfo.CompanyCode;

                    var tempStatusInfo = _neweggAmbassadorDA.GetNeweggAmbassadorInfo(entity);
                    if (tempStatusInfo != null && tempStatusInfo.CustomerMark != null && tempStatusInfo.CustomerMark == AmbassadorStatus.UnActive)
                    {
                        bool result = _neweggAmbassadorDA.CancelRequestNeweggAmbassador(statusInfo);
                        if (result)
                        {
                            statusInfo.OrignCustomerMark = null;

                            //Log信息。
                            NeweggAmbassadorMaintainLogInfo logInfo = new NeweggAmbassadorMaintainLogInfo();
                            logInfo.AmbassadorSysNo = statusInfo.AmbassadorSysNo.Value;
                            logInfo.Status = "E";
                            logInfo.CompanyCode = batchInfo.CompanyCode;
                            logInfo.Note = null;

                            //记录下激活和取消的泰隆优选大使Log
                            _neweggAmbassadorDA.LogNeweggAmbassadorMaintainInfo(logInfo);
                        }
                    }

                }
            }
        }

    }
}
