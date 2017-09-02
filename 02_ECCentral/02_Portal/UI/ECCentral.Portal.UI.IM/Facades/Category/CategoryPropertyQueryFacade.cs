//************************************************************************
// 用户名				泰隆优选
// 系统名				类别管理
// 子系统名		        类别管理查询Facades端
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;


namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryPropertyQueryFacade
    {
        #region 字段以及构造函数
        private readonly RestClient restClient;
        const string GetRelativeUrl = "/IMService/Property/GetPropertyListByPropertyName";
        const string CreateRelativeUrl = "/IMService/CategoryProperty/CreateCategoryProperty";
        const string DelRelativeUrl = "/IMService/CategoryProperty/DelCategoryPropertyBySysNo";
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

        public CategoryPropertyQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryPropertyQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        /// <summary>
        /// 转换分类视图和分类实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private CategoryProperty CovertQueryVMtoEntity(CategoryPropertyQueryVM data)
        {
            if (data == null)
            {
                return null;
            }
            var tempCategoryProperty = new CategoryProperty
                                           {
                                               Property = new PropertyInfo { SysNo = data.PropertySysNo },
                                               PropertyGroup =
                                                   new PropertyGroupInfo { PropertyGroupName = new LanguageContent(data.GroupName) }
                                           };
            tempCategoryProperty.EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.userSysNo };
            tempCategoryProperty.EditDate = DateTime.Now;
            tempCategoryProperty.CategoryInfo = new CategoryInfo { SysNo = data.CategorySysNo };
            tempCategoryProperty.PropertyType = data.PropertyType;
            tempCategoryProperty.CompanyCode = CPApplication.Current.CompanyCode;
            tempCategoryProperty.LanguageCode = CPApplication.Current.LanguageCode;
            return tempCategoryProperty;
        }

        /// <summary>
        /// 查询分类属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void QueryCategoryProperty(CategoryPropertyQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CategoryPropertyQueryFilter filter = model.ConvertVM<CategoryPropertyQueryVM, CategoryPropertyQueryFilter>();

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            const string relativeUrl = "/IMService/CategoryProperty/QueryCategoryProperty";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (!(args.Result == null || args.Result.Rows == null))
                    {
                        foreach (var item in args.Result.Rows)
                        {
                            item.IsChecked = false;
                        }
                    }
                    callback(obj, args);
                }
                );
        }

        /// <summary>
        /// 根据属性名获取属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="callback"></param>
        public void GetPropertyByPropertyName(string propertyName, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Query(GetRelativeUrl, propertyName, callback);
        }

        /// <summary>
        /// 创建分类属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateCategoryProperty(CategoryPropertyQueryVM data, EventHandler<RestClientEventArgs<CategoryProperty>> callback)
        {
            restClient.Create(CreateRelativeUrl, CovertQueryVMtoEntity(data), callback);
        }

        /// <summary>
        /// 删除分类属性
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void DelCategoryProperty(IList<dynamic> sysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Delete(DelRelativeUrl, sysNo, callback);
        }
    }
}
