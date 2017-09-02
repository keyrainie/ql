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
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.Invoice.Models;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class SysAccountAjustPointFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        /// <summary>
        /// InvoiceService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Invoice", "ServiceBaseUrl");
            }
        }

        public SysAccountAjustPointFacade(): this(null)
        {
        }

        public SysAccountAjustPointFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 获取系统账户
        /// </summary>
        /// <param name="needAll"></param>
        /// <param name="callback"></param>
        public void LoadSysAccountList(string channelID, bool needAll, EventHandler<RestClientEventArgs<List<CustomerBasicInfo>>> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/SysAccount/LoadAll/{0}", channelID);
            restClient.Query<List<CustomerBasicInfo>>(relativeUrl,(obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<CustomerBasicInfo> userList = args.Result;
                if (needAll)
                {
                    userList.Insert(0, new CustomerBasicInfo()
                    {
                        CustomerID = ResCommonEnum.Enum_All
                    });
                }
                RestClientEventArgs<List<CustomerBasicInfo>> eventArgs = new RestClientEventArgs<List<CustomerBasicInfo>>(userList, viewPage);
                callback(obj, eventArgs);
            });
        }

        /// <summary>
        /// 获取系统账户有效积分
        /// </summary>
        /// <param name="needAll"></param>
        /// <param name="callback"></param>
        public void LoadSysAccountValidScore(int customerSysNo, EventHandler<RestClientEventArgs<int>> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/SysAccount/GetVaildScore/{0}", customerSysNo);
            restClient.Query<int>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        /// 调整积分
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void AjustPoint(AdjustSysAccountPointVM vm, EventHandler<RestClientEventArgs<AdjustPointRequest>> callback)
        {
            AdjustPointRequest data = vm.ConvertVM<AdjustSysAccountPointVM, AdjustPointRequest>();
            data.OperationType = AdjustPointOperationType.AddOrReduce;
            data.PointType = (int)AdjustPointType.AddPointToSysAccounts;
            data.Source = "Invoice Domain";
            data.PointExpiringDate = DateTime.Now.AddYears(2);
            data.Memo = string.Format("系统账户加积分{0}", string.IsNullOrEmpty(vm.Memo) ? string.Empty : string.Format(":{0}", vm.Memo));
            string relativeUrl = "/InvoiceService/SysAccount/AjustPoint";

            restClient.Update<AdjustPointRequest>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
    }
}
