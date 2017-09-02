using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.SO.BizProcessor;

namespace ECCentral.Service.SO.AppService
{
    [VersionExport(typeof(SOInternalMemoAppService))]
    public class SOInternalMemoAppService
    {
        /// <summary>
        /// 添加跟踪
        /// </summary>
        /// <param name="info"></param>
        public virtual void AddSOInternalMemoInfo(SOInternalMemoInfo info,string companyCode)
        {
            ObjectFactory<SOInternalMemoProcessor>.Instance.AddSOInternalMemoInfo(info, companyCode);
        }
        /// <summary>
        /// 修改跟踪信息
        /// </summary>
        /// <param name="info"></param>
        public virtual void UpdateSOInternalMemoInfo(SOInternalMemoInfo info)
        {
            ObjectFactory<SOInternalMemoProcessor>.Instance.UpdateSOInternalMemoInfo(info);
        }

        /// <summary>
        /// 批量派发
        /// </summary>
        /// <param name="infoList"></param>
        public virtual void BatchAssignSOInternalMemo(List<SOInternalMemoInfo> infoList)
        {
            ObjectFactory<SOInternalMemoProcessor>.Instance.BatchAssignSOInternalMemo(infoList);         
        }

        /// <summary>
        /// 批量撤销派发
        /// </summary>
        /// <param name="infoList"></param>
        public virtual void BatchCanceAssignSOInternalMemo(List<SOInternalMemoInfo>infoList)        
        {
            ObjectFactory<SOInternalMemoProcessor>.Instance.BatchCanceAssignSOInternalMemo(infoList);           
        }

        /// <summary>
        /// 关闭跟踪
        /// </summary>
        /// <param name="info"></param>
        public virtual void CloseSOInternalMemo(SOInternalMemoInfo info)
        {
            ObjectFactory<SOInternalMemoProcessor>.Instance.CloseSOInternalMemo(info);           
        }

        public virtual string GetReasonCodePath(int reasonCodeSysNo, string companyCode)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetReasonCodePath(reasonCodeSysNo, companyCode);
        }

        public virtual List<CSInfo> GetSOLogCreater(string companyCode)
        {
            return ObjectFactory<SOInternalMemoProcessor>.Instance.GetSOLogCreaterByCompanyCode(companyCode);
        }

        public virtual List<CSInfo> GetSOLogUpdater(string companyCode)
        {
            return ObjectFactory<SOInternalMemoProcessor>.Instance.GetSOLogUpdaterByCompanyCode(companyCode);
        }
    }
}
