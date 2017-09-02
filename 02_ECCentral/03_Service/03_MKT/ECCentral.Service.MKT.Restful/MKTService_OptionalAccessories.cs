using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;

using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.AppService.Promotion;
using ECCentral.Service.MKT.Restful.RequestMsg;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.WCF;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        /// <summary>
        /// 加载一个随心配信息
        /// </summary>
        [WebGet(UriTemplate = "/OptionalAccessories/{sysNo}")]
        public virtual OptionalAccessoriesInfo LoadOptionalAccessories(string sysNo)
        {
            int id = 0;
            if (!int.TryParse(sysNo, out id))
            {
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_ActivitySysNoIsNotActive"));
            }
            return ObjectFactory<OptionalAccessoriesAppService>.Instance.Load(id);
        }

        [WebInvoke(UriTemplate = "/OptionalAccessories/Query", Method = "POST")]
        public QueryResult QueryOptionalAccessories(OptionalAccessoriesQueryFilter msg)
        {
            int totalCount;
            var dataTable = ObjectFactory<IOptionalAccessoriesQueryDA>.Instance.Query(msg, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/OptionalAccessories/Create", Method = "POST")]
        public int? CreateOptionalAccessories(OptionalAccessoriesInfo info)
        {
            return ObjectFactory<OptionalAccessoriesAppService>.Instance.CreateOptionalAccessories(info);
        }

        [WebInvoke(UriTemplate = "/OptionalAccessories/Update", Method = "PUT")]
        public void UpdateOptionalAccessories(OptionalAccessoriesInfo info)
        {
            ObjectFactory<OptionalAccessoriesAppService>.Instance.UpdateOptionalAccessories(info);
        }

        [WebInvoke(UriTemplate = "/OptionalAccessories/ApproveOptionalAccessories", Method = "PUT")]
        public void ApproveOptionalAccessories(ComboUpdateStatusReq msg)
        {
            ObjectFactory<OptionalAccessoriesAppService>.Instance.ApproveOptionalAccessories(msg.SysNo.Value, msg.TargetStatus.Value);
        }

        [WebInvoke(UriTemplate = "/OptionalAccessories/CheckOptionalAccessoriesItemIsPass", Method = "POST")]
        public List<string> CheckOptionalAccessoriesItemIsPass(OptionalAccessoriesInfo info)
        {
            return ObjectFactory<OptionalAccessoriesAppService>.Instance.CheckOptionalAccessoriesItemIsPass(info);
        }

        [WebInvoke(UriTemplate = "/OptionalAccessories/CheckSaleRuleItemAndDiys", Method = "POST")]
        public List<string> CheckSaleRuleItemAndDiys(List<int> sysNos)
        {
            return ObjectFactory<OptionalAccessoriesAppService>.Instance.CheckSaleRuleItemAndDiys(sysNos);
        }
    }
}