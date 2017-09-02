using System;
using System.Collections.Generic;
using System.Json;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductProfileTemplateFacade
    {
        #region 字段以及构造函数

        private readonly RestClient _restClient;
        const string QueryProductProfileTemplateListUrl = "/IMService/ProductProfileTemplate/QueryProductProfileTemplateList/{0}/{1}";
        const string QueryProductProfileTemplateUrl = "/IMService/ProductProfileTemplate/QueryProductProfileTemplate";
        const string CreateProductProfileTemplateUrl = "/IMService/ProductProfileTemplate/CreateProductProfileTemplate";
        const string DeleteProductProfileTemplateUrl = "/IMService/ProductProfileTemplate/DeleteProductProfileTemplate";
        const string ModifyProductProfileTemplateUrl = "/IMService/ProductProfileTemplate/ModifyProductProfileTemplate";
        #endregion

        #region 构造函数以及字段
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

        public ProductProfileTemplateFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductProfileTemplateFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }

        #endregion

        /// <summary>
        /// 获取查询模板
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="templateType"></param>
        /// <param name="callback"></param>
        public virtual void QueryProductProfileTemplateList(string userId, string templateType, EventHandler<RestClientEventArgs<List<ProductProfileTemplateInfo>>> callback)
        {
            string relativeUrl = string.Format(QueryProductProfileTemplateListUrl, userId, templateType);
            _restClient.Query(relativeUrl, callback);
        }


        /// <summary>
        /// 获取单个查询模板
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void QueryProductProfileTemplate(int sysNo, EventHandler<RestClientEventArgs<ProductProfileTemplateInfo>> callback)
        {
            _restClient.Query(QueryProductProfileTemplateUrl, sysNo, callback);
        }


        /// <summary>
        /// 插入查询模板
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void CreateProductProfileTemplate(ProductProfileTemplateInfo model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            _restClient.Create(CreateProductProfileTemplateUrl, model, callback);
        }

        /// <summary>
        /// 删除查询模板
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void DeleteProductProfileTemplate(ProductProfileTemplateInfo model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            _restClient.Delete(DeleteProductProfileTemplateUrl, model, callback);
        }

        /// <summary>
        /// 修改查询模板
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void ModifyProductProfileTemplate(ProductProfileTemplateInfo model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            _restClient.Update(ModifyProductProfileTemplateUrl, model, callback);
        }

        /// <summary>
        /// 转换成查询条件实体
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <param name="currentType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public object ConvertToProductQueryExVM(JsonValue jsonValue, Type currentType, object source)
        {
            if (source == null)
                source = Activator.CreateInstance(currentType);
            try
            {
                if (jsonValue == null) return source;
                var jsonList = (JsonObject)jsonValue;
                foreach (var key in jsonList.Keys)
                {
                    var isExist = Invoker.ExistProperty(currentType, key);
                    var value = jsonList[key];
                    if (isExist)
                    {
                        var proType = Invoker.GetPropertyType(currentType, key);
                        if (proType.IsGenericType && proType.GetGenericTypeDefinition() == typeof(Nullable<>)
                               && proType.GetGenericArguments() != null
                               && proType.GetGenericArguments().Length == 1)
                        {
                            var sourceType = proType.GetGenericArguments()[0];
                            if (sourceType != null)
                            {
                                if (value == null)
                                {
                                    Invoker.PropertySet(source, key, null);
                                    continue;
                                }
                                if (sourceType.IsEnum)
                                {
                                    var numberValue = int.Parse(value.ToString());
                                    Invoker.PropertySet(source, key, numberValue);

                                }
                                if (sourceType.Name.ToLower().Contains("datetime"))
                                {
                                    var reg = new Regex("\"");
                                    var tempValue = reg.Replace(value.ToString(), "");
                                    var numberValue = DateTime.Parse(tempValue.ToString());
                                    Invoker.PropertySet(source, key, numberValue);

                                }
                                if (sourceType.Name.ToLower().Contains("decimal"))
                                {
                                    var numberValue = decimal.Parse(value.ToString());
                                    Invoker.PropertySet(source, key, numberValue);
                                }
                                else if (sourceType.Name.ToLower().Contains("int"))
                                {
                                    var numberValue = Int32.Parse(value.ToString());
                                    Invoker.PropertySet(source, key, numberValue);
                                   
                                }


                            }
                        }
                        else if (proType.IsValueType || proType.Name.ToLower().Contains("string"))
                        {
                            if(value!=null)
                            {
                                var reg = new Regex("\"");
                                var tempValue = reg.Replace(value.ToString(), "");
                                Invoker.PropertySet(source, key, tempValue);
                            }
                          
                        }
                        else if (proType.IsClass)
                        {
                            var tempSource = Invoker.PropertyGet(source, key);
                            var tempValue = ConvertToProductQueryExVM(value, proType, tempSource);
                            Invoker.PropertySet(source, key, tempValue);
                        }
                    }
                }
                return source;
            }
            catch (Exception)
            {
                return source;
            }
        }
    }
}
