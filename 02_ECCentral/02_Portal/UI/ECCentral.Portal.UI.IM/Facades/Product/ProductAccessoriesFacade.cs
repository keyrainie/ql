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
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.IM;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductAccessoriesFacade
    {
         private readonly RestClient restClient;

         private const string GetProductAccessoriesByQueryUrl = "/IMService/ProductAccessories/GetProductAccessoriesByQuery";
         private const string CreateAccessoriesQueryMasterUrl = "/IMService/ProductAccessories/CreateAccessoriesQueryMaster";
         private const string UpdateAccessoriesQueryMasterUrl = "/IMService/ProductAccessories/UpdateAccessoriesQueryMaster";
         private const string GetAccessoriesQueryConditionBySysNoUrl = "/IMService/ProductAccessories/GetAccessoriesQueryConditionBySysNo";
         private const string CreateAccessoriesQueryConditionUrl = "/IMService/ProductAccessories/CreateAccessoriesQueryCondition";
         private const string UpdateAccessoriesQueryConditionUrl = "/IMService/ProductAccessories/UpdateAccessoriesQueryCondition";
         private const string DeleteAccessoriesQueryConditionUrl = "/IMService/ProductAccessories/DeleteAccessoriesQueryCondition";
         private const string GetProductAccessoriesConditionValueByQueryUrl = "/IMService/ProductAccessories/GetProductAccessoriesConditionValueByQuery";
         private const string GetProductAccessoriesConditionValueByConditionUrl = "/IMService/ProductAccessories/GetProductAccessoriesConditionValueByCondition";
         private const string CreateProductAccessoriesQueryConditionValueUrl = "/IMService/ProductAccessories/CreateProductAccessoriesQueryConditionValue";
         private const string UpdateProductAccessoriesQueryConditionValueUrl = "/IMService/ProductAccessories/UpdateProductAccessoriesQueryConditionValue";
         private const string DeleteProductAccessoriesQueryConditionValueUrl = "/IMService/ProductAccessories/DeleteProductAccessoriesQueryConditionValue";
         private const string GetConditionValueByQueryUrl = "/IMService/ProductAccessories/GetConditionValueByQuery";
         private const string QueryAccessoriesQueryConditionBindUrl = "/IMService/ProductAccessories/QueryAccessoriesQueryConditionBind";
         private const string DeleteAccessoriesQueryConditionBindUrl = "/IMService/ProductAccessories/DeleteAccessoriesQueryConditionBind";
         private const string GetAccessoriesQueryExcelOutputUrl = "/IMService/ProductAccessories/GetAccessoriesQueryExcelOutput";



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

        public ProductAccessoriesFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductAccessoriesFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        #region "配件查询操作"
        
       
        /// <summary>
        /// 根据query获取配件查询信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetProductAccessoriesByQuery(ProductAccessoriesQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductAccessoriesQueryFilter query;
            query = model.ConvertVM<ProductAccessoriesQueryVM, ProductAccessoriesQueryFilter>();
            query.PagingInfo = new PagingInfo() 
            {
                PageIndex = PageIndex,
                PageSize=PageSize,
                SortBy = SortField
            };
            restClient.QueryDynamicData(GetProductAccessoriesByQueryUrl, query, callback);
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void CreateProductAccessories(ProductAccessoriesVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {

            restClient.Create(CreateAccessoriesQueryMasterUrl, ConvertEntity(model), callback);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void UpdateProductAccessories(ProductAccessoriesVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update(UpdateAccessoriesQueryMasterUrl, ConvertEntity(model), callback);
        }
        #endregion

        #region "查询条件操作"
        
       
        /// <summary>
        /// 根据配件查询的SysNo获取查询条件信息
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="callback"></param>
        public void GetAccessoriesQueryConditionBySysNo(int SysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData(GetAccessoriesQueryConditionBySysNoUrl, SysNo, callback);
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="modle"></param>
        /// <param name="callback"></param>
        public void CreateAccessoriesQueryCondition(ProductAccessoriesQueryConditionVM modle, EventHandler<RestClientEventArgs<dynamic>> callback)
        {

            restClient.Create(CreateAccessoriesQueryConditionUrl, ConvertyConditionEntity(modle), callback);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="modle"></param>
        /// <param name="callback"></param>
        public void UpdateAccessoriesQueryCondition(ProductAccessoriesQueryConditionVM modle, EventHandler<RestClientEventArgs<dynamic>> callback) 
        {
            restClient.Update(UpdateAccessoriesQueryConditionUrl, ConvertyConditionEntity(modle), callback);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Sysno"></param>
        /// <param name="callback"></param>
        public void DeleteAccessoriesQueryCondition(int Sysno, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Delete(DeleteAccessoriesQueryConditionUrl, Sysno, callback);
        }

        #endregion

        #region "条件选项值操作"
        
    
        /// <summary>
        /// 根据query获取选项值
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="callback"></param>
        public void GetProductAccessoriesConditionValueByQuery(ProductAccessoriesConditionValueQueryVM mode, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductAccessoriesConditionValueQueryFilter query = new ProductAccessoriesConditionValueQueryFilter()
            {
                ConditionSysNo = mode.Condition.SysNo,
                ConditionValue = mode.ConditionValue.ConditionValue,
                MasterSysNo = mode.MasterSysNo,
                PagingInfo = new PagingInfo() 
                {
                    PageIndex=PageIndex,
                    PageSize=PageSize,
                    SortBy = SortField
                }
            };
          
            restClient.QueryDynamicData(GetProductAccessoriesConditionValueByQueryUrl, query, callback);
        }
        /// <summary>
        ///  得到某个条件的父节点的所有选项值
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="callback"></param>
        public void GetProductAccessoriesConditionValueByCondition(int ConditionSysNo, int MasterSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductAccessoriesConditionValueQueryFilter query = new ProductAccessoriesConditionValueQueryFilter()
            {
                ConditionSysNo = ConditionSysNo,
                MasterSysNo = MasterSysNo,
             };
            restClient.QueryDynamicData(GetProductAccessoriesConditionValueByConditionUrl, query, callback);
        }
        /// <summary>
        /// 新建选项值
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void CreateProductAccessoriesQueryConditionValue(ProductAccessoriesConditionValueVM model,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Create(CreateProductAccessoriesQueryConditionValueUrl, ConvertConditionValueEntity(model), callback);
        }
        /// <summary>
        /// 更新选项值
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void UpdateProductAccessoriesQueryConditionValue(ProductAccessoriesConditionValueVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update(UpdateProductAccessoriesQueryConditionValueUrl, ConvertConditionValueEntity(model), callback);
        }
        /// <summary>
        /// 删除选项值
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="callback"></param>
        public void DeleteProductAccessoriesQueryConditionValue(int SysNo , EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Delete(DeleteProductAccessoriesQueryConditionValueUrl, SysNo, callback);
        }
        #endregion

        #region "查询效果设置"

        /// <summary>
        /// 得到某个条件下的所有选项值
        /// </summary>
        /// <param name="ConditionSysNo"></param>
        /// <param name="MasterSysNo"></param>
        /// <param name="callback"></param>
        public void GetConditionValueByQuery(int ConditionSysNo, int MasterSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductAccessoriesConditionValueQueryFilter query = new ProductAccessoriesConditionValueQueryFilter()
            {
                ConditionSysNo = ConditionSysNo,
                MasterSysNo = MasterSysNo,
            };
            restClient.QueryDynamicData(GetConditionValueByQueryUrl, query, callback);
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void QueryAccessoriesQueryConditionBind(ProductAccessoriesQueryConditionPreViewQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductAccessoriesQueryConditionPreViewQueryFilter query = model.ConvertVM<ProductAccessoriesQueryConditionPreViewQueryVM, ProductAccessoriesQueryConditionPreViewQueryFilter>();
            query.PagingInfo = new PagingInfo()
            {
                PageIndex = PageIndex,
                PageSize = PageSize,
                SortBy = SortField
            };
            restClient.QueryDynamicData(QueryAccessoriesQueryConditionBindUrl, query, callback);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ProductSysNo"></param>
        /// <param name="ConditionValueSysNo"></param>
        /// <param name="masterSysNo"></param>
        /// <param name="callback"></param>
        public void DeleteAccessoriesQueryConditionBind(List<ProductAccessoriesQueryConditionPreViewInfo> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Delete(DeleteAccessoriesQueryConditionBindUrl, list, callback);
        }
        /// <summary>
        /// 得到导出EXECL的数据
        /// </summary>
        /// <param name="MasterSysNo"></param>
        /// <param name="IsTreeQuery"></param>
        /// <param name="callback"></param>
        public void GetAccessoriesQueryExcelOutput(int MasterSysNo, string IsTreeQuery,ColumnSet[] columns)
        {
            ProductAccessoriesConditionValueQueryFilter query = new ProductAccessoriesConditionValueQueryFilter() 
            {
                MasterSysNo=MasterSysNo,
                IsTreeQuery=IsTreeQuery
            };
            restClient.ExportFile(GetAccessoriesQueryExcelOutputUrl, query, columns);
        }
        #endregion



        /// <summary>
        /// 选项值model转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ProductAccessoriesQueryConditionValueInfo ConvertConditionValueEntity(ProductAccessoriesConditionValueVM data)
        {
            ProductAccessoriesQueryConditionValueInfo info= new ProductAccessoriesQueryConditionValueInfo() 
            {
                ConditionSysNo=data.Condition.SysNo,
                ConditionValue=data.ConditionValue.ConditionValue,
                MasterSysNo=data.MasterSysNo,
                SysNo=data.SysNo
            };
            info.ConditionValueParentSysNo = data.ParentConditionValue == null ? 0 : data.ParentConditionValue.SysNo;
            return info;

        }



        /// <summary>
        /// 配件查询model转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ProductAccessoriesInfo ConvertEntity(ProductAccessoriesVM data)
        {

            return new ProductAccessoriesInfo() 
            {
                AccessoriesQueryName=data.AccessoriesQueryName,
                Status=data.Status,
                BackPictureBigUrl=data.BackPictureBigUrl,
                IsTreeQuery=data.IsTreeQuery?BooleanEnum.Yes:BooleanEnum.No,
                SysNo=data.SysNo,
                User=new BizEntity.Common.UserInfo(){UserName=CPApplication.Current.LoginUser.LoginName,SysNo=CPApplication.Current.LoginUser.UserSysNo},
                CompanyCode=CPApplication.Current.CompanyCode,
                LanguageCode=CPApplication.Current.LanguageCode
            };
        }
        /// <summary>
        /// 查询条件model转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ProductAccessoriesQueryConditionInfo ConvertyConditionEntity(ProductAccessoriesQueryConditionVM data) 
        {
            return new ProductAccessoriesQueryConditionInfo()
            {
                Condition = new AccessoriesQueryConditionInfo() { ConditionName = data.Condition.ConditionName, MasterSysNo = data.Condition.MasterSysNo, Priority = data.Priority, SysNo = data.Condition.SysNo },
                ParentCondition = new AccessoriesQueryConditionInfo() {ConditionName=data.ParentCondition.ConditionName,SysNo=data.ParentCondition.SysNo },
               };
        }

        
    }
}
