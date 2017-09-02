using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductChannelInfoFacade
    {
        #region 构造函数和字段
        private readonly RestClient _restClient;

        const string CreateRelativeUrl = "/IMService/ProductChannelInfo/CreateProductChannelInfo";

        const string UpdateRelativeUrl = "/IMService/ProductChannelInfo/UpdateProductChannelInfo";

        const string BatchUpdateRelativeUrl = "/IMService/ProductChannelInfo/BatchUpdateProductChannelInfo";

        const string GetRelativeUrl = "/IMService/ProductChannelInfo/GetProductChannelInfoBySysNo";

        const string CreatePeriodPriceRelativeUrl = "/IMService/ProductChannelInfo/CreateProductChannelPeriodPrice";

        const string UpdatePeriodPriceRelativeUrl = "/IMService/ProductChannelInfo/UpdateProductChannelPeriodPrice";

        const string GetPeriodPriceRelativeUrl = "/IMService/ProductChannelInfo/GetProductChannelPeriodPriceBySysNo";

        const string GetChannelListRelativeUrl = "/IMService/ProductChannelInfo/GetChannelInfoList";

        const string BatchUpdateChannelProductInfoStatusUrl = "/IMService/ProductChannelInfo/BatchUpdateChannelProductInfoStatus";

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

        public ProductChannelInfoFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductChannelInfoFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        #region 渠道商品信息函数
        /// <summary>
        /// 转换分类视图和分类实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ProductChannelInfo CovertVMtoEntity(ProductChannelVM data)
        {
            ProductChannelInfo msg = data.ConvertVM<ProductChannelVM, ProductChannelInfo>();

            msg.CreateUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            msg.EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };

            msg.SysNo = data.SysNo;
            return msg;
        }

        /// <summary>
        /// 创建配件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateProductChannelInfo(List<ProductChannelInfo> data, EventHandler<RestClientEventArgs<ProductChannelInfo>> callback)
        {          

            _restClient.Create(CreateRelativeUrl, data, callback);
        }

        /// <summary>
        /// 批量更新渠道商品信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void BatchUpdateProductChannelInfo(List<ProductChannelVM> data, EventHandler<RestClientEventArgs<ProductChannelInfo>> callback)
        {
            List<ProductChannelInfo> entityList = new List<ProductChannelInfo>();
            data.ForEach(p => entityList.Add(CovertVMtoEntity(p)));

            _restClient.Update(BatchUpdateRelativeUrl, entityList, callback);
        }

        /// <summary>
        /// 修改渠道商品信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateProductChannelInfo(ProductChannelVM data, EventHandler<RestClientEventArgs<ProductChannelInfo>> callback)
        {

            _restClient.Update(UpdateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 批量更新多渠道商品记录状态
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void BatchUpdateChannelProductInfoStatus(ProductChannelVM data, EventHandler<RestClientEventArgs<ProductChannelInfo>> callback)
        {
            _restClient.Update(BatchUpdateChannelProductInfoStatusUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 获取渠道商品信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetProductChannelInfoBySysNo(int sysNo, EventHandler<RestClientEventArgs<ProductChannelInfo>> callback)
        {
            _restClient.Query(GetRelativeUrl, sysNo, callback);
        }
        #endregion

        #region 渠道商品价格信息函数
        /// <summary>
        /// 转换分类视图和分类实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ProductChannelPeriodPrice CovertVMtoEntity(ProductChannelPeriodPriceVM data)
        {
            ProductChannelPeriodPrice msg = data.ConvertVM<ProductChannelPeriodPriceVM, ProductChannelPeriodPrice>();


            msg.CreateUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            msg.EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };
            msg.AuditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName };

            msg.SysNo = data.SysNo;
            return msg;
        }

        /// <summary>
        /// 创建渠道商品价格信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateProductChannelPeriodPrice(ProductChannelPeriodPriceVM data, EventHandler<RestClientEventArgs<ProductChannelPeriodPrice>> callback)
        {
            _restClient.Create(CreatePeriodPriceRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 修改渠道商品价格信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateProductChannelPeriodPrice(ProductChannelPeriodPriceVM data, EventHandler<RestClientEventArgs<ProductChannelPeriodPrice>> callback)
        {

            _restClient.Update(UpdatePeriodPriceRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 获取渠道商品价格信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetProductChannelPeriodPriceBySysNo(int sysNo, EventHandler<RestClientEventArgs<ProductChannelPeriodPrice>> callback)
        {
            _restClient.Query(GetPeriodPriceRelativeUrl, sysNo, callback);
        }
        #endregion

        /// <summary>
        /// 获取渠道商品信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetChannelInfoList(EventHandler<RestClientEventArgs<List<ChannelInfo>>> callback)
        {
            _restClient.Query(GetChannelListRelativeUrl, callback);
        }
    }
}
