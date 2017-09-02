using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT.Floor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(FloorAppService))]
    public class FloorAppService
    {
        private FloorProcessor _floorProcessor = ObjectFactory<FloorProcessor>.Instance;

        /// <summary>
        /// 获取所有有效的模板信息
        /// </summary>
        /// <returns></returns>
        public virtual List<FloorTemplate> GetFloorTemplateList()
        {
            return _floorProcessor.GetFloorTemplateList();
        }

        /// <summary>
        /// 获取或有页面编号
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<int, string> GetFloorAllPageCodeList()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            var allC1List = ExternalDomainBroker.GetCategory1List();
            result.Add(0, "首页");
            if (allC1List != null && allC1List.Count > 0)
            {
                allC1List.ForEach(m =>
                {
                    result.Add(m.SysNo.Value, m.CategoryName.Content);
                });
            }
            return result;
        }

        /// <summary>
        /// 获取所有楼层主信息，包括Active和Deactive
        /// </summary>
        /// <returns></returns>
        public virtual List<FloorMaster> GetAllFloorMasterList()
        {
            return _floorProcessor.GetAllFloorMasterList();
        }

        /// <summary>
        /// 获取所有楼层主信息，包括Active和Deactive
        /// </summary>
        /// <returns></returns>
        public virtual FloorMaster GetFloorMaster(string sysno)
        {
            return _floorProcessor.GetFloorMaster(sysno);
        }

        /// <summary>
        /// 创建楼层主信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int CreateFloorMaster(FloorMaster entity)
        {
            return _floorProcessor.CreateFloorMaster(entity);
        }

        /// <summary>
        /// 更新楼层主信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateFloorMaster(FloorMaster entity)
        {
            _floorProcessor.UpdateFloorMaster(entity);
        }

        /// <summary>
        /// 删除楼层，会删除楼层所有相关信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void DeleteFloor(string sysno)
        {
            _floorProcessor.DeleteFloor(sysno);
        }


        /// <summary>
        /// 获取当前楼层所有的Section list
        /// </summary>
        /// <param name="floorMasterSysNo"></param>
        /// <returns></returns>
        public virtual List<FloorSection> GetFloorSectionList(string floorMasterSysNo)
        {
            return _floorProcessor.GetFloorSectionList(floorMasterSysNo);
        }

        /// <summary>
        /// 获取当前指定编号的Section
        /// </summary>
        /// <param name="floorMasterSysNo"></param>
        /// <returns></returns>
        public virtual FloorSection GetFloorSection(string floorSectionSysNo)
        {
            return _floorProcessor.GetFloorSection(floorSectionSysNo);
        }

        /// <summary>
        /// 创建Section基础信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int CreateFloorSection(FloorSection entity)
        {
            return _floorProcessor.CreateFloorSection(entity);
        }

        /// <summary>
        /// 更新Section基础信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateFloorSection(FloorSection entity)
        {
            _floorProcessor.UpdateFloorSection(entity);
        }

        /// <summary>
        /// 删除Section，会删除Section所有相关信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void DeleteFloorSection(string sysno)
        {
            _floorProcessor.DeleteFloorSection(sysno);
        }

        /// <summary>
        /// 获取指定Section下Item的List
        /// </summary>
        /// <param name="floorSectionSysNo"></param>
        /// <returns></returns>
        public virtual List<FloorSectionItem> GetFloorSectionItemList(string floorSectionSysNo)
        {
            return _floorProcessor.GetFloorSectionItemList(floorSectionSysNo);
        }

        /// <summary>
        /// 创建 Item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int CreateFloorSectionItem(FloorSectionItem entity)
        {
            return _floorProcessor.CreateFloorSectionItem(entity);
        }

        /// <summary>
        /// 批量创建 Item
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        public virtual List<int> BatchCreateFloorSectionItem(List<FloorSectionItem> entityList)
        {
            using (var scope = TransactionScopeFactory.CreateTransactionScope())
            {
                List<int> result = new List<int>();
                entityList.ForEach(m =>
                {
                    result.Add(_floorProcessor.CreateFloorSectionItem(m));
                });
                scope.Complete();
                return result;
            }
        }

        /// <summary>
        /// 更新 Item
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateFloorSectionItem(FloorSectionItem entity)
        {
            _floorProcessor.UpdateFloorSectionItem(entity);
        }

        /// <summary>
        /// 删除指定的Item
        /// </summary>
        /// <param name="sysno"></param>
        public virtual void DeleteFloorSectionItem(string sysno)
        {
            _floorProcessor.DeleteFloorSectionItem(sysno);
        }
    }
}
