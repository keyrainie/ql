using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    /// <summary>
    /// 内需要调用其它Domain的Restful Service统一放到这里来管理
    /// </summary>
    public class OtherDomainDataFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient SO_restClient;
        private readonly RestClient Common_restClient;
        private readonly RestClient RMA_restClient;
        private readonly RestClient IM_restClient;

        /// <summary>
        /// SOService服务基地址
        /// </summary>
        private string SOServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_SO, ConstValue.Key_ServiceBaseUrl);
            }
        }

        /// <summary>
        /// CommonServier服务基地址
        /// </summary>
        private string CommonServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Common, ConstValue.Key_ServiceBaseUrl);
            }
        }

        /// <summary>
        /// RMAServier服务基地址
        /// </summary>
        private string RMAServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_RMA, "ServiceBaseUrl");
            }
        }

        /// <summary>
        /// IMService服务基地址
        /// </summary>
        private string IMServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_IM, "ServiceBaseUrl");
            }
        }

        public OtherDomainDataFacade()
        {
            SO_restClient = new RestClient(SOServiceBaseUrl);
            Common_restClient = new RestClient(CommonServiceBaseUrl);
            RMA_restClient = new RestClient(RMAServiceBaseUrl);
            IM_restClient = new RestClient(IMServiceBaseUrl);
        }

        public OtherDomainDataFacade(IPage page)
        {
            viewPage = page;
            SO_restClient = new RestClient(SOServiceBaseUrl, page);
            Common_restClient = new RestClient(CommonServiceBaseUrl, page);
            RMA_restClient = new RestClient(RMAServiceBaseUrl, page);
            IM_restClient = new RestClient(IMServiceBaseUrl, page);
        }

        #region SO_Service

        /// <summary>
        /// 根据订单编号取得订单基本信息
        /// </summary>
        public void GetSOBaseInfo(int soSysNo, Action<SOBaseInfoVM> callback)
        {
            string relativeUrl = string.Format("/SOService/SO/BaseInfo/{0}", soSysNo);
            SO_restClient.Query<SOBaseInfo>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                SOBaseInfoVM soInfoVM = null;
                if (args.Result != null)
                {
                    soInfoVM = args.Result.Convert<SOBaseInfo, SOBaseInfoVM>((s, t) =>
                    {
                        t.SOSysNo = s.SysNo;
                    });
                }
                callback(soInfoVM);
            });
        }

        /// <summary>
        /// 根据订单编号列表取得订单基本信息列表
        /// </summary>
        /// <param name="soSysNoList"></param>
        /// <param name="callback"></param>
        public void GetSOBaseInfoList(List<int> soSysNoList, Action<List<SOBaseInfoVM>> callback)
        {
            string relativeUrl = "/SOService/SO/BaseInfoList";
            SO_restClient.Query<List<SOBaseInfo>>(relativeUrl, soSysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<SOBaseInfoVM> soInfoVMList = new List<SOBaseInfoVM>();
                if (args.Result != null && args.Result.Count > 0)
                {
                    soInfoVMList.AddRange(
                        args.Result.Select(x => x.Convert<SOBaseInfo, SOBaseInfoVM>((s, t) =>
                        {
                            t.SOSysNo = s.SysNo;
                        }))
                        .ToList()
                    );
                }
                callback(soInfoVMList);
            });
        }

        #endregion SO_Service

        #region RMA_Service

        /// <summary>
        /// 取得退款原因列表
        /// </summary>
        /// <param name="callback"></param>
        public void GetRefundReaons(bool needAll, EventHandler<RestClientEventArgs<List<CodeNamePair>>> callback)
        {
            string relativeUrl = "/RMAService/Refund/GetRefundReaons";
            RMA_restClient.Query<List<CodeNamePair>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var reasonList = args.Result;
                if (needAll)
                {
                    reasonList.Insert(0, new CodeNamePair()
                    {
                        Name = ResCommonEnum.Enum_All
                    });
                }
                RestClientEventArgs<List<CodeNamePair>> eventArgs = new RestClientEventArgs<List<CodeNamePair>>(reasonList, viewPage);
                callback(obj, eventArgs);
            });
        }

        #endregion RMA_Service

        #region Common_Service

        /// <summary>
        /// 取得业务操作用户
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void GetBizOperationUser(BizOperationUserQueryFilter filter, bool needAll, Action<List<UserInfo>> callback)
        {
            string relativeUrl = "/CommonService/User/GetBizOperationUser";
            Common_restClient.Query<List<UserInfo>>(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var userList = args.Result;
                if (needAll)
                {
                    userList.Insert(0, new UserInfo()
                    {
                        UserName = ResCommonEnum.Enum_All
                    });
                }
                callback(userList);
            });
        }

        /// <summary>
        /// 取得投递员列表，应用场景是绑定下拉列表框
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="callback"></param>
        public void GetFreightManList(bool needAll, Action<List<UserInfo>> callback)
        {
            string relativeUrl = string.Format("/CommonService/User/FreightMan/{0}", CPApplication.Current.CompanyCode);
            Common_restClient.Query<List<UserInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<UserInfo> freightManList = args.Result;
                if (needAll)
                {
                    freightManList.Insert(0, new UserInfo()
                    {
                        UserName = ResCommonEnum.Enum_All
                    });
                }
                callback(freightManList);
            });
        }

        /// <summary>
        /// 加载配送方式列表
        /// </summary>
        /// <param name="callback"></param>
        public void LoadPayTypeList(Action<List<PayType>> callback)
        {
            string relativeUrl = string.Format("CommonService/PayType/GetAll/{0}", CPApplication.Current.CompanyCode);
            Common_restClient.Query<List<PayType>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        #endregion Common_Service

        #region IM_Service
        /// <summary>
        /// 查询PMGroup
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryPMGroupList(EventHandler<RestClientEventArgs<List<ProductManagerGroupInfo>>> callback)
        {
            string relativeUrl = "/ProductManagerGroup/QueryProductManagerGroupInfo";
            IM_restClient.Query<List<ProductManagerGroupInfo>>(relativeUrl, callback);
        }  
        #endregion

    }
}