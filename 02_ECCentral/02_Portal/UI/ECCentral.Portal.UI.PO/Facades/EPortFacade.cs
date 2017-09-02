using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Models.EPort;
using Newegg.Oversea.Silverlight.Controls;
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

namespace ECCentral.Portal.UI.PO.Facades
{
    public class EPortFacade
    {
        private readonly RestClient restClient;
        public IPage Page { get; set; }
        /// <summary>
        /// 服务的地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public EPortFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl,page);
        }
        /// <summary>
        /// 获取电子口岸信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetEPortEntity(int? sysNo,EventHandler<RestClientEventArgs<EPortVM>> callback)
        {
            string relativeUrl = string.Format("/POService/EPort/GetEport/{0}",sysNo.ToString());
            if(sysNo.HasValue)
            {
                restClient.Query<EPortEntity>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    EPortVM viewModel = null;
                    EPortEntity entity = args.Result;
                    if (entity == null)
                    {
                        viewModel = new EPortVM();
                    }
                    else
                    {
                        viewModel = entity.Convert<EPortEntity, EPortVM>();
                    }
                    callback(obj, new RestClientEventArgs<EPortVM>(viewModel,restClient.Page));
                });
            }
        }
        /// <summary>
        /// 创建电子口岸
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void CreatEPort(EPortVM vm, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/POService/EPort/CreatEPort";
            var msg = vm.ConvertVM<EPortVM, EPortEntity>();
            restClient.Create(relativeUrl, msg, callback);
        }
        /// <summary>
        /// 更新电子口岸信息
        /// </summary>
        /// <param name="_viewInfo"></param>
        /// <param name="callback"></param>
        public void UpdateEPortr(EPortVM vm, EventHandler<RestClientEventArgs<EPortEntity>> callback)
        {
            string relativeUrl = "/POService/EPort/SaveEport";
            var msg = vm.ConvertVM<EPortVM, EPortEntity>();
            restClient.Update<EPortEntity>(relativeUrl, msg, callback);
        }
        /// <summary>
        /// 查询电子口岸列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void QueryEPortList(EPortFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/POService/EPort/QueryEport";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }
        /// <summary>
        /// 删除电子口岸
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void DeleteEPort(int sysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/POService/EPort/DeleteEport";
            restClient.Update(relativeUrl, sysNo, (obj, args) =>
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
