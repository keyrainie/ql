using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(ITopItemDA))]
    public class TopItemDA : ITopItemDA
    {
        #region ITopItemDA Members

        public void CreateTopItem(TopItemInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("TopItem_Create");
            dc.SetParameterValue<TopItemInfo>(entity);
            dc.ExecuteNonQuery();
        }

        public void DeleteTopItem(TopItemInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("TopItem_Remove");
            dc.SetParameterValue<TopItemInfo>(entity);
            dc.ExecuteNonQuery();
        }

        public void UpdateTopItemPriority(TopItemInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("TopItem_Update");
            dc.SetParameterValue<TopItemInfo>(entity);
            dc.ExecuteNonQuery();
        }

        public List<TopItemInfo> QueryTopItem(int PageType, int RefSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("TopItem_Query");
            dc.SetParameterValue("@CategorySysNo", RefSysNo);
            dc.SetParameterValue("@CategoryType", PageType);
            return dc.ExecuteEntityList<TopItemInfo>();
        }


        public void CreateTopItemConfig(TopItemConfigInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("TopItemConfig_Create");
            dc.SetParameterValue<TopItemConfigInfo>(entity);
            dc.ExecuteNonQuery();
        }

        public void UpdateTopItemConfig(TopItemConfigInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("TopItemConfig_Update");
            dc.SetParameterValue<TopItemConfigInfo>(entity);
            dc.ExecuteNonQuery();
        }

        public TopItemConfigInfo LoadItemConfig(int PageType, int RefSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("TopItemConfig_Load");
            dc.SetParameterValue("@CategorySysNo", RefSysNo);
            dc.SetParameterValue("@CategoryType", PageType);
            return dc.ExecuteEntity<TopItemConfigInfo>();
        }

        #endregion


    }
}
