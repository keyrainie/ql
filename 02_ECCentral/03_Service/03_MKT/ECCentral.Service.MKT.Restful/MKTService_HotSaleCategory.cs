using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.AppService;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.Data;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        //分页查询首页排行
        [WebInvoke(UriTemplate = "/HotSaleCategory/Query", Method = "POST")]
        public virtual QueryResult QueryHotSaleCategory(HotSaleCategoryQueryFilter filter)
        {
            var queryDA = ObjectFactory<IHotSaleCategoryQueryDA>.Instance;
            int totalCount;
            var data = queryDA.Query(filter, out totalCount);

            //获取当前渠道下所有的页面类型
            var pageTypes = ObjectFactory<PageTypeAppService>.Instance.GetPageType(filter.CompanyCode, filter.ChannelID, ModuleType.HotSale);
            //获取页面类型对应的位置
            List<CodeNamePair> positions = new List<CodeNamePair>();
            foreach (var p in pageTypes)
            {
                var list = _hotAppService.GetPosition(filter.CompanyCode, filter.ChannelID, int.Parse(p.Code));
                positions.AddRange(list);
            }
            //将DataTable中的PageType,Position转换成描述
            data.Columns.Add("PageTypeName", typeof(string));
            data.Columns.Add("PositionName", typeof(string));
            foreach (DataRow row in data.Rows)
            {
                var foundPageType = pageTypes.FirstOrDefault(pageType => pageType.Code == row["PageType"].ToString());
                if (foundPageType != null)
                {
                    row["PageTypeName"] = foundPageType.Name;
                }
                var foundPosition = positions.FirstOrDefault(item => item.Code == row["Position"].ToString());
                if (foundPosition != null)
                {
                    row["PositionName"] = foundPosition.Name;
                }
            }

            return new QueryResult
            {
                Data = data,
                TotalCount = totalCount
            };
        }

        private HotSaleCategoryAppService _hotAppService = ObjectFactory<HotSaleCategoryAppService>.Instance;

        [WebInvoke(UriTemplate = "/HotSaleCategory/Insert", Method = "POST")]
        public void InsertHotSaleCategory(HotSaleCategory msg)
        {
            _hotAppService.Insert(msg);
        }

        [WebInvoke(UriTemplate = "/HotSaleCategory/Update", Method = "PUT")]
        public void UpdateHotSaleCategory(HotSaleCategory msg)
        {
            _hotAppService.Update(msg);
        }

        /// <summary>
        /// 更新同位置同组下所有记录
        /// </summary>
        /// <param name="msg"></param>
        [WebInvoke(UriTemplate = "/HotSaleCategory/UpdateSameGroupAll", Method = "PUT")]
        public void UpdateSameGroupAllHotSaleCategory(HotSaleCategory msg)
        {
            _hotAppService.UpdateSameGroupAll(msg);
        }

        [WebInvoke(UriTemplate = "/HotSaleCategory/Delete/{id}", Method = "PUT")]
        public void DeleteHotSaleCategory(string id)
        {
            int sysNo = int.Parse(id);
            _hotAppService.Delete(sysNo);
        }

        [WebGet(UriTemplate = "/HotSaleCategory/{id}")]
        public HotSaleCategory LoadHotSaleCategory(string id)
        {
            int sysNo = int.Parse(id);
            return _hotAppService.Load(sysNo);
        }

        /// <summary>
        /// 获取首页排行的位置
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <param name="pageType">页面类型</param>
        /// <returns>位置列表</returns>
        [WebGet(UriTemplate = "/HotSaleCategory/GetPosition/{companyCode}/{channelID}/{pageType}")]
        public List<CodeNamePair> GetPosition(string companyCode, string channelID, string pageType)
        {
            int pageTypeID = int.Parse(pageType);

            return _hotAppService.GetPosition(companyCode, channelID, pageTypeID);
        }
    }
}
