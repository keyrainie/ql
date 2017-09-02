using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Common.AppService;
using ECCentral.Service.Common.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Common.Restful
{
    public partial class CommonDataService
    {
        /// <summary>
        /// ��ȡ˰�ʷ�����Ϣ
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Tariff/QueryTariffCategory/{tariffcode}", Method = "GET")]
        public List<TariffInfo> QueryTariffCategory(string tariffcode)
        {
            var s = ObjectFactory<TariffAppService>.Instance.QueryTariffCategory(tariffcode);
            return s;
        }

        /// <summary>
        /// ����˰�ʹ���
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Tariff/Create", Method = "POST")]
        public TariffInfo CreateTariffInfo(TariffInfo request)
        {
            return ObjectFactory<TariffAppService>.Instance.CreateTariffInfo(request);
        }


        /// <summary>
        /// ��ȡ˰�ʹ�����Ϣ
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Tariff/Load/{sysno}", Method = "GET")]
        public TariffInfo GetTariffInfo(string sysno)
        {
            int temp = 0;
            if (int.TryParse(sysno, out temp))
            {
                return ObjectFactory<TariffAppService>.Instance.GetTariffInfo(temp);
            }
            return null;

        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Tariff/Update", Method = "PUT")]
        public bool UpdateTariffInfo(TariffInfo entity)
        {
            return ObjectFactory<TariffAppService>.Instance.UpdateTariffInfo(entity);
        }


        /// <summary>
        /// ��ѯ˰�ʱ���Ϣ
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Tariff/Query", Method = "POST")]
        public QueryResult QueryTariffInfo(TariffInfoQueryFilter request)
        {
          
            int totalCount;
            var dataTable = ObjectFactory<ITariffInfoQueryDA>.Instance.QueryTariffInfo(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
    }
}
