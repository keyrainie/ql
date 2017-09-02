using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(SOInternalMemoProcessor))]
    public class SOInternalMemoProcessor
    {
        private ISOInternalMemoDA m_da = ObjectFactory<ISOInternalMemoDA>.Instance;

        //添加跟踪日志
        /// <summary>
        /// 添加跟踪日志
        /// </summary> 
        /// <param name="info">请求实体</param>
        public virtual void AddSOInternalMemoInfo(SOInternalMemoInfo info, string companyCode)
        {
            //如果状态为需要跟进，则允许选择提醒时间
            if (info.Status == SOInternalMemoStatus.FollowUp)
            {
                //如果没有输入时间，则给一个默认时间
                if (!info.RemindTime_Date.HasValue || !info.RemainTime_Time.HasValue)
                {
                    info.RemindTime = DateTime.Now.AddDays(1);
                }
                else
                {
                    //当前日期加上选择的时间点
                    DateTime remindTime = info.RemindTime_Date.Value
                                        .AddHours(info.RemainTime_Time.Value.Hour)
                                        .AddMinutes(info.RemainTime_Time.Value.Minute);
                    if (DateTime.Compare(remindTime, DateTime.Now.AddHours(1)) > 0)
                    {
                        info.RemindTime = remindTime;
                    }
                    else
                    {
                        BizExceptionHelper.Throw("SO_InternalMemo_RemainMoreOneHour");
                        return;
                    }
                }
            }
            else
            {
                info.Importance = null;
            }
            m_da.AddSOInternalMemoInfo(info, companyCode);
        }

        //修改跟踪信息
        /// <summary>
        /// 修改跟踪信息
        /// </summary>
        /// <param name="info">请求实体</param>
        public void UpdateSOInternalMemoInfo(SOInternalMemoInfo info)
        {
            m_da.UpdateSOInternalMemoInfo(info);
        }

        //批量分配跟进日志
        /// <summary>
        /// 批量分配跟进日志
        /// </summary>
        /// <param name="infoList">待分配的跟进日志集合</param>
        public void BatchAssignSOInternalMemo(List<SOInternalMemoInfo> infoList)
        {
            string sysNoList = string.Empty;
            foreach (var item in infoList)
            {
                var obj = m_da.GetBySysNo(item.SysNo.Value);
                if (obj != null)
                {
                    if (!obj.OperatorSysNo.HasValue
                        && obj.Status == SOInternalMemoStatus.FollowUp)
                    {
                        m_da.Update_AssignInfo(item);
                    }
                    else
                    {
                        sysNoList += "," + item.SysNo.Value.ToString();
                    }
                }
            }
            if (!string.IsNullOrEmpty(sysNoList))
            {
                BizExceptionHelper.Throw("SO_InternalMemo_AssignError", sysNoList.Substring(1));
            }
        }

        //批量取消分配
        /// <summary>
        /// 批量取消分配
        /// </summary>
        /// <param name="infoList">待取消分配的跟进日志集合</param>
        public void BatchCanceAssignSOInternalMemo(List<SOInternalMemoInfo> infoList)
        {
            string sysNoList = string.Empty;
            foreach (var item in infoList)
            {
                var obj = m_da.GetBySysNo(item.SysNo.Value);
                if (obj != null)
                {
                    if (!obj.OperatorSysNo.HasValue
                        || obj.Status == SOInternalMemoStatus.Complete)
                    {
                        sysNoList += "," + item.SysNo.Value.ToString();
                    }
                    else
                    {
                        item.OperatorSysNo = null;
                        m_da.Update_AssignInfo(item);
                    }
                }
            }
            if (!string.IsNullOrEmpty(sysNoList))
            {
                BizExceptionHelper.Throw("SO_InternalMemo_CancelAssignError", sysNoList.Substring(1));
            }
        }

        //关闭跟进日志(更新 Status 和 Note)
        /// <summary>
        /// 关闭跟进日志(更新 Status 和 Note)
        /// </summary>
        /// <param name="info">请求实体</param>
        public void CloseSOInternalMemo(SOInternalMemoInfo info)
        {
            info.Status = SOInternalMemoStatus.Complete;
            info.RemindTime = null;
            m_da.Update_StatusInfo(info);
        }

        //获取公司的订单跟进日志的创建者列表
        /// <summary>
        /// 获取公司的订单跟进日志的创建者列表
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns>订单跟进日志的创建者列表</returns>
        public virtual List<CSInfo> GetSOLogCreaterByCompanyCode(string companyCode)
        {
            return m_da.GetSOLogCreater(companyCode);
        }

        //获取公司的订单跟进日志的更新者列表
        /// <summary>
        /// 获取公司的订单跟进日志的更新者列表
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns>订单跟进日志的更新者列表</returns>
        public virtual List<CSInfo> GetSOLogUpdaterByCompanyCode(string companyCode)
        {
            return m_da.GetSOLogUpdate(companyCode);
        }

        public virtual List<SOInternalMemoInfo> GetBySOSysNo(int soSysNo)
        {
            return m_da.GetBySOSysNo(soSysNo);
        }
    }
}
