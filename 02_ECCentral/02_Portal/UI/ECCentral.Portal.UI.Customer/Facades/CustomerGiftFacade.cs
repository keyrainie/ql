using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class CustomerGiftFacade
    {
        private readonly RestClient restClient;
        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public CustomerGiftFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(CustomerGiftQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/Gift/Query";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void Create(CustomerGiftCreateVM vm, EventHandler<RestClientEventArgs<string>> callback)
        {
            List<CustomerGift> giftList = new List<CustomerGift>();
            //原方法 采用SysNo判断
            //foreach (var cid in vm.CustomerIDList)
            //{
            //    var gift = new CustomerGift();
            //    gift.CustomerSysNo = cid;
            //    gift.ProductID = vm.ProductID;
            //    gift.ProductSysNo = int.Parse(vm.ProductSysNo);
            //    gift.Quantity = int.Parse(vm.Quantity);
            //    gift.Status = CustomerGiftStatus.Origin;
            //    giftList.Add(gift);
            //}
            //新方法 采用CustomerID判断
            foreach (var cid in vm.CusIDList)
            {
                var gift = new CustomerGift();
                gift.CustomerID = cid;
                gift.ProductID = vm.ProductID;
                gift.ProductSysNo = int.Parse(vm.ProductSysNo);
                gift.Quantity = int.Parse(vm.Quantity);
                gift.Status = CustomerGiftStatus.Origin;
                giftList.Add(gift);
            }
            string relativeUrl = "/CustomerService/Gift/Create";
            restClient.Create<string>(relativeUrl, giftList, callback);
        }

        private List<CustomerGift> TransformVMForBatch(List<CustomerGiftListVM> vmList)
        {
            List<CustomerGift> dataList = new List<CustomerGift>(vmList.Count);
            foreach (var vm in vmList)
            {
                var data = vm.ConvertVM<CustomerGiftListVM, CustomerGift>();
                dataList.Add(data);
            }

            return dataList;
        }

        public void Notify(List<CustomerGiftListVM> vm, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/CustomerService/Gift/Notify";
            restClient.Update<string>(relativeUrl, TransformVMForBatch(vm), callback);
        }

        public void Remind(List<CustomerGiftListVM> vm, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/CustomerService/Gift/Remind";
            restClient.Update<string>(relativeUrl, TransformVMForBatch(vm), callback);
        }

        public void Void(List<CustomerGiftListVM> vm, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/CustomerService/Gift/Void";
            restClient.Update<string>(relativeUrl, TransformVMForBatch(vm), callback);
        }

        public void Abandon(List<CustomerGiftListVM> vm, EventHandler<RestClientEventArgs<List<int>>> callback)
        {
            string relativeUrl = "/CustomerService/Gift/Abandon";
            restClient.Update<List<int>>(relativeUrl, TransformVMForBatch(vm), callback);
        }

        public void CancelAbandon(List<CustomerGiftListVM> vm, EventHandler<RestClientEventArgs<List<int>>> callback)
        {
            string relativeUrl = "/CustomerService/Gift/CancelAbandon";
            restClient.Update<List<int>>(relativeUrl, TransformVMForBatch(vm), callback);
        }
    }
}
