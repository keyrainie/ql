using System;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ComputerConfigFacade
    {
        private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public ComputerConfigFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void GetAllConfigTypes(EventHandler<RestClientEventArgs<List<ComputerConfigType>>> callback)
        {
            string relativeUrl = "/MKTService/ComputerConfig/AllConfigType";
            restClient.Query<List<ComputerConfigType>>(relativeUrl, callback);

        }

        public void GetEditUsers(string companyCode, string channelID, EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = string.Format("/MKTService/ComputerConfig/AllEditUser/{0}/{1}", companyCode, channelID);
            restClient.Query<List<UserInfo>>(relativeUrl, callback);

        }

        public void Create(ComputerConfigMasterVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var data = ConvertVMToEntity(vm);
            string relativeUrl = "/MKTService/ComputerConfig/Create";
            restClient.Create(relativeUrl, data, callback);
        }

        public void Update(ComputerConfigMasterVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var data = ConvertVMToEntity(vm);

            string relativeUrl = "/MKTService/ComputerConfig/Update";
            restClient.Update(relativeUrl, data, callback);
        }

        public void CheckOptionalAccessoriesItemAndCombos(List<int> sysNos, EventHandler<RestClientEventArgs<List<string>>> callback)
        {
            string relativeUrl = "/MKTService/ComputerConfig/CheckOptionalAccessoriesItemAndCombos";
            restClient.Create<List<string>>(relativeUrl, sysNos, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        private ComputerConfigMaster ConvertVMToEntity(ComputerConfigMasterVM vm)
        {
            var data = vm.ConvertVM<ComputerConfigMasterVM, ComputerConfigMaster>();
            data.CompanyCode = CPApplication.Current.CompanyCode;
            data.WebChannel = new WebChannel
            {
                ChannelID = vm.ChannelID
            };
            data.ConfigItemList = new List<ComputerConfigItem>();
            foreach (var item in vm.ConfigItemList)
            {
                if (item.ProductSysNo > 0 && item.ProductQty > 0)
                {
                    var result = item.ConvertVM<ComputerConfigItemVM, ComputerConfigItem>();
                    data.ConfigItemList.Add(result);
                }
            }
            return data;
        }

        public void Load(int sysNo, Action<ComputerConfigMasterVM> callback)
        {
            string relativeUrl = "/MKTService/ComputerConfig/" + sysNo.ToString();
            restClient.Query<ComputerConfigMaster>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle() || callback == null) return;
                    var result = args.Result.Convert<ComputerConfigMaster, ComputerConfigMasterVM>((info, vm) =>
                        {
                            if (info.WebChannel != null)
                            {
                                vm.ChannelID = info.WebChannel.ChannelID;
                            }
                            if (info.ConfigItemList != null)
                            {
                                vm.ConfigItemList = new List<ComputerConfigItemVM>();
                                foreach (var item in info.ConfigItemList)
                                {
                                    var itemVM = item.Convert<ComputerConfigItem, ComputerConfigItemVM>();
                                    if (item.PartsCategories != null)
                                    {
                                        string c3SysNoList = "";
                                        foreach (var pc in item.PartsCategories)
                                        {
                                            c3SysNoList += pc.CategorySysNo.ToString() + ",";
                                        }
                                        itemVM.ValidC3List = c3SysNoList.TrimEnd(',');
                                    }
                                    vm.ConfigItemList.Add(itemVM);
                                }
                            }
                        });
                    result.CalcTotal();
                    callback(result);
                });
        }

        public void GetAllParts(Action<List<ComputerConfigItemVM>> callback)
        {
            string relativeUrl = "/MKTService/ComputerConfig/AllComputerParts";
            restClient.Query<List<ComputerParts>>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle() || callback == null) return;
                    List<ComputerConfigItemVM> result = new List<ComputerConfigItemVM>();
                    foreach (var part in args.Result)
                    {
                        var itemVM = new ComputerConfigItemVM();
                        itemVM.ComputerPartSysNo = part.SysNo;
                        itemVM.ComputerPartName = part.ComputerPartName;
                        itemVM.Note = part.Note;
                        itemVM.Priority = part.Priority;
                        itemVM.IsMust = part.IsMust;
                        itemVM.Discount = 0m;
                        if (part.PartsCategories != null)
                        {
                            string c3SysNoList = "";
                            foreach (var pc in part.PartsCategories)
                            {
                                c3SysNoList += pc.CategorySysNo.ToString() + ",";
                            }
                            itemVM.ValidC3List = c3SysNoList.TrimEnd(',');
                        }
                        result.Add(itemVM);
                    }
                    callback(result);
                });

        }

        public void Query(ComputerConfigMasterQueryVM vm, PagingInfo pagingInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<ComputerConfigMasterQueryVM, ComputerConfigQueryFilter>();
            data.PageInfo = pagingInfo;
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/MKTService/ComputerConfig/Query";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        public void BuildConfigItem(ComputerConfigItemVM vm, Action<ComputerConfigItemVM> callback)
        {
            var data = vm.ConvertVM<ComputerConfigItemVM, ComputerConfigItem>();
            string relativeUrl = "/MKTService/ComputerConfig/BuildConfigItem";
            restClient.Query<ComputerConfigItem>(relativeUrl, data, (s, args) =>
                {
                    if (args.FaultsHandle() || callback == null)
                    {
                        callback(null);
                        return;
                    }
                    var result = args.Result.Convert<ComputerConfigItem, ComputerConfigItemVM>();
                    callback(result);
                });
        }

        //作废
        public void Void(List<int> sysNos, Action cb)
        {
            string relativeUrl = "/MKTService/ComputerConfig/Void";
            restClient.Update(relativeUrl, sysNos, (s, args) =>
            {
                if (args.FaultsHandle() || cb == null) return;
                cb();
            });

        }

        //审核通过
        public void ApprovePass(List<int> sysNos, Action cb)
        {
            string relativeUrl = "/MKTService/ComputerConfig/ApprovePass";
            restClient.Update(relativeUrl, sysNos, (s, args) =>
            {
                if (args.FaultsHandle() || cb == null) return;
                cb();
            });

        }

        //审核拒绝
        public void ApproveDecline(List<int> sysNos, Action cb)
        {
            string relativeUrl = "/MKTService/ComputerConfig/ApproveDecline";
            restClient.Update(relativeUrl, sysNos, (s, args) =>
            {
                if (args.FaultsHandle() || cb == null) return;
                cb();
            });

        }
    }
}
