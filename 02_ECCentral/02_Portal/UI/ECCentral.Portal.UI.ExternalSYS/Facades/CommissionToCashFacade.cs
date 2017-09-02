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
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.Facades
{
    public class CommissionToCashFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ServiceBaseUrl);

        public CommissionToCashFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public CommissionToCashFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        /// <summary>
        /// 获取申请信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetCommissionToCashByQuery(CommissionToCashQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string GetCommissionToCashByQueryUrl = "ExternalSYSService/CommissionToCash/GetCommissionToCashByQuery";
            CommissionToCashQueryFilter query = ConvertQuery(model);
            query.PageInfo = new QueryFilter.Common.PagingInfo() 
            {
                PageIndex=PageIndex,
                PageSize=PageSize,
                SortBy=SortField
            };
            restClient.QueryDynamicData(GetCommissionToCashByQueryUrl, query, callback);
        }
       /// <summary>
       /// 审核完成
       /// </summary>
       /// <param name="model"></param>
       /// <param name="callback"></param>
        public void AuditCommisonToCash(CommissionToCashVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string AuditCommisonToCashUrl = "ExternalSYSService/CommissionToCash/AuditCommisonToCash";
            restClient.Update(AuditCommisonToCashUrl, ConvertInfo(model), callback);
        }

        /// <summary>
        /// 确认支付
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void ConfirmCommisonToCash(CommissionToCashVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string ConfirmCommisonToCashUrl = "ExternalSYSService/CommissionToCash/ConfirmCommisonToCash";
            restClient.Update(ConfirmCommisonToCashUrl, ConvertInfo(model), callback);
        }
        /// <summary>
        /// 更新实际支付金额
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void UpdateCommissionToCashPayAmt(CommissionToCashVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string UpdateCommissionToCashPayAmtUrl = "ExternalSYSService/CommissionToCash/UpdateCommissionToCashPayAmt";
            restClient.Update(UpdateCommissionToCashPayAmtUrl, ConvertInfo(model), callback);
        }

        /// <summary>
        /// Info转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private CommissionToCashInfo ConvertInfo(CommissionToCashVM data)
        {
            CommissionToCashInfo info= new CommissionToCashInfo()
            {
                Memo = data.Memo,
                SysNo = data.SysNo,
                User = new BizEntity.Common.UserInfo() 
                {
                    SysNo=CPApplication.Current.LoginUser.UserSysNo,
                    UserName=CPApplication.Current.LoginUser.DisplayName
                }
            };
            if (!string.IsNullOrEmpty(data.AfterTaxAmt))
            {
                info.AfterTaxAmt = Convert.ToDecimal(data.AfterTaxAmt);
            }
            if (!string.IsNullOrEmpty(data.Bonus))
            {
                info.Bonus = Convert.ToDecimal(data.Bonus);
            }
            if (!string.IsNullOrEmpty(data.ConfirmToCashAmt))
            {
                info.ConfirmToCashAmt = Convert.ToDecimal(data.ConfirmToCashAmt);
            }
            if (!string.IsNullOrEmpty(data.NewPayAmt))
            {
                info.NewPayAmt = Convert.ToDecimal(data.NewPayAmt);
            }
            if (!string.IsNullOrEmpty(data.OldPayAmt))
            {
                info.OldPayAmt = Convert.ToDecimal(data.OldPayAmt);
            }
            return info;
        }

        /// <summary>
        /// 转换query
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private CommissionToCashQueryFilter ConvertQuery(CommissionToCashQueryVM data)
        {
            return new CommissionToCashQueryFilter() 
            {
                ApplicationDateFrom=data.ApplicationDateFrom,
                ApplicationDateTo=data.ApplicationDateTo,
                CustomerID=data.CustomerID,
                Status=data.Status
            };

        }
    }
}
