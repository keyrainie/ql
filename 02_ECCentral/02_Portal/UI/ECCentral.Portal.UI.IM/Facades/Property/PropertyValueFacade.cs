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
using ECCentral.Portal.UI.IM.Models;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class PropertyValueFacade
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

        public PropertyValueFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public PropertyValueFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 创建属性值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreatePropertyValue(PropertyValueVM data, EventHandler<RestClientEventArgs<PropertyValueInfo>> callback)
        {
            string relativeUrl = "/IMService/Property/CreatePropertyValue";
            restClient.Create<PropertyValueInfo>(relativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 编辑属性值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdatePropertyValue(PropertyValueVM data, EventHandler<RestClientEventArgs<PropertyValueInfo>> callback)
        {
            string relativeUrl = "/IMService/Property/UpdatePropertyValue";
            restClient.Update<PropertyValueInfo>(relativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 获取属性根据SysNo
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        //public void GetPropertyBySysNo(int sysNo, EventHandler<RestClientEventArgs<PropertyInfo>> callback)
        //{
        //    string relativeUrl = string.Format("/IMService/Property/LoadPropertyBySysNo/{0}", sysNo);
        //    restClient.Query<PropertyInfo>(relativeUrl, callback);
        //}

        private PropertyValueInfo CovertVMtoEntity(PropertyValueVM data)
        {
            PropertyValueInfo msg = new PropertyValueInfo();
            msg.SysNo = data.SysNo;
            msg.PropertyInfo = new PropertyInfo() { SysNo = data.PropertySysNo };
            msg.Priority = Convert.ToInt32(data.Priority);
            msg.ValueDescription = new LanguageContent(data.ValueDescription);
            msg.ValueStatus = data.Status == "有效" ? PropertyStatus.Active : PropertyStatus.DeActive;

            return msg;
        }
    }
}
