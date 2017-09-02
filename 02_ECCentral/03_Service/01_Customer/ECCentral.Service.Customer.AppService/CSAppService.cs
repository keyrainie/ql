using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.BizProcessor;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(CSAppService))]
    public class CSAppService
    {
        /// <summary>
        /// 新增CS人员
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CSInfo Create(CSInfo entity)
        {
            return ObjectFactory<CSProcessor>.Instance.Create(entity);
        }
        /// <summary>
        /// 更新cs信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(CSInfo entity)
        {
            ObjectFactory<CSProcessor>.Instance.EditCSList(entity);
        }
        /// <summary>
        /// 获得某个部门下所有CS人员
        /// </summary>
        /// <param name="depid"></param>
        /// <returns></returns>
        public virtual List<CSInfo> GetCSWithDepartmentId(int depid)
        {
            return ObjectFactory<CSProcessor>.Instance.GetCSWithDepartmentId(depid);
        }
        /// <summary>
        /// 获取所有的CS人员
        /// </summary>
        /// <returns></returns>
        public virtual List<CSInfo> GetAllCS(string companyCode)
        {
            return ObjectFactory<CSProcessor>.Instance.GetAllCS(companyCode);
        }
        /// <summary>
        /// 获得某个领导的所有下属
        /// </summary>
        /// <param name="leaderSysNo"></param>
        /// <returns></returns>
        public virtual List<CSInfo> GetCSByLeaderSysNo(int leaderSysNo)
        {
            return ObjectFactory<CSProcessor>.Instance.GetCSByLeaderSysNo(leaderSysNo);
        }
    }
}
