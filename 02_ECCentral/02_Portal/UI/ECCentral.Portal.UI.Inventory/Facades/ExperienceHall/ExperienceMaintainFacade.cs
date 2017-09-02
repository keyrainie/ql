using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class ExperienceMaintainFacade
    {
        private readonly RestClient restClient;

        /// <summary>
        /// InventoryService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Inventory, ConstValue.Key_ServiceBaseUrl);
            }
        }

        public ExperienceMaintainFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ExperienceMaintainFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        #region 借货单信息处理

        public void GetLendRequestInfoBySysNo(int sysNo, Action<ExperienceVM> callback)
        {
            string relativeUrl = string.Format("/InventoryService/ExperienceHall/Load/{0}", sysNo);
            restClient.Query<ExperienceInfo>(relativeUrl, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    ExperienceVM vm = null;
                    if (args.Result != null)
                    {
                        vm = ConvertRequestInfoToVM(args.Result);
                    }
                    if (callback != null)
                    {
                        callback(vm);
                    }
                }
            });
        }


        public void CreateRequest(ExperienceVM requestVM, Action<int?> callback)
        {
            string relativeUrl = "/InventoryService/ExperienceHall/CreateExperience";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Create);

            ExperienceInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Create<int?>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    //ExperienceVM vm = null;
                    if (args.Result != null)
                    {
                        //vm = ConvertRequestInfoToVM(args.Result);
                    }
                    if (callback != null)
                    {
                        callback(args.Result);
                    }
                }
            });
        }

        public void UpdateRequest(ExperienceVM requestVM, Action<int?> callback)
        {
            string relativeUrl = "/InventoryService/ExperienceHall/UpdateExperience";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Update);
            ExperienceInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Update<int?>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    //ExperienceVM vm = null;
                    if (args.Result != null)
                    {
                        //vm = ConvertRequestInfoToVM(args.Result);
                    }
                    if (callback != null)
                    {
                        callback(args.Result);
                    }
                }
            });
        }


        private void UpdateRequestStatus(string relativeUrl, ExperienceVM requestVM, Action<int?> callback)
        {
            ExperienceInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Update<int?>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    //ExperienceVM vm = null;
                    if (args.Result != null)
                    {
                        //vm = ConvertRequestInfoToVM(args.Result);
                    }
                    if (callback != null)
                    {
                        callback(args.Result);
                    }
                }
            });
        }

        public void AbandonExperience(ExperienceVM requestVM, Action<int?> callback)
        {
            string relativeUrl = "/InventoryService/ExperienceHall/AbandonExperience";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Abandon);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void AuditExperience(ExperienceVM requestVM, Action<int?> callback)
        {
            string relativeUrl = "/InventoryService/ExperienceHall/AuditExperience";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Audit);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void CancelAuditExperience(ExperienceVM requestVM, Action<int?> callback)
        {
            string relativeUrl = "/InventoryService/ExperienceHall/CancelAuditExperience";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.CancelAudit);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void ExperienceInOrOut(ExperienceVM requestVM, Action<int?> callback)
        {
            string relativeUrl = "/InventoryService/ExperienceHall/ExperienceInOrOut";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Audit);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        #endregion  借货单信息处理

        private ExperienceVM ConvertRequestInfoToVM(ExperienceInfo info)
        {
            ExperienceVM vm = info.Convert<ExperienceInfo, ExperienceVM>((i, v) =>
            {

            });
            //ExperienceVM vm = new ExperienceVM()
            //{
            //    AllocateType = info.AllocateType,
            //    AuditDate = info.AuditDate,
            //    AuditUser = info.AuditUser,
            //    EditDate = info.EditDate,
            //    EditUser = info.EditUser,
            //    InDate = info.InDate,
            //    InUser = info.InUser,
            //    Meno = info.Meno,
            //    Status = info.Status,
            //    StockSysNo = info.StockSysNo,
            //    SysNo = info.SysNo
            //};

            //if (info.ExperienceItemInfoList != null)
            //{
            //    info.ExperienceItemInfoList.ForEach(p =>
            //    {
            //        ExperienceItemVM item = new ExperienceItemVM()
            //        {
            //            AllocateQty = p.AllocateQty,
            //            AllocateSysNo = p.AllocateSysNo,
            //            ProductID = p.ProductID,
            //            ProductName = p.ProductName,
            //            ProductSysNo = p.ProductSysNo,
            //            SysNo = p.SysNo
            //        };

            //        vm.ExperienceItemInfoList.Add(item);
            //    });
            //}
            return vm;
        }

        private ExperienceInfo ConvertRequestVMToInfo(ExperienceVM vm)
        {
            ExperienceInfo info = vm.ConvertVM<ExperienceVM, ExperienceInfo>((v, i) =>
            {

            });

            info.ExperienceItemInfoList = new List<ExperienceItemInfo>();

            vm.ExperienceItemInfoList.ForEach(item =>
            {
                //info.ExperienceItemInfoList.Add(new ExperienceItemInfo()
                //{
                //    AllocateQty = item.AllocateQty,
                //    ProductID = item.ProductID,
                //    ProductName = item.ProductName,
                //    ProductSysNo = item.ProductSysNo
                //});

                ExperienceItemInfo itemInfo = item.ConvertVM<ExperienceItemVM, ExperienceItemInfo>();
                info.ExperienceItemInfoList.Add(itemInfo);
            });

            return info;
        }

        private void SetRequestUserInfo(ExperienceVM requestVM, InventoryAdjustSourceAction action)
        {
            int? currentUserSysNo = CPApplication.Current.LoginUser.UserSysNo;

            if (action == InventoryAdjustSourceAction.Create)
            {
                requestVM.InUser = currentUserSysNo;
            }
            else if (action == InventoryAdjustSourceAction.Audit || action == InventoryAdjustSourceAction.CancelAudit)
            {
                requestVM.AuditUser = currentUserSysNo;
            }
            else
            {
                requestVM.EditUser = currentUserSysNo;
            }
        }




        #region 体验库存查询

        public void QueryExperienceHallInventory(ExperienceHallInventoryInfoQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/ExperienceHall/ExperienceHallInventoryQuery";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void ExportExcelQueryExperienceHallInventory(ExperienceHallInventoryInfoQueryFilter queryFilter, ColumnSet[] columns)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/ExperienceHall/ExperienceHallInventoryQuery";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        #endregion

        #region 体验库调拨单查询

        public void ExperienceHallAllocateOrderQuery(ExperienceHallAllocateOrderQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/InventoryService/ExperienceHall/ExperienceHallAllocateOrderQuery";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void ExportExcelExperienceHallAllocateOrderQuery(ExperienceHallAllocateOrderQueryFilter queryFilter, ColumnSet[] columns)
        {
            //queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/ExperienceHall/ExperienceHallAllocateOrderQuery";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        #endregion
    }
}
