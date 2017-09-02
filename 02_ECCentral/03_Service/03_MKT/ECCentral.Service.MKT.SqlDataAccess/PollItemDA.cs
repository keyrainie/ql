using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IPollItemDA))]
    public class PollItemDA : IPollItemDA
    {        
        #region  投票

        /// <summary>
        /// 创建投票，作为聚合根，只创建活动的基本信息，得到活动ID
        /// </summary>
        /// <param name="item"></param>
        public int CreatePollMaster(PollMaster item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_CreatePollMaster");
            dc.SetParameterValue<PollMaster>(item);
            dc.ExecuteNonQuery();
            return Convert.ToInt32(dc.GetParameterValue("@SysNo"));
        }

        /// <summary>
        /// 创建投票，作为聚合根，只创建活动的基本信息，得到活动ID
        /// </summary>
        /// <param name="item"></param>
        public void UpdatePollMaster(PollMaster item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_UpdatePollMaster");
            dc.SetParameterValue<PollMaster>(item);     
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 检查 PageType=4 AND Status='A'的情况，不能超过三次
        /// </summary>
        /// <returns></returns>
        public int CheckPageTypeForCreatePollMaster(PollMaster item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_CheckPageTypeForCreatePollMaster");
            dc.SetParameterValue<PollMaster>(item);
            DataTable dt = dc.ExecuteDataTable();
            return Convert.ToInt32(dt.Rows[0][0].ToString());
        }

        /// <summary>
        /// 查检其它PageType类型是否已经创建两次。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int CheckForCreatePollMaster(PollMaster item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_CheckForCreatePollMaster");
            dc.SetParameterValue<PollMaster>(item);
            DataTable dt = dc.ExecuteDataTable();
            return Convert.ToInt32(dt.Rows[0][0].ToString());
        }

        /// <summary>
        /// 加载投票基本信息
        /// </summary>
        /// <param name="item"></param>
        public PollMaster LoadPollMaster(int itemID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_LoadPollMasterBySysno");
            dc.SetParameterValue("@SysNo", itemID);
            DataTable dt = dc.ExecuteDataTable<ADStatus>("Status");
            return DataMapper.GetEntity<PollMaster>(dt.Rows[0]);
        }

        #endregion

        #region 投票--问题组（PollItemGroup）
        /// <summary>
        /// 创建投票问题组，只创建问题组的基本信息
        /// </summary>
        /// <param name="item"></param>
        public void CreatePollItemGroup(PollItemGroup item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_CreatePollItemGroup");
            dc.SetParameterValue<PollItemGroup>(item);
            dc.ExecuteNonQuery();
        }


        /// <summary>
        /// 编辑投票问题组基本信息
        /// </summary>
        /// <param name="item"></param>
        public void UpdatePollItemGroup(PollItemGroup item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_UpdatePollGroupName");
            dc.SetParameterValue<PollItemGroup>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除投票问题组
        /// </summary>
        /// <param name="item"></param>
        public void DeletePollItemGroup(int itemID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_DeletePollGroup");
            dc.SetParameterValue("@SysNo", itemID);
            dc.ExecuteNonQuery();
        }


        /// <summary>
        /// 加载投票问题组所有问题
        /// </summary>
        /// <param name="sysNo"></param>
        public List<PollItemGroup> GetPollItemGroupList(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_GetPollItemGroupList");
            dc.SetParameterValue("@PollSysNo", sysNo);
            EnumColumnList enumList = new EnumColumnList();
            enumList.Add("Type", typeof(PollType));
            DataTable dt = dc.ExecuteDataTable(enumList);
            List<PollItemGroup> list = new List<PollItemGroup>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(DataMapper.GetEntity<PollItemGroup>(dr));
            }
            return list;
        }

        #endregion

        #region  投票问题组--选项（PollItem）

        /// <summary>
        /// 检查是否可以创建该投票子项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckCreatePollItem(PollItem item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_CheckCreatePollItem");
            dc.SetParameterValue<PollItem>(item);
            return dc.ExecuteScalar() != null;
        }

        /// <summary>
        /// 创建投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        public void CreatePollItem(PollItem item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_CreatePollItem");
            dc.SetParameterValue<PollItem>(item);
            dc.ExecuteNonQuery();
        }


        /// <summary>
        /// 编辑投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        public void UpdatePollItem(PollItem item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_UpdatePollItem");
            dc.SetParameterValue<PollItem>(item);
            dc.ExecuteNonQuery();
        }


        /// <summary>
        /// 删除投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        public void DeletePollItem(int itemID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_DeletePollItem");
            dc.SetParameterValue("@SysNo", itemID);
            dc.ExecuteNonQuery();
        }


        /// <summary>
        /// 加载投票问题组选项
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public List<PollItem> GetPollItemList(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_GetPollItemList");
            dc.SetParameterValue("@PollItemGroupSysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();
            List<PollItem> list = new List<PollItem>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(DataMapper.GetEntity<PollItem>(dr));
            }
            return list;
        }
        public List<PollItemAnswer> GetPollAnswer(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Poll_GetPollAnswer");
            dc.SetParameterValue("@PollItemGroupSysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();
            List<PollItemAnswer> list = new List<PollItemAnswer>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(DataMapper.GetEntity<PollItemAnswer>(dr));
            }
            return list;
        }
        #endregion
    }
}
