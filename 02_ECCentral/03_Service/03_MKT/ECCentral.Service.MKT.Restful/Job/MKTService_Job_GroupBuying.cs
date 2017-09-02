using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.AppService;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.Restful
{
    /// <summary>
    /// MKT Job所调用的服务接口
    /// </summary>
    public partial class MKTService
    {
        private GroupBuyingAppService _groupBuyingAppService = ObjectFactory<GroupBuyingAppService>.Instance;


        //[WebInvoke(UriTemplate = "/MKTService/GetGroupBuyingList", Method = "POST")]
        //public virtual List<GroupBuyingInfo> GetGroupBuyingList(int groupBuyingSysNo, int companyCode)
        //{
        //    return _groupBuyingAppService.GetGroupBuyingList(groupBuyingSysNo, companyCode);
        //}
    }
}
