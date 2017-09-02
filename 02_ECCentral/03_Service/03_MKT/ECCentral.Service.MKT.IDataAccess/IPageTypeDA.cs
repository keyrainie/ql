using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT.PageType;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IPageTypeDA
    {
        /// <summary>
        /// 获取系统中所有的页面类型
        /// </summary>
        /// <returns>页面类型Key,Value列表</returns>
        List<CodeNamePair> GetPageTypes(string companyCode, string channelID);

        void Create(PageType entity);

        void Update(PageType entity);

        PageType Load(int sysNo);

        int CountPageTypeName(int excludeSysNo, string pageTypeName, string companyCode, string channelID);

        int GetMaxPageTypeID(string companyCode, string channelID);
    }
}
