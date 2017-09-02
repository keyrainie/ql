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
using ECCentral.Service.MKT.Restful.RequestMsg;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private IProductPriceCompareQueryDA _ProductPriceCompareQueryDA = ObjectFactory<IProductPriceCompareQueryDA>.Instance;
        private ProductPriceCompareAppService _ProductPriceCompareAppService = ObjectFactory<ProductPriceCompareAppService>.Instance;

        [WebInvoke(UriTemplate = "/ProductPriceCompare/Query", Method = "POST")]
        public virtual QueryResult QueryProductPriceCompare(ProductPriceCompareQueryFilter filter)
        {
            int totalCount = 0;
            var dt = _ProductPriceCompareQueryDA.Query(filter, out totalCount);
            QueryResult queryResult = new QueryResult();
            queryResult.Data = dt;
            queryResult.TotalCount = totalCount;
            return queryResult;
        }

        //价格举报有效
        [WebInvoke(UriTemplate = "/ProductPriceCompare/AuditPass/{id}", Method = "PUT")]
        public void UpdateProductPriceCompareValid(string id)
        {
            int sysNo = int.Parse(id);
            _ProductPriceCompareAppService.UpdateProductPriceCompareValid(sysNo);
        }

        //价格举报无效
        [WebInvoke(UriTemplate = "/ProductPriceCompare/AuditDecline", Method = "PUT")]
        public void UpdateProductPriceCompareInvalid(ProductPriceCompareInvalidReq req)
        {
            _ProductPriceCompareAppService.UpdateProductPriceCompareInvalid(req.SysNo, req.CommaSeperatedReasonCodes);
        }

        //价格举报恢复
        [WebInvoke(UriTemplate = "/ProductPriceCompare/UpdateResetLinkShow/{id}", Method = "PUT")]
        public void UpdateProductPriceCompareResetLinkShow(string id)
        {
            int sysNo = int.Parse(id);
            _ProductPriceCompareAppService.UpdateProductPriceCompareResetLinkShow(sysNo);
        }

        [WebGet(UriTemplate = "/ProductPriceCompare/GetInvalidReasons")]
        public List<CodeNamePair> GetProductPriceCompareInvalidReasons()
        {
            return _ProductPriceCompareAppService.GetInvalidReasons();
        }

        [WebGet(UriTemplate = "/ProductPriceCompare/{id}")]
        public ProductPriceCompareEntity LoadProductPriceCompare(string id)
        {
            int sysNo = int.Parse(id);
            return _ProductPriceCompareAppService.Load(sysNo);
        }

    }
}
