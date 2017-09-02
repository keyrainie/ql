using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.ServiceModel.Web;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.WCF;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;
using System.Data;
using ECCentral.BizEntity.MKT.Floor;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        /// <summary>
        /// 获取所有有效的模板信息
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "/Floor/GetFloorTemplateList")]
        public virtual List<FloorTemplate> GetFloorTemplateList()
        {
            return ObjectFactory<FloorAppService>.Instance.GetFloorTemplateList();
        }

        /// <summary>
        /// 获取或有页面编号
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "/Floor/GetFloorAllPageCodeList/{companyCode}/{channelID}")]
        public virtual Dictionary<int, string> GetFloorAllPageCodeList(string companyCode, string channelID)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            var specialPageCodeList = CodeNamePairManager.GetList("MKT", "FloorSpecialPageCode");
            if (specialPageCodeList != null && specialPageCodeList.Count > 0)
            {
                foreach (var item in specialPageCodeList)
                {
                    result.Add(int.Parse(item.Code), item.Name);
                }
            }
            return result;

            //return ObjectFactory<FloorAppService>.Instance.GetFloorAllPageCodeList();
        }

        /// <summary>
        /// 获取PageCode对应的页面ID
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "/Floor/GetFloorPageID/{pageCode}/{companyCode}")]
        public virtual Dictionary<int, string> GetFloorPageID(string pageCode, string companyCode)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();

            switch ((PageCodeType)int.Parse(pageCode))
            {
                case PageCodeType.C1://一级类别
                    var allC1List = ObjectFactory<IECCategoryQueryDA>.Instance.GetAllECCategory1(companyCode, "0");
                    if (allC1List != null && allC1List.Count > 0)
                    {
                        allC1List.ForEach(m =>
                        {
                            result.Add(m.SysNo.Value, m.Name);
                        });
                    }
                    break;
                case PageCodeType.Promotion://促销模板
                    var promotionList = ObjectFactory<ISaleAdvTemplateQueryDA>.Instance.GetNowActiveCodeNames();
                    if (promotionList != null && promotionList.Count > 0)
                    {
                        promotionList.ForEach(m =>
                        {
                            result.Add(m.ID.Value, m.PageName);
                        });
                    }
                    break;
            }
            return result;

            //return ObjectFactory<FloorAppService>.Instance.GetFloorAllPageCodeList();
        }

        /// <summary>
        /// 获取所有楼层主信息，包括Active和Deactive
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "/Floor/GetAllFloorMasterList")]
        public virtual List<FloorMaster> GetAllFloorMasterList()
        {
            return ObjectFactory<FloorAppService>.Instance.GetAllFloorMasterList();
        }

        /// <summary>
        /// 查询楼层信息
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Floor/QueryFloorMaster", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryFloorMaster(FloorMasterQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<ECCentral.Service.MKT.IDataAccess.IFloorDA>.Instance.QueryFloorMaster(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 获取所有楼层主信息，包括Active和Deactive
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "/Floor/GetFloorMaster/{sysno}")]
        public virtual FloorMaster GetFloorMaster(string sysno)
        {
            return ObjectFactory<FloorAppService>.Instance.GetFloorMaster(sysno);
        }

        /// <summary>
        /// 创建楼层主信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Floor/CreateFloorMaster", Method = "POST")]
        public virtual int CreateFloorMaster(FloorMaster entity)
        {
            return ObjectFactory<FloorAppService>.Instance.CreateFloorMaster(entity);
        }

        /// <summary>
        /// 更新楼层主信息
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/Floor/UpdateFloorMaster", Method = "PUT")]
        public virtual void UpdateFloorMaster(FloorMaster entity)
        {
            ObjectFactory<FloorAppService>.Instance.UpdateFloorMaster(entity);
        }

        /// <summary>
        /// 删除楼层，会删除楼层所有相关信息
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/Floor/DeleteFloor", Method = "DELETE")]
        public virtual void DeleteFloor(string sysno)
        {
            ObjectFactory<FloorAppService>.Instance.DeleteFloor(sysno);
        }


        /// <summary>
        /// 获取当前楼层所有的Section list
        /// </summary>
        /// <param name="floorMasterSysNo"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/Floor/GetFloorSectionList/{floorMasterSysNo}")]
        public virtual List<FloorSection> GetFloorSectionList(string floorMasterSysNo)
        {
            return ObjectFactory<FloorAppService>.Instance.GetFloorSectionList(floorMasterSysNo);
        }

        /// <summary>
        /// 获取当前指定编号的Section
        /// </summary>
        /// <param name="floorMasterSysNo"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/Floor/GetFloorSection/{floorSectionSysNo}")]
        public virtual FloorSection GetFloorSection(string floorSectionSysNo)
        {
            return ObjectFactory<FloorAppService>.Instance.GetFloorSection(floorSectionSysNo);
        }

        /// <summary>
        /// 创建Section基础信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Floor/CreateFloorSection", Method = "POST")]
        public virtual int CreateFloorSection(FloorSection entity)
        {
            return ObjectFactory<FloorAppService>.Instance.CreateFloorSection(entity);
        }

        /// <summary>
        /// 更新Section基础信息
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/Floor/UpdateFloorSection", Method = "PUT")]
        public virtual void UpdateFloorSection(FloorSection entity)
        {
            ObjectFactory<FloorAppService>.Instance.UpdateFloorSection(entity);
        }

        /// <summary>
        /// 删除Section，会删除Section所有相关信息
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/Floor/DeleteFloorSection", Method = "DELETE")]
        public virtual void DeleteFloorSection(string sysno)
        {
            ObjectFactory<FloorAppService>.Instance.DeleteFloorSection(sysno);
        }

        /// <summary>
        /// 获取指定Section下Item的List
        /// </summary>
        /// <param name="floorSectionSysNo"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/Floor/GetFloorSectionItemList/{floorSectionSysNo}")]
        public virtual List<FloorSectionItem> GetFloorSectionItemList(string floorSectionSysNo)
        {
            return ObjectFactory<FloorAppService>.Instance.GetFloorSectionItemList(floorSectionSysNo);
        }

        /// <summary>
        /// 创建 Item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Floor/CreateFloorSectionItem", Method = "POST")]
        public virtual int CreateFloorSectionItem(FloorSectionItem entity)
        {
            return ObjectFactory<FloorAppService>.Instance.CreateFloorSectionItem(entity);
        }

        /// <summary>
        /// 批量创建 Item
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Floor/BatchCreateFloorSectionItem", Method = "POST")]
        public virtual List<int> BatchCreateFloorSectionItem(List<FloorSectionItem> entityList)
        {
            return ObjectFactory<FloorAppService>.Instance.BatchCreateFloorSectionItem(entityList);
        }

        /// <summary>
        /// 更新 Item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Floor/UpdateFloorSectionItem", Method = "PUT")]
        public virtual void UpdateFloorSectionItem(FloorSectionItem entity)
        {
            ObjectFactory<FloorAppService>.Instance.UpdateFloorSectionItem(entity);
        }

        /// <summary>
        /// 删除指定的Item
        /// </summary>
        /// <param name="sysno"></param>
        [WebInvoke(UriTemplate = "/Floor/DeleteFloorSectionItem", Method = "DELETE")]
        public virtual void DeleteFloorSectionItem(string sysno)
        {
            ObjectFactory<FloorAppService>.Instance.DeleteFloorSectionItem(sysno);
        }
    }
}
