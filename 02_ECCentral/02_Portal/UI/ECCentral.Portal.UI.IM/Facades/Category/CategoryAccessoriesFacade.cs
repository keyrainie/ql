using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryAccessoriesFacade
    {
        #region 构造函数和字段
        private readonly RestClient _restClient;

        const string CreateRelativeUrl = "/IMService/CategoryAccessory/CreateCategoryAccessory";

        const string UPdateRelativeUrl = "/IMService/CategoryAccessory/UpdateCategoryAccessory";

        const string GetRelativeUrl = "/IMService/CategoryAccessory/GetCategoryAccessoryBySysNo";

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

        public CategoryAccessoriesFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryAccessoriesFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        #region 函数
        /// <summary>
        /// 转换分类视图和分类实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private CategoryAccessory CovertVMtoEntity(CategoryAccessoriesVM data)
        {
            CategoryAccessory msg = data.ConvertVM<CategoryAccessoriesVM, CategoryAccessory>();
            msg.Accessory = data.Accessory.ConvertVM<AccessoryVM, AccessoryInfo>((v, t) =>
            {
                t.AccessoryName = new LanguageContent(v.AccessoryName);
            });
            msg.CategoryInfo = data.CategoryInfo.ConvertVM<CategoryVM, CategoryInfo>((v, t) =>
            {
                t.CategoryName = new LanguageContent(v.CategoryName);
            });
            msg.SysNo = data.SysNo;
            return msg;
        }

        /// <summary>
        /// 创建配件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateCategoryAccessory(CategoryAccessoriesVM data, EventHandler<RestClientEventArgs<CategoryAccessory>> callback)
        {
            _restClient.Create(CreateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 修改分类配件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateCategoryAccessory(CategoryAccessoriesVM data, EventHandler<RestClientEventArgs<CategoryAccessory>> callback)
        {

            _restClient.Update(UPdateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 获取分类属性
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetCategoryAccessoryBySysNo(int sysNo, EventHandler<RestClientEventArgs<CategoryAccessory>> callback)
        {
            _restClient.Query(GetRelativeUrl, sysNo, callback);
        }
        #endregion
    }
}
