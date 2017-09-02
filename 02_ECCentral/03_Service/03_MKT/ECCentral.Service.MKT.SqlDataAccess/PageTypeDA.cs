using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT.PageType;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IPageTypeDA))]
    public class PageTypeDA : IPageTypeDA
    {
        public List<CodeNamePair> GetPageTypes(string companyCode, string channelID)
        {
            var cmd = DataCommandManager.GetDataCommand("PageType_GetAllPageType");
            List<CodeNamePair> allPageTypes = cmd.ExecuteEntityList<CodeNamePair>();

            return allPageTypes;
        }

        //插入页面类型
        public void Create(PageType entity)
        {
            var cmd = DataCommandManager.GetDataCommand("PageType_CreatePageTypeData");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
        }

        //更新页面类型
        public void Update(PageType entity)
        {
            var cmd = DataCommandManager.GetDataCommand("PageType_UpdatePageTypeData");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
        }

        //加载页面类型
        public PageType Load(int sysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("PageType_LoadPageTypeData");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteEntity<PageType>();
        }

        //检查PageTypeName是否重复
        public int CountPageTypeName(int excludeSysNo, string pageTypeName, string companyCode, string channelID)
        {
            var cmd = DataCommandManager.GetDataCommand("PageType_CountPageTypeName");
            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.SetParameterValue("@PageTypeName", pageTypeName);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            //TODO:添加ChannelID参数

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        //获取最大的PageTypeID
        public int GetMaxPageTypeID(string companyCode, string channelID)
        {
            var cmd = DataCommandManager.GetDataCommand("PageType_GetMaxPageTypeID");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            //TODO:添加ChannelID参数

            object result = cmd.ExecuteScalar();
            if (result == DBNull.Value)
                return 0;
            return Convert.ToInt32(result);
        }
    }
}
