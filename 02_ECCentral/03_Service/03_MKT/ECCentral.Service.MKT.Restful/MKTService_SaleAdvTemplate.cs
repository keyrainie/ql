using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.MKT.Restful.ResponseMsg;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        [WebGet(UriTemplate = "/SaleAdvTemplate/GetActiveCodeNames/{companyCode}/{channelID}")]
        public List<WebPage> GetSaleAdvTemplateActiveCodeNames(string companyCode, string channelID)
        {
            return ObjectFactory<ISaleAdvTemplateQueryDA>.Instance.GetActiveCodeNames(companyCode, channelID);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/Query", Method = "POST")]
        public QueryResult QuerySaleAdvTemplates(SaleAdvTemplateQueryFilter msg)
        {
            int totalCount;
            var dataTable = ObjectFactory<ISaleAdvTemplateQueryDA>.Instance.Query(msg, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/{sysNo}", Method = "GET")]
        public SaleAdvertisement LoadBySysNo(string sysNo)
        {
            int no;
            if (int.TryParse(sysNo, out no))
            {
                return ObjectFactory<SaleAdvAppService>.Instance.LoadBySysNo(no);
            }
            throw new ArgumentException("Invalid SysNo");
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/Create", Method = "POST")]
        public SaleAdvertisement CreateSaleAdv(SaleAdvertisement msg)
        {
            return ObjectFactory<SaleAdvAppService>.Instance.Create(msg);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/Update", Method = "PUT")]
        public void UpdateSaleAdv(SaleAdvertisement msg)
        {
            ObjectFactory<SaleAdvAppService>.Instance.Update(msg);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/Lock", Method = "PUT")]
        public void LockSaleAdv(int sysNo)
        {
            var entity = new SaleAdvertisement { SysNo = sysNo, IsHold = "Y" };
            ObjectFactory<SaleAdvAppService>.Instance.UpdateIsHoldSaleAdvertisementBySysNo(entity);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/UnLock", Method = "PUT")]
        public void UnLockSaleAdv(int sysNo)
        {
            var entity = new SaleAdvertisement { SysNo = sysNo, IsHold = "N" };
            ObjectFactory<SaleAdvAppService>.Instance.UpdateIsHoldSaleAdvertisementBySysNo(entity);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/CreateSaleAdvItem", Method = "POST")]
        public SaleAdvertisementItem CreateSaleAdvItem(SaleAdvertisementItem msg)
        {
            return ObjectFactory<SaleAdvAppService>.Instance.CreateItem(msg);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/BatchCreateSaleAdvItem", Method = "POST")]
        public BatchResultRsp BatchCreateSaleAdvItem(SaleAdvertisementItem msg)
        {
            var BatchResultRsp = new BatchResultRsp
            {
                SuccessRecords = new List<string>(),
                FailureRecords = new List<string>()
            };

            var productIDList = msg.Introduction.Split('\r').Distinct().ToList();
            SaleAdvertisementItem itemTmp = null;

            foreach (string pId in productIDList)
            {
                itemTmp = new SaleAdvertisementItem();
                itemTmp.ProductID = pId;
                itemTmp.SaleAdvSysNo = msg.SaleAdvSysNo;
                itemTmp.RecommendType = msg.RecommendType;
                itemTmp.GroupPriority = msg.GroupPriority;
                itemTmp.Priority = msg.Priority++;
                itemTmp.GroupSysNo = msg.GroupSysNo;
                itemTmp.GroupName = msg.GroupName;
                itemTmp.IconAddr = msg.IconAddr;

                try
                {
                    ObjectFactory<SaleAdvAppService>.Instance.CreateItem(itemTmp);
                    BatchResultRsp.SuccessRecords.Add(pId);
                }
                catch (Exception ex)
                {
                    BatchResultRsp.FailureRecords.Add(pId + ex.Message);
                    continue;
                }
            }
            return BatchResultRsp;
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/UpdateSaleAdvItem", Method = "PUT")]
        public void UpdateSaleAdvItem(SaleAdvertisementItem msg)
        {
            ObjectFactory<SaleAdvAppService>.Instance.UpdateItem(msg);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/BatchUpdateSaleAdvItem", Method = "PUT")]
        public void BatchUpdateSaleAdvItem(List<SaleAdvertisementItem> list)
        {
            ObjectFactory<SaleAdvAppService>.Instance.BatchUpdateItem(list);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/DeleteSaleAdvItem", Method = "DELETE")]
        public void DeleteSaleAdvItem(int sysNo)
        {
            ObjectFactory<SaleAdvAppService>.Instance.DeleteItem(sysNo);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/BatchDeleteSaleAdvItem", Method = "DELETE")]
        public void BatchDeleteSaleAdvItem(List<int> list)
        {
            ObjectFactory<SaleAdvAppService>.Instance.BatchDeleteItem(list);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/UpdateSaleAdvItemStatus", Method = "PUT")]
        public void UpdateSaleAdvItemStatus(SaleAdvertisementItem msg)
        {
            ObjectFactory<SaleAdvAppService>.Instance.UpdateItemStatus(msg);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/BatchUpdateSaleAdvItemStatus", Method = "PUT")]
        public void BatchUpdateSaleAdvItemStatus(List<SaleAdvertisementItem> list)
        {
            ObjectFactory<SaleAdvAppService>.Instance.BatchUpdateItemStatus(list);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/CreateSaleAdvGroup", Method = "POST")]
        public SaleAdvertisementGroup CreateSaleAdvGroup(SaleAdvertisementGroup msg)
        {
            return ObjectFactory<SaleAdvAppService>.Instance.CreateSaleAdvGroup(msg);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/UpdateSaleAdvGroup", Method = "PUT")]
        public SaleAdvertisementGroup UpdateSaleAdvGroup(SaleAdvertisementGroup msg)
        {
            return ObjectFactory<SaleAdvAppService>.Instance.UpdateSaleAdvGroup(msg);
        }

        [WebInvoke(UriTemplate = "/SaleAdvTemplate/DeleteSaleAdvGroup", Method = "DELETE")]
        public void DeleteSaleAdvGroup(int sysNo)
        {
            ObjectFactory<SaleAdvAppService>.Instance.DeleteSaleAdvGroup(sysNo);
        }
    }
}
