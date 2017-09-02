using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    /// <summary>
    /// 默认退换货政策
    /// </summary>
    public interface IDefaultRMAPolicy
    {
        //获取退换货政策
        DataTable GetDefaultRMAPolicyByQuery(DefaultRMAPolicyFilter query, out int totalCount);
        //插入退换货政策
        int InsertDefaultRMAPolicyInfo(DefaultRMAPolicyInfo defaultRMAPolicy);
        //更新退换货政策
        void UpdateDefaultRMAPolicyBySysNo(DefaultRMAPolicyInfo defaultRMAPolicy);
        //查询默认退换货政策是否存在重复
        List<DefaultRMAPolicyInfo> DefaultRMAPolicyByAll();
        //批量删除退换货政策
        void DelDefaultRMAPolicyBySysNo(Int32 SysNo);

        /// <summary>
        /// 根据商品编号查询退换货政策
        /// </summary>
        /// <returns></returns>
        DefaultRMAPolicyInfo GetDefaultRMAPolicy(int c3sysno, int brandsysno);
        
    }
}
