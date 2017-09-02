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
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity;

namespace ECCentral.Portal.UI.IM.Facades
{


    public class PMGroupFacade
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

        public PMGroupFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public PMGroupFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 创建PM组信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreatePMGroup(PMGroupVM data, EventHandler<RestClientEventArgs<ProductManagerGroupInfo>> callback)
        {
            string relativeUrl = "/IMService/ProductManagerGroup/CreateProductManagerGroupInfo";
            restClient.Create<ProductManagerGroupInfo>(relativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 修改PM组信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdatePMGroup(PMGroupVM data, EventHandler<RestClientEventArgs<ProductManagerGroupInfo>> callback)
        {
            string relativeUrl = "/IMService/ProductManagerGroup/UpdateProductManagerGroupInfo";
            restClient.Update<ProductManagerGroupInfo>(relativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 获取PM列表
        /// 未在PM组的PM和已在某个PM组的PM列表
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetPMListByPMGroupSysNo(int sysNo, EventHandler<RestClientEventArgs<List<ProductManagerInfo>>> callback)
        {
            string relativeUrl = string.Format("/IMService/ProductManagerGroup/LoadPMList/{0}", sysNo);
            restClient.Query<List<ProductManagerInfo>>(relativeUrl, callback);
        }

        /// <summary>
        /// 查询PM Group信息根据SysNo
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetPMGroupBySysNo(int sysNo, EventHandler<RestClientEventArgs<ProductManagerGroupInfo>> callback)
        {
            string relativeUrl = string.Format("/IMService/ProductManagerGroup/LoadPMGroup/{0}", sysNo);
            restClient.Query<ProductManagerGroupInfo>(relativeUrl, callback);
        }

        private ProductManagerGroupInfo CovertVMtoEntity(PMGroupVM data)
        {
            ProductManagerGroupInfo msg = new ProductManagerGroupInfo();
            msg.SysNo = data.SysNo;
            msg.PMGroupName = new LanguageContent(data.PMGroupName);
            msg.UserInfo = new UserInfo() { SysNo =Convert.ToInt32(data.PMUserSysNo) };
            msg.ProductManagerInfoList = data.PMList;
            msg.Status = data.Status == "有效" ? PMGroupStatus.Active : PMGroupStatus.DeActive;

            return msg;
        }

    }
}
