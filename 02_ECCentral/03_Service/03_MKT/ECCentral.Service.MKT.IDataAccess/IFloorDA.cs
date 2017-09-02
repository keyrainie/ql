using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT.Floor;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IFloorDA
    {
        /// <summary>
        /// 获取所有有效的模板信息
        /// </summary>
        /// <returns></returns>
        List<FloorTemplate> GetFloorTemplateList();

        /// <summary>
        /// 获取所有楼层主信息，包括Active和Deactive
        /// </summary>
        /// <returns></returns>
        List<FloorMaster> GetAllFloorMasterList();

        /// <summary>
        /// 查询楼层信息
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryFloorMaster(FloorMasterQueryFilter filter, out int totalCount);

        /// <summary>
        /// 获取知道编号楼层主信息
        /// </summary>
        /// <returns></returns>
        FloorMaster GetFloorMaster(string sysno);

        /// <summary>
        /// 创建楼层主信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int CreateFloorMaster(FloorMaster entity);

        /// <summary>
        /// 更新楼层主信息
        /// </summary>
        /// <param name="entity"></param>
        void UpdateFloorMaster(FloorMaster entity);

        /// <summary>
        /// 删除楼层，会删除楼层所有相关信息
        /// </summary>
        /// <param name="entity"></param>
        void DeleteFloor(string sysno);

        /// <summary>
        /// 获取当前楼层所有的Section list
        /// </summary>
        /// <param name="floorMasterSysNo"></param>
        /// <returns></returns>
        List<FloorSection> GetFloorSectionList(string floorMasterSysNo);

        /// <summary>
        /// 获取当前指定编号的Section
        /// </summary>
        /// <param name="floorMasterSysNo"></param>
        /// <returns></returns>
        FloorSection GetFloorSection(string floorSectionSysNo);

        /// <summary>
        /// 创建Section基础信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int CreateFloorSection(FloorSection entity);

        /// <summary>
        /// 更新Section基础信息
        /// </summary>
        /// <param name="entity"></param>
        void UpdateFloorSection(FloorSection entity);

        /// <summary>
        /// 删除Section，会删除Section所有相关信息
        /// </summary>
        /// <param name="entity"></param>
        void DeleteFloorSection(string sysno);

        /// <summary>
        /// 获取指定Section下Item的List
        /// </summary>
        /// <param name="floorSectionSysNo"></param>
        /// <returns></returns>
        List<FloorSectionItem> GetFloorSectionItemList(string floorSectionSysNo);

        /// <summary>
        /// 创建 Item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int CreateFloorSectionItem(FloorSectionItem entity);

        /// <summary>
        /// 更新 Item
        /// </summary>
        /// <param name="entity"></param>
        void UpdateFloorSectionItem(FloorSectionItem entity);

        /// <summary>
        /// 删除指定的Item
        /// </summary>
        /// <param name="sysno"></param>
        void DeleteFloorSectionItem(string sysno);
    }
}
