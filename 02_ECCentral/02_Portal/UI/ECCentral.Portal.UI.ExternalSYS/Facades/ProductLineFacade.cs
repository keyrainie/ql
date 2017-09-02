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
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.Facades
{
    public class ProductLineFacade
    {
         private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ServiceBaseUrl);

        public ProductLineFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public ProductLineFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }
        /// <summary>
        /// 获取产品线信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetProductLineByQuery(ProductLineQueryVM model,int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback) 
        {
            string GetProductLineByQueryUrl = "ExternalSYSService/ProductLine/GetProductLineByQuery";
            ProductLineQueryFilter query = new ProductLineQueryFilter()
            {
                ProductLineName = model.ProductLineName,
                PageInfo = new QueryFilter.Common.PagingInfo() 
                {
                    SortBy=SortField,
                    PageSize=PageSize,
                    PageIndex=PageIndex
                }
            };
            restClient.QueryDynamicData(GetProductLineByQueryUrl, query, callback);
        }

        /// <summary>
        /// 获取生产线分类
        /// </summary>
        /// <param name="callback"></param>
        public void GetAllProductLineCategory(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string GetAllProductLineCategoryUrl = "ExternalSYSService/ProductLine/GetAllProductLineCategory";
            restClient.QueryDynamicData(GetAllProductLineCategoryUrl,null,callback);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void CreateProductLine(ProductLineVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string CreateProductLineUrl = "ExternalSYSService/ProductLine/CreateProductLine";

            restClient.Create(CreateProductLineUrl, ConvertToProductLine(model), callback);

        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void UpdateProductLine(ProductLineVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string UpdateProductLineUrl = "ExternalSYSService/ProductLine/UpdateProductLine";

            restClient.Update(UpdateProductLineUrl, ConvertToProductLine(model), callback);
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="callback"></param>
        public void DeleteProductLine(int SysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string DeleteProductLineUrl = "ExternalSYSService/ProductLine/DeleteProductLine";
            restClient.Delete(DeleteProductLineUrl, SysNo, callback);

        }

        private ProductLineInfo ConvertToProductLine(ProductLineVM model)
        {
            return new ProductLineInfo()
            {
                CompanyCode = CPApplication.Current.CompanyCode,
                LanguageCode = CPApplication.Current.LanguageCode,
                Priority = Convert.ToInt32(model.Priority),
                ProductLineCategorySysNo = model.Category.SysNo,
                ProductLineName = model.ProductLineName,
                User = new BizEntity.Common.UserInfo() { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.DisplayName },
                UseScopeDescription = model.UseScopeDescription,
                SysNo=model.SysNo
            };
        }
    }
}
