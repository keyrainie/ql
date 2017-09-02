using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IDefaultRMAPolicy))]
    public class DefaultRMAPolicy : IDefaultRMAPolicy
    {
        //获取退换货政策
        public DataTable GetDefaultRMAPolicyByQuery(DefaultRMAPolicyFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetDefaultRMAPolicyByQuery");
            cmd.SetParameterValue("@RMAPolicySysNo", query.RMAPolicySysNo);
            cmd.SetParameterValue("@BrandSysNo", query.BrandSysNo);
            cmd.SetParameterValue("@C1SysNo", query.C1SysNo);
            cmd.SetParameterValue("@C2SysNo", query.C2SysNo);
            cmd.SetParameterValue("@C3SysNo", query.C3SysNo);
            cmd.SetParameterValue("@PageSize", query.PageInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", query.PageInfo.PageIndex);
            cmd.SetParameterValue("@SortField", query.PageInfo.SortBy);
            DataTable dt = cmd.ExecuteDataTable();
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;
        }
        //插入退换货政策
        public int InsertDefaultRMAPolicyInfo(DefaultRMAPolicyInfo defaultRMAPolicy)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertDefaultRMAPolicyInfo");
            cmd.SetParameterValue("@BrandSysNo", defaultRMAPolicy.BrandSysNo);
            cmd.SetParameterValue("@C3SysNo", defaultRMAPolicy.C3SysNo);
            cmd.SetParameterValue("@RMAPolicySysNo", defaultRMAPolicy.RMAPolicySysNo);
            cmd.SetParameterValue("@InUser", defaultRMAPolicy.CreateUser.UserDisplayName);
            cmd.SetParameterValue("@LanguageCode", "Zh-Cn");
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@SysNo");
        }
        //更新退换货政策
        public void UpdateDefaultRMAPolicyBySysNo(DefaultRMAPolicyInfo defaultRMAPolicy)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateDefaultRMAPolicyBySysNo");
            cmd.SetParameterValue("@SysNo", defaultRMAPolicy.SysNo);
            cmd.SetParameterValue("@RMAPolicySysNo", defaultRMAPolicy.RMAPolicySysNo);
            cmd.SetParameterValue("@EditUser", defaultRMAPolicy.EditUser.UserDisplayName);
            cmd.ExecuteNonQuery();
        }
        //查询默认退换货政策是否存在重复
        public List<DefaultRMAPolicyInfo> DefaultRMAPolicyByAll()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DefaultRMAPolicyByAll");
            return cmd.ExecuteEntityList<DefaultRMAPolicyInfo>();
        }
        //批量删除退换货政策
        public void DelDefaultRMAPolicyBySysNo(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DelDefaultRMAPolicyBySysNo");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据商品编号查询退换货政策
        /// </summary>
        /// <returns></returns>
        public DefaultRMAPolicyInfo GetDefaultRMAPolicy(int c3sysno,int brandsysno) 
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetDefaultRMAPolicy");
            cmd.SetParameterValue("@C3SysNo", c3sysno);
            cmd.SetParameterValue("@BrandSysNo", brandsysno);
            return cmd.ExecuteEntity<DefaultRMAPolicyInfo>();
        }
    }
}
