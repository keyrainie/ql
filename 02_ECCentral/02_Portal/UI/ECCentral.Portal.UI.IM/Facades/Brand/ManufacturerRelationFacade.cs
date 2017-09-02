using System;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ManufacturerRelationFacade
    {
        private readonly RestClient restClient;
        const string GetManufacturerRelationInfoByLocalManufacturerSysNoUrl = "/IMService/ManufacturerRelation/GetManufacturerRelationInfoByLocalManufacturerSysNo";
        const string UPManufacturerRelationUrl = "/IMService/ManufacturerRelation/UpdateManufacturerRelation";

        /// <summary>
        /// IMService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");                
            }
        }

        public ManufacturerRelationFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ManufacturerRelationFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        private ManufacturerRelationInfo CovertVMtoEntity(ManufacturerRelationVM data)
        {
            ManufacturerRelationInfo info = new ManufacturerRelationInfo()
            {
                SysNo = data.SysNo,
                LocalManufacturerSysNo = data.LocalManufacturerSysNo,
                NeweggManufacturer = data.NeweggManufacturer,
                AmazonManufacturer = data.AmazonManufacturer,
                EBayManufacturer = data.EBayManufacturer,
                OtherManufacturerSysNo = data.OtherManufacturerSysNo,
                User = new BizEntity.Common.UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo }
            };
            return info;
        }

        /// <summary>
        /// 根据SysNo获取生产商信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetManufacturerRelationInfoByLocalManufacturerSysNo(int LocalManufacturerSysNo, EventHandler<RestClientEventArgs<ManufacturerRelationInfo>> callback)
        {
            ManufacturerRelationInfo info = new ManufacturerRelationInfo()
            {
                LocalManufacturerSysNo = LocalManufacturerSysNo,
                User = new BizEntity.Common.UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo }
            };
            restClient.Query(GetManufacturerRelationInfoByLocalManufacturerSysNoUrl, info, callback);
        }
        /// <summary>
        /// 修改厂商
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateManufacturer(ManufacturerRelationVM data, EventHandler<RestClientEventArgs<ManufacturerRelationInfo>> callback)
        {

             
            restClient.Update(UPManufacturerRelationUrl, CovertVMtoEntity(data), callback);
        }
    }
}
