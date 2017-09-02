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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models.Product;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Portal.UI.IM.Facades.Product
{
    public class ProductEntryFacade
    {
          private RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public ProductEntryFacade(IPage page)
        {
            
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        
        public void Create(ProductEntryInfoVM item, EventHandler<RestClientEventArgs<bool>> callback)
        {
            ProductEntryInfo entity = EntityConverter<ProductEntryInfoVM, ProductEntryInfo>.Convert(item);
            string url = "/IMService/Entry/Create";
            restClient.Create<bool>(url, entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Update(ProductEntryInfoVM item, Action<bool> callback)
        {
            ProductEntryInfo entity = EntityConverter<ProductEntryInfoVM, ProductEntryInfo>.Convert(item);
            string url = "/IMService/Entry/Update";
            restClient.Update<bool>(url, entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 加载税率规则信息
        /// </summary>
        /// <param name="yyCardSysNo"></param>
        /// <param name="callback"></param>
        public void LoadProductEntryInfo(int sysNo, EventHandler<RestClientEventArgs<ProductEntryInfo>> callback)
        {
            string url = string.Format("/IMService/Entry/Load/{0}", sysNo);
            restClient.Query<ProductEntryInfo>(url, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="callback"></param>
        public void AuditSucess(ProductEntryInfo info, Action<bool> callback)
        {
            string url = "/IMService/Entry/AuditSucess";
            restClient.Update<bool>(url, info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 审核失败
        /// </summary>
        /// <param name="callback"></param>
        public void AuditFail(ProductEntryInfo info, Action<bool> callback)
        {
            string url = "/IMService/Entry/AuditFail";
            restClient.Update<bool>(url, info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 提交商检
        /// </summary>
        /// <param name="callback"></param>
        public void ToInspection(int sysNo, Action<bool> callback)
        {
            string url = "/IMService/Entry/ToInspection";
            ProductEntryInfo info = new ProductEntryInfo();
            info.ProductSysNo = sysNo;
            restClient.Update<bool>(url, info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 商检通过
        /// </summary>
        /// <param name="callback"></param>
        public void InspectionSucess(ProductEntryInfo info, Action<bool> callback)
        {
            string url = "/IMService/Entry/InspectionSucess";
            restClient.Update<bool>(url, info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 商检失败
        /// </summary>
        /// <param name="callback"></param>
        public void InspectionFail(ProductEntryInfo info, Action<bool> callback)
        {
            string url = "/IMService/Entry/InspectionFail";
            restClient.Update<bool>(url, info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 提交报关
        /// </summary>
        /// <param name="callback"></param>
        public void ToCustoms(int sysNo, Action<bool> callback)
        {
            string url = "/IMService/Entry/ToCustoms";
            ProductEntryInfo info = new ProductEntryInfo();
            info.ProductSysNo = sysNo;
            restClient.Update<bool>(url, info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 报关成功
        /// </summary>
        /// <param name="callback"></param>
        public void CustomsSuccess(ProductEntryInfo info, Action<bool> callback)
        {
            string url = "/IMService/Entry/CustomsSuccess";
            restClient.Update<bool>(url, info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 报关失败
        /// </summary>
        /// <param name="callback"></param>
        public void CustomsFail(ProductEntryInfo info, Action<bool> callback)
        {
            string url = "/IMService/Entry/CustomsFail";
            restClient.Update<bool>(url, info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

    }
}
