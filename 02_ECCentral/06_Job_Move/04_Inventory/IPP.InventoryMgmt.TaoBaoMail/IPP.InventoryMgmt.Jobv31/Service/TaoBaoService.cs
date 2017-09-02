using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using IPP.InventoryMgmt.JobV31.Common;
//[Mark:Remove]using IPP.ThirdPart.Interfaces;
using Newegg.Oversea.Framework.ServiceConsole.Client;
//[Mark:Remove]using IPP.ThirdPart.Interfaces.Contract;
//[Mark:Remove]using IPP.ThirdPart.Interfaces.Interface;
namespace IPP.InventoryMgmt.JobV31.Service
{
    public class TaoBaoService
    {
        private static TaobaoResponse Query(string method,Dictionary<string, string> param)
        {
            ITaoBaoMaintain service = ServiceBroker.FindService<ITaoBaoMaintain>();       
            try
            {              
                TaoBaoRequest taobaoRequest = new TaoBaoRequest();
                taobaoRequest.param = param;
                taobaoRequest.method = method;
                taobaoRequest.Header = Util.CreateServiceHeader();
                TaoBaoResponse taoBaoResponse = service.GetTaoBaoInventoryQtyByMethod(taobaoRequest);
                string response = taoBaoResponse.CommonReturnStringValue;
                //string response = TaoBaoUtil.Post(param);
                response = response.Replace("items_onsale_get_response>", "items_inventory_get_response>");
                if (string.IsNullOrEmpty(response))
                {
                    throw new Exception("淘宝未返回任何信息。");
                }
                if (response.IndexOf("error_response") > -1)
                {
                    throw new Exception(response);
                }
                TaobaoResponse taobaoResponse = XmlSerializerHelper.Deserializer<TaobaoResponse>(response, CommonConst.taobao_response_encoding);
                return taobaoResponse;
            }
            finally
            {
                ServiceBroker.DisposeService<ITaoBaoMaintain>(service);
            }          
        }

        private static IEnumerable<TaobaoProduct> Query(string method)
        {
            IEnumerable<TaobaoProduct> list = new List<TaobaoProduct>();
            int page = 1;
            int pageSize = CommonConst.taobao_items_inventory_get_pageSize;
            int pageCount = 1;
            int records = 0;
            for (; page <= pageCount; page++)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("page_no", page.ToString());
                param.Add("page_size", pageSize.ToString());
                param.Add("fields", CommonConst.taobao_items_inventory_get_fields);
                //param = TaobaoParamHelper.CreateParam(method, param);

                TaobaoResponse taobaoResponse = Query(method,param);
                if (taobaoResponse == null || taobaoResponse.Response == null || taobaoResponse.Response.ProductCollection == null || taobaoResponse.Response.ProductCollection.Count == 0)
                {
                    continue;
                }
                else
                {
                    records = taobaoResponse.Records;
                    pageCount = records <= pageSize ? 1 : (records % pageSize == 0 ? records / pageSize : records / pageSize + 1);
                }
                list = list.Union(taobaoResponse.Response.ProductCollection);
            }
            return list;
        }

        public static List<TaobaoProduct> QueryInventory()
        {
            IEnumerable<TaobaoProduct> list = Query(CommonConst.taobao_items_inventory_get);
            IEnumerator<TaobaoProduct> enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Status = "等待上架";
            }
            return list.ToList();
        }

        public static List<TaobaoProduct> QueryOnSale()
        {
            IEnumerable<TaobaoProduct> list = Query(CommonConst.taobao_items_onsale_get);
            IEnumerator<TaobaoProduct> enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Status = "出售中";
            }
            return list.ToList();
        }
    }
}
