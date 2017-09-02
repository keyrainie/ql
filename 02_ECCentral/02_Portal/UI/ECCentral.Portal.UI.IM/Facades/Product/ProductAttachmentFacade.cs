using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductAttachmentFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;
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

        public ProductAttachmentFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductAttachmentFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 转换品牌视图和品牌实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ProductInfo CovertVMtoEntity(ProductAttachmentDetailsListVM data)
        {
            if (data == null || data.ProductSysNo == null || data.ProductAttachmentList == null) return new ProductInfo();
            var msg = new ProductInfo() { SysNo = data.ProductSysNo.Value };
            if (msg.ProductAttachmentList == null) msg.ProductAttachmentList = new List<ProductAttachmentInfo>();
            foreach (var t in data.ProductAttachmentList)
            {
                if (t.ProductAttachmentSysNo == null) continue;
                var entity = new ProductAttachmentInfo
                {
                    AttachmentProduct = new ProductInfo { SysNo = t.ProductAttachmentSysNo.Value },
                    Quantity = Convert.ToInt32(t.AttachmentQuantity),
                    InUser = t.InUser,
                    InDate = t.InDate,
                    EditUser = t.Operator == AttachmentOperator.Update ? new UserInfo { UserName = CPApplication.Current.LoginUser.LoginName } : null,
                    EditDate = t.Operator == AttachmentOperator.Update ? DateTime.Now : (DateTime?) null
                };
                msg.ProductAttachmentList.Add(entity);
            }
            return msg;
        }

        /// <summary>
        /// 创建PM组信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="callback"></param>
        public void DeleteAttachmentByProductSysNo(int productSysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/IMService/ProductAttachment/DeleteProductAttachmentByProductSysNo";

            restClient.Delete(relativeUrl, productSysNo, callback);
        }

        /// <summary>
        ///获取PM组信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="callback"></param>
        public void GetProductAttachmentList(int productSysNo, EventHandler<RestClientEventArgs<List<ProductAttachmentInfo>>> callback)
        {
            string relativeUrl = "/IMService/ProductAttachment/GetProductAttachmentList";

            restClient.Query(relativeUrl, productSysNo, callback);
        }


        /// <summary>
        /// 创建品牌
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CreateAttachment(ProductAttachmentDetailsListVM data, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/IMService/ProductAttachment/CreateProductAttachment";
            restClient.Create(relativeUrl, CovertVMtoEntity(data), callback);
        }

        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateAttachment(ProductAttachmentDetailsListVM data, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/IMService/ProductAttachment/ModifyProductAttachment";
            var vm = CovertVMtoEntity(data);
            restClient.Update(relativeUrl, vm, callback);
        }
    }
}
