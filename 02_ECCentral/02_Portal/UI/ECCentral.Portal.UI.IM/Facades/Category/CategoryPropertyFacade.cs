using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryPropertyFacade
    {
        #region 构造函数和字段
        private readonly RestClient _restClient;

        const string UPdateRelativeUrl = "/IMService/CategoryProperty/UpdateCategoryProperty";

        const string GetRelativeUrl = "/IMService/CategoryProperty/GetCategoryPropertyBySysNo";
        const string CopyCategoryOutputTemplatePropertyUrl = "/IMService/CategoryProperty/CopyCategoryOutputTemplateProperty";
        const string UpdateCategoryPropertyByListUrl = "/IMService/CategoryProperty/UpdateCategoryPropertyByList";
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

        public CategoryPropertyFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryPropertyFacade(IPage page)
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
        private CategoryProperty CovertVMtoEntity(CategoryPropertyVM data)
        {
            CategoryProperty msg = data.ConvertVM<CategoryPropertyVM, CategoryProperty>();
            msg.Property = data.Property.ConvertVM<PropertyVM, PropertyInfo>();
            msg.PropertyGroup = data.PropertyGroup.ConvertVM<PropertyGroupInfoVM, PropertyGroupInfo>((v, t) =>
                                                                                                         {
                                                                                                             t.PropertyGroupName = new LanguageContent(v.PropertyGroupName);
                                                                                                         });
            msg.CategoryInfo = data.CategoryInfo.ConvertVM<CategoryVM, CategoryInfo>((v, t) =>
            {
                t.CategoryName = new LanguageContent(v.CategoryName);
            });
            msg.SysNo = data.SysNo;
            return msg;
        }



        /// <summary>
        /// 修改分类属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateCategoryProperty(CategoryPropertyVM data, EventHandler<RestClientEventArgs<CategoryProperty>> callback)
        {

            _restClient.Update(UPdateRelativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 获取分类属性
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetCategoryPropertyBySysNo(int sysNo, EventHandler<RestClientEventArgs<CategoryProperty>> callback)
        {
            _restClient.Query(GetRelativeUrl, sysNo, callback);
        }
        /// <summary>
        /// 复制类别属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void CopyCategoryOutputTemplateProperty(CategoryPropertyQueryVM model,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CategoryProperty info = new CategoryProperty()
            {
                SourceCategorySysNo = model.SourceCategorySysNo,
                TargetCategorySysNo = model.CategorySysNo
            };
            _restClient.Create(CopyCategoryOutputTemplatePropertyUrl, info, callback);
        }
        /// <summary>
        /// 批量更新属性
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void UpdateCategoryPropertyByList(List<CategoryPropertyVM> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            List<CategoryProperty> listCategoryProperty = new List<CategoryProperty>();
            foreach (var item in list)
            {
                listCategoryProperty.Add(CovertVMtoEntity(item));
            }
            _restClient.Update(UpdateCategoryPropertyByListUrl, listCategoryProperty, callback);
        }
        #endregion
    }
}
