using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.MKT.AppService;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.Restful
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public partial class MKTService
    {
        private IHelpCenterQueryDA _helpCenterQueryDA = ObjectFactory<IHelpCenterQueryDA>.Instance;
        private HelpCenterAppService _helpCenterAppService = ObjectFactory<HelpCenterAppService>.Instance;

        [WebInvoke(UriTemplate = "/HelpCenter/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryHelpCenter(HelpCenterQueryFilter msg)
        {
            int totalCount;
            var dataTable = _helpCenterQueryDA.Query(msg, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/HelpCenter/QueryCategory", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryHelpCenterCategory(HelpCenterCategoryQueryFilter msg)
        {
            var dataTable = _helpCenterQueryDA.QueryCategory(msg);
            return new QueryResult()
            {
                Data = dataTable
            };
        }

        /// <summary>
        /// 创建帮助主题
        /// </summary>
        [WebInvoke(UriTemplate = "/HelpCenter/Create", Method = "POST")]
        public virtual void CreateHelpCenter(HelpTopic item)
        {
            _helpCenterAppService.Create(item);
        }

        /// <summary>
        /// 更新帮助主题
        /// </summary>
        [WebInvoke(UriTemplate = "/HelpCenter/Update", Method = "PUT")]
        public virtual void UpdateHelpCenter(HelpTopic item)
        {
            _helpCenterAppService.Update(item);
        }

        [WebGet(UriTemplate = "/HelpCenter/{sysNo}")]
        public virtual HelpTopic LoadHelpCenter(string sysNo)
        {
            int id = Convert.ToInt32(sysNo);
            return _helpCenterAppService.Load(id);
        }
    }
}
