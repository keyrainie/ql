using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Common.AppService;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Common.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Common.Restful
{
    public partial class CommonDataService
    {
        [WebInvoke(UriTemplate = "/Area/Create", Method = "POST")]
        public AreaInfo Create(AreaInfo entity)
        {
            return ObjectFactory<AreaAppService>.Instance.Create(entity);
        }

        [WebInvoke(UriTemplate = "/Area/Update", Method = "PUT")]
        public AreaInfo Update(AreaInfo entity)
        {
            return ObjectFactory<AreaAppService>.Instance.Update(entity);
        }

        [WebInvoke(UriTemplate = "/Area/Load/{sysNo}", Method = "GET")]
        public AreaInfo Load(string sysNo)
        {
            int num = int.Parse(sysNo);
            return ObjectFactory<AreaAppService>.Instance.Load(num);
        }

        [WebInvoke(UriTemplate = "/Area/QueryAreaList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult QueryAreaList(AreaQueryFilter filter)
        {
            int totalCount;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IAreaDA>.Instance.QueryAreaList(filter, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        [WebInvoke(UriTemplate = "/Area/QueryProvinceAreaList", Method = "GET")]
        public List<AreaInfo> QueryProvinceAreaList()
        {
            return ObjectFactory<IAreaDA>.Instance.QueryProvinceAreaList();
        }

        [WebInvoke(UriTemplate = "/Area/QueryCityAreaListByProvinceSysNo/{provinceSysNo}", Method = "GET")]
        public List<AreaInfo> QueryCityAreaListByProvinceSysNo(string provinceSysNo)
        {
            return ObjectFactory<IAreaDA>.Instance.QueryCityAreaListByProvinceSysNo(int.Parse(provinceSysNo));
        }

        [WebInvoke(UriTemplate = "/Area/QueryDistrictAreaListByCitySysNo/{citySysNo}", Method = "GET")]
        public List<AreaInfo> QueryDistrictAreaListByCitySysNo(string citySysNo)
        {
            return ObjectFactory<IAreaDA>.Instance.QueryDistrictAreaListByCitySysNo(int.Parse(citySysNo));
        }

        [WebInvoke(UriTemplate = "/Area/QueryCurrentAreaStructure/{sysNo}", Method = "GET")]
        public AreaQueryResponse QueryCurrentAreaStructure(string sysNo)
        {
            AreaInfo currentAreaInfo;
            List<AreaInfo> proviceList;
            List<AreaInfo> cityList;
            List<AreaInfo> districtList;

            ObjectFactory<IAreaDA>.Instance.QueryCurrentAreaStructure(int.Parse(sysNo), out currentAreaInfo, out proviceList, out cityList, out districtList);

            return new AreaQueryResponse()
            {
                CurrentAreaInfo = currentAreaInfo,
                ProviceList = proviceList,
                CityList = cityList,
                DistrictList = districtList
            };
        }

        [WebInvoke(UriTemplate = "/Area/QueryCurrentAreaStructure_Old/{sysNo}", Method = "GET")]
        public AreaQueryResponse QueryCurrentAreaStructure_Old(string sysNo)
        {
            AreaInfo currentAreaInfo;
            List<AreaInfo> proviceList;
            List<AreaInfo> cityList;
            List<AreaInfo> districtList;

            ObjectFactory<IAreaDA>.Instance.QueryCurrentAreaStructure_Old(int.Parse(sysNo), out currentAreaInfo, out proviceList, out cityList, out districtList);

            return new AreaQueryResponse()
            {
                CurrentAreaInfo = currentAreaInfo,
                ProviceList = proviceList,
                CityList = cityList,
                DistrictList = districtList
            };
        }

        [WebInvoke(UriTemplate = "/Area/QueryAreaNoDistrictList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult QueryAreaNoDistrictList(AreaQueryFilter filter)
        {
            int totalCount;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IAreaDA>.Instance.QueryAreaNoDistrictList(filter, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }
    }
}