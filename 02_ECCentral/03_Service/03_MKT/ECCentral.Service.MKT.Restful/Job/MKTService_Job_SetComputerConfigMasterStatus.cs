using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;

namespace ECCentral.Service.MKT.Restful.Job
{
    public partial class MKTService
    {
        /// <summary>
        /// DIY装机调度
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/MktJob/CheckComputerConfigInfo", Method = "UPDATE")]
        public void CheckComputerConfigInfo()
        {
            ObjectFactory<ComputerConfigAppService>.Instance.CheckComputerConfigInfo();
        }
    }
}
