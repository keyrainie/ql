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

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private BannerAppService _bannerAppService = ObjectFactory<BannerAppService>.Instance;

        [WebInvoke(UriTemplate = "/Banner/Query", Method = "POST")]
        public virtual QueryResult QueryBanner(BannerQueryFilter filter)
        {
            int totalCount = 0;
            var ds = ObjectFactory<IBannerQueryDA>.Instance.Query(filter, out totalCount);
            var dtBanner = ds.Tables[0];
            var dtBannerArea = ds.Tables[1];
            //在广告信息表中增加一列，容纳主要投放区域
            const string areaInfoColName = "AreaInfo";
            dtBanner.Columns.Add(areaInfoColName, typeof(string));
            foreach (DataRow drBanner in dtBanner.Rows)
            {
                string areaInfo = "";
                foreach (DataRow drArea in dtBannerArea.Rows)
                {
                    if (drArea["RefSysNo"].ToString() == drBanner["SysNo"].ToString())
                    {
                        //用逗号分隔主要投放区域
                        areaInfo += drArea["AreaSysNo"] + ",";
                    }
                }
                drBanner[areaInfoColName] = areaInfo.TrimEnd(',');
            }
            QueryResult queryResult = new QueryResult();
            queryResult.Data = dtBanner;
            queryResult.TotalCount = totalCount;
            return queryResult;
        }

        [WebInvoke(UriTemplate = "/Banner/QueryDimensions", Method = "POST")]
        public virtual List<BannerDimension> QueryBannerDimensions(BannerDimensionQueryFilter filter)
        {
            return ObjectFactory<IBannerQueryDA>.Instance.GetBannerDimensions(filter);
        }

        /// <summary>
        /// 创建广告信息
        /// </summary>
        /// <param name="bannerLocation">广告信息</param>
        [WebInvoke(UriTemplate = "/Banner/Create", Method = "POST")]
        public virtual void CreateBanner(BannerLocation bannerLocation)
        {
            _bannerAppService.Create(bannerLocation);
        }

        /// <summary>
        /// 更新广告信息
        /// </summary>
        /// <param name="bannerLocation">广告信息</param>
        [WebInvoke(UriTemplate = "/Banner/Update", Method = "PUT")]
        public virtual void UpdateBanner(BannerLocation bannerLocation)
        {
            _bannerAppService.Update(bannerLocation);
        }

         /// <summary>
        /// 作废banner
        /// </summary>
        /// <param name="bannerLocationSysNo">系统编号</param>
        [WebInvoke(UriTemplate = "/Banner/Deactive/{bannerLocationSysNo}", Method = "PUT")]
        public virtual void DeleteBanner(string bannerLocationSysNo)
        {
            int id = int.Parse(bannerLocationSysNo);
            _bannerAppService.Delete(id);
        }

        /// <summary>
        /// 加载
        /// </summary>
        [WebGet(UriTemplate = "/Banner/Load/{sysNo}")]
        public virtual BannerLocation LoadBanner(string sysNo)
        {
            int id = int.Parse(sysNo);
            return _bannerAppService.Load(id);
        }

          /// <summary>
        /// 检查页面上的Banner位上已有的有效Banner数量
        /// </summary>
        [WebGet(UriTemplate = "/Banner/CountBannerPosition/{companyCode}/{channelID}/{pageID}/{bannerDimensionSysNo}")]
        public virtual int CountBannerPosition(string companyCode,string channelID,string pageID,string bannerDimensionSysNo)
        {
            return _bannerAppService.CountBannerPosition(int.Parse(bannerDimensionSysNo)
                ,int.Parse(pageID),companyCode,channelID);
        }

        /// <summary>
        /// 获取广告模板
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Banner/GetBannerFrame", Method = "POST")]
        public virtual List<BannerFrame> GetBannerFrame(BannerFrameQueryFilter filter)
        {
            return ObjectFactory<IBannerQueryDA>.Instance.GetBannerFrame(filter);
        }
    }
}
