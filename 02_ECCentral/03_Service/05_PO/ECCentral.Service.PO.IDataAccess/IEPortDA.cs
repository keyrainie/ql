using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface IEPortDA
    {
        /// <summary>
        /// 创建电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        EPortEntity CreateEPort(EPortEntity entity);

        /// <summary>
        /// 保存电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        EPortEntity SaveEPort(EPortEntity entity);

        /// <summary>
        /// 删除电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        int DeleteEPort(int sysNo);

        /// <summary>
        /// 获取电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        EPortEntity GetEPort(string sysNo);

        /// <summary>
        /// 获取电子口岸列表
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        DataTable QueryEPort(EPortFilter entity, out int totalCount);

        /// <summary>
        /// 获取电子口岸列表
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        List<EPortEntity> GetEPort();
    }
}
