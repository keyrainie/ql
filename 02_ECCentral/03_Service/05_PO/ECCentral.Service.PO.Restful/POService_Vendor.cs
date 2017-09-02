using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.AppService;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.PO.Restful.RequestMsg;

namespace ECCentral.Service.PO.Restful
{

    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public partial class POService
    {
        /// <summary>
        /// 创建一个全新的供应商信息。
        /// </summary>
        /// <param name="newVendor"></param>
        [WebInvoke(UriTemplate = "/Vendor/CreateVendor", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public VendorInfo CreateVendor(VendorInfo newVendor)
        {
            return ObjectFactory<VendorAppService>.Instance.CreateNewVendor(newVendor);
        }

        /// <summary>
        /// 锁定供应商
        /// </summary>
        /// <param name="vendor"></param>
        [WebInvoke(UriTemplate = "/Vendor/HoldVendorInfo", Method = "PUT")]
        public int HoldVendor(VendorInfo vendorInfo)
        {
            return ObjectFactory<VendorAppService>.Instance.HoldVendor(vendorInfo);
        }

        /// <summary>
        /// 锁定，解锁关联PM；
        /// </summary>
        /// <param name="requestMsg"></param>
        [WebInvoke(UriTemplate = "/Vendor/HoldOrUnHoldVendorPM", Method = "PUT")]
        public List<int> HoldOrUnHoldVendorPM(VendorHoldPMReq requestMsg)
        {
            return ObjectFactory<VendorAppService>.Instance.HoldOrUnholdVendorPM(requestMsg.VendorSysNo, requestMsg.HoldUserSysNo, requestMsg.Reason, requestMsg.HoldSysNoList, requestMsg.UnHoldSysNoList, requestMsg.CompanyCode);
        }

        /// <summary>
        /// 解锁供应商
        /// </summary>
        /// <param name="vendor"></param>
        [WebInvoke(UriTemplate = "/Vendor/UnHoldVendorInfo", Method = "PUT")]
        public int UnHoldVendor(VendorInfo vendorInfo)
        {
            return ObjectFactory<VendorAppService>.Instance.UnHoldVendor(vendorInfo);

        }

        /// <summary>
        /// 审批通过供应商财务信息更改请求。
        /// </summary>
        /// <param name="vendor"></param>
        [WebInvoke(UriTemplate = "/Vendor/ApproveVendorFinanceInfo", Method = "PUT")]
        public void ApproveVendorFinanceInfo(VendorModifyRequestInfo info)
        {
            ObjectFactory<VendorAppService>.Instance.ApproveVendorFinanceInfo(info);
        }

        /// <summary>
        /// 拒绝供应商财务信息更改请求。
        /// </summary>
        /// <param name="vendor"></param>
        [WebInvoke(UriTemplate = "/Vendor/DeclineVendorFinanceInfo", Method = "PUT")]
        public void DeclineVendorFinanceInfo(VendorModifyRequestInfo info)
        {
            ObjectFactory<VendorAppService>.Instance.DeclineVendorFinanceInfo(info);
        }

        /// <summary>
        /// 收回供应商财务信息更改请求。
        /// </summary>
        /// <param name="vendor"></param>
        [WebInvoke(UriTemplate = "/Vendor/WithdrawVendorFinanceInfo", Method = "PUT")]
        public void WithdrawVendorFinanceInfo(VendorModifyRequestInfo info)
        {
            ObjectFactory<VendorAppService>.Instance.WithDrawVendorFinanceInfo(info);
        }

        /// <summary>
        /// 提交供应商财务信息更改请求。
        /// </summary>
        /// <param name="vendor"></param>
        [WebInvoke(UriTemplate = "/Vendor/RequestForApprovalVendorFinanceInfo", Method = "PUT")]
        public void RequestForApprovalVendorFinanceInfo(VendorModifyRequestInfo info)
        {
            ObjectFactory<VendorAppService>.Instance.RequestForApprovalVendorFinanceInfo(info);
        }

        /// <summary>
        /// 获取供应商列表。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Vendor/QueryVendorList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVendorList(VendorQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IVendorQueryDA>.Instance.QueryVendorList(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        /// <summary>
        /// 获取单个供应商的详细信息。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Vendor/GetVendorInfo/{sysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public VendorInfo LoadVendorBySysNo(string sysNo)
        {
            int getVendorSysNo = 0;
            if (!int.TryParse(sysNo, out getVendorSysNo))
            {
                return new VendorInfo() { SysNo = null };
            }
            return ObjectFactory<VendorAppService>.Instance.LoadVendorInfoBySysNo(getVendorSysNo);
        }

        /// <summary>
        /// 加载供应商历史日志信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Vendor/GetVendorHistoryLogs/{sysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<VendorHistoryLog> LoadVendorHistoryLogInfoBySysNo(string sysNo)
        {
            return ObjectFactory<VendorAppService>.Instance.LoadVendorHistoryLogBySysNo(Convert.ToInt32(sysNo));
        }

        /// <summary>
        /// 撤销供应商等级申请
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Vendor/CancelVendorModifyRequest", Method = "PUT")]
        public VendorModifyRequestInfo CancelVendorModifyRequest(VendorModifyRequestInfo requestInfo)
        {
            return ObjectFactory<VendorAppService>.Instance.CancelVendorManufacturer(requestInfo);
        }

        /// <summary>
        /// 供应商等级申请通过
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Vendor/PassVendorModifyRequest", Method = "PUT")]
        public VendorModifyRequestInfo PassVendorModifyRequest(VendorModifyRequestInfo requestInfo)
        {
            return ObjectFactory<VendorAppService>.Instance.PassVendorManufacturer(requestInfo);
        }

        [WebInvoke(UriTemplate = "/Vendor/ApproveVendor", Method = "PUT")]
        public bool ApproveVendor(VendorApproveInfo requestInfo)
        {
            ObjectFactory<VendorAppService>.Instance.ApproveVendor(requestInfo);
            return true;
        }

        [WebInvoke(UriTemplate = "/Vendor/EditVendorInfo", Method = "PUT")]
        public VendorInfo EditVendor(VendorInfo info)
        {

            VendorInfo oldVendor = LoadVendorBySysNo(info.SysNo.Value.ToString());
            if (string.IsNullOrEmpty(oldVendor.VendorFinanceInfo.AccountNumber) || (oldVendor.VendorFinanceInfo.AccountNumber.Trim() != info.VendorFinanceInfo.AccountNumber.Trim()))
            {
                VendorQueryFilter filter = new VendorQueryFilter()
                {
                    PageInfo = new QueryFilter.Common.PagingInfo()
                    {
                        PageIndex = 0,
                        PageSize = 10,
                        SortBy = "v.SysNo"
                    },
                    Account = info.VendorFinanceInfo.AccountNumber,
                    IsConsign = info.VendorBasicInfo.ConsignFlag
                };
                QueryResult result = QueryVendorList(filter);

                if (result.TotalCount > 0)
                {
                    throw new BizException("该供应商银行账号和对应账期属性已存在!");

                }
            }
            info.VendorBasicInfo.VendorRank = info.VendorBasicInfo.RequestVendorRank.Value;

            if (string.IsNullOrEmpty(oldVendor.VendorBasicInfo.BuyWeekDayVendor))
            {
                oldVendor.VendorBasicInfo.BuyWeekDayVendor = "";
            }
            if (oldVendor.VendorBasicInfo.BuyWeekDayVendor != info.VendorBasicInfo.RequestBuyWeekDayVendor)
            {
                info.VendorBasicInfo.BuyWeekDayVendor = info.VendorBasicInfo.RequestBuyWeekDayVendor;
            }
            info.VendorBasicInfo.BuyWeekDayVendor = string.IsNullOrEmpty(info.VendorBasicInfo.BuyWeekDayVendor) ? "" : info.VendorBasicInfo.BuyWeekDayVendor;
            //更新:

            return ObjectFactory<VendorAppService>.Instance.EditVendorInfo(info);

        }

        /// <summary>
        /// 更新供应商邮件地址(采购单-邮件地址维护用到.)
        /// </summary>
        /// <param name="vendorInfo"></param>
        [WebInvoke(UriTemplate = "/Vendor/UpdateVendorMailAddress", Method = "PUT")]
        public void UpdateVendorMailAddress(VendorInfo vendorInfo)
        {
            ObjectFactory<VendorAppService>.Instance.UpdateVendorEmailAddress(vendorInfo);
        }

        [WebInvoke(UriTemplate = "/Vendor/CreateVendorHistoryLog", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public VendorHistoryLog CreateVendorHistoryLog(VendorHistoryLog logInfo)
        {
            return ObjectFactory<VendorAppService>.Instance.CreateVendorHistoryLog(logInfo);
        }

        [WebInvoke(UriTemplate = "/Vendor/LoadWarehouseInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<WarehouseInfo> LoadWarehouseInfo(string companyCode)
        {
            return ObjectFactory<VendorAppService>.Instance.LoadWareHouseInfo(companyCode);
        }

        /// <summary>
        /// 获取Vendor下可以锁定的PM列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Vendor/QueryCanLockPMListVendorSysNo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCanLockPMListVendorSysNo(VendorQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            result.Data = ObjectFactory<IVendorQueryDA>.Instance.QueryCanLockPMListByVendorSysNo(queryFilter);
            result.TotalCount = result.Data.Rows.Count;
            return result;
        }

        /// <summary>
        /// 获取已经锁定的PM列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Vendor/QueryVendorPMHoldInfoByVendorSysNo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVendorPMHoldInfoByVendorSysNo(VendorQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            result.Data = ObjectFactory<IVendorQueryDA>.Instance.QueryVendorPMHoldInfoByVendorSysNo(queryFilter);
            result.TotalCount = result.Data.Rows.Count;
            return result;
        }


        /// <summary>
        /// 查询供应商应付款余额与负PO金额
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Vendor/GetVendorPayBalanceByVendorSysNo/{vendorSysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult GetVendorPayBalanceByVendorSysNo(string vendorSysNo)
        {
            QueryResult result = new QueryResult()
            {
                Data = ObjectFactory<IVendorQueryDA>.Instance.QueryVendorPayBalanceByVendorSysNo(Convert.ToInt32(vendorSysNo))
            };
            return result;
        }

        /// <summary>
        /// 移动上传的附件
        /// </summary>
        /// <param name="fileIdentity"></param>
        [WebInvoke(UriTemplate = "/Vendor/MoveVendorAttachmentFile", Method = "PUT")]
        public string MoveVendorAttachmentFile(string fileIdentity)
        {
            return ObjectFactory<VendorAppService>.Instance.MoveVendorFileAttachments(fileIdentity);
        }

        /// <summary>
        /// 获取仓库列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Vendor/GetStockList", Method = "POST")]
        public List<WarehouseInfo> GetWarehouseList(string companyCode)
        {
            return ObjectFactory<VendorAppService>.Instance.LoadWareHouseInfo(companyCode);
        }

        [WebInvoke(UriTemplate = "/Vendor/QueryVendorStoreList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVendorStoreList(int vendorSysNo)
        {
            QueryResult result = new QueryResult();
            result.Data = ObjectFactory<IVendorStoreQueryDA>.Instance.QueryVendorStoreList(vendorSysNo);
            return result;
        }

        /// <summary>
        /// 查询商家页面类型
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Vendor/QueryStorePageType", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult QueryStorePageType()
        {
            QueryResult result = new QueryResult()
            {
                Data = ObjectFactory<IVendorQueryDA>.Instance.QueryStorePageType()
            };
            return result;
        }

        /// <summary>
        /// 查询商家页面
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Vendor/QueryStorePageInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult QueryStorePageInfo(StorePageQueryFilter filter)
        {
            int totalCount = 0;

            QueryResult result = new QueryResult()
            {
                Data = ObjectFactory<IVendorQueryDA>.Instance.QueryStorePageInfo(filter,out totalCount)
            };
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 批量删除商家页面
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Vendor/BatchDeleteStorePageInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult BatchDeleteStorePageInfo(List<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                ObjectFactory<IVendorQueryDA>.Instance.DeleteStorePageInfo(list[i]);
            }

            QueryResult result = new QueryResult();
            result.TotalCount = 1;
            return result;
        }

        [WebInvoke(UriTemplate = "/Vendor/getPreviewPath", Method = "PUT")]
        public string getPreviewPath(string fileIdentity)
        {
            return AppSettingManager.GetSetting("Store", "PreviewBaseUrl");
        }

        /// <summary>
        /// 审核装修页面
        /// </summary>
        /// <param name="sysStatus"></param>
        [WebInvoke(UriTemplate = "/Vendor/CheckStorePageInfo", Method = "PUT")]
        public void CheckStorePageInfo(string sysStatus)
        {
            string[] sysStatuses = sysStatus.Split(',');
            int sysNo = Convert.ToInt32(sysStatuses[0]);
            int status = Convert.ToInt32(sysStatuses[1]);
            ObjectFactory<IVendorQueryDA>.Instance.CheckStorePageInfo(sysNo, status);
        }
    }
}
