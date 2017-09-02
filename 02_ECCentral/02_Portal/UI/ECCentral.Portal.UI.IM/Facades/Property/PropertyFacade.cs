using System;
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class PropertyFacade
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

        public PropertyFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public PropertyFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 创建属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateProperty(PropertyVM data, EventHandler<RestClientEventArgs<PropertyInfo>> callback)
        {
            string relativeUrl = "/IMService/Property/CreateProperty";
            restClient.Create<PropertyInfo>(relativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 编辑属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateProperty(PropertyVM data, EventHandler<RestClientEventArgs<PropertyInfo>> callback)
        {
            string relativeUrl = "/IMService/Property/UpdateProperty";
            restClient.Update<PropertyInfo>(relativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 获取属性根据SysNo
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetPropertyBySysNo(int sysNo, EventHandler<RestClientEventArgs<PropertyInfo>> callback)
        {
            string relativeUrl = string.Format("/IMService/Property/LoadPropertyBySysNo/{0}", sysNo);
            restClient.Query<PropertyInfo>(relativeUrl, callback);
        }

        private PropertyInfo CovertVMtoEntity(PropertyVM data)
        {
            PropertyInfo msg = new PropertyInfo();
            msg.SysNo = data.SysNo;
            msg.PropertyName = new LanguageContent(data.PropertyName);
            msg.Status = data.Status == "有效" ? PropertyStatus.Active : PropertyStatus.DeActive;

            return msg;
        }

        /// <summary>
        /// 根据属性名称属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="callback"></param>
        public void GetPropertyListByPropertyName(string propertyName, EventHandler<RestClientEventArgs<List<PropertyInfo>>> callback)
        {
            const string relativeUrl = "/IMService/Property/GetPropertyListByPropertyName";
            restClient.Query<List<PropertyInfo>>(relativeUrl, propertyName, callback);
        }
    }
}
