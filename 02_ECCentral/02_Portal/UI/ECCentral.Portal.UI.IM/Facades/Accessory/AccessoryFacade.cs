using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class AccessoryFacade
    {
        #region 构造函数和字段
        private readonly RestClient _restClient;
        const string CreateRelativeUrl = "/IMService/Accessory/CreateAccessory";
        const string UPdateRelativeUrl = "/IMService/Accessory/UpdateAccessory";
        const string GetRelativeUrl = "/IMService/Accessory/Load/{0}";
        const string GetAllRelativeUrl = "/IMService/Accessory/GetAllAccessory";
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

        public AccessoryFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public AccessoryFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        #region 函数
        /// <summary>
        /// 转换配件视图和品牌实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private AccessoryInfo CovertVMtoEntity(AccessoryVM data)
        {
            AccessoryInfo msg = data.ConvertVM<AccessoryVM, AccessoryInfo>((v, t) =>
                                                                               {
                                                                                   t.AccessoryName = new LanguageContent(v.AccessoryName);
                                                                               });
            msg.SysNo = data.SysNo;
            return msg;
        }

        /// <summary>
        /// 创建配件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateAccessory(AccessoryVM data, EventHandler<RestClientEventArgs<AccessoryInfo>> callback)
        {
            _restClient.Create(CreateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 修改配件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateAccessory(AccessoryVM data, EventHandler<RestClientEventArgs<AccessoryInfo>> callback)
        {

            _restClient.Update(UPdateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 获取配件
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetAccessoryBySysNo(int sysNo, EventHandler<RestClientEventArgs<AccessoryInfo>> callback)
        {
            string relativeUrl = string.Format(GetRelativeUrl, sysNo);
            _restClient.Query<AccessoryInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 获取配件
        /// </summary>
        /// <param name="callback"></param>
        public void GeAccessoryBySysNo(EventHandler<RestClientEventArgs<AccessoryInfo>> callback)
        {
            string relativeUrl = string.Format(GetRelativeUrl);
            _restClient.Query<AccessoryInfo>(relativeUrl, callback);
        }
        #endregion
    }
}
