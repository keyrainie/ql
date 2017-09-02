using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class PMFacade
    {

        private readonly IPage viewPage;
        private readonly RestClient restClient;
        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");                
            }
        }

        public PMFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public PMFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void CreatePM(PMVM data, EventHandler<RestClientEventArgs<ProductManagerInfo>> callback)
        {
            string relativeUrl = "/IMService/ProductManager/CreateProductManagerInfo";
            restClient.Create<ProductManagerInfo>(relativeUrl, CovertVMtoEntity(data), callback);
        }

        public void UpdatePM(PMVM data, EventHandler<RestClientEventArgs<ProductManagerInfo>> callback)
        {
            string relativeUrl = "/IMService/ProductManager/UpdateProductManagerInfo";
            restClient.Update<ProductManagerInfo>(relativeUrl, CovertVMtoEntity(data), callback);
        }

        public void GetPMBySysNo(int sysNo, EventHandler<RestClientEventArgs<ProductManagerInfo>> callback)
        {
            string relativeUrl = string.Format("/IMService/ProductManager/Load/{0}", sysNo);            
            restClient.Query<ProductManagerInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 得到所有PM
        /// </summary>
        /// <param name="callback"></param>
        public void QueryAllProductManagerInfo(EventHandler<RestClientEventArgs<List<ProductManagerInfo>>> callback)
        {
            string relativeUrl = "/IMService/ProductManagerGroup/QueryAllProductManagerInfo";
            restClient.Query<List<ProductManagerInfo>>(relativeUrl,null,callback);

        }

        private ProductManagerInfo CovertVMtoEntity(PMVM data)
        {
            ProductManagerInfo msg = new ProductManagerInfo();
            msg.SysNo = data.SysNo;
            msg.UserInfo = new UserInfo() { UserID = data.PMID};
            msg.Status = data.Status;
            msg.BackupUserList=data.BackupUserList;
            msg.ITMaxWeightforPerDay=Convert.ToDouble(data.ITMaxWeightforPerDay);
            msg.ITMaxWeightforPerOrder = Convert.ToDouble(data.ITMaxWeightforPerOrder);
            msg.MaxAmtPerDay = Convert.ToDouble(data.MaxAmtPerDay);
            msg.MaxAmtPerOrder = Convert.ToDouble(data.MaxAmtPerOrder);
            msg.SaleRatePerMonth = Convert.ToDouble(string.IsNullOrEmpty(data.SaleRatePerMonth) ? "0" : data.SaleRatePerMonth);
            msg.SaleTargetPerMonth = Convert.ToDouble(string.IsNullOrEmpty(data.SaleTargetPerMonth) ? "0" : data.SaleTargetPerMonth);
            msg.PMDMaxAmtPerDay = Convert.ToDouble(data.PMDMaxAmtPerDay);
            msg.PMDMaxAmtPerOrder = Convert.ToDouble(data.PMDMaxAmtPerOrder);
            msg.WarehouseNumber = data.WarehouseNumber;
            return msg;
        }

    }
}
