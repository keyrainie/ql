using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.WCF;
using System.Data;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private SEOItemAppService seoItemAppService = ObjectFactory<SEOItemAppService>.Instance;

        /// <summary>
        /// 获取SEO查询列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SEOInfo/QuerySEO", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QuerySEO(SEOQueryFilter filter)
        {
            int totalCount;
            DataTable dataTable = ObjectFactory<ISEOQueryDA>.Instance.QuerySEO(filter, out totalCount);
            List<CodeNamePair> pair = ObjectFactory<PageTypeAppService>.Instance.GetPageType(filter.CompanyCode, filter.ChannelID.ToString(), ModuleType.SEO);
            //foreach (DataRow dr in dataTable.Rows)
            //{
            //    foreach (CodeNamePair p in pair)
            //    {
            //        if (dr["PageType"].ToString() == p.Code)
            //        {

            //            PageResult pr = ObjectFactory<PageTypeAppService>.Instance.GetPage(filter.CompanyCode, filter.ChannelID.ToString(), ModuleType.SEO, p.Code);
            //            if(pr.PageList!=null && pr.PageList.Count>0)
            //                dr["PageIDName"] = pr.PageList[0].PageName;
            //            dr["PageTypeName"] = p.Name;
            //            //还需要转换页面名称
            //        }
            //    }
            //}

            foreach (DataRow dr in dataTable.Rows)
            {
                SEOItem item = new SEOItem();
                item.PageType = int.Parse(dr["PageType"].ToString());
                item.PageID = int.Parse(dr["PageID"].ToString());
                item.CompanyCode = dr["CompanyCode"].ToString();
                foreach (CodeNamePair p in pair)
                {
                    if (dr["PageType"].ToString() == p.Code)
                    {
                        dr["PageTypeName"] = p.Name;
                    }
                }
                switch (int.Parse(dr["PageType"].ToString()))
                {
                    case 1:
                        dr["PageIDName"] = seoItemAppService.GetCNameFromCategory1(item);
                        break;
                    case 2:
                        dr["PageIDName"] = seoItemAppService.GetCNameFromCategory2(item);
                        break;
                    case 3:
                        dr["PageIDName"] = seoItemAppService.GetCNameFromCategory3(item);
                        break;
                    case 4:
                        dr["PageIDName"] = seoItemAppService.GetBrandName(item);
                        break;
                    case 5:
                        dr["PageIDName"] = seoItemAppService.GetSaleAdvertisement(item);
                        break;
                    case 9:
                        dr["PageIDName"] = seoItemAppService.GetBrandNameSpe(item);
                        break;
                    case 29:
                        dr["PageIDName"] = seoItemAppService.GetVendorName(item);
                        break;
                }
            }
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 加载SEO
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SEOInfo/LoadSEOInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual SEOItem LoadSEOInfo(int sysNo)
        {
            return seoItemAppService.LoadSEOInfo(sysNo);
        }

        /// <summary>
        /// 添加SEO维护 
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/SEOInfo/AddSEOInfo", Method = "POST")]
        public virtual void AddSEOInfo(SEOItem item)
        {
            seoItemAppService.AddSEOInfo(item);
        }

        /// <summary>
        /// 更新SEO维护
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/SEOInfo/UpdateSEOInfo", Method = "PUT")]
        public virtual void UpdateSEOInfo(SEOItem item)
        {
            seoItemAppService.UpdateSEOInfo(item);
        }
    }
}
