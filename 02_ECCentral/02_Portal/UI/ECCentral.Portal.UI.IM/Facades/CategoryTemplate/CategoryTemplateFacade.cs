using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Service.IM.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryTemplateFacade
    {
           private readonly RestClient restClient;
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

        public CategoryTemplateFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryTemplateFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void GetCategoryTemplateDataByC3SysNo(int? C3SysNo, EventHandler<RestClientEventArgs<CategoryTemplateRsp>> callback)
        {
            string relativeUrl = "/IMService/CategoryTemplate/GetCategoryTemplateInfoByC3SysNo";
            restClient.Query<CategoryTemplateRsp>(relativeUrl,C3SysNo,callback);
        }

        public void SaveCategoryTemplate(CategoryTemplateDataVM vm,int? C3SysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
             string relativeUrl = "/IMService/CategoryTemplate/SaveCategoryTemplate";
             restClient.Create(relativeUrl, ConvertEntity(vm,C3SysNo), callback);
        }

        private List<CategoryTemplateInfo> ConvertEntity(CategoryTemplateDataVM vm,int? C3SysNo)
        {
            string DescriptionTemplates = string.Empty;
            vm.CategoryTemplateProductDescription.CategoryTemplatePropertyList.ForEach(s =>
            {
                 DescriptionTemplates = DescriptionTemplates + "," + s.SysNo.ToString();
                  
                
            });
            string ProductNameTemplates = string.Empty;
          
            vm.CategoryTemplateProductDescription.CategoryTemplatePropertyList.ForEach(s =>
            {
                
                    ProductNameTemplates = ProductNameTemplates + "," + s.SysNo.ToString();
                   
                
            });
            string ProductTitleTemplates = string.Empty;
            vm.CategoryTemplateProductTitle.CategoryTemplatePropertyList.ForEach(s =>
            {
                
                    ProductTitleTemplates = ProductTitleTemplates + "," + s.SysNo.ToString();
                
            });
            string WebTemplates = string.Empty;
            vm.CategoryTemplateWeb.CategoryTemplatePropertyList.ForEach(s =>
            {
                WebTemplates = WebTemplates + "," + s.SysNo.ToString();
            });

            UserInfo user = new UserInfo() {SysNo=CPApplication.Current.LoginUser.UserSysNo,UserName=CPApplication.Current.LoginUser.LoginName };
            List<CategoryTemplateInfo> list = new List<CategoryTemplateInfo>() 
            {
                new CategoryTemplateInfo(){
                    TemplateType=vm.CategoryTemplateProductDescription.TemplateType,
                    Templates=subString(DescriptionTemplates),
                    TargetSysNo=C3SysNo,
                    CompanyCode=CPApplication.Current.CompanyCode,
                    LanguageCode=CPApplication.Current.LanguageCode,
                    User=user
                },
               new CategoryTemplateInfo(){
                    TemplateType=vm.CategoryTemplateProductName.TemplateType,
                   Templates=subString(ProductNameTemplates),
                    TargetSysNo=C3SysNo,
                    CompanyCode=CPApplication.Current.CompanyCode,
                    LanguageCode=CPApplication.Current.LanguageCode,
                    User=user
                },
                new CategoryTemplateInfo(){
                    TemplateType=vm.CategoryTemplateProductTitle.TemplateType,
                      Templates=subString(ProductTitleTemplates),
                    TargetSysNo=C3SysNo,
                    CompanyCode=CPApplication.Current.CompanyCode,
                    LanguageCode=CPApplication.Current.LanguageCode,
                    User=user
                },
                new CategoryTemplateInfo(){
                    TemplateType=vm.CategoryTemplateWeb.TemplateType,
                     Templates=subString(WebTemplates),
                    TargetSysNo=C3SysNo,
                    CompanyCode=CPApplication.Current.CompanyCode,
                    LanguageCode=CPApplication.Current.LanguageCode,
                    User=user
                }
            };
            return list;
            
        }
        private string subString(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return s.Substring(1, s.Length - 1);
            }
            return string.Empty;
        }
    }
}
