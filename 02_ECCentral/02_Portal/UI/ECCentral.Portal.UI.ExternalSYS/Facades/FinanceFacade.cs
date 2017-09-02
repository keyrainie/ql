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
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.ExternalSYS.Facades
{
    public class FinanceFacade
    {
         private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ServiceBaseUrl);

        public FinanceFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public FinanceFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        /// <summary>
        /// 获取所有尺寸
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetAllFinance(FinanceQueryVM model, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string GetAllFinanceUrl = "ExternalSYSService/Finance/GetAllFinance";            

            FinanceQueryFilter query= model.ConvertVM<FinanceQueryVM, FinanceQueryFilter>();

            query.PageInfo = p;

            restClient.QueryDynamicData(GetAllFinanceUrl, query, callback);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void UpdateCommisonConfirmAmt(FinanceVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string UpdateCommisonConfirmAmtUrl = "ExternalSYSService/Finance/UpdateCommisonConfirmAmt";
            FinanceInfo info = new FinanceInfo()
            {
                SysNo = model.SysNo,
                ConfirmCommissionAmt = model.CommisonConfirmAmt,
                User = new BizEntity.Common.UserInfo() {SysNo=CPApplication.Current.LoginUser.UserSysNo,UserName=CPApplication.Current.LoginUser.DisplayName}
            };
            restClient.Update(UpdateCommisonConfirmAmtUrl, info, callback);
        }
    }
}
